(function () {
    'use strict';
    angular.module('App.Controllers', []);
    angular.module('rssApp', [
        'appverse.rest',
        'ngAnimate',
        'ui.bootstrap',
        'appverse.router',
        'App.Controllers',
        'appverse.logging',
        'appverse',
        'xml'
    ]).run(function ($log) {
        $log.debug('rssApp run');
    });
    AppInit.setConfig({
        environment: {
            'REST_CONFIG': {
                'BaseUrl': '/api',
                'RequestSuffix': ''
            }
        },
        appverseMobile: {},
        mobileBrowser: {}
    });
}());