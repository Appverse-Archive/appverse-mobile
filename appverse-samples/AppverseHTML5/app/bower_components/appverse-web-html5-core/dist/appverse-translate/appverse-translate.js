(function() {
    'use strict';

    /**
     * @ngdoc module
     * @name appverse.translate
     * @description
     * The Internationalization module handles languages in application.
     * It should be directly configurable by developers.
     * **Warning**: Items in each translations object must match items defined in the Configuration module.
     *
     * @requires https://github.com/angular-translate/angular-translate pascalprecht.translate
     * @requires https://github.com/lgalfaso/angular-dynamic-locale tmh.dynamicLocale
     * @requires appverse.configuration
     */
    angular.module('appverse.translate', [
        'pascalprecht.translate',
        'appverse.configuration',
        'tmh.dynamicLocale'
    ])

    // Get module and set config and run blocks
    //angular.module('appverse.translate')
    .config(configBlock)
    .run(runBlock);


    function configBlock($translateProvider, I18N_CONFIG, tmhDynamicLocaleProvider, $provide) {

        var filesConfig = {
            prefix: 'resources/i18n/',
            suffix: '.json'
        };
        var locationPattern = I18N_CONFIG.LocaleFilePattern;

        $translateProvider.useStaticFilesLoader(filesConfig);
        $translateProvider.preferredLanguage(I18N_CONFIG.PreferredLocale);
        tmhDynamicLocaleProvider.localeLocationPattern(locationPattern);

        // Decorate translate directive to change the original behaviour
        // by not removing <i> tags included in the translation text
        $provide.decorator('translateDirective',  decorateTranslateDirective);

    }
    configBlock.$inject = ["$translateProvider", "I18N_CONFIG", "tmhDynamicLocaleProvider", "$provide"];


    function runBlock($log) {

        $log.info('appverse.translate run');

    }
    runBlock.$inject = ["$log"];


    /**
     * Function used by Angular Decorator to override the behaviour of the original
     * translate directive, which does not keep html tags included in the text to be translated.
     * This will make the directive able to keep no-text tags like <i class="icon"></i>
     * after the translation
     *
     * @param  {array}      $delegate       The original instance (provided by decorator)
     * @param  {function}   translateFilter
     * @return {array}                      The modified delegate object
     */
    function decorateTranslateDirective($delegate, translateFilter) {

        // Get the original directive and its linking function
        var directive = $delegate[0];
        var originalLinkFunction = directive.link;

        var newLinkFunction = function(scope, $element, attr, ctrl) {

            // Get the element's html and replaces the text to be translated
            // by a placeholder '%%text%%', so that we can later replace this
            // with the translated string
            var text = $element.text();
            var html = $element.html();
            var htmlOnlyTags = html.replace(text, '%%text%%');

            // First we call the original linking function
            // and afterwards we override the '$on' and '$watch' events
            // to maintain html tags.
            originalLinkFunction.apply(this, [scope, $element, attr, ctrl]);

            scope.$on('$translateChangeSuccess', function () {
                translateElement();
            });

            scope.$watch('[translationId, interpolateParams]', function () {
              if (scope.translationId) {
                translateElement();
              }
            }, true);

            function translateElement() {
                $element.html(translateFilter(scope.translationId, scope.interpolateParams, scope.interpolation));
                var translatedText = $element.text();
                var finalHtml =  htmlOnlyTags.replace('%%text%%', translatedText);
                $element.html(finalHtml);
            }

            return;
        };

        // Since this has already been built via directive provider
        // need to put this on compile, not link, property
        directive.compile = function () {
            return newLinkFunction;
        };

        return $delegate;
    }
    decorateTranslateDirective.$inject = ['$delegate', 'translateFilter'];

})();
