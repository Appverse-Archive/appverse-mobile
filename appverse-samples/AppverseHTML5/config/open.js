'use strict';

// Renames files for browser caching purposes
module.exports = {
    server: {
        url: 'http://' + require('os').hostname() + ':<%= ports.app %>'
    },
    dist: {
        url: 'http://' + require('os').hostname() + ':<%= ports.dist %>'
    },
    doc: {
        url: 'http://' + require('os').hostname() + ':<%= ports.doc %>'
    }
};
