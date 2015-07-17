(function() { 'use strict';

/**
 * @ngdoc module
 * @name appverse.configuration.default
 * @moduleFile appverse-configuration.js
 * @description
 * This module defines default settings.
 *
 */
angular.module('appverse.configuration.default', ['$browser']);

})();
(function() { 'use strict';

/**
 * @ngdoc module
 * @name appverse.configuration.loader
 * @description
 * Load default and custom settings into appverse.configuration
 *
 * @requires appverse.utils
 */
angular.module('appverse.configuration.loader', ['appverse.utils']);


})();

(function() { 'use strict';

/**
 * @ngdoc module
 * @name appverse.configuration
 * @requires appverse.detection
 * @description
 * It includes constants for all the common API components. This module is initially empty.
 * When the application bootstraps, it is populated with the combination of default and custom configuration values
 *
 * @requires appverse.configuration.loader
 */
angular.module('appverse.configuration', ['appverse.configuration.loader'])
    .run(run);

function run($log) {
    $log.info('appverse.configuration run');
}
run.$inject = ["$log"];

})();
(function () {
    'use strict';

    /**
     * @ngdoc module
     * @name  appverse
     * @description Main module. Bootstraps the application by integrating services that have any relation.
     * It will automatically initialize any of these modules, whose scripts have been loaded:
     * * Bootstrap-based styling and gadgets
     * * Routing
     * * External Configuration
     * * REST Integration
     * * Cache Service
     * * ServerPush
     * * Security
     * * Internationalization
     * * Logging
     *
     * @requires  appverse.utils
     * @requires  appverse.configuration
     */

    /**

     * Main module.
     * Bootstraps the application by integrating services that have any relation.
     */
    angular.module('appverse', ['appverse.utils', 'appverse.configuration'])
        .config(config).run(run);


    /**
     * Preliminary configuration.
     *
     * Configures the integration between modules that need to be integrated
     * at the config phase.
     */
    function config($compileProvider, $injector, $provide, ModuleSeekerProvider, REST_CONFIG) {

        //Mock backend if necessary
        if (REST_CONFIG.MockBackend) {

            $provide.decorator('$httpBackend', angular.mock.e2e.$httpBackendDecorator);
        }

        // sanitize hrefs
        $compileProvider.aHrefSanitizationWhitelist(/^\s*(https?|ftp|mailto|itms-services):/);

        // Integrate modules that have a dependency
        if (ModuleSeekerProvider.exists('appverse.detection')) {
            var detectionProvider = $injector.get('DetectionProvider');
            var configLoaderProvider = $injector.get('ConfigLoaderProvider');
            configLoaderProvider.setDetection(detectionProvider);

            if (ModuleSeekerProvider.exists('appverse.logging')) {
                var formattedLoggerProvider = $injector.get('FormattedLoggerProvider');
                formattedLoggerProvider.setDetection(detectionProvider);
            }

        }
    }
    config.$inject = ["$compileProvider", "$injector", "$provide", "ModuleSeekerProvider", "REST_CONFIG"];

    function run($log, REST_CONFIG) {
        if (REST_CONFIG.MockBackend) {
            $log.debug('REST: You are using a MOCKED backend!');
        }
    }
    run.$inject = ["$log", "REST_CONFIG"];


})();

