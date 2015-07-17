'use strict';

module.exports = function (grunt) {

    grunt.registerTask('doc', [
        'clean:doc',
        'docular'
    ]);
};
