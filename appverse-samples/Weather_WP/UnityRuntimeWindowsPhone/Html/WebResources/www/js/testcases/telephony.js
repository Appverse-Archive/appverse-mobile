
var testCallNumber = "+34666111111";
var testCallType = 0; //Voice(0), Fax(1), DialUp(2)

//********** UI COMPONENTS

//********** MEDIA TESTCASES
var TestCase_Telephony = [Appverse.Telephony,
	[['Call','{"param1":' + JSON.stringify(testCallNumber) +',"param2":' + JSON.stringify(testCallType) + '}']]];	

	
//********** HANDLING CALLBACKS
