(function() { 'use strict';

/**
 * @ngdoc module
 * @name appverse.detection
 *
 * @description
 * Provides browser and network detection.
 *
 * @requires appverse.utils
 */
angular.module('appverse.detection', ['appverse.utils']);


})();
(function() {
    'use strict';

    angular.module('appverse.detection')
        .provider('MobileDetector', MobileDetectorProvider);

    /**
     * @ngdoc provider
     * @name MobileDetector
     * @module appverse.detection
     *
     * @description
     * Detects if the browser is mobile
     */
    function MobileDetectorProvider() {

        this.$get = function () {
            return this;
        };

        /**
         * @ngdoc method
         * @name MobileDetector#hasAppverseMobile
         * @return {Boolean}
         */
        this.hasAppverseMobile = function() {
            return hasUnity() && unityHasOSInfo();
        };

        /**
         * @ngdoc method
         * @name MobileDetector#isMobileBrowser
         * @return {Boolean}
         */
        this.isMobileBrowser = function (customAgent) {
            var agent = customAgent || navigator.userAgent || navigator.vendor || window.opera;
            return agentContainsMobileKeyword(agent);
        };

        function hasUnity () {
            return typeof Unity !== 'undefined';
        }

        function unityHasOSInfo () {
            return Unity.System.GetOSInfo() !== null;
        }

        function agentContainsMobileKeyword(agent) {

            /*jshint ignore:start,-W101*/
            // Code adapted from http://detectmobilebrowser.com
            return /(android|bb\d+|meego).+mobile|avantgo|bada\/|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od)|iris|kindle|lge |maemo|midp|mmp|mobile.+firefox|netfront|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\/|plucker|pocket|psp|series(4|6)0|symbian|treo|up\.(browser|link)|vodafone|wap|windows (ce|phone)|xda|xiino|android|ipad|playbook|silk/i.test(agent) || /1207|6310|6590|3gso|4thp|50[1-6]i|770s|802s|a wa|abac|ac(er|oo|s\-)|ai(ko|rn)|al(av|ca|co)|amoi|an(ex|ny|yw)|aptu|ar(ch|go)|as(te|us)|attw|au(di|\-m|r |s )|avan|be(ck|ll|nq)|bi(lb|rd)|bl(ac|az)|br(e|v)w|bumb|bw\-(n|u)|c55\/|capi|ccwa|cdm\-|cell|chtm|cldc|cmd\-|co(mp|nd)|craw|da(it|ll|ng)|dbte|dc\-s|devi|dica|dmob|do(c|p)o|ds(12|\-d)|el(49|ai)|em(l2|ul)|er(ic|k0)|esl8|ez([4-7]0|os|wa|ze)|fetc|fly(\-|_)|g1 u|g560|gene|gf\-5|g\-mo|go(\.w|od)|gr(ad|un)|haie|hcit|hd\-(m|p|t)|hei\-|hi(pt|ta)|hp( i|ip)|hs\-c|ht(c(\-| |_|a|g|p|s|t)|tp)|hu(aw|tc)|i\-(20|go|ma)|i230|iac( |\-|\/)|ibro|idea|ig01|ikom|im1k|inno|ipaq|iris|ja(t|v)a|jbro|jemu|jigs|kddi|keji|kgt( |\/)|klon|kpt |kwc\-|kyo(c|k)|le(no|xi)|lg( g|\/(k|l|u)|50|54|\-[a-w])|libw|lynx|m1\-w|m3ga|m50\/|ma(te|ui|xo)|mc(01|21|ca)|m\-cr|me(rc|ri)|mi(o8|oa|ts)|mmef|mo(01|02|bi|de|do|t(\-| |o|v)|zz)|mt(50|p1|v )|mwbp|mywa|n10[0-2]|n20[2-3]|n30(0|2)|n50(0|2|5)|n7(0(0|1)|10)|ne((c|m)\-|on|tf|wf|wg|wt)|nok(6|i)|nzph|o2im|op(ti|wv)|oran|owg1|p800|pan(a|d|t)|pdxg|pg(13|\-([1-8]|c))|phil|pire|pl(ay|uc)|pn\-2|po(ck|rt|se)|prox|psio|pt\-g|qa\-a|qc(07|12|21|32|60|\-[2-7]|i\-)|qtek|r380|r600|raks|rim9|ro(ve|zo)|s55\/|sa(ge|ma|mm|ms|ny|va)|sc(01|h\-|oo|p\-)|sdk\/|se(c(\-|0|1)|47|mc|nd|ri)|sgh\-|shar|sie(\-|m)|sk\-0|sl(45|id)|sm(al|ar|b3|it|t5)|so(ft|ny)|sp(01|h\-|v\-|v )|sy(01|mb)|t2(18|50)|t6(00|10|18)|ta(gt|lk)|tcl\-|tdg\-|tel(i|m)|tim\-|t\-mo|to(pl|sh)|ts(70|m\-|m3|m5)|tx\-9|up(\.b|g1|si)|utst|v400|v750|veri|vi(rg|te)|vk(40|5[0-3]|\-v)|vm40|voda|vulc|vx(52|53|60|61|70|80|81|83|85|98)|w3c(\-| )|webc|whit|wi(g |nc|nw)|wmlb|wonu|x700|yas\-|your|zeto|zte\-/i.test(agent.substr(0, 4));
            /*jshint ignore:end,-W101*/
        }
    }

})();
(function () {
    'use strict';

angular.module('appverse.detection')
    .provider('Detection', DetectionProvider);

/**
 * @ngdoc provider
 * @name Detection
 * @module appverse.detection
 *
 * @description
 * Contains methods for browser and network detection.
 *
 * @requires  MobileDetectorProvider
 */
function DetectionProvider (MobileDetectorProvider) {

    this.mobileDetector        = MobileDetectorProvider;
    this.bandwidth             = 0;
    this.isPollingBandwidth    = false;
    // Injected when the detection service is created
    this.$http                 = undefined;

    // Get the service
    this.$get = ["$http", function ($http) {
        this.$http = $http;
        return this;
    }];

    /**
     * @ngdoc method
     * @name  AppDetection#hasAppverseMobile
     * @return {Boolean} Whether the application has Appverse mobile or not
     */
    this.hasAppverseMobile = function() {
        return this.mobileDetector.hasAppverseMobile();
    };

    /**
     * @ngdoc method
     * @name  AppDetection#isMobileBrowser
     * @return {Boolean} Whether the application is running on a mobile browser
     */
    this.isMobileBrowser = function() {
        return this.mobileDetector.isMobileBrowser();
    };

    // Do some initialization
    if (this.hasAppverseMobile() || this.isMobileBrowser()) {
        // Do something for mobile...
    }

    var fireEvent = function (name, data) {
        var e = document.createEvent("Event");
        e.initEvent(name, true, true);
        e.data = data;
        window.dispatchEvent(e);
    };

    var fetch = function (url, callback) {
        var xhr = new XMLHttpRequest();

        var noResponseTimer = setTimeout(function () {
            xhr.abort();
            fireEvent("connectiontimeout", {});
        }, 5000);

        xhr.onreadystatechange = function () {
            if (xhr.readyState !== 4) {
                return;
            }

            if (xhr.status === 200) {
                fireEvent("goodconnection", {});
                clearTimeout(noResponseTimer);
                if (callback) {
                    callback(xhr.responseText);
                }
            } else {
                fireEvent("connectionerror", {});
            }
        };
        xhr.open("GET", url);
        xhr.send();
    };

    this.isOnline = window.navigator.onLine;
    this.isPollingOnlineStatus = false;

    /**
     * @ngdoc method
     * @name Detection#testOnlineStatus
     *
     * @param {String} path The item URL
     * @description Tries to fetch a file on the server and fire events for fail and success.
     */
    this.testOnlineStatus = function () {
        fetch("resources/detection/ping.json");
    };

    /**
     * @ngdoc method
     * @name Detection#startPollingOnlineStatus
     *
     * @param {number} interval Time in milliseconds
     * @description Tries to fetch a file on the server at regular intervals and fire events for fail and success.
     */
    this.startPollingOnlineStatus = function (interval) {
        this.isPollingOnlineStatus = setInterval(this.testOnlineStatus, interval);
    };

    /**
     * @ngdoc method
     * @name Detection#stopPollingOnlineStatus
     *
     * @description Stops fetching the file from the server.
     */
    this.stopPollingOnlineStatus = function () {
        clearInterval(this.isPollingOnlineStatus);
        this.isPollingOnlineStatus = false;
    };

    /**
     * @ngdoc method
     * @name Detection#testBandwidth
     */
    this.testBandwidth = function () {
        var jsonUrl = "resources/detection/bandwidth.json?bust=" +  (new Date()).getTime();
        fireEvent("onBandwidthStart");
        this.$http.get(jsonUrl).success(function(data, status, headersFn) {
                fireEvent("onBandwidthEnd", {
                    status: status,
                    data: data,
                    getResponseHeader: headersFn
        });
            });
    };

    /**
     * @ngdoc method
     * @name Detection#startPollingBandwidth
     *
     * @param {number} interval Time in milliseconds
     */
    this.startPollingBandwidth = function (interval) {
        this.testBandwidth();
        this.isPollingBandwidth = setInterval(this.testBandwidth.bind(this), interval);
    };

    /**
     * @ngdoc method
     * @name Detection#stopPollingBandwidth
     *
     * @param {number} interval Time in milliseconds
     */
    this.stopPollingBandwidth = function () {
        clearInterval(this.isPollingBandwidth);
        this.isPollingBandwidth = false;
    };
}
DetectionProvider.$inject = ["MobileDetectorProvider"];


})();

