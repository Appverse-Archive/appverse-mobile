'use strict';

var fs            = require('fs'),
connectLiveReload = require('connect-livereload'),
bowerFile         = require('./bower.json'),
LIVERELOAD_PORT   = 35729,
liveReloadSnippet = connectLiveReload({
    port: LIVERELOAD_PORT
});

module.exports = function (grunt) {

    // Load grunt tasks automatically
    require('load-grunt-tasks')(grunt);

    // Time how long tasks take. Can help when optimizing build times
    require('time-grunt')(grunt);

    // Configurable paths
    var configPaths = {
        src: 'src',
        bowerComponents: 'bower_components',
        dist: 'dist',
        doc: 'doc',
        test: 'test',
        demo: 'demo',
        testsConfig: 'config/test',
        reports: 'reports',
        coverage: 'reports/coverage',
        e2eCoverage: 'reports/coverage/e2e',
        e2eInstrumented: 'reports/coverage/e2e/_instrumented'
    };

    // If app path is defined in bower.json, use it
    try {
        configPaths.src = bowerFile.appPath || configPaths.src;
    } catch (e) {}

    // Define file to load in the demo, ordering and the way they are
    // concatenated for distribution
    var files = {
        '<%= appverse.dist %>/appverse-cache/appverse-cache.js':
            moduleFilesToConcat('<%= appverse.src %>/appverse-cache'),

        '<%= appverse.dist %>/appverse-detection/appverse-detection.js' :
            moduleFilesToConcat('<%= appverse.src %>/appverse-detection', [
                // this order must be preseved as there are dependencies between these providers
                '<%= appverse.src %>/appverse-detection/mobile-detector.provider.js',
                '<%= appverse.src %>/appverse-detection/detection.provider.js',
            ]),

        '<%= appverse.dist %>/appverse-logging/appverse-logging.js' :
            moduleFilesToConcat('<%= appverse.src %>/appverse-logging'),

        '<%= appverse.dist %>/appverse-performance/appverse-performance.js' :
            moduleFilesToConcat('<%= appverse.src %>/appverse-performance'),

        '<%= appverse.dist %>/appverse-translate/appverse-translate.js' :
            moduleFilesToConcat('<%= appverse.src %>/appverse-translate'),

        '<%= appverse.dist %>/appverse-utils/appverse-utils.js' :
            moduleFilesToConcat('<%= appverse.src %>/appverse-utils'),

        '<%= appverse.dist %>/appverse-serverpush/appverse-serverpush.js' :
            moduleFilesToConcat('<%= appverse.src %>/{appverse-serverpush,appverse-socketio}'),

        '<%= appverse.dist %>/appverse-rest/appverse-rest.js' :
            moduleFilesToConcat('<%= appverse.src %>/appverse-rest'),

        '<%= appverse.dist %>/appverse-router/appverse-router.js' :
            moduleFilesToConcat('<%= appverse.src %>/appverse-router'),

        '<%= appverse.dist %>/appverse/appverse.js' : [
            ['<%= appverse.src %>/appverse/integrator.js'].concat(
                moduleFilesToConcat('<%= appverse.src %>/{appverse-configuration*,appverse}')
            ),
        ]
    };

    // Start Grunt config definition
    grunt.initConfig({

        pkg: grunt.file.readJSON('package.json'),

        // Project settings
        appverse: configPaths,

        maven: {
            options: {
                goal: 'install',
                groupId: 'org.appverse.web.framework.modules.frontend.html5',
                repositoryId: 'my-nexus',
                releaseRepository: 'url'

            },
            'install-src': {
                options: {
                    classifier: 'sources'
                },
                files: [{
                    expand: true,
                    cwd: '<%= appverse.src %>/',
                    src: ['**', '!bower_components/**'],
                    dest: '.'
                }]
            },
            'install-min': {
                options: {
                    classifier: 'min'
                },
                files: [{
                    expand: true,
                    cwd: '<%= appverse.dist %>/',
                    src: ['**'],
                    dest: '.'
                }]
            },
            'deploy-src': {
                options: {
                    goal: 'deploy',
                    url: '<%= releaseRepository %>',
                    classifier: 'sources'
                },
                files: [{
                    expand: true,
                    cwd: '<%= appverse.src %>/',
                    src: ['**', '!bower_components/**'],
                    dest: '.'
                }]
            },
            'deploy-min': {
                options: {
                    goal: 'deploy',
                    url: '<%= releaseRepository %>',
                    classifier: 'min'
                },
                files: [{
                    expand: true,
                    cwd: '<%= appverse.dist %>/',
                    src: ['**'],
                    dest: '.'
                }]
            }
        },

        clean: {
            dist: {
                files: [{
                    dot: true,
                    src: [
                        '.tmp',
                        '<%= appverse.dist %>/**',
                        '!<%= appverse.dist %>/.git*'
                    ]
                }]
            },
            coverage: '<%= appverse.coverage %>/**',
            server: '.tmp',
            doc: 'doc/' + bowerFile.version
        },

        jshint: {
            options: {
                jshintrc: '.jshintrc',
                reporter: require('jshint-stylish'),
                //Show failures but do not stop the task
                force: true
            },
            all: [
                '<%= appverse.src %>/{,*/}*.js'
            ]
        },

        // concatenate source files
        concat: {

            // Concatenate all files for a module in a single module file
            modules: {
                files: files
            },

            // Concatenate all modules into a full distribution
            dist: {
                src: [
                    '<%= appverse.dist %>/*/*.js',
                ],
                dest: '<%= appverse.dist %>/appverse-html5-ui.js',
            },
        },

        // ng-annotate tries to make the code safe for minification automatically
        // by using the Angular long form for dependency injection.
        ngAnnotate: {
            dist: {
                files: [{
                    expand: true,
                    cwd: '<%= appverse.dist %>',
                    src: ['**/*.js', '!oldieshim.js'],
                    dest: '<%= appverse.dist %>',
                    extDot: 'last'
            }]
            }
        },

        // Uglifies already concatenated files
        uglify: {
            options: {
                banner: '/*! <%= pkg.name %> - v<%= pkg.version %> - */',
                sourceMap: true,
            },
            dist: {
                files: [{
                        expand: true, // Enable dynamic expansion.
                        cwd: '<%= appverse.dist %>', // Src matches are relative to this path.
                        src: ['**/*.js'], // Actual pattern(s) to match.
                        dest: '<%= appverse.dist %>', // Destination path prefix.
                        ext: '.min.js', // Dest filepaths will have this extension.
                        extDot: 'last' // Extensions in filenames begin after the last dot
                    }
                ]
            }
        },

        karma: {
            unit: {
                configFile: '<%= appverse.testsConfig %>/karma.unit.conf.js',
                autoWatch: false,
                singleRun: true
            },
            unitAutoWatch: {
                configFile: '<%= appverse.testsConfig %>/karma.unit.watch.conf.js',
                autoWatch: true
            },
            midway: {
                configFile: '<%= appverse.testsConfig %>/karma.midway.conf.js',
                autoWatch: false,
                singleRun: true
            },
        },

        // Runs protractor and generate coverage report for e2e tests.
        // Unit and midway are already managedby Karma
        protractor_coverage: {
            options: {
                configFile: '<%= appverse.testsConfig %>/protractor.e2e.conf.js',
                coverageDir: '<%= appverse.e2eCoverage %>',
                keepAlive: false,
                noColor: false,
                args: {},
            },
            run: {}
        },

        // After the tests have been run and the coverage has been measured and captured
        // you want to create a report.
        makeReport: {
            src: '<%= appverse.e2eCoverage %>/*.json',
            options: {
                type: ['html', 'clover'],
                dir: '<%= appverse.e2eCoverage %>'
            }
        },

        // Measuring coverage from protractor tests does not work out of the box.
        // To measure coverage Protractor coverage,
        // all sources need to be instrumented using Istanbul
        instrument: {
            files: '<%= appverse.src %>/**/*.js',
            options: {
                lazy: true,
                basePath: "<%= appverse.e2eInstrumented %>"
            }
        },

        bump: {
            options: {
                files: ['package.json', 'bower.json'],
                updateConfigs: [],
                commit: true,
                commitMessage: 'Release v%VERSION%',
                commitFiles: ['package.json', 'bower.json'],
                createTag: true,
                tagName: 'v%VERSION%',
                tagMessage: 'Version %VERSION%',
                push: true,
                pushTo: 'origin',
                gitDescribeOptions: '--tags --always --abbrev=1 --dirty=-d'
            }
        },

        // Web server
        connect: {

            // General options
            options: {
                protocol: 'http',
                port: 9000,
                hostname: 'localhost'
            },

            // For demo app in chrome
            livereload: {
                options: {
                    port: 9000,
                    middleware: function (connect) {
                        return [
                            delayApiCalls,
                            liveReloadSnippet,
                            mountFolder(connect, configPaths.src),
                            mountFolder(connect, configPaths.bowerComponents),
                            mountFolder(connect, configPaths.demo),
                            httpMethods
                        ];
                    }
                }
            },

            // For e2e tests on demo app, with coverage reporting
            e2e: {
                options: {
                    port: 9091,
                    middleware: function (connect) {
                        return [
                            mountFolder(connect, configPaths.e2eInstrumented + '/src'),
                            mountFolder(connect, configPaths.src),
                            mountFolder(connect, configPaths.bowerComponents),
                            mountFolder(connect, configPaths.demo),
                            httpMethods
                        ];
                    }
                }
            },

            // For e2e tests on built demo app
            e2e_dist: {
                options: {
                    port: 9090,
                    middleware: function (connect) {
                        return [
                            mountFolder(connect, configPaths.src),
                            mountFolder(connect, configPaths.bowerComponents),
                            mountFolder(connect, configPaths.dist),
                            mountFolder(connect, configPaths.demo, {
                                index: 'index-dist.html'
                            }),
                            httpMethods
                        ];
                    }
                }
            },

            // Docs
            doc: {
                options: {
                    port: 9999,
                    keepalive : true,
                    middleware: function (connect) {
                        return [
                            require('connect-modrewrite')(['!^/partials/api/.* /index.html [L]']),
                            mountFolder(connect, configPaths.doc),

                        ];
                    }
                }
            },
        },

        watch: {
            livereload: {
                options: {
                    livereload: LIVERELOAD_PORT
                },
                tasks: ['injector:js'],
                files: [
                    '<%= appverse.demo %>/*.html',
                    '<%= appverse.demo %>/partials/*.html',
                    '<%= appverse.demo %>/js/*.js',
                    //For performance reasons only match one level
                    '<%= appverse.src %>/{,*/}*.js',
                ],
            }
        },

        open: {
            demo: {
                url: '<%= connect.options.protocol %>://<%= connect.options.hostname %>:<%= connect.options.port %>'
            },
            demo_dist: {
                url: '<%= connect.options.protocol %>://<%= connect.options.hostname %>:<%= connect.e2e_dist.options.port %>'
            },
        },

        // Execute commands that cannot be specified with tasks
        exec: {
            // These commands are defined in package.json for
            // automatic resoultion of any binary included in node_modules/
            protractor_start: 'npm run protractor-dist',
            webdriver_update: 'npm run update-webdriver'
        },

        protractor_webdriver: {
            start: {
                options: {
                    command: 'node_modules/.bin/webdriver-manager start --standalone'
                }
            }
        },

        // Automatically include all src/ files in demo's html as script tags
        injector: {
            options: {
                relative: false,
                transform: function (path) {
                    // Demo server directly mounts src folder so the reference to src is not required
                    path = path.replace('/src/', '');
                    return '<script src="' + path + '"></script>';
                }
            },
            js: {
                files: {
                    '<%= appverse.demo %>/index.html': getAllFilesForDemo(files),
                }
            }
        },

        // Generate code analysis reports
        plato: {
            main: {
                options: {
                    jshint: grunt.file.readJSON('.jshintrc')
                },
                files: {
                    '<%= appverse.reports %>/analysis/': [
                        '<%= appverse.src %>/**/*.js',
                        '<%= appverse.test %>/unit/**/*.js',
                        '<%= appverse.test %>/midway/**/*.js',
                        '<%= appverse.test %>/e2e/**/*.js',
                     ]
                }
            }
        },

        concurrent: {
            dist: ['jshint', 'unit', 'midway', 'test:e2e:report', 'analysis']
        }
    });


    /*---------------------------------------- TASKS DEFINITION -------------------------------------*/


    // ------ Dist task. Builds the project -----

    grunt.registerTask('default', [
        'dist'
    ]);

    grunt.registerTask('dist', [
        'concurrent:dist',
        'make_dist_and_test'
    ]);

    grunt.registerTask('make_dist_and_test', [
        'dist:make',
        'test:e2e:dist',
    ]);

    grunt.registerTask('dist:make', [
        'clean:dist',
        'concat',
        'ngAnnotate',
        'uglify'
    ]);

    // ------ Tests tasks -----

    grunt.registerTask('test', [
        'test:all'
    ]);

    grunt.registerTask('test:all', [
        'clean:coverage',
        'unit',
        'midway',
        'e2e',
    ]);

    grunt.registerTask('unit', [
        'test:unit:once'
    ]);

    grunt.registerTask('midway', [
        'test:midway'
    ]);

    grunt.registerTask('e2e', [
        'dist:make',
        'test:e2e:dist'
    ]);

    grunt.registerTask('test:unit:watch', [
        'karma:unitAutoWatch'
    ]);

    grunt.registerTask('test:unit:once', [
        'karma:unit'
    ]);

    grunt.registerTask('test:midway', [
        'karma:midway'
    ]);

    grunt.registerTask('test:e2e:report', [
        'injector:js',
        'instrument',
        'exec:webdriver_update',
        'connect:e2e',
        'protractor_webdriver',
        'protractor_coverage',
        'makeReport'
    ]);

    grunt.registerTask('test:e2e:dist', [
        'injector:js',
        'exec:webdriver_update',
        'connect:e2e_dist',
        'protractor_webdriver',
        'exec:protractor_start',
    ]);

    // ------ Dev tasks. To be run continously while developing -----

    grunt.registerTask('dev', [
        // For now, only execute unit tests when a file changes?
        // midway and e2e are slow and do not give innmedate
        // feedback after a change
        'test:unit:watch'
    ]);


    // ------ Demo tasks. Starts a webserver with a demo app -----

    grunt.registerTask('demo', [
        'injector:js',
        'connect:livereload',
        'open:demo',
        'watch'
    ]);

    grunt.registerTask('demo:dist', [
        'dist:make',
        'open:demo_dist',
        'connect:e2e_dist:keepalive',
    ]);


    // ------ Doc tasks -----

   /* grunt.registerTask('doc', [
        'clean:doc',
        'docgen'
    ]);

    grunt.registerTask('docgen', 'Generates docs', require('./config/grunt-tasks/docgen/grunt-task'));*/


    // ------ Analysis tasks. Runs code analysis -----

    grunt.registerTask('analysis', ['plato']);


    // ------ Deployment tasks -----

    grunt.registerTask('install', [
        'clean',
        'maven:install-src',
        'dist',
        'maven:install-min'
    ]);

    grunt.registerTask('deploy', [
        'clean',
        'maven:deploy-src',
        'dist',
        'maven:deploy-min'
    ]);

    // -------- Special task for websockets demo ---------

    grunt.registerTask('wsserver', 'Start a new web socket demo server', function () {

        var http = require('http');
        var CpuUsage = require('./config/grunt-tasks/cpu-usage');
        var server = http.createServer(function handler() {});

        // Never end grunt task
        this.async();

        server.listen(8080, function () {
            console.log('Websockets Server is listening on port 8080');
        });

        var WebSocketServer = require('websocket').server;

        var wsServer = new WebSocketServer({
            httpServer: server,
            autoAcceptConnections: false
        });

        var cpuUsage = new CpuUsage();

        wsServer.on('request', function (request) {
            var connection = request.accept('', request.origin);
            console.log(' Connection accepted from peer ' + connection.remoteAddress);

            var sendInterval = setInterval(function () {
                var payLoad = (cpuUsage.get() * 100).toFixed(0);
                connection.sendUTF(payLoad);
            }, 100);

            connection.on('close', function (reasonCode, description) {
                clearInterval(sendInterval);
                console.log('Peer ' + connection.remoteAddress + ' disconnected.');
                console.log('Closing Reason: ' + reasonCode);
                console.log('Closing Description: ' + description);
            });
        });

    });

};



