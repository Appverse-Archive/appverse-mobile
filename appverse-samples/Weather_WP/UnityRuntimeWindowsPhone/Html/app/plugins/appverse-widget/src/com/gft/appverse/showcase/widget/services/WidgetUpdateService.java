package com.gft.appverse.showcase.widget.services;

import android.app.Service;
import android.content.Intent;
import android.os.IBinder;

/**
 * Main class for widget update service. This class allow us to define a method for updating the
 * widget every time we want regardless the minimum update time o 30 minutes of the specification
 */
public class WidgetUpdateService extends Service {

	/**
	 * This method is called every time the service is updated. This method contains all the logic
	 * of the update process
	 */
	private void update() {

		// TODO: Perform actions on the update period
	}

	/**
	 * TCalled by the system every time a client explicitly starts the service by calling
	 * startService(Intent), providing the arguments it supplied and a unique integer token
	 * representing the start request.
	 */
	@Override
	public int onStartCommand(Intent intent, int flags, int startId) {

		// Calling the update method
		this.update();

		return super.onStartCommand(intent, flags, startId);
	}

	/**
	 * Called by the system when the service is first created.
	 */
	@Override
	public void onCreate() {

		super.onCreate();
	}

	/**
	 * Return the communication channel to the service.
	 */
	@Override
	public IBinder onBind(Intent intent) {

		return null;
	}
}
