
 var map, marker, helper = null,time = null,
	weatherToken = "/forecast/0236fde6d04c571aa10643106c7d175c/@@LAT@@,@@LON@@?units=si",
	decoderToken="/arcgis/rest/services/World/GeocodeServer/reverseGeocode?location=@@LON@@%2C+@@LAT@@&distance=200&outSR=&f=pjson";
//map.onUpdateEnd = function(){console.log(map.geographicExtent.getCenter());}
//https://api.forecast.io/forecast/0236fde6d04c571aa10643106c7d175c/41.4926,2.0763?units=si
//http://geocode.arcgis.com/arcgis/rest/services/World/GeocodeServer/reverseGeocode?location=2.0763%2C+41.4926&distance=200&outSR=&f=pjson
function jqueryInit(){

	require(["esri/map","esri/symbols/PictureMarkerSymbol","dojo/on","dojo/domReady!"],
		function(Map,PictureMarkerSymbol,on) {

			
			$('#log').hide();
			
			
			// Create map
			map = new Map("mapDiv",{
				basemap: "streets",
				center: [2.0758125,41.4918536],
				zoom: 18
			});

			// Wait until map has loaded before starting geolocation
			map.on("load",init);

			// Create the marker symbol
			markerSymbol = new PictureMarkerSymbol({
				"angle":0,
				"xoffset":0,
				"yoffset":0,
				"type":"esriPMS",
				"url":"images/pin.png",
				"width":35,
				"height":35
			});

			function init(){
				try{
					helper = new jQueryHelper(map);
				}
				catch(err) {
					console.log("Unable to initialize jQueryHelper: " + err.message);
				}

				map.reposition();
				map.resize();
				//startGeolocation();
				Appverse.Geo.IsGPSEnabled('GPStest','GPStest');
				
				map.onUpdateEnd = function(){
					pos = map.geographicExtent.getCenter();
					//weatherfn(pos.y,pos.x);
					//decoderFn(pos.y,pos.x);
				};
				pos = map.geographicExtent.getCenter();
				weatherfn(pos.y,pos.x);
				decoderFn(pos.y,pos.x);
				
				$( '#home' ).on( 'pageshow',function(event){
					reloadLocation();
				});
				
				$( '#map' ).on( 'pageshow',function(event){
					if (!marker) {                    
						var wgsPt = new esri.geometry.Point(map.geographicExtent.getCenter().x,map.geographicExtent.getCenter().y);
						marker = map.graphics.add(new esri.Graphic(wgsPt, markerSymbol));
                    }
				});
				
			}

			function startGeolocation(){
				navigator.geolocation.getCurrentPosition(locationSuccess,locationError,{setHighAccuracy:true});				
			}

			Appverse.System.DismissSplashScreen();
			
			$('#longitude').html("Calculating...");
			$('#latitude').html("Calculating...");
			$('#time').html(elapsed()+'s');

		}

	);
}

// Handle location success
function locationSuccess(position){
	if(position.coords.latitude !== null || position.coords.longitude !== null){
		console.log("ps " + position.coords.longitude + ", " + position.coords.latitude);
		var wgsPt = new esri.geometry.Point(position.coords.longitude,position.coords.latitude);
		map.graphics.add(new esri.Graphic(wgsPt, markerSymbol));
		map.centerAndZoom(wgsPt,18);

		//Set jQueryHelper properties
		helper.setCenterPt(position.coords.latitude,position.coords.longitude,4326);
		helper.setZoom(18);
	}
}

function locationError(err){
	console.log("locationError: " + err.message);
}

function validCoord() {
	$('#time').html(elapsed()+'s');
	coordinates = arguments[0];
	if (!coordinates) {
		locationError('Not position Returned');
        return;
    }
	console.log(coordinates);
	
	if (coordinates.XDoP > 200 || coordinates.YDoP > 200 || coordinates.XCoordinate <= 0 || coordinates.YCoordinate <= 0){
		locationError('BAD position Returned');
		if (elapsed() < 5) {
            Appverse.Geo.GetCoordinates('validCoord','validCoord');
        }else{
			var str = 'Tap to reload';
			$('#longitude').html(str);
			$('#latitude').html(str);
			$('#time').html(elapsed()+"s: we need to improve this");
		}
		return false;
	}else {
		var coordinate = {};
		coordinate.coords= {};
		coordinate.coords.longitude = coordinates.YCoordinate;
		coordinate.coords.latitude = coordinates.XCoordinate;
		$('#longitude').html(coordinate.coords.longitude);
		$('#latitude').html(coordinate.coords.latitude);
		$('#time').html(elapsed()+'s');
		locationSuccess(coordinate);
		
		Appverse.Geo.StopUpdatingLocation();
		
		
	}
}
function elapsed(){
	if (!time) {
        return 0;
    }
	return parseInt(((new Date().getTime() - time )/1000),10);
}

