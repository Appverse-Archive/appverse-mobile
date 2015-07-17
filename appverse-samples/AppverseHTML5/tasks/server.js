'use strict';

module.exports = function (grunt) {

    grunt.registerTask('server', [
        'clean:server',
        'configureProxies:livereload',
        'concurrent:server',
        'autoprefixer',
        'connect:livereload',
        'watch'
    ]);

    grunt.registerTask('server:open', [
        'clean:server',
        'configureProxies:livereload',
        'concurrent:server',
        'autoprefixer',
        'connect:livereload',
        'open:server',
        'watch'
    ]);

    grunt.registerTask('server:dist', [
        'connect:dist',
        'open:dist',
        'watch'
    ]);

    grunt.registerTask('server:doc', [
        'connect:doc',
        'open:doc',
        'watch:doc'
    ]);

};
