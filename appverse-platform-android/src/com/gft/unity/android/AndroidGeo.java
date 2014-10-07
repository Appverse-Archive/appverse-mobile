/*
 Copyright (c) 2012 GFT Appverse, S.L., Sociedad Unipersonal.

 This Source  Code Form  is subject to the  terms of  the Appverse Public License 
 Version 2.0  ("APL v2.0").  If a copy of  the APL  was not  distributed with this 
 file, You can obtain one at http://appverse.org/legal/appverse-license/.

 Redistribution and use in  source and binary forms, with or without modification, 
 are permitted provided that the  conditions  of the  AppVerse Public License v2.0 
 are met.

 THIS SOFTWARE IS PROVIDED BY THE  COPYRIGHT HOLDERS  AND CONTRIBUTORS "AS IS" AND
 ANY EXPRESS  OR IMPLIED WARRANTIES, INCLUDING, BUT  NOT LIMITED TO,   THE IMPLIED
 WARRANTIES   OF  MERCHANTABILITY   AND   FITNESS   FOR A PARTICULAR  PURPOSE  ARE
 DISCLAIMED. EXCEPT IN CASE OF WILLFUL MISCONDUCT OR GROSS NEGLIGENCE, IN NO EVENT
 SHALL THE  COPYRIGHT OWNER  OR  CONTRIBUTORS  BE LIABLE FOR ANY DIRECT, INDIRECT,
 INCIDENTAL,  SPECIAL,   EXEMPLARY,  OR CONSEQUENTIAL DAMAGES  (INCLUDING, BUT NOT
 LIMITED TO,  PROCUREMENT OF SUBSTITUTE  GOODS OR SERVICES;  LOSS OF USE, DATA, OR
 PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY,
 WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT(INCLUDING NEGLIGENCE OR OTHERWISE) 
 ARISING  IN  ANY WAY OUT  OF THE USE  OF THIS  SOFTWARE,  EVEN  IF ADVISED OF THE 
 POSSIBILITY OF SUCH DAMAGE.
 */
package com.gft.unity.android;

import java.util.List;

import android.app.Activity;
import android.app.Service;
import android.content.Context;
import android.hardware.Sensor;
import android.hardware.SensorEvent;
import android.hardware.SensorEventListener;
import android.hardware.SensorManager;
import android.location.Location;
import android.location.LocationListener;
import android.location.LocationManager;
import android.os.Bundle;
import android.view.Display;
import android.view.Surface;
import android.view.Window;
import android.view.WindowManager;

import com.gft.unity.core.geo.AbstractGeo;
import com.gft.unity.core.geo.Acceleration;
import com.gft.unity.core.geo.DeviceOrientation;
import com.gft.unity.core.geo.GeoDecoderAttributes;
import com.gft.unity.core.geo.LocationCategory;
import com.gft.unity.core.geo.LocationCoordinate;
import com.gft.unity.core.geo.NorthType;
import com.gft.unity.core.geo.POI;
import com.gft.unity.core.system.SystemLogger;
import com.gft.unity.core.system.SystemLogger.Module;

public class AndroidGeo extends AbstractGeo {

	private static final SystemLogger LOG = AndroidSystemLogger.getInstance();

	// TODO: review this value
	private static final long MIN_TIME = 1000 * 60;
	private static final long UPDATE_INTERVAL = 5 * 1000;

	private LocationManager locationManager;
	private SensorManager sensorManager;

	private Location location;
	private float heading;
	private float[] gravity = new float[3];
	private float[] geomagnetic = new float[3];
	private float[] orientation = new float[3];
	private float[] rotation = new float[9];
	private float[] linear_acceleration = new float[3];
	private Acceleration acceleration = new Acceleration();

