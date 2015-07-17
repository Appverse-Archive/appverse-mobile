(function() {
    'use strict';

    /**
     * @ngdoc module
     * @name appverse.utils
     * @description Provides utility objects and functions
     *
     * @requires appverse.configuration
     */
    angular.module('appverse.utils', ['appverse.configuration']);

})();
(function (angular) {
    'use strict';

    angular.module('appverse.utils')
        .provider('BaseUrlSetter', BaseUrlSetterProvider);

    /**
     * @ngdoc provider
     * @name BaseUrlSetter
     * @module appverse.utils
     * @description
     * Preprends a url with a base path
     */
    function BaseUrlSetterProvider() {
        this.$get = function () {
            return this;
        };

        /**
         * @ngdoc method
         * @name BaseUrlSetter#setBasePath
         * @param {string} basePath The base path to prepend
         */
        this.setBasePath = function (basePath) {
            return new BaseUrlSetter(basePath);
        };
    }


    function BaseUrlSetter(basePath) {

        basePath = basePath || '';

        basePath = basePath.trim(basePath);

        this.$get = function () {
            return this;
        };

        this.inUrl = function (url) {
            url = url.trim(url);
            if (endsWithSlash(basePath)) {
                basePath = sliceLastChar(basePath);
            }
            if (startsWithSlash(url)) {
                url = sliceFirstChar(url);
            }
            return basePath + '/' + url;
        };

        function endsWithSlash(path) {
            return (path.slice(-1) === '/');
        }

        function startsWithSlash(path) {
            return (path.slice(0, 1) === '/');
        }

        function sliceLastChar(path) {
            return path.slice(0, -1);
        }

        function sliceFirstChar(path) {
            return path.slice(1);
        }
    }





})(angular);

(function(angular) {
    'use strict';

    angular.module('appverse.utils').provider('ModuleSeeker', ModuleSeeker);

    /**
     * @ngdoc service
     * @name ModuleSeeker
     * @module appverse.utils
     * @description Looks for modules
     */
    function ModuleSeeker() {
        this.$get = function() {
            return this;
        };
    }

    /**
     * $ngdoc function
     * @description Checks if the module exists
     * @param  {string} name Name of the module
     * @return {Boolean}
     */
    ModuleSeeker.prototype.exists = function(name) {
        try {
            angular.module(name);
            return true;
        } catch (e) {
            return false;
        }
    };

})(angular);
/*jslint bitwise: true */
(function() {
    'use strict';

    angular.module('appverse.utils')

    /**
     * @ngdoc service
     * @name BaseUrlSetter
     * @module appverse.utils
     * @description Base64 encoding
     */
    .factory('Base64', function () {
        var keyStr = 'ABCDEFGHIJKLMNOP' +
            'QRSTUVWXYZabcdef' +
            'ghijklmnopqrstuv' +
            'wxyz0123456789+/' +
            '=';
        return {
            encode: function (input) {
                var output = "";
                var chr1, chr2, chr3 = "";
                var enc1, enc2, enc3, enc4 = "";
                var i = 0;

                do {
                    chr1 = input.charCodeAt(i++);
                    chr2 = input.charCodeAt(i++);
                    chr3 = input.charCodeAt(i++);

                    enc1 = chr1 >> 2;
                    enc2 = ((chr1 & 3) << 4) | (chr2 >> 4);
                    enc3 = ((chr2 & 15) << 2) | (chr3 >> 6);
                    enc4 = chr3 & 63;

                    if (isNaN(chr2)) {
                        enc3 = enc4 = 64;
                    } else if (isNaN(chr3)) {
                        enc4 = 64;
                    }

                    output = output +
                        keyStr.charAt(enc1) +
                        keyStr.charAt(enc2) +
                        keyStr.charAt(enc3) +
                        keyStr.charAt(enc4);
                    chr1 = chr2 = chr3 = "";
                    enc1 = enc2 = enc3 = enc4 = "";
                } while (i < input.length);

                return output;
            },

            decode: function (input) {
                var output = "";
                var chr1, chr2, chr3 = "";
                var enc1, enc2, enc3, enc4 = "";
                var i = 0;

                // remove all characters that are not A-Z, a-z, 0-9, +, /, or =
                var base64test = /[^A-Za-z0-9\+\/\=]/g;
                if (base64test.exec(input)) {
                    alert("There were invalid base64 characters in the input text.\n" +
                        "Valid base64 characters are A-Z, a-z, 0-9, '+', '/',and '='\n" +
                        "Expect errors in decoding.");
                }
                input = input.replace(/[^A-Za-z0-9\+\/\=]/g, "");

                do {
                    enc1 = keyStr.indexOf(input.charAt(i++));
                    enc2 = keyStr.indexOf(input.charAt(i++));
                    enc3 = keyStr.indexOf(input.charAt(i++));
                    enc4 = keyStr.indexOf(input.charAt(i++));

                    chr1 = (enc1 << 2) | (enc2 >> 4);
                    chr2 = ((enc2 & 15) << 4) | (enc3 >> 2);
                    chr3 = ((enc3 & 3) << 6) | enc4;

                    output = output + String.fromCharCode(chr1);

                    if (enc3 != 64) {
                        output = output + String.fromCharCode(chr2);
                    }
                    if (enc4 != 64) {
                        output = output + String.fromCharCode(chr3);
                    }

                    chr1 = chr2 = chr3 = "";
                    enc1 = enc2 = enc3 = enc4 = "";

                } while (i < input.length);

                return output;
            }
        };
    });


})();
(function() {
    'use strict';

    angular.module('appverse.utils')

    /**
     * @ngdoc service
     * @name UtilFactory
     * @module appverse.utils
     * @description This factory provides common utilities for API functionalities.
     */
    .factory('UtilFactory', function () {
            var factory = {};

            /**
             * @ngdoc method
             * @name UtilFactory#findPropertyValueByName
             * @description Deletes an item from a list.
             *
             * @param properties content of the static external properties file
             * @param area group of properties
             * @param property property to know the value in
             */
            factory.findPropertyValueByName = function (properties, area, property) {
                for (var i = 0; i < properties.length; i++) {
                    if (properties[i].area == area) {
                        for (var p = 0; p < properties[i].properties.length; p++) {
                            if (properties[i].properties[p].property == property) {
                                return properties[i].properties[p].value;
                            }
                        }
                    }
                }
                return null;
            };

            /**
             * @ngdoc method
             * @name UtilFactory#newRandomKey
             * @description ...
             *
             * @param coll
             * @param key
             * @param currentKey
             */
            factory.newRandomKey = function (coll, key, currentKey) {
                var randKey;
                do {
                    randKey = coll[Math.floor(coll.length * Math.random())][key];
                } while (randKey === currentKey);
                return randKey;
            };

            return factory;
        });

})();