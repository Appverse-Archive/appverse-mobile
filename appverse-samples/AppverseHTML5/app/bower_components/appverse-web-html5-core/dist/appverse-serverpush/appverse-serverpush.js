(function() {
    'use strict';

    /**
    * @ngdoc module
    * @name appverse.serverPush
    * @description
    * This module handles server data communication when it pushes them to the client
    * exposing the factory SocketFactory, which is an API for instantiating sockets
    * that are integrated with Angular's digest cycle.
    * It is now based on SocketIO (http://socket.io/). Why?
    *
    * Using WebSockets is a modern, bidirectional protocol that enables an interactive communication
    * session between the browser and a server. Its main current drawback is
    * that implementation is generally only available in the latest browsers. However, by
    * using Socket.IO, this low level detail is abstracted away and we, as programmers,
    * are relieved of the need to write browser-specific code.
    *
    * The current release of socket.io is 0.9.10.
    *
    * The module appverse.serverPush is included in the main module.
    *
    * The private module appverse.socket.io simply wraps SocketIO API to be used by appverse.serverPush.
    *
    * So, appverse.serverPush is ready to integrate other Server Push approaches (e.g. Atmosphere) only by including
    * a new module and injecting it to appverse.serverPush.
    *
    *
    * NOTE ABOUT CLIENT DEPENDENCIES WITH SOCKET.IO
    *
    * The Socket.IO server will handle serving the correct version of the Socket.IO client library;
    *
    * We should not be using one from elsewhere on the Internet. From the top example on http://socket.io/:
    *
    *  <script src="/socket.io/socket.io.js"></script>
    *
    * This works because we wrap our HTTP server in Socket.IO (see the example at How To Use) and it intercepts
    * requests for /socket.io/socket.io.js and sends the appropriate response automatically.
    *
    * That is the reason it is not a dependency handled by bower.
    *
    * @requires  appverse.socket.io
    * @requires  appverse.configuration
    */
    angular.module('appverse.serverPush', ['appverse.socket.io', 'appverse.configuration'])
    /*
         To make socket error events available across an app, in one of the controllers:

         controller('MyCtrl', function ($scope) {
             $scope.on('socket:error', function (ev, data) {
                ...
         });
         */
    .run(['$log',
        function ($log) {
            $log.info('appverse.serverPush run');
            //socket.forward('error');
        }]);

})();
(function() {
    'use strict';

    /**
     * @ngdoc module
     * @name appverse.socket.io
     * @description
     * Private module implementing SocketIO. It provides the common API module appverse.serverPush
     * with the socket object wrapping the SocketIO client. This is initializated according
     * to the pre-existing external configuration.
     *
     * @requires  appverse.configuration
     */
    angular.module('appverse.socket.io', ['appverse.configuration']);

})();
(function() {
    'use strict';

    angular.module('appverse.socket.io')

    /**
     * @ngdoc provider
     * @name Socket
     * @module appverse.socket.io
     * @description
     * This provider provides the appverse.serverPush module with the SocketIO
     * client object from pre-existing configuration in application.
     *
     * This object helps the common API module  making  easier to add/remove
     * listeners in a way that works with AngularJS's scope.
     *
     * socket.on / socket.addListener: Takes an event name and callback.
     * Works just like the method of the same name from Socket.IO.
     *
     * socket.removeListener: Takes an event name and callback.
     * Works just like the method of the same name from Socket.IO.
     *
     * socket.emit: sends a message to the server. Optionally takes a callback.
     * Works just like the method of the same name from Socket.IO.
     *
     * socket.forward: allows you to forward the events received by Socket.IO's socket to AngularJS's event system.
     * You can then listen to the event with $scope.$on.
     * By default, socket-forwarded events are namespaced with socket:.
     * The first argument is a string or array of strings listing the event names to be forwarded.
     * The second argument is optional, and is the scope on which the events are to be broadcast.
     * If an argument is not provided, it defaults to $rootScope.
     * As a reminder, broadcasted events are propagated down to descendant scopes.
     *
     * @requires SERVERPUSH_CONFIG
     */
     .provider('Socket', ['SERVERPUSH_CONFIG',
        function (SERVERPUSH_CONFIG) {

            // when forwarding events, prefix the event name
            var prefix = 'socket:',
                ioSocket;

            // expose to provider
            this.$get = ["$rootScope", "$timeout", function ($rootScope, $timeout) {
                /* global io */

                /*
                Initialization of the socket object by using params in configuration module.
                Please read below for configuration detals:
                * Client configuration: https://github.com/LearnBoost/Socket.IO/wiki/Configuring-Socket.IO#client
                * Server configuration: https://github.com/LearnBoost/Socket.IO/wiki/Configuring-Socket.IO#server
                */
                var socket = ioSocket || io.connect(
                    SERVERPUSH_CONFIG.BaseUrl, {
                        'resource': SERVERPUSH_CONFIG.Resource,
                        'connect timeout': SERVERPUSH_CONFIG.ConnectTimeout,
                        'try multiple transports': SERVERPUSH_CONFIG.TryMultipleTransports,
                        'reconnect': SERVERPUSH_CONFIG.Reconnect,
                        'reconnection delay': SERVERPUSH_CONFIG.ReconnectionDelay,
                        'reconnection limit': SERVERPUSH_CONFIG.ReconnectionLimit,
                        'max reconnection attempts': SERVERPUSH_CONFIG.MaxReconnectionAttempts,
                        'sync disconnect on unload': SERVERPUSH_CONFIG.SyncDisconnectOnUnload,
                        'auto connect': SERVERPUSH_CONFIG.AutoConnect,
                        'flash policy port': SERVERPUSH_CONFIG.FlashPolicyPort,
                        'force new connection': SERVERPUSH_CONFIG.ForceNewConnection
                    }
                );

                var asyncAngularify = function (callback) {
                    return function () {
                        var args = arguments;
                        $timeout(function () {
                            callback.apply(socket, args);
                        }, 0);
                    };
                };

                var addListener = function (eventName, callback) {
                    socket.on(eventName, asyncAngularify(callback));
                };

                var removeListener = function () {
                    socket.removeAllListeners();
                };


                var wrappedSocket = {
                    on: addListener,
                    addListener: addListener,
                    off: removeListener,

                    emit: function (eventName, data, callback) {
                        if (callback) {
                            socket.emit(eventName, data, asyncAngularify(callback));
                        } else {
                            socket.emit(eventName, data);
                        }
                    },

                    //                removeListener: function () {
                    //                    var args = arguments;
                    //                    return socket.removeListener.apply(socket, args);
                    //                },

                    forward: function (events, scope) {
                        if (events instanceof Array === false) {
                            events = [events];
                        }
                        if (!scope) {
                            scope = $rootScope;
                        }
                        angular.forEach(events, function (eventName) {
                            var prefixed = prefix + eventName;
                            var forwardEvent = asyncAngularify(function (data) {
                                scope.$broadcast(prefixed, data);
                            });
                            scope.$on('$destroy', function () {
                                socket.removeListener(eventName, forwardEvent);
                            });
                            socket.on(eventName, forwardEvent);
                        });
                    }
                };

                return wrappedSocket;
            }];

            this.prefix = function (newPrefix) {
                prefix = newPrefix;
            };

            this.ioSocket = function (socket) {
                ioSocket = socket;
            };
        }]);


})();
(function() {
    'use strict';

    angular.module('appverse.serverPush')

    /**
     * @ngdoc service
     * @name SocketFactory
     * @module appverse.serverPush
     * @description
     * Although Socket.IO exposes an io variable on the window, it's better to encapsulate it
     * into the AngularJS's Dependency Injection system.
     * So, we'll start by writing a factory to wrap the socket object returned by Socket.IO.
     * This will make easier to test the application's controllers.
     * Notice that the factory wrap each socket callback in $scope.$apply.
     * This tells AngularJS that it needs to check the state of the application and update
     * the templates if there was a change after running the callback passed to it by using dirty checking.
     * Internally, $http works in the same way. After some XHR returns, it calls $scope.$apply,
     * so that AngularJS can update its views accordingly.
     *
     * @requires https://docs.angularjs.org/api/ng/service/$rootScope $rootScope
     * @requires Socket
     */
    .factory('SocketFactory', ['$rootScope', 'Socket',
        function ($rootScope, Socket) {
        var factory = {};

        /**
             @ngdoc method
             @name SocketFactory#listen
             @param {string} eventName The name of the event/channel to be listened
             The communication is bound to rootScope.
             @param {object} callback The function to be passed as callback.
             @description Establishes a communication listening an event/channel from server.
             Use this method for background communication although the current scope is destyroyed.
             You should cancel communication manually or when the $rootScope object is destroyed.
             */
        factory.listen = function (eventName, callback) {
            Socket.on(eventName, function () {
                var args = arguments;
                $rootScope.$apply(function () {
                    callback.apply(Socket, args);
                });
            });
        };

        /**
             @ngdoc method
             @name SocketFactory#sendMessage
             @param {string} eventName The name of the event/channel to be sent to server
             @param {object} scope The scope object to be bound to the listening.
             The communication will be cancelled when the scope is destroyed.
             @param {object} callback The function to be passed as callback.
             @description Establishes a communication listening an event/channel from server.
             It is bound to a given $scope object.
             */
        factory.sendMessage = function (eventName, data, callback) {
            Socket.emit(eventName, data, function () {
                var args = arguments;
                $rootScope.$apply(function () {
                    if (callback) {
                        callback.apply(Socket, args);
                    }
                });
            });
        };

        /**
             @ngdoc method
             @name SocketFactory#unsubscribeCommunication
             @param {object} callback The function to be passed as callback.
             @description Cancels all communications to server.
             The communication will be cancelled without regarding other consideration.
             */
        factory.unsubscribeCommunication = function (callback) {
            Socket.off(callback());
        };


        return factory;

    }]);

})();
(function() {
    'use strict';

    angular.module('appverse.serverPush')

    /**
     * @ngdoc service
     * @name WebSocketService
     * @module appverse.serverPush
     *
     * @requires https://docs.angularjs.org/api/ngMock/service/$log $log
     * @requires WEBSOCKETS_CONFIG
     */
    .factory('WebSocketFactory', ['$log', 'WEBSOCKETS_CONFIG',
        function($log, WEBSOCKETS_CONFIG) {
            var factory = {};

            /**
                @ngdoc method
                @name WebSocketFactory#connect
                @param {string} itemId The id of the item
                @description Establishes a connection to a swebsocket endpoint.
            */
            factory.connect = function(url) {

                if(factory.ws) {
                    return;
                }

                var ws;
                if ('WebSocket' in window) {
                    ws = new WebSocket(url);
                } else if ('MozWebSocket' in window) {
                    ws = new window.MozWebSocket(url);
                }
                ws.onopen = function () {
                    if (ws !== null) {
                        ws.send('');
                        factory.callback(WEBSOCKETS_CONFIG.WS_CONNECTED);
                    } else {
                        factory.callback(WEBSOCKETS_CONFIG.WS_DISCONNECTED);
                     }
                };

                ws.onerror = function() {
                  factory.callback(WEBSOCKETS_CONFIG.WS_FAILED_CONNECTION);
                };

                ws.onmessage = function(message) {
                  factory.callback(message.data);
                };

                ws.onclose = function () {
                    if (ws != null) {
                        ws.close();
                        ws = null;
                    }
                };

                factory.ws = ws;
            };

            /**
                @ngdoc method
                @name WebSocketFactory#send
                @param {object} message Message payload in JSON format.
                @description Send a message to the ws server.
            */
            factory.send = function(message) {
              $log.debug('factory.ws: ' + factory.ws);
              factory.ws.send(message);
            };
            /**
                @ngdoc method
                @name WebSocketFactory#subscribe
                @param {object} callback .
                @description Retrieve the currentcallback of the endpoint connection.
            */
            factory.subscribe = function(callback) {
              factory.callback = callback;
            };

            /**
                @ngdoc method
                @name WebSocketFactory#disconnect
                @param {string} itemId The id of the item
                @description Close the WebSocket connection.
            */
            factory.disconnect = function() {
                factory.ws.close();
            };



             /**
                @ngdoc method
                @name WebSocketFactory#status
                @param {string} itemId The id of the item
                @description WebSocket connection status.
            */
            factory.status = function() {
                if (factory.ws == null || angular.isUndefined(factory.ws)){
                    return WebSocket.CLOSED;
                }
                return factory.ws.readyState;
            };

            /**
                @ngdoc method
                @name WebSocketFactory#statusAsText
                @param {string} itemId The id of the item
                @description Returns WebSocket connection status as text.
            */
            factory.statusAsText = function() {
                        var readyState = factory.status();
                        if (readyState == WebSocket.CONNECTING){
                                return WEBSOCKETS_CONFIG.CONNECTING;
                        } else if (readyState == WebSocket.OPEN){
                                return WEBSOCKETS_CONFIG.OPEN;
                        } else if (readyState == WebSocket.CLOSING){
                                return WEBSOCKETS_CONFIG.WS_CLOSING;
                        } else if (readyState == WebSocket.CLOSED){
                                return WEBSOCKETS_CONFIG.WS_CLOSED;
                        } else {
                                return WEBSOCKETS_CONFIG.WS_UNKNOWN;
                        }
            };


            return factory;
    }]);


})();