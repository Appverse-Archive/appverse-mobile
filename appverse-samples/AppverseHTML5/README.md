rss
============

Appverse Web 
============

This is the repository of the Appverse Web HTML5 Bolierplate subproject. It provides an starting point for an Appverse Web HTML5 project
Appverse Web is a multi-frontend technology capable web application framework incorporating the best-of-breed open source stack to meet the challenges of secure and robust server-side service development, integration and delivery.

## More Information

* **About this project**: <http://appverse.github.com/appverse-web-html5-boilerplate>
* **About Appverse Web HTML5 Module**: <http://appverse.github.com/appverse-web-html5-core>
* **About licenses & groups**: <http://appverse.github.com>
* **About The Appverse Project**: <http://appverse.org>

### Quick Start

#### Before you start, tools you will need

* install npm
* bower and grunt (run the following commands):

```
npm install -g bower
```

```
npm install -g grunt
```

## Running

* configure project:

```
npm install
```

```
bower install
```

* run project

Run the application. 

`grunt server`

Run the appliction and open the browser.

`grunt server:open` 

## Testing

Exexecute the following command to launch tests

`grunt test`
 
 <!-- Available Grunt task (generated running 'grunt list') -->
 Grunt tasks list 
---------------- 
###karma
run karma. (Multitask)
###jshint
Validate files with JSHint. (Multitask)
###useminPrepare
Using HTML markup as the primary source of information (Multitask)
###connect
Start a connect web server. (Multitask)
###watch
Run predefined tasks whenever watched files change.
###concurrent
Run grunt tasks concurrently (Multitask)
###usemin
Replaces references to non-minified scripts / stylesheets (Multitask)
###rev
Prefix static asset file names with a content hash (Multitask)
###autoprefixer
Prefix CSS files. (Multitask)
###open
Open urls and files from a grunt task (Multitask)
###uglify
Minify files with UglifyJS. (Multitask)
###htmlmin
Minify HTML (Multitask)
###cssmin
Minify CSS (Multitask)
###list
List all the available grunt tasks and write them to a file.
###license
Generate an HTML report of all NPM modules licenses.
###copy
Copy files. (Multitask)
###concat
Concatenate files. (Multitask)
###sass
Compile Sass to CSS (Multitask)
###coffee
Compile CoffeeScript files into JavaScript (Multitask)
###clean
Clean files and folders. (Multitask)
###test:e2e
Alias for "testserver", "karma:e2e_auto" tasks.
###default
Alias for "server" task.
###test:unit
Alias for "karma:unit_auto" task.
###test
Alias for "karma:unit", "testserver", "karma:e2e" tasks.
###server:dist
Alias for "dist", "connect:dist", "watch" tasks.
###server:dist:open
Alias for "dist", "connect:dist", "open:dist", "watch" tasks.
###server:doc
Alias for "connect:doc", "open:doc", "watch:doc" tasks.
###testserver
Alias for "clean:server", "concurrent:server", "autoprefixer", "connect:test" tasks.
###server
Alias for "clean:server", "concurrent:server", "autoprefixer", "connect:livereload", "watch" tasks.
###server:open
Alias for "clean:server", "concurrent:server", "autoprefixer", "connect:livereload", "open:server", "watch" tasks.
###doc
Alias for "clean:doc", "docular" tasks.
###dist
Alias for "clean:dist", "useminPrepare", "concurrent:dist", "autoprefixer", "concat", "copy:dist", "ngAnnotate", "cssmin", "uglify", "rev", "usemin", "htmlmin" tasks.
###ngAnnotate
Add, remove and rebuild AngularJS dependency injection annotations (Multitask)
###grunt-license-report
No info 