(function() { 'use strict';

angular.module('appverse.configuration.loader').provider('ConfigLoader', ConfigLoaderProvider);

/**
 * @ngdoc provider
 * @name ConfigLoader
 * @module appverse.configuration.loader
 *
 * @description
 * Loads configuration parameters int the AppConfiguration module.
 */
function ConfigLoaderProvider() {

    // By default, no detection is present
    var detection = new NoDetection(),
    // Object used to perfom default config overriding
    appConfigTemp = {};

    /**
     * @ngdoc method
     * @name  ConfigLoader#$get
     * @description Factory function. Gets the service instance
     */
    this.$get = function() {
        return this;
    };

    /**
     * @ngdoc method
     * @name  ConfigLoader#load
     * @param {object} settings See appverse.configuration.default for available settings
     * @description Loads the custom config, overriding defaults
     */
    this.load = function(settings) {
        this.loadDefaultConfig()
            .loadCustomConfig(settings)
            .overrideDefaultConfig();
    };

    /**
     * @ngdoc method
     * @name  ConfigLoader#setDetection
     * @param {object} detectionProvider Detection provider from appverse.detection
     */
    this.setDetection = function(detectionProvider) {
        detection = detectionProvider;
    };


    // ---- Privates -----

    this.loadDefaultConfig = function() {
        angular.forEach(angular.module('appverse.configuration.default')._invokeQueue, function (element) {
            appConfigTemp[element[2][0]] = element[2][1];
        });
        return this;
    };

    this.loadCustomConfig = function(settings) {
        if (settings) {
            this.settings =  settings;
        }
        this.loadMobileConfigIfRequired();
        this.loadEnvironmentConfig();
        return this;
    };

    this.overrideDefaultConfig = function() {
        angular.forEach(appConfigTemp, function (propertyValue, propertyName) {
            angular.module('appverse.configuration').constant(propertyName, propertyValue);
        });
    };


    this.loadMobileConfigIfRequired = function() {
        if (detection.hasAppverseMobile()) {
            this.loadAppverseMobileConfig();
        } else if (detection.isMobileBrowser()) {
            this.loadMobileBrowserConfig();
        }
    };

    this.loadEnvironmentConfig = function() {
        if (this.settings && this.settings.environment) {
            this.addConfig(this.settings.environment);
        } else {
            this.addConfigFromJSON('resources/configuration/environment-conf.json');
        }
        return this;
    };

    this.loadAppverseMobileConfig = function() {
        if (this.settings && this.settings.appverseMobile) {
            this.addConfig(this.settings.appverseMobile);
        } else {
            this.addConfigFromJSON('resources/configuration/appversemobile-conf.json');
        }
        return this;
    };

    this.loadMobileBrowserConfig = function() {
        if (this.settings && this.settings.mobileBrowser) {
            this.addConfig(this.settings.mobileBrowser);
        } else {
            this.addConfigFromJSON('resources/configuration/mobilebrowser-conf.json');
        }

        return this;
    };

    this.addConfig = function(settings) {
        angular.forEach(settings, function (constantObject, constantName) {
            var appConfigObject = appConfigTemp[constantName];

            if (appConfigObject) {
                angular.forEach(constantObject, function (propertyValue, propertyName) {
                    appConfigObject[propertyName] = propertyValue;
                });
                appConfigTemp[constantName] = appConfigObject;
            } else {
                appConfigTemp[constantName] = constantObject;
            }
        });

    };

    this.addConfigFromJSON = function(jsonUrl) {

        // Make syncrhonous request.
        // TODO: make asyncrhonous. Synchronous requests block the browser.
        // Making requests asyncronous will require to manually bootstrap angular
        // when the response is received.
        // Another option is to let the developer inject the configuration in the config phase
        var request = new XMLHttpRequest();
        // `false` makes the request synchronous
        request.open('GET', jsonUrl, false);
        request.send(null);
        var jsonData = JSON.parse(request.responseText);

        this.addConfig(jsonData);
    };


}


/**
 * Used when no detection is provided
 */
function NoDetection() {

    this.hasAppverseMobile = function() {
        return false;
    };

    this.isMobileBrowser = function() {
        return false;
    };

}


})();