	// listen for location (GPS, NETWORK) changes
	private LocationListener locationListener = new LocationListener() {

		@Override
		public void onStatusChanged(String arg0, int arg1, Bundle arg2) {
			// nothing do to
		}

		@Override
		public void onProviderEnabled(String arg0) {
			// nothing do to
		}

		@Override
		public void onProviderDisabled(String arg0) {
			// nothing do to

		}

		@Override
		public void onLocationChanged(Location newLocation) {

			// updates location with the newest (if better) location
			LOG.Log(Module.PLATFORM,
					"New Location aquired: "
							+ (newLocation == null ? null : "acc: "
									+ newLocation.getAccuracy() + "-"
									+ newLocation.getLatitude() + ","
									+ newLocation.getLongitude()));
			if (isBetterLocation(newLocation, getLocation())) {
				LOG.Log(
						Module.PLATFORM,
						"newLocation: "
								+ (newLocation == null ? "null" : newLocation
										.getProvider())
								+ " is better than current location: "
								+ (getLocation() == null ? "null"
										: getLocation().getProvider()));
				setLocation(newLocation);
			}
		}
	};

	private Location getLocation() {
		return location;
	}

	private void setLocation(Location location) {
		this.location = location;
	}

	// listen to sensor (ACCELEROMETER, MAGNETIC FIELD) changes
	private SensorEventListener sensorListener = new SensorEventListener() {

		@Override
		public void onSensorChanged(SensorEvent event) {
			if (event.sensor.getType() == Sensor.TYPE_ACCELEROMETER) {
				final float alpha = 0.8F;// TODO calculate the alpha value
				gravity[0] = alpha * gravity[0] + (1 - alpha) * event.values[0];
				gravity[1] = alpha * gravity[1] + (1 - alpha) * event.values[1];
				gravity[2] = alpha * gravity[2] + (1 - alpha) * event.values[2];
				linear_acceleration[0] = event.values[0] - gravity[0];
				linear_acceleration[1] = event.values[1] - gravity[1];
				linear_acceleration[2] = event.values[2] - gravity[2];
				// updating the rotation array
				SensorManager.getRotationMatrix(rotation, null, gravity,
						geomagnetic);
				// updating the orientation array
				SensorManager.getOrientation(rotation, orientation);
			} else if (event.sensor.getType() == Sensor.TYPE_MAGNETIC_FIELD) {
				geomagnetic = event.values.clone();
			} else if (event.sensor.getType() == Sensor.TYPE_PROXIMITY) {
				System.out.println("PROXIMITY = " + event.values[0]);
				if (event.values[0] == 0.0) {
					dimScreem();
				} else {
					// TODO make it work!!!!!
					wakeScreen();
				}
			}
		}

		@Override
		public void onAccuracyChanged(Sensor arg0, int arg1) {
			// has nothing to do
		}
	};

	@Override
	public Acceleration GetAcceleration() {
		acceleration.setX(linear_acceleration[0]);
		acceleration.setY(linear_acceleration[1]);
		acceleration.setZ(linear_acceleration[2]);

		/*
		 * see
		 * 
		 * https://builder.gft.com/wiki/display/Unity/GeoLocation#GeoLocation-
		 * GettingCurrentAcceleration
		 * 
		 * sqrt(X^2 + Y^2 + Z^2) = Acceleration in m/s
		 */
		Double acc = Math.pow(linear_acceleration[0], 2)
				+ Math.pow(linear_acceleration[1], 2)
				+ Math.pow(linear_acceleration[2], 2);
		acc = Math.sqrt(acc);
		acceleration.setAccel(acc.floatValue());

		// TODO calculate acceleration based on location if the device doesn't
		// have ACCELEROMETER
		return acceleration;
	}

	@Override
	public LocationCoordinate GetCoordinates() {
		LocationCoordinate coordinate = new LocationCoordinate();

		Location loc = getLocation();
		if (loc != null) {
			coordinate.setXCoordinate(loc.getLatitude());
			coordinate.setYCoordinate(loc.getLongitude());
			coordinate.setZCoordinate(loc.getAltitude());
			coordinate.setXDoP(loc.getAccuracy());
			coordinate.setYDoP(loc.getAccuracy());
		}

		return coordinate;
	}

	@Override
	public float GetHeading() {
		// the azimuth (in radians) is always orientation[0]
		float azimuth = orientation[0];
		// calculate heading in degrees
		heading = Double.valueOf(Math.toDegrees(azimuth)).floatValue();
		if (heading < 0) {
			// can't be negative value
			heading = 360 + heading;
		}

		return heading;
	}

	@Override
	public float GetHeading(NorthType type) {
		// TODO implement ILocation.GetHeading based on type
		return GetHeading();
	}

