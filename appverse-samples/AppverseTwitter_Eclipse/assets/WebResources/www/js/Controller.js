PROJECTROOTOBJ.config = {
    //items: []...
	coordinates: '0,0',  // Knutsford Latitude: 53.3  Longitude: -2.3667
        coords: {},
        defaultLatitude: 53.3,
        defaultLongitude: -2.3667,
        twitterQuery: 'appversed',
        searchLocation: '',
	countdown: 20,
        radius: '5mi',
        //tweetBubble : new google.maps.InfoWindow()
        tweetBubble:new InfoBubble({					
                shadowStyle : 1,
                padding : 10,
                backgroundColor : '#e9f5fe',
                borderRadius : 10,
                minHeigth: 500,
                minWidth: 300,
                maxWidth: 300,
                maxHeight: 500,
                arrowSize : 10,
                borderWidth : 1,
                borderColor : '#90bbde',
                disableAutoPan : true,	
                hideCloseButton : true,
                arrowPosition : 30,
                backgroundClassName : 'bubbleBG',
                arrowStyle : 0
        })
};

PROJECTROOTOBJ.Controller = function(){
    return this;
};

PROJECTROOTOBJ.Controller.prototype.functionExample = function(param){
    return "something if you want";
};

PROJECTROOTOBJ.Controller.prototype.getCoordinates = function(){
    var coordinates = '';
    
    try{
       Unity.Geo.StartUpdatingLocation(); 
       coordinates = Unity.Geo.GetCoordinates();
       PROJECTROOTOBJ.config.coords.latitude = coordinates.XCoordinate;
       PROJECTROOTOBJ.config.coords.longitude = coordinates.YCoordinate;
       coordinates = coordinates.XCoordinate + ',' + coordinates.YCoordinate;
    }catch (e){
       console.error('Controller -- getCoordinates() ', e) 
    }

    Unity.Geo.StopUpdatingLocation();
       
    return coordinates;
};

PROJECTROOTOBJ.Controller.prototype.getLocationCoordinates = function(userLocation, callback){
    var coordinates = '';
    
    try{
        
        var geocoder = new google.maps.Geocoder();
        
        //convert location into longitude and latitude
        geocoder.geocode({
            address: userLocation
        }, callback);
        
    }catch (e){
       console.error('Controller -- getLocationCoordinates() ', e) 
    }

       
    return coordinates;
};

PROJECTROOTOBJ.Controller.prototype.refreshLocation = function(){
    PROJECTROOTOBJ.config.coordinates = '0,0';
    this.getLocation();
}

PROJECTROOTOBJ.Controller.prototype.getLocation = function(){
	
	try{
		
                if(PROJECTROOTOBJ.config.searchLocation == '') {
                    // get device current location
                    PROJECTROOTOBJ.config.coordinates = this.getCoordinates();
                    
                } else {
                    
                    var callbackFn = function(locResult) {
                        //console.log("get location using GeoCoder ", locResult);
                        var lat1 = locResult[0].geometry.location.lat();
                        var lng1 = locResult[0].geometry.location.lng();
                        PROJECTROOTOBJ.config.coords.latitude = lat1;
                        PROJECTROOTOBJ.config.coords.longitude = lng1;
                        PROJECTROOTOBJ.config.coordinates = lat1 + ',' + lng1;
                        
                        //console.log("setting new coordinates ", PROJECTROOTOBJ.config.coordinates);
                    };
                    
                    this.getLocationCoordinates(PROJECTROOTOBJ.config.searchLocation, callbackFn);
                }
                
                var c = PROJECTROOTOBJ.config.coordinates;
		if(c == "0.0,0.0" || c == "0,0"){
                    PROJECTROOTOBJ.config.coords.latitude = PROJECTROOTOBJ.config.defaultLatitude;
                    PROJECTROOTOBJ.config.coords.longitude = PROJECTROOTOBJ.config.defaultLongitude;
                    window.setTimeout(function (){
                            console.log('try to collect coord data');
                            PROJECTROOTOBJ.ct.getLocation();
                    }, 5000);
                    return;
                }else{
                    console.log('coord collected', PROJECTROOTOBJ.config.coordinates);
                    
                    // center map with the current coords
                    PROJECTROOTOBJ.ui.getView('tweetMap').update(PROJECTROOTOBJ.config.coords);
                    // collect tweets for the current coords
                    PROJECTROOTOBJ.ct.getLocationTweets();
                }
	}catch(e){
		
	}
	
}

PROJECTROOTOBJ.Controller.prototype.tweetRefresh = function(){
    
        //Unity.Notification.StartNotifyLoading("Loading tweets...");
        
	PROJECTROOTOBJ.ct.getSearchTweets();
        this.refreshLocation();
};

/*PROJECTROOTOBJ.Controller.prototype.tweetCountdown = function(){
    var f = function(){
		window.setTimeout(function(){
			if(PROJECTROOTOBJ.config.countdown > 0){
				PROJECTROOTOBJ.config.countdown--;
				Ext.getCmp('refreshButton').setText(PROJECTROOTOBJ.ct.secToMin(PROJECTROOTOBJ.config.countdown));
				f();
			}else{
				PROJECTROOTOBJ.ct.tweetRefresh();
			}
		},1000);
	}
	f();
}*/

PROJECTROOTOBJ.Controller.prototype.secToMin = function(sec){
	var min = Math.floor(sec / 60);
	var secs = sec % 60;
	//@TODO SECS
	return min + ":" + secs;
}

PROJECTROOTOBJ.Controller.prototype.markers = new Array();

