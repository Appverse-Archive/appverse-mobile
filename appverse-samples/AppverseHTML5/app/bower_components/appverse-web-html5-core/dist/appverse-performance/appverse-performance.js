(function() {
    'use strict';

    /**
     * @ngdoc module
     * @name appverse.performance
     *
     * @description
     * The AppPerformance provides services to handle usage of several performance elements:
     * 1. Webworkers. Multithreaded-parallelized execution of tasks separated of the main JavaScript thread.
     * 2. High Performance UI directives support.
     *
     * @requires appverse.configuration
     */
    angular.module('appverse.performance', ['appverse.configuration'])
        .run(run);

    function run ($log) {
        $log.info('appverse.performance run');
    }
    run.$inject = ["$log"];

})();
(function () {
    'use strict';

    angular.module('appverse.performance')


    /**
    * @ngdoc directive
    * @name webworker
    * @module appverse.performance
    * @restrict E
    *
    * @description
    * Establishes comm with messages to a selected web worker.
    * Allows send messages to the worker and receive results from.
    * Messages from the worker are displayed in a div.
    *
    * @example
    <example module="appverse.performance">
    <file name="index.html">
    <p>Web Worker test</p>
    <webworker  id="101" message="Hans Walter" template=""/>
    </file>
    </example>
    *
    * @param {string} id Id of the pre-configured worker or path to the worker's file
    * @param {string} message Message to be passed to the worker.
    *
    * @requires  https://docs.angularjs.org/api/ngMock/service/$log $log
    * @requires  WebWorkerFactory
    * @requires  PERFORMANCE_CONFIG
    */
    .directive('webworker', ['$log', 'WebWorkerFactory', 'PERFORMANCE_CONFIG',
        function ($log, WebWorkerFactory, PERFORMANCE_CONFIG) {

            return {
                restrict: 'E', //E = element, A = attribute, C = class, M = comment
                scope: {
                    //(required) set the worker id in configuration or the complete path
                    //if it is not included in config.
                    workerid: '@',
                    //(required) set the message to be passed to the worker
                    message: '@',
                    //(optional) custom template to render the received message from the worker
                    template: '@'
                },
                priority: 1000,
                terminal: true,
                compile: function () {},
                link: function postLink(scope, element, attrs) {
                    var workerid = attrs.id;
                    var template = attrs.template;

                    scope.$watch(function () {
                        return WebWorkerFactory._resultMessage;
                    }, function (newVal) {
                        $log.debug('Cache watch {' + name + '}:', newVal);
                        scope.messageFromWorker = WebWorkerFactory._resultMessage;
                    });

                    scope.$watch('message', function (value) {
                        init(value); // set defaults
                        compileTemplate(); // gets the template and compile the desired layout

                    });


                    //                    scope.$watch(function () {
                    //                        return CacheFactory.getScopeCache().get(name);
                    //                    }, function (newVal) {
                    //                        $log.debug('Cache watch {' + name + '}:', newVal);
                    //                        scope[name] = CacheFactory.getScopeCache().get(name);
                    //                    });
                    //
                    //                    scope.$watch(name, function (newVal) {
                    //                        $log.debug('Cache watch {' + name + '}:', newVal);
                    //                        CacheFactory.getScopeCache().put(name, scope[name]);
                    //                    });



                    /**
                     * @function
                     * @description
                     * Set defaults into the scope object
                     */
                    function init(message) {
                        scope.workerid = workerid;
                        scope.template = template || PERFORMANCE_CONFIG.webworker_Message_template;
                        initWorker(scope.workerid, message);
                    }

                    /**
                     * @function
                     * @description
                     * Gets the message from the worker.
                     */
                    function initWorker(workerid, message) {
                        WebWorkerFactory.runTask(workerid, message);
                        var messageFromWorker = WebWorkerFactory._resultMessage;

                        if (messageFromWorker) {
                            scope.messageFromWorker = messageFromWorker;
                        }
                    }

                    /**
                     * @function
                     * @description
                     * Gets the template and compile the desired layout.
                     * Based on $compile, it compiles a piece of HTML string or DOM into the retrieved
                     * template and produces a template function, which can then be used to link scope and
                     * the template together.
                     */
                    function compileTemplate($http, $templateCache, $compile) {
                        $http.get(scope.template, {
                                //This allows you can get the template again by consuming the
                                //$templateCache service directly.
                                cache: $templateCache
                            })
                            .success(function (html) {
                                element.html(html);
                                $compile(element.contents())(scope);
                            });
                    }
                }
            };
        }]);

})();

