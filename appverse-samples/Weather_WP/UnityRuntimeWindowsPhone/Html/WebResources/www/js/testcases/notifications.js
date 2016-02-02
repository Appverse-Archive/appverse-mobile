
// ALERT
var testAlertMessage = "This is the alert message.";
var testAlertTitle = "Alert Title";
var testAlertButtonText = "CLick me";

// REMOTE NOTIFICATIONS (push)

var testRemoteNotificationsSenderId = "995819883750";  // Google Project ID for the unityversa@gmail.com account
var testRemoteNotificationsTypes = [];
if(Appverse.PushNotifications) {
	testRemoteNotificationsTypes = [Appverse.PushNotifications.REMOTE_NOTIFICATION_TYPE_ALERT, Appverse.PushNotifications.REMOTE_NOTIFICATION_TYPE_SOUND, Appverse.PushNotifications.REMOTE_NOTIFICATION_TYPE_BADGE];
}

// LOCAL NOTIFICATIONS
var testLocalNotification = {};
testLocalNotification.AlertMessage = "My local notification";
testLocalNotification.Sound = "default";
testLocalNotification.Badge = 2;
// custom data JSON string does only accept a plain structure (depth level 1).. only pairs of key/value
// values could be: strings, numbers, and arrays of this kind of values
var CustomDataJsonString = {};  
CustomDataJsonString.acme1 = "custom";
CustomDataJsonString.acme3 = 3;
CustomDataJsonString.acme2 = [1,2,3];

testLocalNotification.CustomDataJsonString = JSON.stringify(CustomDataJsonString);

var testLocalNotificationScheduling = {};
var now = new Date();
now.setMinutes(now.getMinutes() + 5); // scheduled for now + 5 minute
var testLocalNotificationFireDate = ShowcaseUtils.createDateTimeObjectFromDate(now);
testLocalNotificationScheduling.FireDate = testLocalNotificationFireDate;
// repeat Interval
testLocalNotificationScheduling.RepeatInterval = Appverse.Notification.LOCAL_NOTIFICATION_REPEAT_INTERVAL_NO_REPEAT;

var testLocalNotification2 = testLocalNotification;
testLocalNotification2.AlertMessage = testLocalNotification.AlertMessage + " scheduled";

var testLocalNotificationId = 4;

// NOTIFICATIONS TEST CASE
var TestCase_Notification = [Appverse.Notification,
			[['IncrementApplicationIconBadgeNumber',''],
			['DecrementApplicationIconBadgeNumber',''],
			['SetApplicationIconBadgeNumber', '{"param1":' + JSON.stringify(4) + '}'],
			['RegisterForRemoteNotifications', '{"param1":' + JSON.stringify(testRemoteNotificationsSenderId) + ', "param2":' + JSON.stringify(testRemoteNotificationsTypes) + '}'],
			['UnRegisterForRemoteNotifications',''],
			['PresentLocalNotificationNow', '{"param1":' + JSON.stringify(testLocalNotification) + '}'],
			['ScheduleLocalNotification', '{"param1":' + JSON.stringify(testLocalNotification2) + ', "param2":' + JSON.stringify(testLocalNotificationScheduling) + '}'],
			['CancelLocalNotification', '{"param1":' + JSON.stringify(testLocalNotificationScheduling.FireDate) + '}'],
			['CancelAllLocalNotifications', ''],
			['StartNotifyActivity',''],
			['StopNotifyActivity',''],
			['IsNotifyActivityRunning'],
			['StartNotifyAlert','{"param1":' + JSON.stringify(testAlertMessage) +',"param2":' + JSON.stringify(testAlertTitle) + ', "param3":' + JSON.stringify(testAlertButtonText) + '}'],
			['StopNotifyAlert',''],
			['StartNotifyBeep',''],
			['StopNotifyBeep',''],
			['StartNotifyBlink',''],
			['StopNotifyBlink',''],
			['StartNotifyLoading',''],
			['StopNotifyLoading',''],
			['IsNotifyLoadingRunning',''],
			['UpdateNotifyLoading','{"param1":' + JSON.stringify(0.5) + '}'],
			['StartNotifyVibrate',''],
			['StopNotifyVibrate','']]
		];
		