(function() { 'use strict';

angular.module('appverse.detection')
    .run(run);

function run($log, Detection, $rootScope, $window) {
    $log.info('appverse.detection run');

    if ($window.addEventListener) {
        $window.addEventListener("online", function () {
            $log.debug('detectionController online');
            Detection.isOnline = true;
            $rootScope.$digest();
        }, true);

        $window.addEventListener("offline", function () {
            $log.debug('detectionController offline');
            Detection.isOnline = false;
            $rootScope.$digest();
        }, true);
    } else {
        $log.warn('Detection module: $window.addEventListener not supported.');
    }

    if ($window.applicationCache) {
        $window.applicationCache.addEventListener("error", function () {
            $log.debug("Error fetching manifest: a good chance we are offline");
        });
    } else {
        $log.warn('Detection module: $window.applicationCache not supported.');
    }

    if (window.addEventListener) {
        window.addEventListener("goodconnection", function () {
            $log.debug('detectionController goodconnection');
            Detection.isOnline = true;
            $rootScope.$digest();
        });

        window.addEventListener("connectiontimeout", function () {
            $log.debug('detectionController connectiontimeout');
            Detection.isOnline = false;
            $rootScope.$digest();
        });

        window.addEventListener("connectionerror", function () {
            $log.debug('detectionController connectionerror');
            Detection.isOnline = false;
            $rootScope.$digest();
        });

        window.addEventListener("onBandwidthStart", function () {
            $log.debug('detectionController onBandwidthStart');
            Detection.bandwidthStartTime = new Date();
        });

        window.addEventListener("onBandwidthEnd", function (e) {
            $log.debug('detectionController onBandwidthEnd');
            var contentLength = parseInt(e.data.getResponseHeader('Content-Length'), 10);
            var delay = new Date() - Detection.bandwidthStartTime;
            Detection.bandwidth = parseInt((contentLength / 1024) / (delay / 1000));
            setTimeout(function () {
                $rootScope.$digest();
            });
        });
    } else {
        $log.warn('Detection module: window.addEventListener not supported.');
    }
}
run.$inject = ["$log", "Detection", "$rootScope", "$window"];

})();