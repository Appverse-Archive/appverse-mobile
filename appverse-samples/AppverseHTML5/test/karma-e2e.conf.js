/*jshint node:true */
'use strict';

var sharedConfig = require('./karma-shared.conf');

module.exports = function (config) {
    var conf = sharedConfig();

    conf.coverageReporter.dir += 'e2e';

    conf.browsers = ['Chrome'];

    conf.files = [
        'app/bower_components/appverse-web-html5-core/dist/appverse/appverse.min.js',
        'app/scripts/**/*.js',
        'test/e2e/**/*.js'
    ];

    conf.proxies = {
        '/': 'http://localhost:9003/',
        '/scripts/': 'http://localhost:9876/base/app/scripts/'
    };

    conf.urlRoot = '/__karma__/';

    conf.frameworks = ['ng-scenario'];

    config.set(conf);
};
