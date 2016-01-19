/**
 * Helper library for building mobile ArcGIS mapping applications with jQuery mobile.
 * Specifically designed for use in multiple view applications.
 * Requires: jQuery Mobile 1.3+ and ArcGIS API for JavaScript v3.5+
 * @author Andy Gup
 * @param map esri.Map
 */
var jQueryHelper = function(/* Map */ map){
    this.map = map;
    this.mapId = map.id;
    this.mapDiv = document.getElementById(map.id);
    this.basemap = map._basemap;
    this.currentPageID = null;
    this.slider = this.map._slider;

    /**
     * By default the library will automatically recenter the map
     * after an orientation event in a child view.
     * Override this if you wish to handle your own recentering
     * when the view is returned to home.
     * If you override, then listen for the jquery "helper-map-loaded" event.
     * @type {boolean}
     */
    this.autoCenter = true;

    /**
     * Map debounce delay in milliseconds
     * @type {number}
     */
    this.DEBOUNCE_DELAY = 250;

    /**
     * The delay that allows the application to "settle" before
     * attempting to reinflate. You may need to tweak this!
     * @type {number}
     */
    this.REINFLATION_DELAY = 1000;

    /**
     * Constant variables
     * @type {Object}
     */
    this.localEnum = (function(){
        var values = {
            PORTRAIT:"portrait",
            LANDSCAPE:"landscape",
            DELAY: 400  //Delay in ms to wait for view to settle.
                        //You might have to experiment with this to get max performance.
        }

        return values;
    });

    this.setZoom = function(/* int */ zoom){
        localStorage.setItem("_zoomLevel",zoom);
    }

    this.getZoom = function(){
        var value = null;

        try{
            value = localStorage.getItem("_zoomLevel");
        }
        catch(err)
        {
            console.log("getZoom: " + err.message);
        }

        return value;
    }

    this.setWidth = function(/* int */ value){
        localStorage.setItem("_width",value);
    }

    this.getWidth = function(){
        var value = null;

        try{
            value = localStorage.getItem("_width");
        }
        catch(err)
        {
            console.log("getZoom: " + err.message);
        }

        return value;
    }

    this.setHeight = function(/* int */ value){
        localStorage.setItem("_height",value);
    }

    this.getHeight = function(){
        var value = null;

        try{
            value = localStorage.getItem("_height");
        }
        catch(err)
        {
            console.log("getZoom: " + err.message);
        }

        return value;
    }

    /**
     * Determines if phone is in PORTRAIT or LANDSCAPE mode
     * See localEnum() for constant values
     * @returns {string}
     */
    this.getOrientation = function(){

        if(typeof window.orientation != "undefined"){
            if(window.orientation == 0 || window.orientation == 180) return this.localEnum().PORTRAIT;
            if(window.orientation == 90 || window.orientation == 270) return this.localEnum().LANDSCAPE;
        }
        else{
            return  window.innerHeight > window.innerWidth ?
                this.localEnum().PORTRAIT :
                this.localEnum().LANDSCAPE;
        }
    }

    /**
     * Uses localStorage to save a location.
     * @param lat
     * @param lon
     * @param spatialReference
     */
    this.setCenterPt = function(lat,lon,spatialReference){
        localStorage.setItem("_centerPtX", lat);
        localStorage.setItem("_centerPtY", lon);
        localStorage.setItem("_spatialReference", spatialReference);
    }

    /**
     * Pulls a saved location from localStorage
     * Requires that setCenterPt() has been set.
     * @returns String x,y,spatialReference
     */
    this.getCenterPt = function(){
        var value = null;

        try{
            value = localStorage.getItem("_centerPtX") + "," + localStorage.getItem("_centerPtY") + "," +
                localStorage.getItem("_spatialReference");
        }
        catch(err)
        {
            console.log("getCenterFromLocalStorage: " + err.message);
        }

        return value;
    }

    /**
     * Activates the orientation listener and listens for native events.
     * Handle orientation events to allow for resizing the map and working around
     * jQuery mobile bugs related to how and when the view settles after such an event
     */
    this.setOrientationListener = function(){
        var supportsOrientationChange = "onorientationchange" in window,
            orientationEvent = supportsOrientationChange ? "orientationchange" : "resize";

        window.addEventListener(orientationEvent, this.debounceMap(function(){
            if(this._getActivePage() == this.currentPageID){
                this.recenterOnRotate(400);
            }
        },this.DEBOUNCE_DELAY).bind(this), false);
    }

    /**
     * Minimize the number of times window readjustment fires a function
     * http://davidwalsh.name/javascript-debounce-function
     * @param func
     * @param wait
     * @param immediate
     * @returns {Function}
     */
    this.debounceMap = function (func, wait, immediate) {
        var timeout;
        return function() {
            var context = this, args = arguments;
            clearTimeout(timeout);
            timeout = setTimeout(function() {
                timeout = null;
                if (!immediate) func.apply(context, args);
            }, wait);
            if (immediate && !timeout) func.apply(context, args);
        };
    };

    /**
     * Automatically sets new center point in local storage.
     */
    this.setPanListener = function(){
        this.map.on("pan-end",function(){
            var center = this.map.extent.getCenter();
            this.setCenterPt(center.x,center.y,this.map.spatialReference);
        }.bind(this))
    }

    /**
     * Automatically sets new center point and zoom level in
     * local storage.
     */
    this.setZoomListener = function(){
        this.map.on("zoom-end",function(){
            var center = this.map.extent.getCenter();
            this.setCenterPt(center.x,center.y,this.map.spatialReference);
            this.setZoom(this.map.getZoom());
        }.bind(this))
    }

    /**
     * Sets internal page listeners
     * @param pageId
     */
    this.setPageChangeListeners = function(pageId){
        $('#' + pageId).on("pageshow",function(){
            console.log("home pageshow event");
            var currentOrientation = this.getOrientation();
            this._reinflatMap(currentOrientation);
            this.recenterOnRotate(400);
        }.bind(this))
    }

    this.recenterOnRotate = function(/* int */ timerDelay){

        if(this.map.height == 0 || this.map.width == 0){
            var currentOrientation = this.getOrientation();
            this._reinflatMap(currentOrientation);
        }
        else{
            var timeout = null;
            timerDelay != "undefined" ? timeout = timerDelay : timeout = 500;
            setTimeout((function(){
                console.log("rotate timer complete");

                var locationStr = this.getCenterPt().split(",");
                this._centerMap(locationStr[0],locationStr[1],locationStr[2])

            }).bind(this),timeout);
        }
    }

    this._getMapDivVisibility = function(){
        var id = this.map.id;
        return $("#" + id).is(":visible");
    }

    this.resetMap = function(height,width,zoom){
        if(this.getOrientation() == this.localEnum().PORTRAIT){
            //adjust map height/width
            this.map.height = height;
            this.map.width = width;

            //adjust map div height/width
            this.mapDiv.style.height = height;
            this.mapDiv.style.width = width;

        }
        else{
            //adjust map height/width
            this.map.width = height;
            this.map.height = width;

            //adjust map div height/width
            this.mapDiv.style.width = height;
            this.mapDiv.style.height = width;
        }
    }

    /**
     * Reinflate map based on specific conditions
     * @param currentOrientation
     * @private
     */
    this._reinflatMap = function(currentOrientation){
        if(this.map.height == 0 || this.map.width ==0){
            this.debounceMap(function(){
                this.resetMap(this.getHeight(),this.getWidth(),this.getZoom());
                this.map.resize();
                this.map.reposition();
                setTimeout(function(){
                    this.map.setZoom(this.getZoom());
                    var locationStr = this.getCenterPt().split(",");
                    this._centerMap(locationStr[0],locationStr[1],locationStr[2])
                    $.event.trigger({
                        type: "helper-map-loaded",
                        message: "jQueryHelper map loaded",
                        time: new Date()
                    })
                }.bind(this),this.REINFLATION_DELAY) //resize and reposition need to settle before this fires!
            }.bind(this),this.DEBOUNCE_DELAY)()
        }
        else{
            this.map.resize();
            this.map.reposition();
        }
    }


    /**
     * Deprecated Mar 4, 2014
     */
    this.destroyMap = function(){
        this.map.destroy();
    }

    /**
     * Deprecated Mar 4, 2014
     * @param autoCenter
     * @private
     */
    this._createNewMap = function(/* boolean */ autoCenter){
        var locationStr = this.getCenterPt().split(",");
        if(locationStr instanceof Array){
            var slider = this.slider != null ? "small" : false;
            this.map = new esri.Map(this.mapId, {
                basemap: this.basemap,
                center: [locationStr[1],locationStr[0]], // long, lat
                zoom: this.getZoom(),
                sliderStyle: slider
            });

            this.map.on("load",function(){
                this.map.resize();
                this.map.reposition();
                this._centerMap(locationStr[0],locationStr[1],locationStr[2])
                $.event.trigger({
                    type: "helper-map-loaded",
                    message: "jQueryHelper map loaded",
                    time: new Date()
                })
            }.bind(this));
        }
    }

    this._centerMap = function(/* number */ x, /* number */ y, /* int */ wkid){
        if(!isNaN(x) && !isNaN(y) && !isNaN(wkid)){
            var m_map = this.map;

            require(["esri/geometry/Point","esri/SpatialReference"],function(Point,SpatialReference){
                var wgsPt = null;
                if(wkid == 4326){
                    wgsPt = new Point(y,x);
                }
                else if(wkid = 102100){
                    wgsPt = new Point(x,y, new esri.SpatialReference({ wkid: wkid }));
                }

                m_map.centerAt(wgsPt);
                console.log("map centered");
            });
        }
        else{
            console.log("Null value detected. Is setCenterPt() set?");
        }
    }

    this._getActivePage = function(){
        return $.mobile.activePage[0].id;
    }

    this._init = function(){
        this.currentPageID = this._getActivePage();

        this.setOrientationListener();
        this.setPageChangeListeners(this.currentPageID);

//        Set these listeners from within your application!
//        this.setPanListener();
//        this.setZoomListener();

        this.setWidth((this.map).width);
        this.setHeight((this.map).height);

        var center = this.map.extent.getCenter();
        this.setCenterPt(center.x,center.y,this.map.spatialReference.wkid);
        this.setZoom(this.map.getZoom());

    }.bind(this)()
}