	@Override
	public void GetMap() {
		// TODO implement IMap.GetMap
		throw new UnsupportedOperationException("Not supported yet.");
	}

	@Override
	public POI GetPOI(String id) {
		
		if (id == null || id.equals("")) {
			return null;
		} else {
			return new AndroidPOIDatabase().getPOI(id);
		}
	}

	@Override
	public POI[] GetPOIList(LocationCoordinate location, float radius) {
		List<POI> pois = new AndroidPOIDatabase().getPOIList(location, radius);
		return pois.toArray(new POI[pois.size()]);
	}

	@Override
	public POI[] GetPOIList(LocationCoordinate location, float radius,
			String queryText) {
		// TODO implement IMap.GetPOIList based on queryText
		return GetPOIList(location, radius);
	}

	@Override
	public POI[] GetPOIList(LocationCoordinate location, float radius,
			String queryText, LocationCategory category) {
		// TODO implement IMap.GetPOIList based on queryText and category
		return GetPOIList(location, radius);
	}

	@Override
	public POI[] GetPOIList(LocationCoordinate location, float radius,
			LocationCategory category) {
		// TODO implement IMap.GetPOIList based on category
		return GetPOIList(location, radius);
	}

	@Override
	public float GetVelocity() {
		float speed = 0;
		if (getLocation() != null) {
			speed = getLocation().getSpeed();
		}
		return speed;
	}

	@Override
	public boolean RemovePOI(String id) {
		
		if (id != null && !id.equals("")) {
			return new AndroidPOIDatabase().removePOI(id);
		} else {
			return true;
		}
	}

	@Override
	public void SetMapSettings(float scale, float boundingBox) {
		// TODO implement IMap.SetMapSettings
		throw new UnsupportedOperationException("Not supported yet.");
	}

	@Override
	public boolean StartUpdatingHeading() {
		
		try {
			if (sensorManager == null) {
				sensorManager = (SensorManager) AndroidServiceLocator
						.getContext().getSystemService(Service.SENSOR_SERVICE);
			}
			Sensor accelerometer = sensorManager
					.getDefaultSensor(Sensor.TYPE_ACCELEROMETER);
			Sensor magnetic = sensorManager
					.getDefaultSensor(Sensor.TYPE_MAGNETIC_FIELD);

			sensorManager.registerListener(sensorListener, accelerometer,
					SensorManager.SENSOR_DELAY_NORMAL);
			sensorManager.registerListener(sensorListener, magnetic,
					SensorManager.SENSOR_DELAY_NORMAL);

		} catch (Exception e) {
			LOG.Log(Module.PLATFORM,
					"Error starting to update heading", e);
			return false;
		}
		
		return true;
	}

	@Override
	public boolean StartUpdatingLocation() {
		
		Context context = AndroidServiceLocator.getContext();
		if (locationManager == null) {
			locationManager = (LocationManager) context
					.getSystemService(Service.LOCATION_SERVICE);
		}

		boolean isGPSRegistered = false;
		boolean isNetworkRegistered = false;
		if (locationManager.isProviderEnabled(LocationManager.GPS_PROVIDER)) {
			Runnable rGPS = new Runnable() {

				@Override
				public void run() {
					locationManager.requestLocationUpdates(
							LocationManager.GPS_PROVIDER, UPDATE_INTERVAL, 0,
							locationListener);
				}
			};
			((Activity) context).runOnUiThread(rGPS);
			LOG.Log(Module.PLATFORM, "GPS provider is enabled");
			isGPSRegistered = true;
		}

		if (locationManager.isProviderEnabled(LocationManager.NETWORK_PROVIDER)) {
			Runnable rNet = new Runnable() {

				@Override
				public void run() {
					locationManager.requestLocationUpdates(
							LocationManager.NETWORK_PROVIDER, UPDATE_INTERVAL,
							0, locationListener);
				}
			};
			((Activity) context).runOnUiThread(rNet);
			LOG.Log(Module.PLATFORM, "Network provider is enabled");
			isNetworkRegistered = true;
		}

		/* DO NOT STORE ANY LAST KNOWN LOCATION.
		 * OTHER PLATFORMS DO NOT HAVE THIS DATA AVAILABLE.
		 * SAME BEHAVOUR SHOULD BE PRESERVED ACROSS PLATFORMS.
		 * 
		Location lastGps = locationManager
				.getLastKnownLocation(LocationManager.GPS_PROVIDER);
		Location lastNetwork = locationManager
				.getLastKnownLocation(LocationManager.NETWORK_PROVIDER);

		if (!isBetterLocation(lastGps, lastNetwork)) {
			LOG.Log(Module.PLATFORM,
					"NETWORK location is better than GPS.");
			setLocation(lastNetwork);
		} else {
			LOG.Log(Module.PLATFORM,
					"GPS location is better than NETWORK.");
			setLocation(lastGps);
		}
		*/

		return (isGPSRegistered || isNetworkRegistered);
	}

