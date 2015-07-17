'use strict';

module.exports = function (grunt) {

    grunt.registerTask('mockserver', 'Run mock server.', function (arg1) {
        grunt.log.writeln('Running Mock Server ');
        var jsonServer = require('json-server');
        var fs = require("fs");
        var path = require("path");

        grunt.log.writeln('Reading MOCK - JSON directory. Loading al the JSON files to the MockServer database.');

        var apiFolder = path.join(__dirname, "../api/");
        if (!fs.existsSync(apiFolder)) {
            fs.mkdirSync(apiFolder);
        }
        var db = {};
        fs.readdir(apiFolder, function (err, files) {

            files.forEach(function (file) {
                if (path.extname(apiFolder + file) === '.json') {
                    db[path.basename(apiFolder + file, '.json')] = require(apiFolder + file);
                }
            });
        });
        var router = jsonServer.router(db); // Express router
        var server = jsonServer.create(); // Express server

        server.use(router);
        server.listen( 8888 );
        grunt.task.run('configureProxies:mocklivereload');
        grunt.task.run('clean:server');
        grunt.task.run('concurrent:server');
        grunt.task.run('autoprefixer');
        grunt.task.run('connect:livereload');
        if (arguments.length > 0 && arg1 === 'open') {
            grunt.task.run('open:server');
        }
        grunt.task.run('watch');
    });
};
