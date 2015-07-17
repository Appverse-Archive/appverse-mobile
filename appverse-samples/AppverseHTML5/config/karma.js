'use strict';

module.exports = {
    unit: {
        configFile: './test/karma-unit.conf.js',
        autoWatch: false,
        singleRun: true
    },
    unit_auto: {
        configFile: './test/karma-unit.conf.js'
    },
    e2e: {
        configFile: './test/karma-e2e.conf.js',
        autoWatch: false,
        singleRun: true
    },
    e2e_auto: {
        configFile: './test/karma-e2e.conf.js'
    }
};
