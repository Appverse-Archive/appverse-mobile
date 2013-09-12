var newsSearch = 'mobile html5',
        query = '?hl=en&gl=us&q=@@query@@&um=1&ie=UTF-8&output=rss',
        request = {Session: {}},
        services = Unity.IO.GetService('googleRss');

new Ext.regModel('news', {
    fields: [{name: 'title'},
        {name: 'link'}]
});

var newsStore = new Ext.data.Store({
    model: 'news',
    data: []
});

newsRefresh = function newsRefresh() {

    request.Content = query.replace('@@query@@', escape(newsSearch));
    response = Unity.IO.InvokeService(request, services);
    if (!response.Content)
        return;
    data = xml2json.parser(response.Content);

    newsStore.loadData(data.rss.channel.item);
};


Ext.setup({
    onReady: function() {

        newsTpl = new Ext.Template('<div class="title">{title}</div>');

        newsList = new Ext.List({
            //style: 'height:100%;',
            itemTpl: newsTpl,
            store: newsStore,
            cls: 'newsList',
            id: 'newsList',
            layout: 'fit',
            preventSelectionOnDisclose: true,
            listeners: {
                itemtap: function(dataView, index, item, e) {
                    record = dataView.getStore().getAt(index);
                    console.log(record.data.link);
                    url = record.data.link.split('url=');
                    if (url.length > 1) {
                        url = url[1];
                        Unity.Net.OpenBrowser(record.data.title, 'Volver', url);
                    } else {
                        Ext.Msg.alert('No hay enlace.');
                    }
                }
            }
        });

        search = new Ext.form.Search({
            cls: 'searchBar',
            placeHolder: 'Search...',
            useClearIcon: true,
            value: newsSearch,
            listeners: {
                keyup: {
                    element: 'el',
                    fn: function(e) {
                        // Happens when enter is pressed
                        newsSearch = searchBar.getValue();
                        if (e.browserEvent.keyCode == 13) {
                            newsRefresh();
                        }
                    }
                }
            }
        });

        searchTopToolbar = new Ext.Toolbar({
            dock: 'top',
            cls: 'searchToolbar',
            defaults: {iconMask: true},
            layout: {pack: 'center'},
            items: [search]
        });

        viewPort = new Ext.Panel({
            id: 'viewPort',
            cls: 'viewPort',
            fullscreen: true,
            dockedItems: [{
                    xtype: 'toolbar',
                    dock: 'top',
                    id: 'titleBar',
                    cls: 'titleBar',
                    height: 100,
                    layout: {pack: 'center'},
                    items: [
                        {xtype: 'spacer'},
                        {xtype: 'container', html: '<div class="logo"></div>'},
                        {xtype: 'spacer'}]
                },
                searchTopToolbar],
            items: [newsList],
            layout: 'fit'
        });
        newsRefresh();

    }// End onReady
});// End Ext.setup	


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
Unity.backgroundApplicationListener = function() {

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
