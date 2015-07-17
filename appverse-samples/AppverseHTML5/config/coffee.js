'use strict';

module.exports = {
    options: {
        sourceMap: true,
        sourceRoot: ''
    },
    app: {
        files: [{
            expand: true,
            cwd: '<%= paths.app %>' + '/scripts',
            src: '**/*.coffee',
            dest: '.tmp/scripts',
            ext: '.js'
                }]
    },
    test: {
        files: [{
            expand: true,
            cwd: 'test/spec',
            src: '{,*/}*.coffee',
            dest: '.tmp/spec',
            ext: '.js'
                }]
    }
};
