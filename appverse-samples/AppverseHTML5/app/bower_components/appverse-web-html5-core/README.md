appverse-web-html5-core
=======================
Appverse Web Client Side  core module based on HTML5 and Java Script

## More Information

* **About this project**: <http://appverse.github.com/appverse-web>
* **About licenses & groups**: <http://appverse.github.com>
* **About The Appverse Project**: <http://appverse.org>

###Download

[GitHub page](https://github.com/Appverse/appverse-web-html5-core)
[GitHub boilerplate page](https://github.com/Appverse/appverse-web-html5-boilerplate)
  or
`bower install appverse-web-html5-core`

##Running (For using Appverse in your Angular app)

####Before you start, tools you will need
* Download and install [git](http://git-scm.com/downloads)
* Download and install [nodeJS](http://nodejs.org/download/)
* Install bower `npm install -g bower`

####Inside of your app:
* Run `bower install appverse-web-html5-core -S`
* Add the following to your index.html
```html
<!-- build:js scripts/scripts.js -->
    <script src="bower_components/jquery/dist/jquery.min.js"></script>
    <script src="bower_components/angular/angular.js"></script>
    <script src="bower_components/angular-touch/angular-touch.min.js"></script>
    <script src="bower_components/modernizr/modernizr.js"></script>
    <script src="bower_components/jquery-flot/jquery.flot.js"></script>
    <script src="bower_components/jquery-flot/jquery.flot.resize.js"></script>
    <script src="bower_components/jquery-flot/jquery.flot.time.js"></script>

    <!-- uibootstrap -->
    <script src="bower_components/bootstrap-sass-official/vendor/assets/javascripts/bootstrap/transition.js"></script>
    <script src="bower_components/bootstrap-sass-official/vendor/assets/javascripts/bootstrap/collapse.js"></script>
    <script src="bower_components/angular-bootstrap/ui-bootstrap-tpls.min.js"></script>

    <!-- modules -->
    <script src="bower_components/angular-cookies/angular-cookies.min.js"></script>
    <script src="bower_components/angular-sanitize/angular-sanitize.min.js"></script>
    <script src="bower_components/angular-ui-router/release/angular-ui-router.min.js"></script>
    <script src="bower_components/angular-cache/dist/angular-cache.min.js"></script>
    <script src="bower_components/ng-grid/build/ng-grid.debug.js"></script>

    <!-- UI components -->
    <script src="bower_components/venturocket-angular-slider/build/angular-slider.min.js"></script>
    <script src="bower_components/qrcode/lib/qrcode.min.js"></script>
    <script src="bower_components/angular-qr/angular-qr.min.js"></script>
    <script src="bower_components/angular-xeditable/dist/js/xeditable.js"></script>
  <!-- ########## API modules ########## -->

    <!-- Cache module -->
    <script src="bower_components/angular-cache/dist/angular-cache.min.js"></script>
    <script src="bower_components/appverse-web-html5-core/src/modules/api-cache.js"></script>

    <script src="bower_components/appverse-web-html5-core/src/modules/api-configuration.js"></script>
    <script src="bower_components/appverse-web-html5-core/src/modules/api-detection.js"></script>
    <script src="bower_components/appverse-web-html5-core/src/modules/api-logging.js"></script>
    <script src="bower_components/appverse-web-html5-core/src/modules/api-main.js"></script>

    <!-- REST module -->
    <script src="bower_components/lodash/lodash.min.js"></script>
    <script src="bower_components/restangular/dist/restangular.min.js"></script>
    <script src="bower_components/appverse-web-html5-core/src/modules/api-rest.js"></script>


    <!-- Server Push module -->
    <script src="bower_components/sockjs-client/dist/sockjs.js"></script>
    <script src="bower_components/stomp-websocket/lib/stomp.js"></script>
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


    <!-- your scripts here-->

    <!-- endbuild -->
```


* Add the `appverse-web-html5-core` module to your Angular module list (e.g. in a main app.js file: `angular.module('yourMainModule',['COMMONAPI'])`)

* On your main.scss file add the following line on the begining of the dile

```scss
@import '../bower_components/bootstrap-sass-official/vendor/assets/stylesheets/bootstrap';
```



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
