'use strict';
// Performs rewrites based on rev and the useminPrepare configuration

module.exports = {
    html: ['<%=paths.dist%>/*.html', '<%=paths.dist %>/views/**/*.html'],
    css: '<%=paths.dist%>/styles/**/*.css',
    js: '<%=paths.dist%>/scripts/**/*.js',
    options: {
        assetsDirs: ['<%= paths.dist %>/**']
    }
};