	@Override
	public boolean StopUpdatingHeading() {
		
		try {
			if (sensorManager == null) {
				sensorManager = (SensorManager) AndroidServiceLocator
						.getContext().getSystemService(Service.SENSOR_SERVICE);
			}

			Sensor accelerometer = sensorManager
					.getDefaultSensor(Sensor.TYPE_ACCELEROMETER);
			Sensor magnetic = sensorManager
					.getDefaultSensor(Sensor.TYPE_MAGNETIC_FIELD);

			sensorManager.unregisterListener(sensorListener, accelerometer);
			sensorManager.unregisterListener(sensorListener, magnetic);
		} catch (Exception e) {
			LOG.Log(Module.PLATFORM,
					"Exception stop updating heading", e);
			return false;
		}
		
		return true;
	}

	@Override
	public boolean StopUpdatingLocation() {

		Runnable rRemove = new Runnable() {

			@Override
			public void run() {
				if (locationManager == null) {
					locationManager = (LocationManager) AndroidServiceLocator.getContext()
							.getSystemService(Service.LOCATION_SERVICE);
				}
				locationManager.removeUpdates(locationListener);
			}
		};
		((Activity) AndroidServiceLocator.getContext()).runOnUiThread(rRemove);
		
		return true;
	}

	@Override
	public boolean UpdatePOI(POI poi) {
		
		if (poi != null) {
			AndroidPOIDatabase db = new AndroidPOIDatabase();
			if (db.exists(poi)) {
				return db.updatePOI(poi);
			} else {// new POI, needs to be inserted
				return db.insertPOI(poi);
			}
		} else {
			return false;
		}
	}

	private boolean isBetterLocation(Location location,
			Location currentBestLocation) {

		// if both locations are null, no one is better...it's a draw! ;-)
		if (location == null && currentBestLocation == null) {
			return false;
		}

		if (currentBestLocation == null) {
			// A new location is always better than no location
			return true;
		}

		if (location == null) {
			// A null location will never be better than current one
			return false;
		}

		if (LocationManager.GPS_PROVIDER.equals(location.getProvider())
				&& !LocationManager.GPS_PROVIDER.equals(currentBestLocation
						.getProvider())) {
			return true;
		} else if (LocationManager.GPS_PROVIDER.equals(currentBestLocation
				.getProvider())
				&& !LocationManager.GPS_PROVIDER.equals(location.getProvider())) {
			return false;
		}

		// Check whether the new location fix is newer or older
		long timeDelta = location.getTime() - currentBestLocation.getTime();
		boolean isSignificantlyNewer = timeDelta > MIN_TIME;
		boolean isSignificantlyOlder = timeDelta < -MIN_TIME;
		boolean isNewer = timeDelta > 0;

		// If it's been more than MIN_TIME since the current location, use the
		// new location
		// because the user has likely moved
		if (isSignificantlyNewer) {
			return true;
			// If the new location is more than MIN_TIME older, it must be worse
		} else if (isSignificantlyOlder) {
			return false;
		}

		// Check whether the new location fix is more or less accurate
		int accuracyDelta = (int) (location.getAccuracy() - currentBestLocation
				.getAccuracy());
		boolean isLessAccurate = accuracyDelta > 0;
		boolean isMoreAccurate = accuracyDelta < 0;
		boolean isSignificantlyLessAccurate = accuracyDelta > 10;

		// Check if the old and new location are from the same provider
		boolean isFromSameProvider = isSameProvider(location.getProvider(),
				currentBestLocation.getProvider());

		// Determine location quality using a combination of timeliness and
		// accuracy
		if (isMoreAccurate) {
			return true;
		} else if (isNewer && !isLessAccurate) {
			return true;
		} else if (isNewer && !isSignificantlyLessAccurate
				&& isFromSameProvider) {
			return true;
		}
		return false;
	}

