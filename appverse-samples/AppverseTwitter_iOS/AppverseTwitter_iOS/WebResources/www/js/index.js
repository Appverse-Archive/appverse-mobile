/**
 *  Project Name
 * 
 */

var PROJECTROOTOBJ = {}; 

var i18nController =  {
    currentLocale : null
};

i18nController.localizeText = function(key) {
    return Unity.I18N.GetResourceLiteral(key, this.currentLocale);
};

new Ext.regModel('tweet',{
	fields: ['text','from_user_name', 'profile_image_url','id_str','from_user_id_str']
});

var tweetStore = new Ext.data.Store({
	model: 'tweet',
	data: []
});

markerArg = 'marker.png';

markerURL = markerArg;

Ext.setup({	

    onReady: function() {
    
		PROJECTROOTOBJ.ui = new PROJECTROOTOBJ.UI();
        PROJECTROOTOBJ.ct = new PROJECTROOTOBJ.Controller();
        PROJECTROOTOBJ.ui.cacheElements();
		
		PROJECTROOTOBJ.ui.buildBasePanel({
			id: 'basePanel',
			cls: 'basePanel',
			fullscreen: true,
			dockedItems: [PROJECTROOTOBJ.ui.getView('toolbar'),PROJECTROOTOBJ.ui.getView('searchToolBar'), PROJECTROOTOBJ.ui.getView('botToolbar')],
			listeners:{
				afterlayout: function(){
                                        PROJECTROOTOBJ.ct.tweetRefresh();
				},
                                afterrender: function(){
                                        google.maps.event.addListener(PROJECTROOTOBJ.ui.getView('tweetMap').map, "mousedown", function(e) {
                                            PROJECTROOTOBJ.config.tweetBubble.close();
                                        });
				}
			},
			items: [PROJECTROOTOBJ.ui.getView('tweetList'), PROJECTROOTOBJ.ui.getView('tweetMap')],
			layout: 'card',
                        cardSwitchAnimation : 'slide'
		});
		
		Unity.System.DismissSplashScreen();
	
	// End onReady
	}
// End Ext.setup	
});


/***** Unity Overrided Functions to provide Orientation Features ****/
Unity.getCurrentOrientation = function() { 
    
};

Unity.setOrientationChange = function(orientation, width, height) { 	
   
};

/**
 * Applications should override/implement this method to be aware of application being send to background, and should perform the desired javascript code on this case.
 * @method
 * @version 2.0
 * 
 */
Unity.backgroundApplicationListener= function() {
	
};


/**
 * Applications should override/implement this method to be aware of application coming back from background, and should perform the desired javascript code on this case.
 * @method
 * @version 2.0
 * 
 */
Unity.foregroundApplicationListener = function() {
	
};



/***********************  end overrinding **************************/
