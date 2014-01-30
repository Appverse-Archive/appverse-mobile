
/*
 * Overrided Sencha Platform utilities using Appverse Platform.
 */
Ext.is = Appverse.is;

/**
 * @ignore
 * METHOD OVERRIDDEN FROM unity.js FILE. THIS IS DONE TO AVOID ONCHANGEORIENTATION LAYOUT ERRORS FOR ALL APPLICATIONS.
 * Related JIRA ticket (APPSENCHAUI-6)
 * @method
 * @version 2.1
 */
var refreshOrientation = function() {
	Ext.getBody().setWidth(window.innerWidth);
	Ext.getBody().setHeight(window.innerHeight);
	Appverse.setOrientationChange( Appverse.getCurrentOrientation() , window.innerWidth , window.innerHeight );
}

// FIX ERROR ON SENCHA 1.1.0.. view port is not correctly being applied for android tablets (window.orientation has no the expected values)
Ext.Viewport.getOrientation = function() {
	var size = this.getSize();
	//console.log("***** viewport OVERRIDED getOrientation() method sw:" + size.width + "- sh: " + size.height+ "slw:" + this.lastSize.width + "- slh: " + this.lastSize.height + "this.orientation: " + this.orientation);
	//console.log("***** viewport OVERRIDED getOrientation(), window.orientation: " + window.orientation);
	if (window.hasOwnProperty('orientation')) {
		//console.log("***** viewport OVERRIDED getOrientation() method hasOwnProperty, window.orientation: " + window.orientation);
		if(Ext.is.Android && Ext.is.Tablet) {
			return (window.orientation == 90 || window.orientation == -90) ? 'portrait' : 'landscape';
		}
	    return (window.orientation == 0 || window.orientation == 180) ? 'portrait' : 'landscape';
    }
    else {
        if (!Ext.is.iOS && !Ext.is.Desktop) {
            if ((size.width == this.lastSize.width && size.height < this.lastSize.height) ||
                (size.height == this.lastSize.height && size.width < this.lastSize.width)) {
				//console.log("***** viewport OVERRIDED getOrientation() method this.orientation: " + this.orientation);
                return this.orientation;
            }
        }
		//console.log("***** viewport OVERRIDED getOrientation(): H:" + window.innerHeight +"- W:"+ window.innerWidth );
		return (window.innerHeight > window.innerWidth) ? 'portrait' : 'landscape';
    }

}

/*
 * Namespace for all Appverse Sencha UI components.
 */
Ext.ns('Appverse.ui');

/**
 * @class Appverse.ui.UniversalUI
 * @namespace Appverse.ui
 * @extends Ext.Panel
 * Universal UI component. Different behaviour on Phone - iPhone, Android, etc - than on Tablet - iPad, etc. <br/>
 * @author Marga Parets maps@gft.com
 * @version 1.0
 */
