/*jshint node:true */

'use strict';

// Make sure code styles are up to par and there are no obvious mistakes
module.exports = {
    options: {
        jshintrc: '.jshintrc'
    },
    all: [
        'Gruntfile.js',
        'app/scripts/**/*.js',
        'test/*.js',
        'test/{e2e,unit}/**/*.js'
    ]
};
