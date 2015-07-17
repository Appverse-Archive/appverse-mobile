'use strict';

angular.module('App.Controllers')

.controller('homeController',
    function($log, $scope, $window, $http, x2js) {
        $log.debug('homeController loading');
    
        $scope.toJSON = function(xml) {
            var x2js = new X2JS();
            var json = x2js.xml_str2json(xml);
            return json;
        };
    
        $scope.open = function(item) {
            try{
                Appverse.Net.OpenBrowser(item.title.__text,"Appverse",item.link._href);
             } catch (e) {
                console.log('NO Appverse Mobile');
                item.linkSplit = item.link.split('/');
                item.linkSplit[2]="20minutos.feedsportal.com";
                item.link = item.linkSplit.join('/');
                $log.debug(item.link);
                window.open(item.link, '_blank');
             }

        }
        $scope.greeting = 'Welcome';
        $window.getNews = function(result, id) {
            if(Appverse){
                $log.debug("News refreshed @ "+new Date(parseFloat(id)));
                //$log.debug(result.Content);
                $scope.data = $scope.toJSON(result.Content);
                $scope.title = $scope.data.feed.title;
                $scope.items = $scope.data.feed.entry;
                $scope.$evalAsync();
            }else{
                $log.debug("News refreshed @ " + new Date(parseFloat(id)));            
                $scope.data = $scope.toJSON(result.Content);
                $scope.title = $scope.data.rss.channel.title;
                $scope.items = $scope.data.rss.channel.item;
                $scope.$evalAsync();
            }
        };

        /*$http.defaults.headers.put = {
            'Access-Control-Allow-Origin': '*',
            'Access-Control-Allow-Methods': 'GET, POST, PUT, DELETE, OPTIONS',
            'Access-Control-Allow-Headers': 'Content-Type, X-Requested-With',
            'Content-Type':'text/xml'

        };
        $http.defaults.useXDomain = true;
        */
        var request = {};
        request.Session = {};
        //Appverse.IO.InvokeService(request,Appverse.IOServices['bbc-4'],null,'getNews',new Date().getTime());
        try {
            /*
            Appverse.IOServices[0].Type = Appverse.IO.SERVICETYPE_XMLRPC_XML;
            Appverse.IOServices[0].Endpoint.Host = "http://20minutos.feedsportal.com";
            Appverse.IOServices[0].Endpoint.Path = "/c/32489/f/478284/index.rss";
            */
            Appverse.IO.InvokeService(request, Appverse.IOServices["bbc-4"], null, 'getNews', new Date().getTime());
        } catch (e) {
            console.log('NO Appverse Mobile');
            Appverse  = false;
            $http.get('/20m').
            success(function(data, status, headers, config) {
                // this callback will be called asynchronously
                // when the response is available
                $log.debug('success');
                $window.getNews({
                    "Content": data
                }, 'From Web');
            }).
            error(function(data, status, headers, config) {
                // called asynchronously if an error occurs
                // or server returns response with an error status.
                $log.debug('error');
                $log.debug(headers);
            });

            /*$http.get('/granada').
            success(function(data, status, headers, config) {
                // this callback will be called asynchronously
                // when the response is available
                $log.debug('success');
                $log.debug(headers);
            }).
            error(function(data, status, headers, config) {
                // called asynchronously if an error occurs
                // or server returns response with an error status.
                $log.debug('error');
                $log.debug(headers);
            });

            var req = {
                method: 'GET',
                url: 'http://www.ideal.es/granada/rss/atom/portada',
                headers: {
                    'Content-Type': 'text/xml',
                    'Access-Control-Allow-Origin': '*'
                }
            }

            $http(req).success(function() {
                $log.debug('success');

            }).error(function() {
                $log.debug('error');

            });*/

        }
    });