(function() { 'use strict';

angular.module('appverse.configuration.default')

/*
PROJECT CONFIGURATION
This constants can be used to set basic information related to the application.
All data are auto-explained because their names ;)
 */

/**
 * @ngdoc object
 * @name PROJECT_DATA
 * @module  appverse.configuration.default
 * @description Basic information related to the application.
 */
.constant('PROJECT_DATA', {
    ApplicationName: 'Appverse Web HTML5 Incubator Demo',
    Version: '0.1',
    Company: 'GFT',
    Year: '2013',
    Team: 'GFT Appverse Web',
    URL: '',
    LoginViewPath: '/login',
    myUrl: '',
    VendorLibrariesBaseUrl: 'bower_components'
})

/**
 * @ngdoc object
 * @name LOGGING_CONFIG
 * @module  appverse.configuration.default
 * @description This section contains basic configuration for appverse.logging
 * These params do not affect normal usage of $log service.
 */
.constant('LOGGING_CONFIG', {
    /*
    This param enables (if true) sending log messages to server.
    The server side REST service must record messages from client in order to be analyzed.
    ALL messages are sent. It is not yet possible select which type of log messages are sent.
     */
    ServerEnabled: false,
    LogServerEndpoint: 'http://localhost:9000/log',
    /*
    This preffix will be included at the beginning of each message.
     */
    CustomLogPreffix: 'APPLOG',
    /*
    Enabled levels will be written in the custom format.
    This param does not affect to $log service.
     */
    EnabledLogLevel: true,
    EnabledErrorLevel: true,
    EnabledDebugLevel: true,
    EnabledWarnLevel: true,
    EnabledInfoLevel: true,
    /*
    Format of the datetime information.
     */
    LogDateTimeFormat: '%Y-%M-%d %h:%m:%s:%z',
    /*
    Fields that will be included in the log message if containing information.
     */
    LogTextFormat: ''
})

/**
 * @ngdoc object
 * @name CACHE_CONFIG
 * @module  appverse.configuration.default
 * @description This section contains basic configuration for appverse.cache
 */
.constant('CACHE_CONFIG', {
    /////////////////////////////
    //SCOPE CACHE
    /////////////////////////////
    ScopeCache_Enabled: true,
    DefaultScopeCacheName: 'appverseScopeDataCache',
    /*
     Max duration in milliseconds of the scope cache
      */
    ScopeCache_duration: 10000,
    /*
     This param turns the scope cache into a LRU one.
     The cache’s capacity is used together to track available memory.
      */
    ScopeCache_capacity: 10,

    /////////////////////////////
    //BROWSER STORAGE TYPE
    //This sets the preferred browser storage in the app.
    //Most of times it is convenient follow a policy for browser storage, using only one of the two types.
    //If you prefer flexibility (the developer makes a choice for each case) do not use the provided API.
    /////////////////////////////
    BrowserStorageCache_Enabled: true,
    /*
     1 = $localStorage
     2 = $sessionStorage
      */
    BrowserStorage_type: '2',
    DefaultBrowserCacheName: 'appverseBrowserCache',
    // Items added to this cache expire after 15 minutes.
    MaxAge: 900000,
    // This cache will clear itself every hour.
    CacheFlushInterval: 3600000,
    // Items will be deleted from this cache right when they expire.
    DeleteOnExpire: 'aggressive',
    //Constant for the literal
    SessionBrowserStorage: 'sessionStorage',
    //Constant for the literal
    LocalBrowserStorage: 'localStorage',
    //Constant for the literal
    NoBrowserStorage: 'none',

    //Direct browser storage (0 local | 1 session)
    browserDirectCacheType: '1',
    /*
     * Specify whether to verify integrity of data saved in localStorage on every operation.
     * If true, angular-cache will perform a full sync with localStorage on every operation.
     * Increases reliability of data synchronization, but may incur a performance penalty.
     * Has no effect if storageMode is set to "none".
     */
    VerifyIntegrity: true,
    /////////////////////////////
    //$http SERVICE CACHE
    /////////////////////////////
    HttpCache_Enabled: true,
    /*
     Max duration in milliseconds of the http service cache.
     */
    HttpCache_duration: 20000,
    /*
     This param turns the http cache into a LRU one.
     The cache’s capacity is used together to track available memory.
     */
    HttpCache_capacity: 10,
    /////////////////////////////
    //BROWSER'S INDEXED DB CACHE
    /////////////////////////////
    IndexedDBCache_Enabled: false,
    /*
     Name of the default object store
      */
    IndexedDB_name: 'DefaultIDBCache',
    /*
     The version for the version (mandatory)
      */
    IndexedDB_version: 1,
    /*
     * The options for the db.
     * The default structure is defined as id/name pairs.
     * It is possible to add more indexes:
     * indexes : [{ name : 'indexName', unique : 'true/false' },{},...]
     */
    IndexedDB_options: [
        {
            storeName: 'structure-of-items',
            keyPath: 'id',
            indexes: [
                {
                    name: 'name',
                    unique: false
                }
            ]
        }
    ]

})

/**
 * @ngdoc object
 * @name SERVERPUSH_CONFIG
 * @module  appverse.configuration.default
 * @description This section contains basic configuration for appverse.serverpush.
 * It si related to socket.io configuration params.
 * Read Configuration section in socket.io documentation for further details.
 * https://github.com/LearnBoost/Socket.IO/wiki/Configuring-Socket.IO
 */
.constant('SERVERPUSH_CONFIG', {
    /*
     URL of the listened server
      */
    BaseUrl: 'http://localhost:3000',
    /*
     Port to be listened at the base url.
      */
    ListenedPort: '3000',
    /*
      resource
      defaults to socket.io
      Note the subtle difference between the server, this one is missing a /.
      These 2 should be in sync with the server to prevent mismatches.
      */
    Resource: 'socket.io',
    /*
      connect timeout
      defaults to 10000 ms
      How long should Socket.IO wait before it aborts the connection attempt with the server to try another fall-back.
      Please note that some transports require a longer timeout than others.
      Setting this really low could potentially harm them.
      */
    ConnectTimeout: '10000',
    /*
      try multiple transports
      defaults to true
      When Socket.IO reconnects and it keeps failing over and over again,
      should it try all available transports when it finally gives up.
      */
    TryMultipleTransports: true,
    /*
      reconnect
      defaults to true
      Should Socket.IO automatically reconnect when it detects a dropped connection or timeout.
      */
    Reconnect: true,
    /*
      reconnection delay
      defaults to 500 ms
      The initial timeout to start a reconnect,
      this is increased using an exponential back off algorithm each
      time a new reconnection attempt has been made.
      */
    ReconnectionDelay: 1000,
    /*
      reconnection limit
      defaults to Infinity
      The maximum reconnection delay in milliseconds, or Infinity.
      */
    ReconnectionLimit: 'Infinity',
    /*
      max reconnection attempts
      defaults to 10
      How many times should Socket.IO attempt to reconnect with the server after a a dropped connection.
      After this we will emit the reconnect_failed event.
      */
    MaxReconnectionAttempts: 5,
    /*
      sync disconnect on unload
      defaults to false
      Do we need to send a disconnect packet to server when the browser unloads.
      */
    SyncDisconnectOnUnload: false,
    /*
      auto connect
      defaults to true
      When code calls io.connect() should Socket.IO automatically establish a connection with the server.
      */
    AutoConnect: true,
    /*
      flash policy port
      defaults to 10843
      If the server has Flashsocket enabled, this should match the same port as the server.
      */
    FlashPolicyPort: '',
    /*
      force new connection
      defaults to false
      Force multiple io.connect() calls to the same server to use different connections.
      */
    ForceNewConnection: false
})

/**
 * @ngdoc object
 * @name REST_CONFIG
 * @module  appverse.configuration.default
 * @description This section contains basic configuration for appverse.rest.
 * This module (and/or) its clones is based on Restangular (https://github.com/mgonto/restangular).
 * So, all configuration params are based on its configuration
 * (https://github.com/mgonto/restangular#configuring-restangular).
 * Future updates of Restangular imply review of this section in order
 * to keep consistency between config and the module.
 */
.constant('REST_CONFIG', {
    /*
    The base URL for all calls to your API.
    For example if your URL for fetching accounts is http://example.com/api/v1/accounts, then your baseUrl is /api/v1.
    The default baseUrl is an empty string which resolves to the same url that AngularJS is running,
    so you can also set an absolute url like http://api.example.com/api/v1
    if you need do set another domain.
    */
    BaseUrl: '/api/v1',

    /*
    If enabled, requests via REST module will be multicasted.
     */
//    Multicast_enabled: true,

     /*
     The base URLs array for all multicast calls to your API.
     */
//    Multicast_baseUrl: ['/api/v1', '/api/v2', '/api/v3', '/api/v4'],

    /*
    Number of requests to be spawned in multicast mode for each
     */
//    Multicast_spawn: 1,

    /*
    These are the fields that you want to save from your parent resources if you need to display them.
    By default this is an Empty Array which will suit most cases.
    */
    ExtraFields: [],

    /*
    Use this property to control whether Restangularized elements to have a parent or not.
    This method accepts 2 parameters:
    Boolean: Specifies if all elements should be parentless or not
    Array: Specifies the routes (types) of all elements that should be parentless. For example ['buildings']
    */
    ParentLess: false,

    /*
    HTTP methods will be validated whether they are cached or not.
    */
    NoCacheHttpMethods: {
        'get': false,
        'post': true,
        'put': false,
        'delete': true,
        'option': false
    },

    /*
    This is a hook. After each element has been "restangularized" (Added the new methods from Restangular),
    the corresponding transformer will be called if it fits.
    This should be used to add your own methods / functions to entities of certain types.
    You can add as many element transformers as you want.
    The signature of this method can be one of the following:

    1-Transformer is called with all elements that have been restangularized, no matter if they're collections or not:
    addElementTransformer(route, transformer)
    2-Transformer is called with all elements that have been restangularized and match the specification regarding
    if it's a collection or not (true | false):
    addElementTransformer(route, isCollection, transformer)
    */
    ElementTransformer: [],

    /*
    This is a hook. After each element has been "restangularized" (Added the new methods from Restangular),
    this will be called. It means that if you receive a list of objects in one call, this method will be called
    first for the collection and then for each element of the collection.
    It is recommended the usage of addElementTransformer instead of onElemRestangularized whenever
    possible as the implementation is much cleaner.
    This callback is a function that has 3 parameters:
    @param elem: The element that has just been restangularized. Can be a collection or a single element.
    @param isCollection: Boolean indicating if this is a collection or a single element.
    @param what: The model that is being modified. This is the "path" of this resource. For example buildings
    @param Restangular: The instanced service to use any of its methods
    */
    OnElemRestangularized: function (elem) {
        return elem;
    },

    /*
    The requestInterceptor is called before sending any data to the server.
    It's a function that must return the element to be requested.

    @param element: The element to send to the server.
    @param operation: The operation made. It'll be the HTTP method used except for a GET which returns a list
    of element which will return getList so that you can distinguish them.
    @param what: The model that's being requested. It can be for example: accounts, buildings, etc.
    @param url: The relative URL being requested. For example: /api/v1/accounts/123
    */
    RequestInterceptor: null,

    /*
    The fullRequestInterceptor is similar to the requestInterceptor but more powerful.
    It lets you change the element, the request parameters and the headers as well.
    It's a function that receives the same as the requestInterceptor
    plus the headers and the query parameters (in that order).
    It must return an object with the following properties:
    headers: The headers to send
    params: The request parameters to send
    element: The element to send
    httpConfig: The httpConfig to call with
    */
    FullRequestInterceptor: null,

    /*
    The errorInterceptor is called whenever there's an error.
    It's a function that receives the response as a parameter.
    The errorInterceptor function, whenever it returns false, prevents the promise
    linked to a Restangular request to be executed.
    All other return values (besides false) are ignored and the promise follows the usual path,
    eventually reaching the success or error hooks.
    The feature to prevent the promise to complete is useful whenever you need to intercept
    each Restangular error response for every request in your AngularJS application in a single place,
    increasing debugging capabilities and hooking security features in a single place.
    */
    ErrorInterceptor: function (response) {
        console.log("ErrorInterceptor, server response:", response);
    },

    /*
    Restangular required 3 fields for every "Restangularized" element. These are:

    id: Id of the element. Default: id
    route: Name of the route of this element. Default: route
    parentResource: The reference to the parent resource. Default: parentResource
    restangularCollection: A boolean indicating if this is a collection or an element. Default: restangularCollection
    cannonicalId: If available, the path to the cannonical ID to use. Usefull for PK changes
    etag: Where to save the ETag received from the server. Defaults to restangularEtag
    selfLink: The path to the property that has the URL to this item. If your REST API doesn't return a
    URL to an item, you can just leave it blank. Defaults to href
    Also all of Restangular methods and functions are configurable through restangularFields property.
    All of these fields except for id and selfLink are handled by Restangular,
    so most of the time you won't change them.
    You can configure the name of the property that will be binded to all
    of this fields by setting restangularFields property.
    */
    RestangularFields: {
        id: 'id',
        route: 'route'
    },

    /*
    You can now Override HTTP Methods. You can set here the array of methods to override.
    All those methods will be sent as POST and Restangular will add an X-HTTP-Method-Override
    header with the real HTTP method we wanted to do.
    */
    MethodOverriders: [],

    /*
    You can set default Query parameters to be sent with every request and every method.
    Additionally, if you want to configure request params per method, you can use
    requestParams configuration similar to $http.
    For example RestangularProvider.requestParams.get = {single: true}.
    Supported method to configure are: remove, get, post, put, common (all).
    */
    DefaultRequestParams: {},

    /*
    You can set fullResponse to true to get the whole response every time you do any request.
    The full response has the restangularized data in the data field,
    and also has the headers and config sent. By default, it's set to false.
    */
    FullResponse: false,

    /*
    You can set default Headers to be sent with every request.
    Example:
    DefaultHeaders: {'Content-Type': 'application/json'}
    */
    DefaultHeaders: {
        'Content-Type': 'application/json',
        'X-XSRF-TOKEN': 'juan'
        //SECURITY_GENERAL.XSRFCSRFHeaderName: SECURITY_GENERAL.XSRFCSRFCookieValue

    },

    /*
    If all of your requests require to send some suffix to work, you can set it here.
    For example, if you need to send the format like /users/123.json you can add that .json
    to the suffix using the setRequestSuffix method
    */
    RequestSuffix: '.json',

    /*
    You can set this to either true or false.
    If set to true, then the cannonical ID from the element will be used for URL creation
    (in DELETE, PUT, POST, etc.).
    What this means is that if you change the ID of the element and then you do a put,
    if you set this to true, it'll use the "old" ID which was received from the server.
    If set to false, it'll use the new ID assigned to the element.
    */
    UseCannonicalId: false,

    /*
    You can set here if you want to URL Encode IDs or not.
    */
    EncodeIds: true,
    /*
     *
     */
    DefaultContentType: 'application/json',

    /**
     * If true, it will mock backend $http calls
     * by decorating the default "real" $http service with a mocked
     * one from angular-mocks.
     * (remember to include the  angular-mocks.js script if this option is set to true)
     * @type {Boolean}
     */
    MockBackend: false
})
/**
 * @ngdoc object
 * @name AD_CONFIG
 * @module appverse.configuration.default
 * @description Defines ConsumerKey and ConsumerSecret
 */
.constant('AD_CONFIG', {
    ConsumerKey: '',
    ConsumerSecret: ''
})

/**
 * @ngdoc object
 * @name I18N_CONFIG
 * @module appverse.configuration.default
 * @description This section contains basic configuration for appverse.translate.
 */
.constant('I18N_CONFIG', {
    PreferredLocale: 'en-US',
    LocaleFilePattern: 'angular-i18n/angular-locale_{{locale}}.js',
    DetectLocale: true
})

/**
 * @ngdoc object
 * @name SECURITY_GENERAL
 * @module appverse.configuration.default
 * @description Includes default information about authentication and authorization configuration based on OAUTH 2.0.
 */
.constant('SECURITY_GENERAL', {
    securityEnabled: false,
    XSRFCSRFRequestHeaderName: 'X-XSRF-TOKEN',
    XSRFCSRFResponseCookieName: 'XSRF-TOKEN',
    BearerTokenResponseHeader: 'access_token',
    BearerTokenRequestHeader: 'Authorization',
    RefreshTokenResponseHeader: 'refresh_token',
    BearerTokenExpiringResponseHeader: 'expires_in',
    TokenTypeResponseHeader: 'token_type',
    /*
    The XSRF policy type is the level of complexity to calculate the value to be returned in the xsrf header in request
    against the authorization server:
    0: No value is included (The domain is the same one)
    1: $http service built-in solution. The $http service will extract this token from the response header,
     and then included in the X-XSRF-TOKEN header to every HTTP request. The server must check the token
     on each request, and then block access if it is not valid.
    2: Additional calculation of the cookie value using a secret hash. The value is included in the X-XSRF-TOKEN
     request header.
     */
    XSRFPolicyType: 1,
    XSRFSecret: '',
    Headers_ContentType: 'application/json',
    loginHTTPMethod: 'POST',
    loginURL: 'http://localhost:8080/html5-incubator-server/rest/sec/login',
    username: 'admin',
    password: 'admin',
    connected: 'connected',
    disconnected: 'disconnected',
    notEnabled: 'Security not enabled'

})

/**
 * @ngdoc object
 * @name SECURITY_OAUTH
 * @module appverse.configuration.default
 * @description Includes default specific settings for OAUTH
 */
.constant('SECURITY_OAUTH', {
    oauth2_endpoint: 'appverse',
    clientID: '',
    profile: 'http://localhost:8080/html5-incubator-server',
    scope: 'resources',
    scopeURL: 'http://localhost:8080/html5-incubator-server',
    scope_authorizePath: '/oauth/authorize',
    scope_tokenPath: '/oauth/token',
    scope_flow: 'implicit',
    scope_view: 'standard',
    scope_storage: 'none',
    scope_template: 'views/demo/security/oauth_default.html',
    redirectURL: 'http://localhost:9000',
    storage: 'cookies',
    storage_cookies: 'cookies',
    storage_header: 'header',
    tokenResponseHeaderName: 'Authorization'
})

/**
 * @ngdoc object
 * @name GOOGLE_AUTH
 * @module appverse.configuration.default
 * @description Defines settings to use Google Oauth2 autentication service
 */
.constant('GOOGLE_AUTH', {
    clientID: '75169325484-8cn28d7o3dre61052o8jajfsjlnrh53i.apps.googleusercontent.com',
    scopeURL: 'https://www.googleapis.com/auth/plus.login',
    requestvisibleactionsURL: 'http://schemas.google.com/AddActivity',
    theme: 'dark',
    cookiepolicy: 'single_host_origin',
    revocationURL: 'https://accounts.google.com/o/oauth2/revoke?token=',
    /*
     * Policy about token renewal:
     * revocation: if the token is invalid the user is fordec to logout and warned.
     * manual_renovation: the user is warned about the token validity. Renewal is proposed.
     * automatic_renovation: the token is automatically renewed.
     */
    revocation: 'revocation',
    manual_renovation: 'manual_renovation',
    automatic_renovation: 'automatic_renovation',
    tokenRenewalPolicy: 'automatic_renovation'
})

/**
 * @ngdoc object
 * @name AUTHORIZATION_DATA
 * @module appverse.configuration.default
 * @description Defines default authorization and roles data
 */
.constant('AUTHORIZATION_DATA', {
    roles: ['user', 'admin', 'editor'],
    adminRoles: ["ROLE_EXAMPLE","ROLE_EXAMPLE_2","ROLE_REMOTE_LOGGING_WRITER","ROLE_USER"],
    users: ['Jesus de Diego'],
    userRoleMatrix: [
        {
            'user': 'Jesus de Diego',
            'roles': ["ROLE_EXAMPLE","ROLE_EXAMPLE_2","ROLE_REMOTE_LOGGING_WRITER","ROLE_USER"]
        },
        {
            'user': 'Antoine Charnoz',
            'roles': ["ROLE_EXAMPLE","ROLE_EXAMPLE_2","ROLE_REMOTE_LOGGING_WRITER","ROLE_USER"]
        }
    ],
    routesThatDontRequireAuth: ['/home'],
    routesThatRequireAdmin: ['/about']
})


/**
 * @ngdoc object
 * @name WEBSOCKETS_CONFIG
 * @module appverse.configuration.default
 * @description Configuration parameters for web sockets
 */
.constant('WEBSOCKETS_CONFIG', {

    WS_ECHO_URL: "ws://echo.websocket.org",
    WS_CPU_URL: "ws://localhost:8080/websocket/services/websocket/statistics/get/cpuload",
    WS_CPU_INTERVAL: 30,
    WS_CONNECTED: 'Websocket connected',
    WS_DISCONNECTED: 'Websocket disconnected',
    WS_CONNECTING: 'Connecting Websocket...',
    WS_CLOSED: 'Websocket connection closed',
    WS_CLOSING: 'Websocket connection closing...',
    WS_OPEN: 'Websocket connection is open',
    WS_UNKNOWN: 'Websocket status is unknown',
    WS_FAILED_CONNECTION: 'Failed to open a Websocket connection',
    WS_NOT_SUPPORTED: 'HTML5 Websockets specification is not supported in this browser.',
    WS_SUPPORTED: 'HTML5 Websockets specification is supported in this browser.'
})

/**
 * @ngdoc object
 * @name PERFORMANCE_CONFIG
 * @module appverse.configuration.default
 * @description Includes default information about the different facets for a better performance in the app.
 * There are three main sections: webworkers management, shadow dom objetc and High performance DOM directive.
 */
.constant('PERFORMANCE_CONFIG', {
/*
 * WEBWORKERS SECTION
 * To test multiple parallelized threads with web workers a thread pool or task queue is defined.
 * The goal is focused on using enough threads to improve the execution but not too much or the browser system can turn
 * into unstable.
 * You can configure the maximum number of concurrent web workers when this pool is instantiated,
 * and any 'task' you submit will be executed using one of the available threads from the pool.
 * Note that the app is not really pooling threads, but just using this pool to control the number of concurrently
 * executing web workers due to the high cost for start them.
 */
    /*
    Maximum number of simultaneous executing threads used by workers
     */
    webworker_pooled_threads: 4,
    /*
    If true, only workers in the web worker_authorized_workers property might be executed.
    Other invoked workers will not result in a worker call.
     */
    webworker_authorized_workers_only: true,
    /*
    Folder for workers' files
     */
    webworker_directory: "resources/webworkers/",
    /*
    List of authorized workers with its ID.
    The ID is used to be passed in the directive's attribute.
     */
    webworker_authorized_workers: [
        {
            'id': 'w1',
            'type': 'dedicated',
            'poolSize': 4,
            'file': 'RenderImage.js'
        },
        {
            'id': 'w2',
            'type': 'dedicated',
            'poolSize': 4,
            'file': 'RestMultiRequest.js'
        }
    ],
    webworker_dedicated_literal: "dedicated",
    webworker_shared_literal: "shared",
    webworker_Message_template: 'scripts/api/directives/webworkerMessage.html'
});

})();
/**
 * @ngdoc object
 * @name  AppInit
 * @module  appverse
 * @description
 * This file includes functionality to initialize settings in an appverse-web-html5 app
 * Just call the initalization code after having loaded angular and the configuration module:
 * <pre><code>AppInit.setConfig(settings).bootstrap();</code></pre>
 */