	/** Checks whether two providers are the same */
	private boolean isSameProvider(String provider1, String provider2) {
		
		if (provider1 == null) {
			return provider2 == null;
		}
		
		return provider1.equals(provider2);
	}

	@Override
	public DeviceOrientation GetDeviceOrientation() {
		
		/* First, get the Display from the WindowManager */
		Display display = ((WindowManager) AndroidServiceLocator.getContext()
				.getSystemService(Service.WINDOW_SERVICE)).getDefaultDisplay();
		int rotation = display.getRotation();
		// TODO Face Up and Face Down - needs to start updating the
		// accelerometer
		switch (rotation) {
		case Surface.ROTATION_0:
			return DeviceOrientation.Portrait;
		case Surface.ROTATION_90:
			return DeviceOrientation.LandscapeRight;
		case Surface.ROTATION_180:
			return DeviceOrientation.PortraitUpsideDown;
		case Surface.ROTATION_270:
			return DeviceOrientation.LandscapeLeft;
		default:
			return DeviceOrientation.Unknown;
		}
	}

	@Override
	public GeoDecoderAttributes GetGeoDecoder() {
		// TODO implement ILocation.GetGeoDecoder
		throw new UnsupportedOperationException("Not supported yet.");
	}

	@Override
	public boolean StartProximitySensor() {
		
		try {
			if (sensorManager == null) {
				sensorManager = (SensorManager) AndroidServiceLocator
						.getContext().getSystemService(Service.SENSOR_SERVICE);
			}
			Sensor prox = sensorManager.getDefaultSensor(Sensor.TYPE_PROXIMITY);
			sensorManager.registerListener(sensorListener, prox,
					SensorManager.SENSOR_DELAY_NORMAL);
		} catch (Exception e) {
			LOG.Log(Module.PLATFORM,
					"Error starting to update proximity", e);
			return false;
		}
		
		return true;
	}

	@Override
	public boolean StopProximitySensor() {
		
		try {
			if (sensorManager == null) {
				sensorManager = (SensorManager) AndroidServiceLocator
						.getContext().getSystemService(Service.SENSOR_SERVICE);
			}
			Sensor prox = sensorManager.getDefaultSensor(Sensor.TYPE_PROXIMITY);
			sensorManager.unregisterListener(sensorListener, prox);
			wakeScreen();
		} catch (Exception e) {
			LOG.Log(Module.PLATFORM,
					"Exception stop updating proximity", e);
			return false;
		}
		
		return true;
	}

	private void setBright(float value) {
		
		Window mywindow = ((Activity) AndroidServiceLocator.getContext())
				.getWindow();

		WindowManager.LayoutParams lp = mywindow.getAttributes();

		lp.screenBrightness = value;

		mywindow.setAttributes(lp);
	}

	private void dimScreem() {
		
		Runnable task = new Runnable() {

			@Override
			public void run() {
				setBright(0.0F);
			}
		};
		((Activity) AndroidServiceLocator.getContext()).runOnUiThread(task);
	}

	private void wakeScreen() {
		Runnable task = new Runnable() {

			@Override
			public void run() {
				setBright(1.0F);
			}
		};
		((Activity) AndroidServiceLocator.getContext()).runOnUiThread(task);
	}

	@Override
	public boolean IsGPSEnabled() {
		try {
			if (locationManager == null) {
				locationManager = (LocationManager) AndroidServiceLocator.getContext()
						.getSystemService(Service.LOCATION_SERVICE);
			}
			
			return locationManager.isProviderEnabled(LocationManager.GPS_PROVIDER);
		
		} catch (Exception e) {
			LOG.Log(Module.PLATFORM,
					"Exception checking GPS service availability", e);
			return false;
		}
	}
}