/*---------------------------------------- HELPER METHODS -------------------------------------*/

function mountFolder(connect, dir, options) {
    return connect.static(require('path').resolve(dir), options);
}

function delayApiCalls(request, response, next) {
    if (request.url.indexOf('/api/') !== -1) {
        setTimeout(function () {
            next();
        }, 1000);
    } else {
        next();
    }
}

function httpMethods(request, response, next) {

    var rawpath = request.url.split('?')[0],
        path = require('path').resolve(__dirname, 'demo/' + rawpath);

    if ((request.method === 'PUT' || request.method === 'POST')) {
        console.log('inside put/post');
        request.content = '';
        request.addListener("data", function (chunk) {
            request.content += chunk;
        });

        request.addListener("end", function () {
            console.log("request content: " + JSON.stringify(request.content));
            if (fs.existsSync(path)) {
                fs.writeFile(path, request.content, function (err) {
                    if (err) {
                        throw err;
                    }
                    console.log('file saved');
                    response.end('file was saved');
                });
                return;
            }

            if (request.url === '/log') {
                var filePath = 'server/log/server.log';
                var logData = JSON.parse(request.content);
                fs.appendFile(filePath, logData.logUrl + '\n' + logData.logMessage + '\n', function (err) {
                    if (err) {
                        throw err;
                    }
                    console.log('log saved');
                    response.end('log was saved');
                });
                return;
            }
        });
        return;
    }
    next();
}


