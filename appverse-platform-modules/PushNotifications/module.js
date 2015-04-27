/*
 * PUSH NOTIFICATIONS INTERFACES
 */

/**
 * @class Appverse.PushNotifications 
 * Module class to access Appverse PushNotifications module interface. 
 * <br>This interface provides features to register and unregister the device and app to receive remote notifications.<br>
 * <br> @version 5.0.3
 * <pre>Usage: Appverse.PushNotifications.&lt;metodName&gt;([params]).<br>Example: Appverse.PushNotifications.DetectQRCode().</pre>
 * @component
 * @aside guide appverse_modules
 * @constructor Constructs a new PushNotifications interface.
 * @return {Appverse.PushNotifications} A new PushNotifications interface.
 */
PushNotifications = function() {
    /**
     * @cfg {String}
     * PushNotifications service name (as configured on Platform Service Locator).
     * <br> @version 5.0.3
     */
    this.serviceName = "push";

    /**
     * None Remote Notification Type.
     * <br> @version 5.0.3
     * @type int
     */
    this.REMOTE_NOTIFICATION_TYPE_NONE = 0;
    /**
     * Badge Remote Notification Type.
     * <br> @version 5.0.3
     * @type int
     */
    this.REMOTE_NOTIFICATION_TYPE_BADGE = 1;
    /**
     * Sound Remote Notification Type.
     * <br> @version 5.0.3
     * @type int
     */
    this.REMOTE_NOTIFICATION_TYPE_SOUND = 2;
    /**
     * Alert Remote Notification Type.
     * <br> @version 5.0.3
     * @type int
     */
    this.REMOTE_NOTIFICATION_TYPE_ALERT = 3;
    /**
     * Content Availability Remote Notification Type.
     * <br> @version 5.0.3
     * @type int
     */
    this.REMOTE_NOTIFICATION_TYPE_CONTENT_AVAILABILITY = 4;
	
    /**
     * Default registration exception code for remote notifications.
     * <br> @version 5.0.3
     * @type String
     */
    this.REMOTE_NOTIFICATION_REGISTRATION_FAILURE_DEFAULT = "99";
	
    /**
     * Registration exception code for remote notifications indicating unsuccessful registration due to a different sender id previous registration.
     * <br> @version 5.0.3
     * @type String
     */
    this.REMOTE_NOTIFICATION_REGISTRATION_FAILURE_MISMATCH_SENDERID = "10";

    /**
     * Registration exception code send by the GCM Server for remote notifications (both registration and unregistration processes)
     * <br> @version 5.0.3
     * @type String
     */
    this.REMOTE_NOTIFICATION_REGISTRATION_FAILURE_GCM_SERVER = "11";

	/**
	 * @event OnRemoteNotificationReceived Fired on remote notification arrival.
	 * <br> For further information see, {@link Appverse.Notification.NotificationData NotificationData}.
     * <br> Method to be overrided by JS applications, to handle this event.
	 * @aside guide application_listeners
	 * <br> @version 5.0.3
	 * @method
	 * @param {Appverse.Notification.NotificationData} notificationData The notification data received (visual data and custom provider data)
	 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/></pre>
	 * 
	 */
	this.OnRemoteNotificationReceived = function(notificationData) {
	};
	
	/**
	 * @event OnRegisterForRemoteNotificationsSuccess Fired on successfully registration for remote notifications.
	 * <br> For further information see, {@link Appverse.PushNotifications.RegistrationToken RegistrationToken}.
     * <br> Method to be overrided by JS applications, to handle this event.
	 * @aside guide application_listeners
	 * <br> @version 5.0.3
	 * @method
	 * @param {Appverse.PushNotifications.RegistrationToken} registrationToken The registration token ("device token" for iOS or "registration ID" for Android) data received from the Notifications Service (APNs for iOS or GMC for Android).
	 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/></pre>
	 * 
	 */
	this.OnRegisterForRemoteNotificationsSuccess = function(registrationToken) {
	};

	/**
	 * @event OnRegisterForRemoteNotificationsFailure Fired on failed registration for remote notifications.
	 * <br> For further information see, {@link Appverse.PushNotifications.RegistrationError RegistrationError}.
     * <br> Method to be overrided by JS applications, to handle this event.
	 * @aside guide application_listeners
	 * <br> @version 5.0.3
	 * @method
	 * @param {Appverse.PushNotifications.RegistrationError} registrationError The registration error data received from the Notifications Service (APNs for iOS or GMC for Android).
	 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/></pre>
	 * 
	 */
	this.OnRegisterForRemoteNotificationsFailure = function(registrationError) {
	};

	/**
	 * @event OnUnRegisterForRemoteNotificationsSuccess Fired on successfully unregistration for remote notifications.
     * <br> Method to be overrided by JS applications, to handle this event.
	 * @aside guide application_listeners
	 * <br> @version 5.0.3
	 * @method
	 * <pre> Available in: <br> iOS <img src="resources/images/error.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/></pre>
	 * 
	 */
	this.OnUnRegisterForRemoteNotificationsSuccess = function() {
	};
    
};

Appverse.PushNotifications = new PushNotifications();

/**
 * Registers this application and device for receiving remote notifications.
 * <br> Returned data should be handled by overriding the corresponding Platform Listeners Appverse.OnRegisterForRemoteNotificationsSuccess and Appverse.OnRegisterForRemoteNotificationsFailure
 * <br> @version 5.0.3
 * @method
 * @param {String} senderId The sender identifier. This parameter is required for some platforms (such as the Android platform), in iOS will be just ignored.
 * @param {int[]} types The remote notifications types accepted by this application. For further information see, {@link Appverse.PushNotifications#REMOTE_NOTIFICATION_TYPE_NONE REMOTE_NOTIFICATION_TYPE_NONE}, {@link Appverse.PushNotifications#REMOTE_NOTIFICATION_TYPE_BADGE REMOTE_NOTIFICATION_TYPE_BADGE}, {@link Appverse.PushNotifications#REMOTE_NOTIFICATION_TYPE_SOUND REMOTE_NOTIFICATION_TYPE_SOUND}, {@link Appverse.PushNotifications#REMOTE_NOTIFICATION_TYPE_ALERT REMOTE_NOTIFICATION_TYPE_ALERT} and {@link Appverse.PushNotifications#REMOTE_NOTIFICATION_TYPE_CONTENT_AVAILABILITY REMOTE_NOTIFICATION_TYPE_CONTENT_AVAILABILITY}
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/></pre>
 */
PushNotifications.prototype.RegisterForRemoteNotifications = function(senderId, types)
{
    post_to_url_async(Appverse.PushNotifications.serviceName, "RegisterForRemoteNotifications", get_params([senderId, types]), null, null);
};

/**
 * Un-registers this application and device from receiving remote notifications.
 * <br> Returned data should be handled by overriding the corresponding Platform Listeners Appverse.OnUnRegisterForRemoteNotificationsSuccess
 * <br> @version 5.0.3 (listener callback only available on 4.0)
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/></pre>
 */
PushNotifications.prototype.UnRegisterForRemoteNotifications = function()
{
    post_to_url_async(Appverse.PushNotifications.serviceName, "UnRegisterForRemoteNotifications", null, null, null);
};
