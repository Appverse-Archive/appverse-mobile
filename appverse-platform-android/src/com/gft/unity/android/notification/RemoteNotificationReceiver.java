package com.gft.unity.android.notification;

import android.content.Context;
import com.google.android.gcm.GCMBroadcastReceiver;;

public class RemoteNotificationReceiver extends GCMBroadcastReceiver{

/*	@Override
	public void onReceive(Context context, Intent intent) {
		RemoteNotificationIntentService.runIntentService(context, intent);
	}*/
	@Override
	protected String getGCMIntentServiceClassName(Context context)
	{
		return RemoteNotificationIntentService.class.getName();
	}
}