Appverse.ui.UniversalUI = Ext.extend(Ext.Panel, {
    /**
     * @cfg {Boolean} fullscreen Force the component to take up 100% width and height available. Defaults to true.
     */
    fullscreen: true,
    /**
     * @cfg {String/Object} layout Specify the layout manager to be used. Defaults to 'card'. 
     */
    layout: 'card',
    /**
     * @cfg {String} backText The default text for the navigation back button. Defaults to 'Back'.
     */
    backText: 'Back',
    /**
     * @cfg {String} navigationText The default text for the navigation button (only visible for Table landscape mode). Defaults to 'Navigation'.
     */
    navigationText: 'Navigation',
    /**
     * @cfg {Boolean} useTitleAsBackText Set this configuration property as false to use the default back  text (see {@link Appverse.ui.UniversalUI#backText backText})as the Back button inner text, or set it as false to use parent card title. Defaults to false.
     */
    useTitleAsBackText: false,
    /**
     * @cfg {Number} navigationPanelWidth The width (in pixels) of the navigation panel to be shown on Tablet case. 
     */
    navigationPanelWidth: 250,
    /**
     * @cfg {Number} navigationPanelWidth The height (in pixels) of the navigation panel to be shown on Tablet case. 
     */
    navigationPanelHeight: 500,
    /**
     * Component initialization method.
     */
    initComponent : function() {
        /**
         * @property navigationButton The Navigation button on the Navigation Bar (only visible for Table landscape mode).
         * @type Ext.Button
         */
    	this.navigationButton = new Ext.Button({
            hidden: Ext.is.Phone || Ext.orientation == 'landscape',
            text: this.navigationText,
            handler: this.onNavButtonTap,
            scope: this
        });
        
        /**
         * @property backButton The Back button on the Navigation Bar.
         * @type Ext.Button
         */
        this.backButton = new Ext.Button({
            hidden: true,
            text: this.backText,
            handler: this.onBackButtonTap,
            hidden: true,
            scope: this
        });
        
        var btns = [this.navigationButton];
        if (Ext.is.Phone) {
            btns.unshift(this.backButton);
        }
        
        /**
         * @property navigationBar The Navigation Bar component rendered as a Top Light Toolbar.
         * @type Ext.Toolbar
         */
        this.navigationBar = new Ext.Toolbar({
            ui: 'light',
            dock: 'top',
            title: this.title,
            items: btns.concat(this.buttons || [])
        });
        
        /**
         * @property navigationPanel <p>The Navigation Panel used as the component navigation menu. It is not always visible on screen, depends on device model and orientation.</p>
         * <div class="mdetail-params">This panel is shown as:<ul>
         * <li>'left docked' panel for Tablet Landscape devices.</li>
         * <li>'floatable' panel for Tablet Portrait devices - when user clicks on navigation button.</li>
         * <li>'fullscreeen' scrollable panel for Phone devices.</li>
         *</ul></div>
         * @type Ext.Panel
         */
        this.navigationPanel = new Ext.Panel({
		    layout: 'fit',
		    dock: 'left',
		    listeners: {
                /**
                 * @event listchange Fires when user changes selection on main navigation panel items list.
                 * @param {Ext.util.MixedCollection} list The current navigation panel items list.
                 * @param {Ext.Component} item The item to be displayed configured as: {card:{@link Ext.Component Ext.Component} - the component to be displayed, text:{@link String String} - the title of navigation bar when component is displayed}.
                 */
                listchange: this.onListChange,
                scope: this
            },
            title: this.title,
            items: this.items || []
		});
		
		if (!Ext.is.Phone) {
            this.navigationPanel.setWidth(this.navigationPanelWidth);
        }
        
        this.defaultDockedToolbar = new Ext.Toolbar({
			        xtype: 'toolbar',
			        ui: 'light',
			        dock: 'top'
				});
        
        this.dockedItems = this.dockedItems || [];
        this.dockedItems.unshift(this.navigationBar);
        
        if (!Ext.is.Phone && Ext.orientation == 'landscape') { 
            this.navigationPanel.insertDocked(0, this.defaultDockedToolbar);
            this.dockedItems.unshift(this.navigationPanel);
            this.items = [this.tabletLandscapeLaunchScreen || {}];
        }
        else if (Ext.is.Phone) {
            this.items = [];
            this.items.unshift(this.navigationPanel);
        } else {
        	this.navigationPanel.hide();
        	this.navigationPanel.setFloating(true);
        	this.navigationPanel.setHeight(this.navigationPanelHeight);
        	this.items = [this.tabletLandscapeLaunchScreen || {}];
        }
        
        this.addEvents(
            /**
             * @event navigate Fires when user navigates through this UniversalUI component elements - forward or backward
             * @param {Appverse.ui.UniversalUI} this
             * @param {Ext.data.Record} record The list record this component is navigating to.
             */
            'navigate'
        );
        Appverse.ui.UniversalUI.superclass.initComponent.call(this);
    },
    
    onListChange : function(list, item) {
    	if (Ext.getOrientation() == 'portrait' && !Ext.is.Phone && !item.items && !item.preventHide) {
        	this.navigationPanel.hide();
        }
        
        if (item.card) {
        	var backButtonText = item.card.parentTitle;
        	this.setActiveItem(item.card, item.animation || 'slide');
            this.currentCard = item.card;
            if (item.text) {
                this.navigationBar.setTitle(item.text);
            }
            if (Ext.is.Phone) {
            	if(this.useTitleAsBackText) {
            		this.backButton.setText(backButtonText);
            	}
                this.backButton.show();
                this.navigationBar.doLayout();
            }
        }     
       
        this.fireEvent('navigate', this, item, list);
    },
    
    onNavButtonTap : function() {
        this.navigationPanel.showBy(this.navigationButton, 'fade');
    },
    
    onBackButtonTap : function() {
    	
    	this.setActiveItem(this.navigationPanel, {type: 'slide', direction: 'right'});
        this.currentCard = this.navigationPanel;
        if (Ext.is.Phone) {
            this.backButton.hide();
            this.navigationBar.setTitle(this.currentCard.title);
            this.navigationBar.doLayout();
        }
        
        this.fireEvent('navigate', this, this.navigationPanel.activeItem, this.navigationPanel);
    },
    
    layoutOrientation : function(orientation, w, h) {
        if (!Ext.is.Phone) {
			
			if (orientation == 'portrait') {
                this.navigationPanel.hide(false);
				try {
					// prevent errors when removing docked items not already docked
					this.removeDocked(this.navigationPanel, false);
				}	catch(e) {
					console.log("WARNING on UnityVersalUI layoutOrientation() method: "+e);
				}
				
				if (this.navigationPanel.rendered) {
                    this.navigationPanel.el.appendTo(document.body);
                }
                this.navigationPanel.setFloating(true);
                this.navigationPanel.setHeight(this.navigationPanelHeight);
                if(this.navigationPanel.getDockedItems() && this.navigationPanel.getDockedItems().length>0) {
					this.navigationPanel.removeDocked(this.defaultDockedToolbar, false);
				}
				this.navigationButton.show(false);
            }
            else {
                if(!this.navigationPanel.getDockedItems() || this.navigationPanel.getDockedItems().length<=0) {
            		this.navigationPanel.insertDocked(0, this.defaultDockedToolbar);
				}
				this.navigationPanel.setFloating(false);
                this.navigationPanel.show(false);
                this.navigationButton.hide(false);
                this.insertDocked(0, this.navigationPanel);
            }
            this.navigationBar.doComponentLayout();
        }
		
        Appverse.ui.UniversalUI.superclass.layoutOrientation.call(this, orientation, w, h);
    }
});