PROJECTROOTOBJ.Controller.prototype.clearMarkers = function() {
    for(var i=0; i<this.markers.length; i++){
        this.markers[i].setMap(null);
    }
    this.markers = new Array();
};


PROJECTROOTOBJ.Controller.prototype.addMarker = function(tweet){
    var latLng = new google.maps.LatLng(tweet.geo.coordinates[0], tweet.geo.coordinates[1]);
    //console.log("tweet geo data: ", latLng);
    
    var mapPanel = PROJECTROOTOBJ.ui.getView('tweetMap');
    var marker = new google.maps.Marker({
            map: mapPanel.map,
            position: latLng,
            icon: './images/'+markerURL //tweet.profile_image_url
    });
    console.log(tweet.profile_image_url);
    this.markers[this.markers.length] = marker;
    
    google.maps.event.addListener(marker, "click", function() {
            //PROJECTROOTOBJ.config.tweetBubble.setContent(tweet.text);
            PROJECTROOTOBJ.config.tweetBubble.setContent(PROJECTROOTOBJ.ui.getView('tweetList').tpl.apply(tweet));
            PROJECTROOTOBJ.config.tweetBubble.open(mapPanel.map, marker);
            
    });
    google.maps.event.addListener(marker, "mousedown", function() {
            //PROJECTROOTOBJ.config.tweetBubble.setContent(tweet.text);
            PROJECTROOTOBJ.config.tweetBubble.setContent(PROJECTROOTOBJ.ui.getView('tweetList').tpl.apply(tweet));
            PROJECTROOTOBJ.config.tweetBubble.open(mapPanel.map, marker);
    });
}

// http://search.twitter.com/search.json?q=%23barclays&geocode=53.3,-2.366,10km
// http://search.twitter.com/search.json?q=%23barclays%26geocode%3D53.3,-2.366,10km

PROJECTROOTOBJ.Controller.prototype.getSearchTweets = function(){
	
	try {
		Ext.util.JSONP.request({
                        url:'http://search.twitter.com/search.json',
			callbackKey: 'callback',
			params: {
				 q: PROJECTROOTOBJ.config.twitterQuery, // '%23'+ PROJECTROOTOBJ.config.twitterQuery
                                 rpp: 100
			},
			callback: function(result){
				
                                tweetStore.loadData(result.results);
								var tweetList = result.results;
                                console.dir("tweets found " + tweetList.length);
                                PROJECTROOTOBJ.ct.clearMarkers();
                                var addedMarkers = 0;
                                for (var i = 0, ln = tweetList.length; i < ln; i++) {
                                        var tweet = tweetList[i];
										console.log(tweet);
										tweet = tweet.geo?tweet:tweet.data;
										if (tweet && tweet.geo && tweet.geo.coordinates) {
                                            console.log("adding marker for tweet ", tweet);
                                        	addedMarkers++;
                                            PROJECTROOTOBJ.ct.addMarker(tweet);
                                        }
                                }
                                console.dir("markers added to map " + addedMarkers);
                                if(!Unity.Notification.IsNotifyLoadingRunning()) {
                                    
                                        Unity.Notification.StopNotifyLoading();
                                }
				console.log(result.results);
                                //if(!Unity.Notification.IsNotifyLoadingRunning()) {
                                //        Unity.Notification.StopNotifyLoading();
                                //}
			}
		});
	}catch(e){}
        
}

PROJECTROOTOBJ.Controller.prototype.getLocationTweets = function(){
	return;
	try {
                console.log("getLocationTweets() for ", PROJECTROOTOBJ.config.coordinates);
		Ext.util.JSONP.request({
                        url:'http://search.twitter.com/search.json',
			callbackKey: 'callback',
			params: {
			         geocode: PROJECTROOTOBJ.config.coordinates + ',' + PROJECTROOTOBJ.config.radius,
                                 rpp: 100
			},
			callback: function(result){
                                // Add points to the map
                                var tweetList = result.results;
                                console.dir("tweets found " + tweetList.length);
                                PROJECTROOTOBJ.ct.clearMarkers();
                                var addedMarkers = 0;
                                for (var i = 0, ln = tweetList.length; i < ln; i++) {
                                        var tweet = tweetList[i];
                                        if (tweet && tweet.geo && tweet.geo.coordinates) {
                                            //console.log("adding marker for tweet ", tweet);
                                            addedMarkers++;
                                            PROJECTROOTOBJ.ct.addMarker(tweet);
                                        }
                                }
                                console.dir("markers added to map " + addedMarkers);
                                if(!Unity.Notification.IsNotifyLoadingRunning()) {
                                    
                                        Unity.Notification.StopNotifyLoading();
                                }
			}
		});
	}catch(e){}
}

PROJECTROOTOBJ.Controller.prototype.cardNavigation = function(){
    
    this.tweetRefresh();
    
    var currentActiveItemID = PROJECTROOTOBJ.ui.getView('basePanel').getActiveItem().id;
    var nextActiveItemID = currentActiveItemID;
    var animation = {type:'slide'};
    var iconCls = 'locate';
    if(currentActiveItemID == 'tweetList') {
      nextActiveItemID = 'tweetMap';
      animation.reverse = false;
      searchBar.hide();
      searchLocationBar.show();
      iconCls = 'home';
    } else {
      nextActiveItemID = 'tweetList';
      animation.reverse = true;
      searchBar.show();
      searchLocationBar.hide();
      iconCls = 'locate';
    }
    
    PROJECTROOTOBJ.ui.getView('toolbar').items.items[0].setIconClass(iconCls);
    
    PROJECTROOTOBJ.ui.getView('basePanel').setActiveItem(PROJECTROOTOBJ.ui.getView(nextActiveItemID), animation);

}



