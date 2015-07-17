'use strict';
var _ = require('lodash');
var path = require('path');

module.exports = function (grunt) {
    /** Alias tasks are at the final **/
    var compare = function (a, b) {
        if (a.info < b.info)
            return 1;
        if (a.info > b.info)
            return -1;
        return 0;
    };


    var colLen = 0;
    var setColLen = function (str) {
        colLen = Math.max(colLen, str.length);
    };
    /** Get onlu name and info from the tasks **/
    var parseTask = function (name, infoOverride) {
        var task = grunt.task._tasks[name];
        var info = infoOverride || task.info;

        if (!task) {
            grunt.log.error('Missing ' + name + ' task');
            return;
        }

        setColLen(task.name);
        if (task.multi) {
            info += ' (Multitask)';
        }
        return {
            name: task.name,
            info: info
        };
    };

    /** Get all the grunt tasks information **/
    var getAllTasks = function () {
        var tasks = [];
        Object.keys(grunt.task._tasks).forEach(function (name) {
            tasks.push(parseTask(name));
        });
        return tasks;
    };

    /** Get all the tasks - order the array to get 'alias' at the end, write the readem.md file */
    grunt.registerTask('list', 'List all the available grunt tasks and write them to a file.', function () {
        var tasks = getAllTasks();
        tasks.sort(compare);
        // Merge task-specific and/or target-specific options with these defaults.
        var gruntOptions = grunt.config('list');

        var options = {
            // If an encoding is not specified, default to grunt.file.defaultEncoding.
            // If specified as null, returns a non-decoded Buffer instead of a string.
            encoding: grunt.file.defaultEncoding
        };
        var filepath = path.join(__dirname, gruntOptions.file.output);
        if (!grunt.file.exists(filepath)) {
            return false;
        }
        var file = grunt.file.read(filepath, options);
        var title = 'Grunt tasks list';
        var pos = file.lastIndexOf(title);
        var output;
        if (pos > 0) {
            output = file.substring(0, pos);
        } else {
            output = file;
        }
        output += ('\n Grunt tasks list \n');
        output += ('---------------- \n');
        tasks.forEach(function (task) {
            if (!_.isUndefined(task)) {
                if (!_.isUndefined(task.name)) {
                    output += '###' + task.name + "\n";
                    if (task.info.length > 0) {
                        output += task.info + "\n";
                    } else {
                        output += 'No info \n';
                    }
                }
            }
        });
        grunt.file.write(filepath, output);

    });


}