(function() {
    'use strict';

    angular.module('appverse.performance')
        .factory('WebWorkerPoolFactory', WebWorkerPoolFactory);


    /**
     * @ngdoc service
     * @name WebWorkerFactory
     * @module appverse.performance
     *

     * @description
     * This factory starts a pooled multithreaded execution of a webworker:
     * <pre></code>                _______
     *                            |       |-> thread 1
     * USER -> message -> task -> | pool  |-> thread 2
     *                            |_______|-> thread N
     * </code></pre>
     *
     * @requires https://docs.angularjs.org/api/ngMock/service/$q $q
     * @requires https://docs.angularjs.org/api/ngMock/service/$log $log
     * @requires PERFORMANCE_CONFIG
     */
    function WebWorkerPoolFactory ($log, $q, PERFORMANCE_CONFIG) {

        var factory = {
            _poolSize: PERFORMANCE_CONFIG.webworker_pooled_threads,
            _authorizedWorkersOnly: PERFORMANCE_CONFIG.webworker_authorized_workers_only,
            _workersDir: PERFORMANCE_CONFIG.webworker_directory,
            _workersList: PERFORMANCE_CONFIG.webworker_authorized_workers,
            _resultMessage: ''
        };

        $log.debug("Initializated webworkers factory preconfigured values." );
        $log.debug("Default pool size: " + factory._poolSize);
        $log.debug("Are only authorized preconfigured workers? " + factory._authorizedWorkersOnly);
        $log.debug("The folder for webworkers in the app: " + factory._workersDir);
        $log.debug("Number of members in the workers list: " + factory._workersList.length);

        /**
         * @ngdoc method
         * @name WebWorkerFactory#runParallelTasksGroup
         *
         * @param {number} workerData WorkerData object with information of the task to be executed
         * @param {object} workerTasks Array with a group of WorkerTask objects for the same WorkerData
         *
         * @description
         * Runs a group of parallelized tasks
         * Run a set of workers according to the pre-defined data in configuration
         * (id, type, size in pool and worker file).
         * Pe-definition in configuration is mandatory.
         * The group of tasks are up to the caller.
         */
        factory.runParallelTasksGroup = function (workerData, workerTasks) {
            this.workerData = workerData;


            $log.debug("Started parallelized execution for worker: ");
            $log.debug(workerData.toString());


            //Initializes the pool with the indicated size for that worker group
            var pool = new factory.WorkerPool(this.workerData.poolSize);
            pool.init();

            //Create a worker task for
            if(workerTasks && workerTasks.length > 0){
                // iterate through all the parts of the image
                for (var x = 0; x < workerTasks.length; x++) {
                    var workerTask = workerTasks[x];

                    pool.addWorkerTask(workerTask);
                }
            }

            return factory._resultMessage;
        };


        /**
         * @ngdoc method
         * @name WebWorkerFactory#passMessage
         *
         * @param {number} id of the called worker
         * @param {object} function as callback
         * @param {string} message to be passed to the worker
         * @description
         * Execute a task in a worker.
         * The group of task is the same as the number indicated in the pool size for that pre-configured worker.
         */
        factory.runTask = function (workerId, message, callback) {

            var pool = new factory.WorkerPool(factory._poolSize);
            pool.init();

            /*
             If only workers in the configuration file are allowed.
             No fallback needed.
             */
            var workerData;
            var workerTask;
            if(factory._authorizedWorkersOnly){
                if(workerId){
                    //Get data from configuration for the worker from the provided ID
                    workerData = factory.getWorkerFromId(workerId);
                }else{
                    //NO VALID WORKER ID ERROR
                    $log.error("NO VALID WORKER ID ERROR");
                }
            }else{
                //If any provided worker is allowed the workerId arg is the complete path to the worker file
                //The ID is not important here
                if(workerId){
                    workerData = new WorkerData(1001, 'dedicated', workerId);
                }else{
                    //NO VALID WORKER ID ERROR
                    $log.error("NO VALID WORKER ID ERROR");
                }
            }

            if(workerData) {
                pool = new factory.WorkerPool(workerData.poolSize);
                /*
                 Create the worker task for the pool (only one task, passed N times):
                 workerName: File of the worker
                 callback: Register the supplied function as callback
                 message: The last argument will be used to send a message to the worker
                 */
                workerTask = new factory.WorkerTask(workerData, callback, message);
                // Pass the worker task object to the execution pool.
                // The default behavior is create one task for each thread in the pool.
                for(var i = 0; i < factory._poolSize; i++){
                    pool.addWorkerTask(workerTask);
                }
            }else{
                //NO WORKER DATA ERROR
                $log.error("NO WORKER DATA ERROR");
            }


            //return _resultMessage;
        };


        factory.WorkerPool = function(poolSize) {
            var _this = this;
            if(!poolSize) {
                this.size = factory._poolSize;
            }else{
                this.size = poolSize;
            }

            //Initialize some vars with default values
            this.taskQueue = [];
            this.workerQueue = [];

            //Start the thread pool. To be used by the caller.
            this.init = function() {
                //Create the 'size' number of worker threads
                for (var i = 0 ; i < _this.size ; i++) {
                    _this.workerQueue.push(new WorkerThread(_this));
                }
            };


            this.addWorkerTask = function(workerTask) {
                if (_this.workerQueue.length > 0) {
                    // get the worker from the front of the queue
                    var workerThread = _this.workerQueue.shift();
                    workerThread.run(workerTask);
                } else {
                    // If there are not free workers the task is put in queue
                    _this.taskQueue.push(workerTask);
                }
            };


            //Execute the queued task. If empty, put the task to the queue.
            this.freeWorkerThread = function(workerThread) {
                if (_this.taskQueue.length > 0) {
                    // It is not put back in the queue, but it is executed on the next task
                    var workerTask = _this.taskQueue.shift();
                    workerThread.run(workerTask);
                } else {
                    _this.taskQueue.push(workerThread);
                }
            };

        };

        //Runner work tasks in the pool
        function WorkerThread(parentPool) {

            var _this = this;
            this.parentPool = parentPool;
            this.workerTask = {};

            //Execute the task
            this.run = function(workerTask) {
                _this.workerTask = workerTask;

                //Create a new web worker
                if (_this.workerTask.script != null) {
                    /*
                     Creation of workers.
                     For both dedicated and shared workers, you can also attach to the
                     message event handler event type by using the addEventListener method.
                     */
                    var worker;
                    if(workerTask.type == PERFORMANCE_CONFIG.webworker_dedicated_literal){
                        worker = new Worker(_this.workerTask.script);
                        worker.addEventListener('message', _this.OnWorkerMessageHandler, false);
                        worker.postMessage(_this.workerTask.startMessage);
                    }else if(workerTask.type == PERFORMANCE_CONFIG.webworker_shared_literal){
                        worker = new SharedWorker(_this.workerTask.script);
                        worker.port.addEventListener('message', _this.OnWorkerMessageHandler, false);
                        worker.port.postMessage(_this.workerTask.startMessage);
                    }else{
                        //NO TYPE ERROR
                        $log.error("NO VALID WORKER TYPE");
                    }
                }
            };

            //We assume we only get a single callback from a worker as a handler
            //It also indicates the end of this worker.
            _this.OnWorkerMessageHandler = function (evt) {
                // pass to original callback
                _this.workerTask.callback(evt);

                // We should use a separate thread to add the worker
                _this.parentPool.freeWorkerThread(_this);
            };
        }


        //The task to run
        factory.WorkerTask = function (workerData, callback, msg) {
            this.script = workerData.file;
            if(callback){
                this.callback = callback;
            }else{
                this.callback = defaultEventHandler;
            }
            this.startMessage = msg;
            this.type = workerData.type;
        };

        /*
         Default event handler.
         */
        function defaultEventHandler(event){
            factory._resultMessage = event.data;
        }

        //Data object for a worker
        function WorkerData(workerId, type, poolSize, worker) {
            this.id = workerId;
            this.type = type;
            this.poolSize = poolSize;
            this.file = worker;
        }

        WorkerData.prototype.toString = function(){
            return "ID: " + this.id + "|TYPE: " + this.type + "|POOL SIZE: " + this.poolSize + "|FILE: " + this.file;

        };

        //Extract worker information from configuration
        factory.getWorkerFromId = function (workerId, poolSize){
            this.id = workerId;
            this.type = '';
            this.poolSize = poolSize;
            this.file = '';

            for(var i = 0; i < factory._workersList.length; i++) {
                if(factory._workersList[i].id === workerId){
                    this.type = factory._workersList[i].type;
                    if(!this.poolSize || this.poolSize == 0){
                        this.poolSize = factory._workersList[i].poolSize;
                    }else{
                        this.poolSize = poolSize;
                    }

                    this.file = factory._workersDir + factory._workersList[i].file;
                    break;
                }
            }

            var workerData = new WorkerData(this.id, this.type, this.poolSize, this.file);

            return workerData;
        };

        return factory;
    }
    WebWorkerPoolFactory.$inject = ["$log", "$q", "PERFORMANCE_CONFIG"];

})();