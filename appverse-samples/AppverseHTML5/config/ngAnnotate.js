'use strict';
//produce minified svg's in the dist folder
module.exports = {
    dist: {
        files: [{
            expand: true,
            cwd: '.tmp/concat/scripts',
            src: '*.js',
            dest: '.tmp/concat/scripts'
                }]
    }
};