Ext.apply(Ext.anims, {
    rotate: new Ext.Anim({
        autoClear: false,
        out: false,
        before: function(el) {
            var d = '';
            if (this.dir == 'ccw'){
              d = '-';
            }

            this.from = {
                '-webkit-transform': 'rotate('+d+''+this.fromAngle+'deg)'
            };

            this.to = {
                '-webkit-transform': 'rotate('+d+''+this.toAngle+'deg)'
            };
                        
        }
    })
}); 

/**
 * @class Appverse.ui.ListPullRefresh
 * @namespace Appverse.ui
 * @extends Ext.util.Observable
 * A plugin to add 'Pull to Refresh' functionality to a List component or just for a panel. <br/>
 * @author Marga Parets maps@gft.com
 * @version 2.0
 */
Appverse.ui.ListPullRefresh = Ext.extend(Ext.util.Observable, {
  constructor: function(config){
    Ext.apply(this,config);
    this.addEvents({
      'released': true
    });
    Appverse.ui.ListPullRefresh.superclass.constructor.call(this, config);
  },
  /*
   * @cfg {String} langPullRefresh The text that will be shown while you are pulling down the list or panel this plugin is added to.
   */
  langPullRefresh: 'Pull down to refresh...',
  /*
   * @cfg {String} langReleaseRefresh The text that will be shown after you have pulled down enough to show the release message.
   */
  langReleaseRefresh: 'Release to refresh...',
  /*
   * @cfg {String} langLoading The text that will be shown while the list is refreshing.
  */
  langLoading: 'Loading...',
  loading: false,
  // private
  init: function(cmp){
    this.cmp = cmp;
    this.lastUpdate = new Date();
    //cmp.loadingText = undefined;
    cmp.on('render', this.initPullHandler, this);
  },
  // private
  initPullHandler: function(){
    this.pullTpl = new Ext.XTemplate(
      '<div class="pullrefresh" style="height: {h}; text-align: bottom;">'+
        '<div class="msgwrap" style="height: 75px; bottom: 0px; position: relative;">'+
          '<span class="arrow {s}"></span>'+
          '<span class="msg">{m}</span>'+
          '<span class="lastupdate">Last Updated: {[Ext.util.Format.date(values.l,"m/d/Y h:iA")]}</span>'+
        '</div>'+
      '</div>');
    this.loadingTpl = new Ext.XTemplate(
      '<div class="loadingspacer" style="height: {h}; text-align: bottom;">'+
        '<div class="msgwrap" style="height: 75px; bottom: 0px; position: relative;">'+
          '<span class="loading {s}"></span>'+
          '<span class="msg">{m}</span>'+
        '</div>'+
      '</div>');
    this.pullEl = this.pullTpl.insertBefore(this.cmp.scroller.el, {h:0,m:this.langPullRefresh,l:this.lastUpdate}, true);
    this.pullEl.hide();
    Ext.Element.cssTranslate(this.pullEl, {x:0, y:-75});
    this.loadingEl = this.loadingTpl.insertBefore(this.pullEl, {h:0,m:this.langLoading}, true);
    this.loadingEl.hide();
    this.cmp.scroller.on('offsetchange', this.handlePull, this);
  },
  //private
  handlePull: function(scroller, offset){
    if (scroller.direction === 'vertical' && !this.loading){
      if (offset.y > 0){
        Ext.Element.cssTranslate(this.pullEl, {x:0, y:offset.y-75});
        if (offset.y > 75){
          // state 1
          if (this.state !== 1){
            this.prevState = this.state;
            this.state = 1;
            this.pullTpl.overwrite(this.pullEl, {h:offset.y,m:this.langReleaseRefresh,l:this.lastUpdate});
            Ext.Anim.run(this.pullEl.select('.arrow').first(),'rotate',{dir:'ccw',fromAngle:0,toAngle:180});
          }
        }else if (!scroller.isDragging()){
          // state 3
          if (this.state !== 3){
            this.prevState = this.state;
            this.state = 3;
            if (this.prevState == 1){
              this.loading = true;
              this.lastUpdate = new Date();
              this.pullEl.hide();
              this.loadingTpl.overwrite(this.loadingEl, {h:offset.y,m:this.langLoading});
              this.loadingEl.show();
              this.fireEvent('released',this,this.cmp);
            }
          }
        }else{
          // state 2
          if (this.state !== 2){
            this.prevState = this.state;
            this.state = 2;
            this.loadingEl.hide();
            this.pullTpl.overwrite(this.pullEl, {h:offset.y,m:this.langPullRefresh,l:this.lastUpdate});
            this.pullEl.show();
            if (this.prevState == 1){
              Ext.Anim.run(this.pullEl.select('.arrow').first(),'rotate',{dir:'cw',fromAngle:180,toAngle:0});
            }
          }
        }
      }
    }
  },
  //private
  processComplete: function(){
    this.loading = false;
    this.lastUpdate = new Date();
    this.loadingTpl.overwrite(this.loadingEl, {h:0,m:this.langLoading});
    this.pullTpl.overwrite(this.pullEl, {h:0,m:this.langPullRefresh,l:this.lastUpdate});
  }
});

Ext.preg('listpullrefresh', Appverse.ui.ListPullRefresh);

// to keep compatibility (just for 4.3 version). Should be DEPRECATED in next releases.
Unity.ui = Appverse.ui;


