'use strict';

module.exports = function (grunt) {

    grunt.registerTask('dist', [
        'clean:dist',
        'useminPrepare',
        'concurrent:dist',
        'autoprefixer',
        'concat',
        'copy:dist',
        'ngAnnotate',
        'cssmin',
        'uglify',
        'rev',
        'usemin',
        'htmlmin'
    ]);


};
