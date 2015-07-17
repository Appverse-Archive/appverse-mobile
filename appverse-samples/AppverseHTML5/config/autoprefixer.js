'use strict';
// Add vendor prefixed styles
module.exports = {
    options: {
        browsers: ['last 1 version']
    },
    tmp: {
        files: [
            {
                expand: true,
                cwd: '.tmp/styles/',
                src: '{,*/}*.css',
                dest: '.tmp/styles/'
   }
  ]
    },
    styles: {
        files: [{
            expand: true,
            cwd: '<%=paths.app%>/styles/',
            src: '**/*.css',
            dest: '.tmp/styles/'
                }]
    }
};