/**
 * Specify concat order to concant files from the same
 * module into a single module file
 *
 * @param  {string} moduleFolderPath
 * @param  {array} filesAfterModule Files to concat inmediately after the module
 * @return {array}                  List of files to concat
 */
function moduleFilesToConcat(moduleFolderPath, filesAfterModule) {

    //Remove trailing slash
    moduleFolderPath = moduleFolderPath.replace(/\/+$/, '');

    // Files using the same module are concatenated in the correct order:
    // · 1st, module.js files are loaded as these are the ones that create the module
    // · 2nd, provider.js files containing are loaded. This is because some modules use their own
    // providers in their config block. Because of this, providers must be loaded prior to config blocks.
    // · 3rd, rest of files
    var files = [moduleFolderPath + '/module.js'];

    if (typeof filesAfterModule === 'object') {
        files = files.concat(filesAfterModule);
    }

    return files.concat([
        moduleFolderPath + '/**/*.provider.js',
        moduleFolderPath + '/**/*.js'
    ]);
}

/**
 * Gets a list of all the files to load as scripts.
 *
 * @param  {object} filesObject Files object of files structured by module
 * @return {array}              Array of files
 */
function getAllFilesForDemo(filesObject) {
    var filesList = [];
    for (var key in filesObject) {
        if (filesObject.hasOwnProperty(key)) {
            filesList = filesList.concat(filesObject[key]);
        }
    }

    return filesList;
}