decoderFn = function(lat,lon){
	var request = {};
	request.Session = {};
	//'41.4926'
	//'2.0763'
	Appverse.IOServices["geodecoder-4"].Endpoint.Path = decoderToken.replace('@@LAT@@', lat).replace('@@LON@@',lon);
	Appverse.IO.InvokeService(request,Appverse.IOServices["geodecoder-4"],null,'geodecoderCallback','geodecoder@'+lat+','+lon);
};
geodecoderCallback = function(result,id){
	console.log(arguments);
	if (result) {
        $('.location').html(JSON.parse(result.Content).address.City);
    }else $('.location').html('Unknown');
	
};
weatherfn = function(lat,lon){
	var request = {};
	request.Session = {};
	//'41.4926'
	//'2.0763'
	Appverse.IOServices["weather-4"].Endpoint.Path = weatherToken.replace('@@LAT@@', lat).replace('@@LON@@',lon);
	Appverse.IO.InvokeService(request,Appverse.IOServices["weather-4"],null,'weatherCallback','weather@'+lat+','+lon);
};

setIcon = function(value){
	$('.wheatherIcon')[0].setAttribute('data-icon-weather',value);
};

weatherCallback = function(result,id){
	console.log(arguments);
	if (result) {
        data = JSON.parse(result.Content);
		switch(data.currently.icon){
			case "clear-night":
				setIcon('C');
				break;
			case "clear-day":
				setIcon('B');
				break;
			case "partly-cloudy-day":
				setIcon('H');
				break;
			case "partly-cloudy-night":
				setIcon('I');
				break;
			case "snow":
				setIcon('W');
				break;
			case "rain":
				setIcon('R');
				break;
			
		}
		$('.temperature').html(data.currently.temperature);
		$('.hour div').html(data.hourly.summary);
		$('.day div').html(data.daily.summary);
    }
};

function syntaxHighlight(json) {
	if (!json) {
        return;
    }
	if (typeof json != 'string') {
		json = JSON.stringify(json, undefined, 2);
	}
	json = json.replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;');
	return json.replace(/("(\\u[a-zA-Z0-9]{4}|\\[^u]|[^\\"])*"(\s*:)?|\b(true|false|null)\b|-?\d+(?:\.\d*)?(?:[eE][+\-]?\d+)?)/g, function(match) {
		var cls = 'number';
		if (/^"/.test(match)) {
			if (/:$/.test(match)) {
				cls = 'key';
			} else {
				cls = 'string';
			}
		} else if (/true|false/.test(match)) {
			cls = 'boolean';
		} else if (/null/.test(match)) {
			cls = 'null';
		}
		return '<span class="' + cls + '">' + match + '</span>';
	});
};

function GPStest(){
	if (arguments[0]) {
        startGPS();
    } else {
		$('#log').html("Activate GPS");
		$('#log').show();
		
	}
};

function startGPS(){
	time = new Date().getTime();
	
	Appverse.Geo.StartUpdatingLocation();
	
	Appverse.Geo.GetCoordinates('validCoord','validCoord');
		
	
}


reloadLocation = function(){
	console.log('reload');
	pos = map.geographicExtent.getCenter();
	weatherfn(pos.y,pos.x);
	decoderFn(pos.y,pos.x);
}
function hideLog(){
		$('#log').hide();
}


/***** Appverse Overrided Functions to provide Orientation Features ****/
Appverse.getCurrentOrientation = function() {

};

Appverse.setOrientationChange = function(orientation, width, height) {
    viewPort.update('You have rotated to ' + orientation);
};

/**
 * Applications should override/implement this method to be aware of application being send to background, and should perform the desired javascript code on this case.
 * @method
 * @version 2.0
 * 
 */
Appverse.backgroundApplicationListener = function() {

};


/**
 * Applications should override/implement this method to be aware of application coming back from background, and should perform the desired javascript code on this case.
 * @method
 * @version 2.0
 * 
 */
Appverse.foregroundApplicationListener = function() {

};



/***********************  end overrinding **************************/

test = function(){console.log(arguments);}; 