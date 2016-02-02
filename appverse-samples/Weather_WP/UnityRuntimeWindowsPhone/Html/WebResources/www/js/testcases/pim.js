
var testContact = new Object();
testContact.ID = "ID_1";
testContact.Name = "appverse";
testContact.Firstname = "MyContactFirstname";
testContact.Lastname = "MyContactLastname";
testContact.DisplayName = "appverse";
testContact.Group = "MyContactGroup";
testContact.Company = "GFT";
testContact.JobTitle = "MyContactJobTitle";
testContact.Department = "MyContactDepartment";
var testContactWebsites = new Array();
testContactWebsites[0] = "http://appverse.org/";
testContact.WebSites = testContactWebsites;
testContact.Notes = "My contact's notes";
var testContactPhones = new Array();
var testContactPhone1 = new Object();
testContactPhone1.Type = 1; //Other, Mobile, FixedLine, Work, HomeFax, WorkFax, Pager
testContactPhone1.Number = "+34666111111";
testContactPhones[0] = testContactPhone1;
var testContactPhone2 = new Object();
testContactPhone2.Type = 0; //Other, Mobile, FixedLine, Work, HomeFax, WorkFax, Pager
testContactPhone2.IsPrimary = true;
testContactPhone2.Number = "+34666222222";
testContactPhones[1] = testContactPhone2;
testContact.Phones = testContactPhones;
var testContactEmails = new Array();
var testContactEmail1 = new Object();
testContactEmail1.Type = 2; //Other, Personal, Work, HomeOffice
testContactEmail1.Address = "unityversal@gmail.com";
testContactEmails[0] = testContactEmail1;
testContact.Emails = testContactEmails;
var testContactAddresses = new Array();
var testContactAddress1 = new Object();
testContactAddress1.Type = 3; // other(0), personal(1), work (2), homeoffice(3)
testContactAddress1.Address = "Park Avenue";
testContactAddress1.AddressNumber = "43";
testContactAddress1.City = "New York";
testContactAddress1.Country = "United States";
testContactAddress1.PostCode = "08210";
testContactAddresses[0] = testContactAddress1;
var testContactAddress2 = new Object();
testContactAddress2.Type = 2; // other(0), personal(1), work (2), homeoffice(3)
testContactAddress2.Address = "Vial Interpolar";
testContactAddress2.AddressNumber = "s/n";
testContactAddress2.City = "Sant Cugat";
testContactAddress2.Country = "Spain";
testContactAddress2.PostCode = "08174";
testContactAddresses[1] = testContactAddress2;
testContact.Addresses = testContactAddresses;
testContact.PhotoBase64Encoded = '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';
/*  DO NOT USE "Photo" FIELD AS BYTE[] TYPES ARE NOT WELL DECODED ON JSON PARSERS
var directoryRootPathForDocuments = Appverse.FileSystem.GetDirectoryRoot().FullName;
var documentsDirectoryIndexof = directoryRootPathForDocuments.indexOf("/Documents");
var directoryRootPathForApp = directoryRootPathForDocuments.substring(0,documentsDirectoryIndexof);
var contactPhotoPath = directoryRootPathForApp + "/AppverseUI.app/WebResources/www/images/rainy.png";
var contactPhotoFileData = new Object();
contactPhotoFileData.FullName = contactPhotoPath;
testContact.Photo = Appverse.FileSystem.ReadFile(contactPhotoFileData);
*/

var testContactQuery = {};
testContactQuery.Column = Appverse.Pim.CONTACTS_QUERY_COLUMN_NAME;
testContactQuery.Value = "appverse";
testContactQuery.Condition = Appverse.Pim.CONTACTS_QUERY_CONDITION_CONTAINS;

var testContactQuery_PhoneAvailable = {};
testContactQuery_PhoneAvailable.Column = Appverse.Pim.CONTACTS_QUERY_COLUMN_PHONE;
testContactQuery_PhoneAvailable.Condition = Appverse.Pim.CONTACTS_QUERY_CONDITION_AVAILABLE;

