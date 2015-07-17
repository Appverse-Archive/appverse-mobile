/*jshint node:true */
'use strict';

module.exports = function () {
    return {
        basePath: '../',
        frameworks: ['jasmine'],

        // coverage reporter generates the coverage
        reporters: ['progress', 'coverage', 'junit'],

        preprocessors: {
            // source files, that you wanna generate coverage for
            // do not include tests or libraries
            // (these files will be instrumented by Istanbul)
            'app/scripts/**/*.js': 'coverage'
        },

        // optionally, configure the reporter
        coverageReporter: {
            type: 'lcov',
            dir: 'test/coverage/',
            includeAllSources: true
        },

        junitReporter: {
            outputFile: 'test/unit/test-results.xml'
        },

        autoWatch: true,

        // these are default values anyway
        singleRun: false,
        colors: true,

        files: [
            //3rd Party Code
            'app/bower_components/jquery/dist/jquery.min.js',
            'app/bower_components/angular/angular.min.js',
            'app/bower_components/angular-touch/angular-touch.min.js',
            'app/bower_components/modernizr/modernizr.js',
            'app/bower_components/angular-bootstrap/ui-bootstrap-tpls.min.js',
            'app/bower_components/angular-cookies/angular-cookies.min.js',
            'app/bower_components/angular-ui-router/release/angular-ui-router.min.js',
            'app/bower_components/angular-resource/angular-resource.min.js',
            'app/bower_components/angular-cache/dist/angular-cache.min.js',
            'app/bower_components/ng-grid/build/ng-grid.min.js',

            //App-specific Code
            'app/bower_components/appverse-web-html5-core/dist/appverse-cache/appverse-cache.js',
//            'app/bower_components/appverse-web-html5-core/dist/appverse-detection/appverse-detection.js',
            //'app/bower_components/appverse-web-html5-core/dist/appverse-logging/appverse-logging.js',
            'app/bower_components/appverse-web-html5-core/dist/appverse-router/appverse-router.min.js',
            'app/bower_components/appverse-web-html5-core/dist/appverse/appverse.min.js',

            'app/bower_components/lodash/lodash.min.js',
            'app/bower_components/restangular/dist/restangular.min.js',
            'app/bower_components/appverse-web-html5-core/dist/appverse-rest/appverse-rest.js',

            'app/bower_components/appverse-web-html5-security/dist/appverse-security/appverse-security.js',

            'app/bower_components/socket.io-client/dist/socket.io.min.js',
            'app/bower_components/appverse-web-html5-core/dist/appverse-serverpush/appverse-serverpush.js',

            'app/bower_components/appverse-web-html5-core/dist/appverse-translate/appverse-translate.js',
            'app/bower_components/angular-translate/angular-translate.min.js',
            'app/bower_components/angular-translate-loader-static-files/angular-translate-loader-static-files.min.js',
            'app/bower_components/angular-dynamic-locale/src/tmhDynamicLocale.js',

            'app/bower_components/appverse-web-html5-core/dist/appverse-utils/appverse-utils.js',
            'app/bower_components/appverse-web-html5-core/dist/appverse-performance/appverse-performance.js',
            'app/scripts/app.js',
            'app/scripts/controllers/*.js',
            'app/scripts/states/*.js',
        ]
    };
};
