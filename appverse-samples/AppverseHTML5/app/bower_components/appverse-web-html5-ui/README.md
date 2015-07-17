appverse-web-html5-ui
=====================
Appverse Web Client Side ui module based on HTML5 and Java Script

## More Information

* **About this project**: <http://appverse.github.com/appverse-web>
* **About licenses & groups**: <http://appverse.github.com>
* **About The Appverse Project**: <http://appverse.org>

###Download

[GitHub page](https://github.com/Appverse/appverse-web-html5-core)
  or
`bower install appverse-web-html5-ui`
##Running (For using Appverse in your Angular app)

####Before you start, tools you will need
* Download and install [git](http://git-scm.com/downloads)
* Download and install [nodeJS](http://nodejs.org/download/)
* Install bower `npm install -g bower`

####Inside of your app:
* Run `bower install appverse-web-html5-ui -S`
* Add the following to your index.html
```html

  <!-- ########## API modules ########## -->

    <!-- Cache module -->
    <script src="bower_components/angular-cache/dist/angular-cache.min.js"></script>
    <script src="bower_components/appverse-web-html5-core/src/modules/api-cache.js"></script>

    <script src="bower_components/appverse-web-html5-core/src/modules/api-configuration.js"></script>
    <script src="bower_components/appverse-web-html5-core/src/modules/api-detection.js"></script>
    <script src="bower_components/appverse-web-html5-core/src/modules/api-logging.js"></script>
    <script src="bower_components/appverse-web-html5-core/src/modules/api-main.js"></script>

    <!-- REST module -->
    <script src="bower_components/lodash/dist/lodash.underscore.min.js"></script>
    <script src="bower_components/restangular/dist/restangular.min.js"></script>
    <script src="bower_components/appverse-web-html5-core/src/modules/api-rest.js"></script>


    <!-- Server Push module -->
    <script src="bower_components/appverse-web-html5-core/src/modules/api-serverpush.js"></script>

    <!-- Translate module -->
    <script src="bower_components/appverse-web-html5-core/src/modules/api-translate.js"></script>
    <script src="bower_components/angular-translate/angular-translate.min.js"></script>
    <script src="bower_components/angular-translate-loader-static-files/angular-translate-loader-static-files.min.js"></script>
    <script src="bower_components/angular-dynamic-locale/src/tmhDynamicLocale.js"></script>

    <script src="bower_components/appverse-web-html5-core/src/modules/api-utils.js"></script>

    <!-- Directives should be included after the modules -->
    <script src="bower_components/appverse-web-html5-core/src/directives/cache-directives.js"></script>
    <script src="bower_components/appverse-web-html5-core/src/directives/rest-directives.js"></script>
    <script src="bower_components/appverse-web-html5-core/src/modules/api-performance.js"></script>
    <!-- endbuild -->    
```

* Add the `appverse-web-html5-core` module to your Angular module list (e.g. in a main app.js file: `angular.module('yourMainModule',['COMMONAPI'])`)


## License

    Copyright (c) 2012 GFT Appverse, S.L., Sociedad Unipersonal.

     This Source  Code Form  is subject to the  terms of  the Appverse Public License 
     Version 2.0  ("APL v2.0").  If a copy of  the APL  was not  distributed with this 
     file, You can obtain one at <http://appverse.org/legal/appverse-license/>.

     Redistribution and use in  source and binary forms, with or without modification, 
     are permitted provided that the  conditions  of the  AppVerse Public License v2.0 
     are met.

     THIS SOFTWARE IS PROVIDED BY THE  COPYRIGHT HOLDERS  AND CONTRIBUTORS "AS IS" AND
     ANY EXPRESS  OR IMPLIED WARRANTIES, INCLUDING, BUT  NOT LIMITED TO,   THE IMPLIED
     WARRANTIES   OF  MERCHANTABILITY   AND   FITNESS   FOR A PARTICULAR  PURPOSE  ARE
     DISCLAIMED. EXCEPT IN CASE OF WILLFUL MISCONDUCT OR GROSS NEGLIGENCE, IN NO EVENT
     SHALL THE  COPYRIGHT OWNER  OR  CONTRIBUTORS  BE LIABLE FOR ANY DIRECT, INDIRECT,
     INCIDENTAL,  SPECIAL,   EXEMPLARY,  OR CONSEQUENTIAL DAMAGES  (INCLUDING, BUT NOT
     LIMITED TO,  PROCUREMENT OF SUBSTITUTE  GOODS OR SERVICES;  LOSS OF USE, DATA, OR
     PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY,
     WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT(INCLUDING NEGLIGENCE OR OTHERWISE) 
     ARISING  IN  ANY WAY OUT  OF THE USE  OF THIS  SOFTWARE,  EVEN  IF ADVISED OF THE 
     POSSIBILITY OF SUCH DAMAGE.