var newTestContact = eval('('+JSON.stringify(testContact)+')'); // clone object.
newTestContact.Name = "UpdatedName";

var testCalendarDate = new Object();
testCalendarDate.Year = 2014;
testCalendarDate.Month = 6;
testCalendarDate.Day = 22;
testCalendarDate.Hour = 19;
testCalendarDate.Minute = 15;
testCalendarDate.Second = 30;

var testCalendarStartDate = new Object();
testCalendarStartDate.Year = 2014;
testCalendarStartDate.Month = 6;
testCalendarStartDate.Day = 20;

var testCalendarEndDate = new Object();
testCalendarEndDate.Year = 2014;
testCalendarEndDate.Month = 11;
testCalendarEndDate.Day = 30;

var testCalendarNewStartDate = new Object();
testCalendarNewStartDate.Year = 2014;
testCalendarNewStartDate.Month = 8;
testCalendarNewStartDate.Day = 28;
testCalendarNewStartDate.Hour = 9;
testCalendarNewStartDate.Minute = 25;
testCalendarNewStartDate.Second = 30;

var testCalendarNewEndDate = new Object();
testCalendarNewEndDate.Year = 2014;
testCalendarNewEndDate.Month = 11;
testCalendarNewEndDate.Day = 31;
testCalendarNewEndDate.Hour = 16;
testCalendarNewEndDate.Minute = 35;
testCalendarNewEndDate.Second = 40;


var testCalendarEntry = new Object();
testCalendarEntry.Uid = "Uid_1";
testCalendarEntry.Type = 1; //Other(0), Birthday(1), CalDAV(2), Exchange(3), IMap(4), Local(5), Subscription(6)
testCalendarEntry.Title = "Calendar Entry Title";
testCalendarEntry.IsEditable = true;
testCalendarEntry.IsAllDayEvent = true;
testCalendarEntry.Location = "Event's Location";
testCalendarEntry.StartDate = testCalendarStartDate;
testCalendarEntry.EndDate = testCalendarEndDate;
testCalendarEntry.Notes ="Notes for calendar entry";
testCalendarEntry.Attendees = new Array();
var testAttendee1 = new Object();
testAttendee1.Name = "Attendee_1 name";
testAttendee1.Address = "Attendee_1 address";
testAttendee1.Status = 1; // NeedsAction(0), Accepted(1), Declined(2), Tentative(3)
testCalendarEntry.Attendees[0] = testAttendee1;
var testAttendee2 = new Object();
testAttendee2.Name = "Attendee_2 name";
testAttendee2.Address = "Attendee_2 address";
testAttendee2.Status = 0;
testCalendarEntry.Attendees[1] = testAttendee2;
testCalendarEntry.Alarms = new Array();
testAlarm1 = new Object();
testAlarm1.Trigger = new Object()
testAlarm1.Trigger.Year = 2014;
testAlarm1.Trigger.Month = 7;
testAlarm1.Trigger.Day = 29;
testAlarm1.EmailAddress = "email@address.alarm1";
testAlarm1.Sound = "Alarm_1 sound";
testAlarm1.Action = 0; //Dispaly(0), Email(1), Sound(2)
testCalendarEntry.Alarms[0] = testAlarm1;
testAlarm2 = new Object();
testAlarm2.Trigger = new Object()
testAlarm2.Trigger.Year = 2014;
testAlarm2.Trigger.Month = 11;
testAlarm2.Trigger.Day = 29;
testAlarm2.EmailAddress = "email@address.alarm2";
testAlarm2.Sound = "Alarm_2 sound";
testAlarm2.Action = 1; //Dispaly(0), Email(1), Sound(2)
testCalendarEntry.Alarms[1] = testAlarm2;
testCalendarEntry.IsRecurrentEvent = true;
testCalendarEntry.RecurrenceNumber = 1;
testCalendarEntry.Recurrence = new Object();
testCalendarEntry.Recurrence.Type = 0 //Weekly(0), Fortnightly(1), FourWeekly(2), Montly(3), Yearly(4)
testCalendarEntry.Recurrence.Interval = 2;
//testCalendarEntry.Recurrence.DayOfTheWeek = 3;
//testCalendarEntry.Recurrence.EndDate = new Object();
//testCalendarEntry.Recurrence.EndDate.Year = 2014;
//testCalendarEntry.Recurrence.EndDate.Month = 7;
//testCalendarEntry.Recurrence.EndDate.Day = 29;

