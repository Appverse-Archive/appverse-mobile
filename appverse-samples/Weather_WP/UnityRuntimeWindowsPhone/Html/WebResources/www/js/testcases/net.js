
var testRemoteFileUrl = "http://dl.dropbox.com/u/12811767/test.pdf";
var testUrl = "www.gft.com";

var testBrowser = new Object();
testBrowser.title = "title";
testBrowser.buttonText = "buttonText";
testBrowser.url = "http://www.google.com";
testBrowser.html = "<html> \
<body> \
<address> \
Written by GFT.com<br /> \
<a href='mailto:barcelona@gft.com'>Email GFT</a><br /> \
Address: Avinguda de la Generalitat 163-167, 08174 Sant Cugat<br /> \
Phone: +34 93 5659-100 \
</address> \
</body> \
</html>";

var testSecondaryBrowserOptions = new Object();
testSecondaryBrowserOptions.Title = "My title";
testSecondaryBrowserOptions.Url = "https://dl.dropboxusercontent.com/u/172657936/HUB_downloads/testwindowopen.html";
testSecondaryBrowserOptions.CloseButtonText = "buttonText2";
testSecondaryBrowserOptions.Html = "<html> \
<body> \
<address> \
Written by GFT.com<br /> \
<a href='mailto:barcelona@gft.com'>Email GFT</a><br /> \
Address: Avinguda de la Generalitat 163-167, 08174 Sant Cugat<br /> \
Phone: +34 93 5659-100 \
</address> \
</body> \
</html>";
var fileExtensions = new Array();
fileExtensions[0] = ".pdf";
testSecondaryBrowserOptions.BrowserFileExtensions = fileExtensions;

//********** UI COMPONENTS

//********** NETWORK TESTCASES
var TestCase_Network = [Appverse.Net,
	[['DownloadFile','{"param1":' +  JSON.stringify(testRemoteFileUrl) + '}'],
	['GetNetworkData',''],
	['GetNetworkTypeReachable','{"param1":' +  JSON.stringify(testUrl) + '}'],
	['GetNetworkTypeReachableList','{"param1":' +  JSON.stringify(testUrl) + '}'],
	['GetNetworkTypeSupported',''],
	['IsNetworkReachable','{"param1":' +  JSON.stringify(testUrl) + '}'],
	['OpenBrowser', '{"param1":' +  JSON.stringify(testBrowser.title) 
		+ ',"param2":' + JSON.stringify(testBrowser.buttonText) 
		+ ',"param3":' + JSON.stringify(testBrowser.url) + '}'],
	['OpenBrowserWithOptions','{"param1":' +  JSON.stringify(testSecondaryBrowserOptions) + '}'],
	['ShowHtml', '{"param1":' +  JSON.stringify(testBrowser.title) 
		+ ',"param2":' + JSON.stringify(testBrowser.buttonText) 
		+ ',"param3":' + JSON.stringify(testBrowser.html) + '}'],
	['ShowHtmlWithOptions','{"param1":' +  JSON.stringify(testSecondaryBrowserOptions) + '}']]];
	
//********** HANDLING CALLBACKS
