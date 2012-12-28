PROJECTROOTOBJ.UI = function(){
    console.log("View -- Constructor() ","PROJECTROOTOBJ.UI has been instantiated");
    this.objectViews = {};
    return this;
};

var searchBar = null;
var searchLocationBar = null;

PROJECTROOTOBJ.UI.prototype.cacheElements = function(){  
    // Elements to be rendered beforehand
	this._topToolbar();
	this._botToolbar();
	this._tweetList();
        this._tweetMap();
};

PROJECTROOTOBJ.UI.prototype.buildBasePanel = function(params){
    var basePanel = new Ext.Panel(params);
	
    this._registerView(basePanel, 'basePanel');
    return basePanel;
};

PROJECTROOTOBJ.UI.prototype._topToolbar = function(){
    
        searchBar = new Ext.form.Search({
                cls:'searchBar',
                placeHolder: 'Search...',	
                useClearIcon:true,
                value: PROJECTROOTOBJ.config.twitterQuery,
                listeners: {
                    keyup: {
                        element: 'el',
                        fn: function(e){    
                            if(e.browserEvent.keyCode == 13){ // Happens when enter is pressed
                                // get tweets using entered query
                                // Ext.Msg.alert(searchBar.getValue());
                                PROJECTROOTOBJ.config.twitterQuery = searchBar.getValue();
                                PROJECTROOTOBJ.ct.tweetRefresh();
                                
                            }
                        }
                    }
                }
        });
        
        searchLocationBar = new Ext.form.Search({
                cls:'searchBar',
                placeHolder: 'Current Location...',	
                useClearIcon:true,
                //value: PROJECTROOTOBJ.config.twitterQuery,
                hidden: true,
                listeners: {
                    keyup: {
                        element: 'el',
                        fn: function(e){    
                            if(e.browserEvent.keyCode == 13){ // Happens when enter is pressed
                                // get tweets using entered query
                                //Ext.Msg.alert(searchLocationBar.getValue());
                                PROJECTROOTOBJ.config.searchLocation = searchLocationBar.getValue();
                                PROJECTROOTOBJ.ct.tweetRefresh();
                                
                            }
                        }
                    }
                }
        });
        
        var searchTopToolbar = new Ext.Toolbar({
                dock:'top',
                cls: 'searchToolbar',
                defaults:{iconMask:true},
                layout:{pack:'center'},
		items:[searchBar, searchLocationBar]
        });
    
	var toolbar = new Ext.Toolbar({
		dock: 'top',
		title: 'Appverse',
                // **** 1) ADD ICON ON THE TOOLBAR
                defaults:{iconMask:true},
                layout:{pack:'right'},
                 items: [
                    {
                        iconCls: 'locate',
                        handler: function() { 
                            PROJECTROOTOBJ.ct.cardNavigation();
                        }
                    }
                ]
	});
	
	this._registerView(toolbar, 'toolbar');
        this._registerView(searchTopToolbar, 'searchToolBar');
	return toolbar;
}

PROJECTROOTOBJ.UI.prototype._botToolbar = function(){
	var toolbar = new Ext.Toolbar({
		dock:'bottom',
                defaults:{iconMask:true, ui:'plain'},
                layout:{pack:'right'} 
                /*,
                 items: [
                    {
                        iconCls: 'info',
                        handler: function() { Ext.Msg.alert('AdaptiveMe', 'Follow me at @adaptiveme', Ext.emptyFn);}
                    }
                ]
                */
	});
	
	//this._registerView(toolbar, 'botToolbar');
	return toolbar;
}

PROJECTROOTOBJ.UI.prototype._tweetList = function(){
    
    var tweetListTpl_tablet_class = ''; //(Unity.is.Tablet?'_tablet':'');
	    
	var list = new Ext.List({
		style: 'height:100%;',
		itemTpl: '<div><div class="tweetImage'+tweetListTpl_tablet_class+'"><img src="{profile_image_url}" /></div><div class="tweetText'+tweetListTpl_tablet_class+'"><div>@{from_user_name}</div><div>{text}</div></div></div>',
		store: tweetStore,
        cls: 'tweetList',
        id: 'tweetList',
        preventSelectionOnDisclose: true,
        listeners:{
        	itemtap:function( DataView, index, item, e){
        		record = DataView.getStore().getAt(index);
        		var id = record.get('id_str');
    			var from = record.get('from_user_id_str');
    			var url = "https://mobile.twitter.com/"+from+"/status/"+id;
    			console.log(url);
        		Unity.Net.OpenBrowser( 'twitter', 'Ok', url );
        	}
        }
	});
	
	this._registerView(list, 'tweetList');
	return list;
}

PROJECTROOTOBJ.UI.prototype._tweetMap = function(){
	var mapPanel = new Ext.Map({
                title: 'Map',
                //location is collect from Unity, not from the browser.... useCurrentLocation: true,
                mapOptions: {streetViewControl:false, zoom:10, mapTypeControl:false}, ///, center: latlng,
                id: 'tweetMap'
        });
        
        this._registerView(mapPanel, 'tweetMap');
	return mapPanel;
}


/**************************************************
 **************************************************
 ******** Component manipulation functions ********
 **************************************************
 *************************************************/

PROJECTROOTOBJ.UI.prototype.getView = function(viewId){
    if(this.objectViews[viewId]){
        //console.log("View -- getView() ","returning object: " + viewId,viewId);
        return this.objectViews[viewId];
    }else{
        //console.warn("View -- getView() ","Object not found: " + viewId + " it will return an empty object");
        return {};
    }
}

PROJECTROOTOBJ.UI.prototype._registerView = function(obj, id){
    if(this.objectViews[id]){
        //console.log("View -- registerView() ","this id - " + id + " - already exists the object cannot be registered");
    }else{
        this.objectViews[id] = obj;
        //console.log("View -- registerView() ","object registered with id: " + id);
    }
}