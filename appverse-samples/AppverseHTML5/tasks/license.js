/*jshint node:true */
'use strict';

module.exports = function (grunt) {

    grunt.registerTask('license', 'Generate an HTML report of all NPM modules licenses.', function () {

        grunt.config.data.pkg = grunt.file.readJSON('package.json');
        grunt.task.run('grunt-license-report');
    });
};
