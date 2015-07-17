/*jshint node:true*/
'use strict';
// Copies remaining files to places other tasks can use

module.exports = {
    dist: {
        files: [{
            expand: true,
            dot: true,
            cwd: '<%= paths.app %>',
            dest: '<%= paths.dist %>',
            src: [
                        '*.{ico,png,txt}',
                        '.htaccess',
                        'images/{,*/}*.{gif,webp}',
                        'resources/**',
                        'styles/fonts/*',
                        'styles/images/*',
                        '*.html',
                        'views/**/*.html',
                        'template/**/*.html'
                    ]
                }, {
            expand: true,
            cwd: '<%=paths.app%>/bower_components/bootstrap-sass/assets/fonts/bootstrap',
            dest: '<%=paths.dist%>/fonts',
            src: '**/*'
                }, {
            expand: true,
            cwd: '.tmp/images',
            dest: '<%=paths.dist%>/images',
            src: 'generated/*'
                }, {
            expand: true,
            cwd: '<%=paths.app%>/bower_components/angular-i18n',
            dest: '<%=paths.dist%>/resources/i18n/angular',
            src: [
                        '*en-us.js',
                        '*es-es.js',
                        '*ja-jp.js',
                        '*ar-eg.js'
                    ]
                }]
    },
    styles: {
        expand: true,
        cwd: '<%= paths.app %>' + '/styles',
        dest: '.tmp/styles',
        src: '**/*.css'
    },
    i18n: {
        expand: true,
        cwd: '<%=paths.app%>/bower_components/angular-i18n',
        dest: '.tmp/resources/i18n/angular',
        src: [
                    '*en-us.js',
                    '*es-es.js',
                    '*ja-jp.js',
                    '*ar-eg.js'
                ]
    },
    fonts: {
        expand: true,
        cwd: '<%=paths.app%>/bower_components/bootstrap-sass/assets/fonts/bootstrap',
        dest: '.tmp/fonts',
        src: '**/*'
    }
};
