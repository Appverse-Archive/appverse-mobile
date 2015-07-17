'use strict';

// The actual grunt server settings
module.exports = {
    options: {
        port: '<%= ports.app %>',
        livereload: '<%= ports.livereload %>',
        // Change this to '0.0.0.0' to access the server from outside
        hostname: '0.0.0.0'
    },
    livereload: {
        options: {
            open: false,
            base: [
                '.tmp', '<%= paths.app %>'
            ],
            middleware: function(connect, options) {
                if (!Array.isArray(options.base)) {
                    options.base = [options.base];
                }
                // Setup the proxy
                var middlewares = [require('grunt-connect-proxy/lib/utils').proxyRequest];
                // Serve static files.
                options.base.forEach(function(base) {
                    middlewares.push(connect.static(base));
                });
                // Make directory browse-able.
                var directory = options.directory || options.base[options.base.length - 1];
                middlewares.push(connect.directory(directory));
                return middlewares;
            }
        },
        proxies: [{
            context: '/api',
            host: "http://127.0.0.1",
            port: 8000,
            https: false,
            rewrite: {
                '^/api': ''
            }
        },{//http://20minutos.feedsportal.com/c/32489/f/478284/index.rss
            context: '/20m',
            host: "20minutos.feedsportal.com",
            port: 80,
            https: false,
            rewrite: {
                '^/20m': '/c/32489/f/478284/index.rss'
            }            
        },{//http://www.ideal.es/granada/rss/atom/portada
            context: '/granada',
            host: "www.ideal.es",
            port: 80,
            https: false,
            rewrite: {
                '^/granada': '/granada/rss/atom/portada'
            }            
        },{//https://en.blog.wordpress.com/feed/
            context: '/feed',
            host: "en.blog.wordpress.com",   
            port:443,
            https: true
        },{//http://www.bbc.com/mundo/temas/tecnologia/index.xml
            context: '/mundo',
            host: "www.bbc.com",
            port: 80,
            https: false,
            rewrite: {
                //'^/mundo': 'mundo/temas/tecnologia/index.xml'
            }
        }]

    },

    mocklivereload: {
        options: {
            middleware: function(connect) {
                if (!Array.isArray(options.base)) {
                    options.base = [options.base];
                }

                // Setup the proxy
                var middlewares = [require('grunt-connect-proxy/lib/utils').proxyRequest];

                // Serve static files.
                options.base.forEach(function(base) {
                    middlewares.push(connect.static(base));
                });

                // Make directory browse-able.
                var directory = options.directory || options.base[options.base.length - 1];
                middlewares.push(connect.directory(directory));

                return middlewares;
            }
        },
        proxies: [{
            context: '/api',
            host: '127.0.0.1',
            port: 8888,
            https: false,
            rewrite: {
                '^/api': ''
            }
        }]

    },
    test: {
        options: {
            port: '<%= ports.test %>',
            base: [
                '.tmp', 'test', '<%= paths.app %>'
            ]
        }
    },
    dist: {
        options: {
            open: false,
            port: '<%= ports.dist %>',
            base: '<%= paths.dist %>',
            livereload: false
        }
    },
    doc: {
        options: {
            port: '<%= ports.doc %>',
            base: '<%= paths.doc %>',
            livereload: false
        }
    }

};
