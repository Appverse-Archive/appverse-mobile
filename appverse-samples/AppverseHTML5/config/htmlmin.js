'use strict';
//produce minified html in the dist folder
module.exports = {
    dist: {
        options: {
            removeComments: true,
            removeCommentsFromCDATA: true,
            removeCDATASectionsFromCDATA: true,
            collapseWhitespace: true,
            //conservativeCollapse: true,
            collapseBooleanAttributes: true,
            removeAttributeQuotes: false,
            removeRedundantAttributes: true,
            useShortDoctype: true,
            removeEmptyAttributes: true,
            removeOptionalTags: true,
            keepClosingSlash: true,
        },
        files: [{
            expand: true,
            cwd: '<%= paths.dist %>',
            src: [
                        '*.html',
                        'views/**/*.html',
                        'template/**/*.html'
                    ],
            dest: '<%= paths.dist %>'
                }]
    }
};