var AppInit = AppInit || (function(angular) { 'use strict';

    var
    settings,
    mainModuleName;

    /**
     * @ngdoc method
     * @name AppInit#setConfig
     * @param {object} settingsObject An object containing custom settings
     * @description Sets custom settings
     */
    function setConfig(settingsObject) {
        settings = settingsObject;
        angular.module('appverse.configuration.loader').config(loadConfig);
        return AppInit;
    }

    /**
     * @ngdoc method
     * @name AppInit#bootstrap
     * @description Manually Bootstraps the application. For automatic bootstrap,
     * use the standard Angular way using the ng-app directive.
     *
     * @param {string} appMainModule The name of the main application module.
     * You can also use setMainModuleName and use this function without any parameters
     */
    function bootstrap(appMainModule) {
        var moduleName = appMainModule || mainModuleName;
        angular.element(document).ready(function() {
            angular.bootstrap(document, [moduleName]);
        });
    }

    /**
     * @ngdoc method
     * @name AppInit#setMainModuleName
     * @param {string} name The name of the main application module.
     */
    function setMainModuleName(name) {
        mainModuleName = name;
    }

    /**
     * @ngdoc method
     * @name AppInit#setMainModuleName
     * @return {string} The name of the main application module.
     */
    function getMainModule() {
        return angular.module(mainModuleName);
    }

    // ---- Privates -----

    function loadConfig(ConfigLoaderProvider) {
        ConfigLoaderProvider.load(settings);
    }
    loadConfig.$inject = ["ConfigLoaderProvider"];

    return {
        setMainModuleName : setMainModuleName,
        setConfig : setConfig,
        bootstrap : bootstrap,
        getMainModule : getMainModule
    };

})(angular);