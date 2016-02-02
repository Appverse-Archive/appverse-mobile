var testModuleContext = {};
testModuleContext.User = "maps";

var testModules = [];
var testModule = {};
var testModuleVersion = {};
testModuleVersion.Major = "1";
testModuleVersion.Minor = "1";
testModuleVersion.Revision = "0";
testModule.Id = "test";
testModule.Version = testModuleVersion;
//testModule.LoadUrl = "http://builder.gft.com/appstore/apploader-test/test-1.1.0.zip";
testModule.LoadUrl = "https://dl.dropboxusercontent.com/u/30557508/apploader-test/test-1.1.0.zip";
testModules[0] = testModule;

var testModule2 = {};
var testModuleVersion2 = {};
testModuleVersion2.Major = "2";
testModuleVersion2.Minor = "4";
testModuleVersion2.Revision = "5";
testModule2.Id = "test2";
testModule2.Version = testModuleVersion2;
//testModule2.LoadUrl = "http://builder.gft.com/appstore/apploader-test/test2-2.4.5.zip";
testModule2.LoadUrl = "https://dl.dropboxusercontent.com/u/30557508/apploader-test/test2-2.4.5.zip";
testModules[1] = testModule2;

var testModuleParameters = [];
testModuleParameters[0] = {};
testModuleParameters[0].Name = "param1";
testModuleParameters[0].Value = "value1";
testModuleParameters[1] = {};
testModuleParameters[1].Name = "param2";
testModuleParameters[1].Value = "value2";

var testModulesToDelete = [];
testModulesToDelete[0] = testModule;

// APP LOADER TEST CASE
var TestCase_AppLoader = [Appverse.AppLoader,
			[['InitializeModuleContext','{"param1":' + JSON.stringify(testModuleContext) + '}'],
			['ListInstalledModules',''],
			['UpdateModules', '{"param1":' + JSON.stringify(testModules) + ',"param2":"UpdateModules"}'],
			['UpdateModule', '{"param1":' + JSON.stringify(testModule) + ',"param2":"UpdateModule"}'],
			['DeleteModules', '{"param1":' + JSON.stringify(testModulesToDelete) + '}'],
			['LoadModule', '{"param1":' + JSON.stringify(testModule) + ',"param2":' + JSON.stringify(testModuleParameters) +',"param3":false}']]
		];

//********** HANDLING REMOTE NOTIFICATIONS

Appverse.AppLoader.onUpdateModulesFinished = function(successUpdates, failedUpdates, callbackId) {
	console.log("calling back from id: " + callbackId);
	console.dir(successUpdates);
	console.log("num of modules sucessful updated: " + (successUpdates?successUpdates.length:0));
	console.dir(failedUpdates);
	console.log("num of modules failed updated: " + (failedUpdates?failedUpdates.length:0));
	var str = "Updated: " + (successUpdates?successUpdates.length:0) + ", Failed: " + (failedUpdates?failedUpdates.length:0);
	//Showcase.app.getController('Main').toast("Updating Modules Finished", false) ;
        Showcase.app.getController('Main').console(feedObj("Appverse.AppLoader.onUpdateModulesFinished","Appverse.AppLoader.onUpdateModulesFinished",str));
	submitCallback(str, "Appverse.AppLoader.onUpdateModulesFinished");
}

Appverse.AppLoader.onDeleteModulesFinished = function(successDeletes, failedDeletes) {
	console.dir(successDeletes);
	console.log("num of modules sucessful deleted: " + (successDeletes?successDeletes.length:0));
	console.dir(failedDeletes);
	console.log("num of modules failed deleted: " + (failedDeletes?failedDeletes.length:0));
	var str = "Deleted: " + (successDeletes?successDeletes.length:0) + ", Failed: " + (failedDeletes?failedDeletes.length:0);
	//Showcase.app.getController('Main').toast("Deleting Modules Finished", false) ;
        Showcase.app.getController('Main').console(feedObj('Appverse.AppLoader.onDeleteModulesFinished','Appverse.AppLoader.onDeleteModulesFinished',str));
	submitCallback(str, "Appverse.AppLoader.onDeleteModulesFinished");
}