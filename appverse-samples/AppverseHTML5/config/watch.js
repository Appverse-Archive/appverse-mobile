'use strict';

// Watches files for changes and runs tasks based on the changed files
module.exports = {
    coffee: {
        files: ['<%=paths.app%>/scripts/**/*.coffee'],
        tasks: ['coffee:app']
    },
    coffeeTest: {
        files: ['test/spec/**/*.coffee'],
        tasks: ['coffee:test']
    },
    sass: {
        files: ['<%=paths.app%>/styles/**/*.{scss,sass}'],
        tasks: ['sass', 'autoprefixer:tmp']
    },
    styles: {
        files: ['<%=paths.app%>/styles/**/*.css'],
        tasks: ['autoprefixer:styles']
    },
    livereload: {
        options: {
            livereload: true
        },
        files: [
                   '<%=paths.app%>/**/*.html',
                    '{.tmp, <%= paths.app %>}/styles/**/*.css',
                    '{.tmp, <%= paths.app %>}/scripts/**/*.js',
                    '<%= paths.app %>/resources/**/*'
                ]
    }
};
