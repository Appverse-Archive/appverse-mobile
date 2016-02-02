
var testI18NKey = "username";
var testI18NLocale = new Object();
testI18NLocale.Language = "en";
testI18NLocale.Country = "US";

//********** UI COMPONENTS

//********** I18N TESTCASES
var TestCase_i18n = [Appverse.I18N,
	[['GetLocaleSupported',''],
	['GetLocaleSupportedDescriptors',''],
	['GetResourceLiteral','{"param1":' + JSON.stringify(testI18NKey) +',"param2":' + JSON.stringify(testI18NLocale) + '}'],
	['GetResourceLiterals','{"param1":' + JSON.stringify(testI18NLocale) + '}']]];

	
//********** HANDLING CALLBACKS