//********** UI COMPONENTS



//********** PIM TESTCASES
var TestCase_Pim_Contacts = [Appverse.Pim,
	[['ListContacts','{"param1":' + JSON.stringify(testContactQuery) + '}'],
	['ListContacts#ALL','{"param1":null}'],
	['ListContacts#PhoneAvailable','{"param1":' + JSON.stringify(testContactQuery_PhoneAvailable) + '}'],
	['GetContact','{"param1":"ID"}'],
	['CreateContact','{"param1":' + JSON.stringify(testContact) + '}'],
	['UpdateContact','{"param1":' + JSON.stringify(testContact.ID) +',"param2":' + JSON.stringify(newTestContact) + '}'],
	['DeleteContact','{"param1":' + JSON.stringify(testContact) + '}']]];
	
var TestCase_Pim_Calendar = [Appverse.Pim,
	[['ListCalendarEntriesByDate','{"param1":' + JSON.stringify(testCalendarDate) + '}'],
	['ListCalendarEntriesByDateRange','{"param1":' + JSON.stringify(testCalendarStartDate) +',"param2":' + JSON.stringify(testCalendarEndDate) + '}'],
	['CreateCalendarEntry','{"param1":' + JSON.stringify(testCalendarEntry) + '}'],
	['DeleteCalendarEntry','{"param1":' + JSON.stringify(testCalendarEntry) + '}'],
	['MoveCalendarEntry','{"param1":' + JSON.stringify(testCalendarEntry) +',"param2":' + JSON.stringify(testCalendarNewStartDate) + ',"param3":' + JSON.stringify(testCalendarNewEndDate) + '}']]];	
	
//********** HANDLING CALLBACKS

Appverse.Pim.onListContactsEnd = function(contacts) {
	//Showcase.app.getController('Main').toast("Contacts found: " + (contacts!=null?contacts.length:0));
        Showcase.app.getController('Main').console(feedObj("Appverse.Pim.onListContactsEnd","Appverse.Pim.onListContactsEnd",contacts));//.length?contacts:"No Contact Found"
};

Appverse.Pim.onContactFound = function(contact) {
	console.log("Appverse.Pim.onContactFound");
	//submitCallback(contact, "Appverse.Pim.onContactFound");
        Showcase.app.getController('Main').console(feedObj("Appverse.Pim.onContactFound","Appverse.Pim.onContactFound",contact));
};

Appverse.Pim.onListCalendarEntriesEnd = function(calendarEntries) {
	//Showcase.app.getController('Main').toast("Calendar Entries found: " + (calendarEntries!=null?calendarEntries.length:0));
        Showcase.app.getController('Main').console(feedObj("Appverse.Pim.onListCalendarEntriesEnd","Appverse.Pim.onListCalendarEntriesEnd","Calendar Entries found: " + (calendarEntries!=null?calendarEntries.length:0)));
};

Appverse.Pim.onAccessToContactsDenied = function() {
	console.log("Appverse.Pim.onAccessToContactsDenied");
	Showcase.app.getController('Main').console(feedObj("Appverse.Pim.onAccessToContactsDenied","Appverse.Pim.onAccessToContactsDenied","Access to Contacts denied by user"));
};