var TestCase_PushNotification = [Appverse.PushNotifications,
			[['RegisterForRemoteNotifications', '{"param1":' + JSON.stringify(testRemoteNotificationsSenderId) + ', "param2":' + JSON.stringify(testRemoteNotificationsTypes) + '}'],
			['UnRegisterForRemoteNotifications','']]
		];


function stopNotifyLoading() {
    var num = 0;
    var interval = setInterval(function() {
        console.log("... waiting to close notify loading #" + num);
        num++;
        if(num == 5) {
            clearInterval(interval);
            console.log("closing notify loading native mask");
            Appverse.Notification.StopNotifyLoading();
        }
    },1000);
}

//********** HANDLING REMOTE NOTIFICATIONS

if(Appverse.PushNotifications) {
	Appverse.PushNotifications.OnRegisterForRemoteNotificationsSuccess = function(registrationToken) {
		console.log("OnRegisterForRemoteNotificationsSuccess");
		console.dir(registrationToken);
		//Showcase.app.getController('Main').toast("Successfully registered", false) ;
	        Showcase.app.getController('Main').console(feedObj("Appverse.OnRegisterForRemoteNotificationsSuccess","Appverse.OnRegisterForRemoteNotificationsSuccess",registrationToken));
		submitCallback("Registration token is " + registrationToken.StringRepresentation, "Appverse.OnRegisterForRemoteNotificationsSuccess");
	};

	Appverse.PushNotifications.OnRegisterForRemoteNotificationsFailure = function(registrationError) {
		console.log("OnRegisterForRemoteNotificationsFailure");
		console.dir(registrationError);
		//Showcase.app.getController('Main').toast("Registration Failure", false);
	        Showcase.app.getController('Main').console(feedObj("Appverse.OnRegisterForRemoteNotificationsFailure","Appverse.OnRegisterForRemoteNotificationsFailure",registrationError));
		submitCallback(registrationError.LocalizedDescription, "Appverse.OnRegisterForRemoteNotificationsFailure");
	};

	Appverse.PushNotifications.OnUnRegisterForRemoteNotificationsSuccess = function() {
		console.log("OnUnRegisterForRemoteNotificationsSuccess");
		//Showcase.app.getController('Main').toast("Successfully unregistered", false) ;
	        Showcase.app.getController('Main').console(feedObj("Appverse.OnUnRegisterForRemoteNotificationsSuccess","Appverse.OnUnRegisterForRemoteNotificationsSuccess","Device successfully unregistered for receiving remote notifications"));
		submitCallback("Device successfully unregistered for receiving remote notifications", "Appverse.OnUnRegisterForRemoteNotificationsSuccess");
	};

	Appverse.PushNotifications.OnRemoteNotificationReceived = function(notificationData) {
		console.log("onRemoteNotificationReceived");
		console.dir("**** TESTING NOTIFICATIONS CustomDataJsonString: " + notificationData.CustomDataJsonString);
		setTimeout(function() {
			//Showcase.app.getController('Main').toast("Remote Notification Received", false) ;
	        Showcase.app.getController('Main').console(feedObj("Appverse.OnRemoteNotificationReceived","Appverse.OnRemoteNotificationReceived",notificationData));
			submitCallback(notificationData.AlertMessage, "Appverse.OnRemoteNotificationReceived");
		},2000);
	};
}

//********** HANDLING LOCAL NOTIFICATIONS

Appverse.OnLocalNotificationReceived = function(notificationData) {
	console.log("onLocalNotificationReceived");
	console.dir("**** TESTING NOTIFICATIONS CustomDataJsonString: " + notificationData.CustomDataJsonString);
	setTimeout(function() {
		//Showcase.app.getController('Main').toast("Local Notification Received", false) ;
                Showcase.app.getController('Main').console(feedObj("Appverse.OnLocalNotificationReceived","Appverse.OnLocalNotificationReceived",notificationData));
		submitCallback(notificationData.AlertMessage, "Appverse.OnLocalNotificationReceived");
	},2000);
};