/*
 Copyright (c) 2012 GFT Appverse, S.L., Sociedad Unipersonal.

 This Source  Code Form  is subject to the  terms of  the Appverse Public License 
 Version 2.0  (“APL v2.0”).  If a copy of  the APL  was not  distributed with this 
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
using System;
using System.Threading;
using System.Drawing;
using System.Text.RegularExpressions;
using Unity.Core.Geo;
using Unity.Core.System;
using Unity.Core.Storage;
using Unity.Core.Storage.Database;
using System.Collections.Generic;
using MonoTouch.CoreLocation;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.MapKit;

namespace Unity.Platform.IPhone
{
	public class UnityLocation
	{
		private double latitude = 0.0f;
		private double longitude = 0.0f;
		private double altitude = 0.0f;
		private double horizontalAccuracy = 0.0f;
		private double verticalAccuracy = 0.0f;
		private double speed = 0.0f;
		private double course = 0.0f;
		private NSDate timeStamp;

		public double Latitude {
			get { return latitude; }
			set { latitude = value; }
		}

		public double Longitude {
			get { return longitude; }
			set { longitude = value; }
		}

		public double Altitude {
			get { return altitude; }
			set { altitude = value; }
		}

		public double HorizontalAccuracy {
			get { return horizontalAccuracy; }
			set { horizontalAccuracy = value; }
		}

		public double VerticalAccuracy {
			get { return verticalAccuracy; }
			set { verticalAccuracy = value; }
		}

		public NSDate TimeStamp {
			get { return timeStamp; }
			set { timeStamp = value; }
		}

		public double Speed {
			get { return speed; }
			set { speed = value; }
		}

		public double Course {
			get { return course; }
			set { course = value; }
		}
		
	}

	public class UnityHeading
	{
		private double trueHeading = 0.0f;
		private double headingAccuracy = 0.0f;
		private double magneticHeading = 0.0f;
		private double xGeomagneticField = 0.0f;
		private double yGeomagneticField = 0.0f;
		private double zGeomagneticField = 0.0f;
		private NSDate timeStamp;

		public double TrueHeading {
			get { return trueHeading; }
			set { trueHeading = value; }
		}

		public double HeadingAccuracy {
			get { return headingAccuracy; }
			set { headingAccuracy = value; }
		}

		public double MagneticHeading {
			get { return magneticHeading; }
			set { magneticHeading = value; }
		}

		public double XGeomagneticField {
			get { return xGeomagneticField; }
			set { xGeomagneticField = value; }
		}

		public double YGeomagneticField {
			get { return yGeomagneticField; }
			set { yGeomagneticField = value; }
		}

		public double ZGeomagneticField {
			get { return zGeomagneticField; }
			set { zGeomagneticField = value; }
		}

		public NSDate TimeStamp {
			get { return timeStamp; }
			set { timeStamp = value; }
		}
		
	}

	public class UnityLocationManagerDelegate : CLLocationManagerDelegate
	{
		private UnityLocation unityLocation;
		private UnityHeading unityHeading;
		private GeoDecoderAttributes geoDecAttributes;
		private UnityGeocoderDelegate unityGeocoderDelegate;
		private MKReverseGeocoder reverseGeocoder;

		/// <summary>
		/// Keep a reference to UnityLocation, UnityHeading and UnityGeoDecoder so that we can access the different location, heading and geodecoder attributes.
		/// </summary>
		public UnityLocationManagerDelegate (UnityLocation unityLoc, UnityHeading unityHead, GeoDecoderAttributes geoDecoderAttributes) : base()
		{
			unityLocation = unityLoc;
			unityHeading = unityHead;
			geoDecAttributes = geoDecoderAttributes;
		}

		/// <summary>
		/// This method is called every time CLLocationManager gets a new
		/// reading from the hardware, _until_ StopUpdatingLocation is called
		/// (which we do call in this method, after the first reading is received)
		/// </summary>
		public override void UpdatedLocation (CLLocationManager locationManager, CLLocation newLocation, CLLocation oldLocation)
		{
			if(newLocation.HorizontalAccuracy > 0) {
				unityLocation.Latitude = newLocation.Coordinate.Latitude;
				unityLocation.Longitude = newLocation.Coordinate.Longitude;
				unityLocation.Altitude = newLocation.Altitude;
				unityLocation.Speed = newLocation.Speed;
				unityLocation.Course = newLocation.Course;
				unityLocation.TimeStamp = newLocation.Timestamp;
			}
			
			unityLocation.HorizontalAccuracy = newLocation.HorizontalAccuracy;
			unityLocation.VerticalAccuracy = newLocation.VerticalAccuracy;
			
			int numVersionMajor = getMajorVersionNumber();
#if DEBUG
			SystemLogger.Log(SystemLogger.Module.PLATFORM, "UpdatedLocation: Major OS version number:" + numVersionMajor);
#endif
			//TO DO 
//			if (numVersionMajor >= 4) {
//				unityGeocoderDelegate = new UnityGeocoderDelegate(geoDecAttributes);
//				reverseGeocoder = new MKReverseGeocoder(locationManager.Location.Coordinate);
//				reverseGeocoder.Delegate = unityGeocoderDelegate;
//				reverseGeocoder.Start();
//			}
		}
		/// <summary>
		/// This method is called every time CLLocationManager gets a new
		/// reading from the hardware, _until_ StopUpdatingLocation is called
		/// (which we do call in this method, after the first reading is received)
		/// </summary>
		public override void UpdatedHeading (CLLocationManager locationManager, CLHeading head)
		{
			if (CLLocationManager.HeadingAvailable) {
				unityHeading.TrueHeading = head.TrueHeading;
				unityHeading.HeadingAccuracy = head.HeadingAccuracy;
				unityHeading.MagneticHeading = head.MagneticHeading;
				unityHeading.TimeStamp = head.Timestamp;
				unityHeading.XGeomagneticField = head.X;
				unityHeading.YGeomagneticField = head.Y;
				unityHeading.ZGeomagneticField = head.Z;
			}
		}
		/// <summary>
		/// Show a message if the CLLocationManager says so
		/// </summary>
		public override void Failed (CLLocationManager m, NSError e)
		{
			SystemLogger.Log (SystemLogger.Module.PLATFORM, "Failed to get location/heading:" + e.ToString ());
		}
		
		private int getMajorVersionNumber()
		{
			AbstractSystem iPhoneSystem = new IPhoneSystem();
			OSInfo osInfo = new OSInfo();
			osInfo = iPhoneSystem.GetOSInfo();
			string version = osInfo.Version;
			int idx = version.IndexOf('.');
			string versionMajor = version.Substring(0, idx);
			int numVersionMajor = Int16.Parse(versionMajor);
			return numVersionMajor;
		}
	}

	public class UnityGeocoderDelegate : MKReverseGeocoderDelegate
	{
		private GeoDecoderAttributes geoDecoderAttributes;

		public UnityGeocoderDelegate (GeoDecoderAttributes geoDecoderAttr) : base()
		{
			geoDecoderAttributes = geoDecoderAttr;
		}

		/// <summary>
		/// MKReverseGeocoderDelegate calls this method when it finds a match
		/// for the latitude,longitude passed to MKReverseGeocoder() constructor
		/// </summary>
		public override void FoundWithPlacemark (MKReverseGeocoder reverseGeoCoder, MKPlacemark placeMark)
		{
			#if DEBUG
			SystemLogger.Log (SystemLogger.Module.PLATFORM, "Inside FoundWithPlacemark ");
			#endif
			geoDecoderAttributes.AdditionalStreetLevelInfo = placeMark.SubThoroughfare;
			geoDecoderAttributes.StreetAddress = placeMark.Thoroughfare;
			geoDecoderAttributes.Locality = placeMark.Locality;
			geoDecoderAttributes.AdditionalCityLevelInfo = placeMark.SubLocality;
			geoDecoderAttributes.Country = placeMark.Country;
			geoDecoderAttributes.CountryCode = placeMark.CountryCode;
			geoDecoderAttributes.PostalCode = placeMark.PostalCode;
			geoDecoderAttributes.AdministrativeArea = placeMark.AdministrativeArea;
			geoDecoderAttributes.SubAdministrativeArea = placeMark.SubAdministrativeArea;
		}

		/// <summary>
		/// Show a message if the MKReverseGeocoder says so
		/// </summary>
		public override void FailedWithError (MKReverseGeocoder gc, NSError e)
		{
			SystemLogger.Log (SystemLogger.Module.PLATFORM, e.ToString ());
			SystemLogger.Log (SystemLogger.Module.PLATFORM, e.LocalizedDescription);
			// PBRequesterErrorDomain error 6001 occurs when too many requests have been sent
		}
		
	}

	/// <summary>
	/// A custom subclass of MKAnnotation is REQUIRED to add annotations to a MKMapView
	/// </summary>
	public class UnityAnnotation : MKAnnotation
	{
		private CLLocationCoordinate2D coordinate;
		private string title, subtitle;
		private MKPinAnnotationColor color;

		public override CLLocationCoordinate2D Coordinate {
			get { return coordinate; }
			set { coordinate = value; }
		}

		public override string Title {
			get { return title; }
		}

		public override string Subtitle {
			get { return subtitle; }
		}

		/// <summary>
		/// Custom property to use when displaying on the map,
		/// not part of the MKAnnotation protocol
		/// </summary>
		public MKPinAnnotationColor Color {
			get { return color; }
		}

		/// <summary>
		/// The custom constructor is required to pass the values to this class,
		/// because in the MKAnnotation base the properties are read-only
		/// </summary>
		public UnityAnnotation (CLLocationCoordinate2D locCoord, string title, string subtitle, MKPinAnnotationColor anotationColor)
		{
			coordinate = locCoord;
			this.title = title;
			this.subtitle = subtitle;
			color = anotationColor;
		}
	}

	public class UnityMapView : MKMapView
	{
		public UnityMapView ()
		{
			this.ShowsUserLocation = true;
			// shows the "blue dot" user location (if available)
			this.MapType = MonoTouch.MapKit.MKMapType.Standard;
			// Hybrid | Satellite
			this.ZoomEnabled = true;
			// if false, cannot zoom
			this.ScrollEnabled = true;
			// if false, cannot scroll
			this.UserInteractionEnabled = true;
			// if false, cannot even click on annotation
		}
	}

	public class IPhoneGeo : AbstractGeo
	{
		private UnityLocation unityLocation = null;
		private UnityHeading unityHeading = null;
		private GeoDecoderAttributes geoDecoderAttributes = new GeoDecoderAttributes();
		private CLLocationManager locationManager = new CLLocationManager ();
		private LocationCoordinate locationCoordinate = new LocationCoordinate ();
		private UIAcceleration uiAcceleration;
		private Acceleration acceleration;
		private UnityMapView unityMapView = null;
		private UISegmentedControl mapSegmentedControl;
		private object[] segmentedCtrlValues = { "Map", "Satellite", "Hybrid" };
		private Dictionary<string, UnityAnnotation> annotationPins = new Dictionary<string, UnityAnnotation> ();
		private const string mapDDBBName = "MAPDDBB";
		private const string poiTable = "POI";
		private string[] poiColumDefinition = { "ID VARCHAR2(30) NOT NULL PRIMARY KEY", "CATEGORYMAIN TEXT" };
		private const string locationCoordinateTable = "LOCATIONCOORDINATE";
		private string[] locationCoordinateColumDefinition = { "LATITUDE REAL", "LONGITUDE REAL", "ALTITUDE REAL", "HORIZONTALACCURACY REAL", "VERTICALACCURACY REAL", "POI_ID VARCHAR2(30) NOT NULL UNIQUE CONSTRAINT FK_LOCATIONCOORDINATE_POI_ID REFERENCES POI(ID) ON DELETE CASCADE" };
		private const string locationDescriptionTable = "LOCATIONDESCRIPTION";
		private string[] locationDescriptionColumnDefinition = { "DESCRIPTION TEXT", "NAME TEXT", "CATEGORYMAINNAME TEXT", "POI_ID VARCHAR2(30) NOT NULL UNIQUE CONSTRAINT FK_LOCATIONDESCRIPTION_POI_ID REFERENCES POI(ID) ON DELETE CASCADE" };
		private const string secondaryCategoryTable = "SECONDARYCATEGORY";
		private string[] secondaryCategoryColumnDefinition = { "SECONDCATEGORYNAME TEXT", "POI_ID VARCHAR2(30) NOT NULL CONSTRAINT FK_SECONDARYCATEGORY_POI_ID REFERENCES POI(ID) ON DELETE CASCADE" };
		private AbstractDatabase mapDDBB = new IPhoneDatabase ();
		private float latitudeDeltaSpan = 0.5f;
		private float longitudeDeltaSpan = 0.5f;
		
		CLLocationCoordinate2D coord;
		MKCoordinateSpan coordSpan;
		MKCoordinateRegion coordRegion;
		

		public IPhoneGeo () : base()
		{
			initAccelerometre();
			initMapDatabase ();
		}

		private void initMapDatabase ()
		{
			if (!mapDDBB.ExistsDatabase (mapDDBBName)) {
				#if DEBUG
				SystemLogger.Log (SystemLogger.Module.PLATFORM, mapDDBBName + " database does not exist; creating database");
				#endif
				mapDDBB.CreateDatabase (mapDDBBName);
				initTable (poiTable, poiColumDefinition);
				initTable (locationCoordinateTable, locationCoordinateColumDefinition);
				initTable (locationDescriptionTable, locationDescriptionColumnDefinition);
				initTable (secondaryCategoryTable, secondaryCategoryColumnDefinition);
			} else {
				#if DEBUG
				SystemLogger.Log (SystemLogger.Module.PLATFORM, mapDDBBName + " database exists");
				#endif
			}
		}

		private void initTable (string table, string[] tableColumnDefinitions)
		{
			if (!mapDDBB.Exists (mapDDBB.GetDatabase (mapDDBBName), table)) {
				#if DEBUG
				SystemLogger.Log (SystemLogger.Module.PLATFORM, table + " does not exist; creating table");
				#endif
				mapDDBB.CreateTable (mapDDBB.GetDatabase (mapDDBBName), table, tableColumnDefinitions);
			} else {
				#if DEBUG
				SystemLogger.Log (SystemLogger.Module.PLATFORM, table + " already exists.");
				#endif
			}
		}

		private void setMapTypesInMap ()
		{
			mapSegmentedControl = new UISegmentedControl (segmentedCtrlValues);
			mapSegmentedControl.AutosizesSubviews = true;
			mapSegmentedControl.ValueChanged += delegate {
				// change map type depending on which segment was touched
				if (mapSegmentedControl.SelectedSegment == 0)
					unityMapView.MapType = MKMapType.Standard; else if (mapSegmentedControl.SelectedSegment == 1)
					unityMapView.MapType = MKMapType.Satellite; else if (mapSegmentedControl.SelectedSegment == 2)
					unityMapView.MapType = MKMapType.Hybrid;
			};
			unityMapView.AddSubview (mapSegmentedControl);
		}

//		private void setAnnotationBehavior ()
//		{
//			//Funtion in order to add annotation points from the map itself (the user can add POI's touching the map): It is not implemented and this CODE DOES NOT WORK
//			unityMapView.GetViewForAnnotation = delegate(MKMapView mapView, NSObject annotation) {
//				// Called by the map whenever an annotation is added and needs to be displayed
//				if (annotation is MKUserLocation)
//					return null;
//			};
//		}

		private void initAccelerometre ()
		{
			acceleration = new Acceleration ();
			uiAcceleration = new UIAcceleration ();
			UIAccelerometer.SharedAccelerometer.UpdateInterval = 1 / 10;
			//This value could be set, in a new development, as a parameter
			UIAccelerometer.SharedAccelerometer.Acceleration += delegate(object sender, UIAccelerometerEventArgs e) {
				uiAcceleration = e.Acceleration;
				acceleration.X = (float)uiAcceleration.X;
				acceleration.Y = (float)uiAcceleration.Y;
				acceleration.Z = (float)uiAcceleration.Z;
				acceleration.Accel = (float)Math.Sqrt (Math.Pow (acceleration.X, 2) + Math.Pow (acceleration.Y, 2) + Math.Pow (acceleration.Z, 2));
			};
		}

		public override Acceleration GetAcceleration ()
		{
			return acceleration;
		}

		public override bool StartUpdatingLocation ()
		{
			bool isStartedOk = false;
			if (unityLocation == null) {
				unityLocation = new UnityLocation ();
			}
			if (unityHeading == null) {
				unityHeading = new UnityHeading ();
			}
			
			if (CLLocationManager.LocationServicesEnabled) {
				try {
					LocationManagerSetup();

					if (UIDevice.CurrentDevice.CheckSystemVersion (8, 0)) {
						#if DEBUG
						SystemLogger.Log (SystemLogger.Module.PLATFORM, "Using new iOS 8 Location Services Authorization");
						#endif
						locationManager.RequestWhenInUseAuthorization();  //only requests for authorization in app running (foreground)
					}

					locationManager.StartUpdatingLocation ();
					isStartedOk = true;
				} catch (Exception) {
					isStartedOk = false;
				}
			} else {
				#if DEBUG
				SystemLogger.Log (SystemLogger.Module.PLATFORM, "StartUpdatingLocation(): Not Supported: LocationServicesEnabled = false");
				#endif
				isStartedOk = false;
			}
			
			return isStartedOk;
		}

		public override bool StopUpdatingLocation ()
		{
			bool isStoppedOk = false;
			if (CLLocationManager.LocationServicesEnabled) {
				try {
					locationManager.StopUpdatingLocation ();
					isStoppedOk = true;
					
				} catch (Exception) {
					isStoppedOk = false;
				}
			} else {
				#if DEBUG
				SystemLogger.Log (SystemLogger.Module.PLATFORM, "StopUpdatingHeading(): Not Supported: LocationServicesEnabled = false");
				#endif
				isStoppedOk = true;
			}
			return isStoppedOk;
		}

		public override bool StartUpdatingHeading ()
		{
			bool isStartedOk = false;
			if (unityLocation == null) {
				unityLocation = new UnityLocation ();
			}
			if (unityHeading == null) {
				unityHeading = new UnityHeading ();
			}
			if (CLLocationManager.HeadingAvailable) {
				try {
					LocationManagerSetup();
					locationManager.StartUpdatingHeading ();
					isStartedOk = true;
				} catch (Exception) {
					isStartedOk = false;
				}
			} else {
				SystemLogger.Log (SystemLogger.Module.PLATFORM, "StartUpdatingheading(): Not Supported: HeadingAvailable = false");
				isStartedOk = false;
			}
			return isStartedOk;
		}

		public override bool StopUpdatingHeading ()
		{
			bool isStoppedOk = false;
			if (CLLocationManager.HeadingAvailable) {
				try {
					locationManager.StopUpdatingHeading ();
					isStoppedOk = true;
					
				} catch (Exception e) {
					#if DEBUG
					SystemLogger.Log (SystemLogger.Module.PLATFORM, "Exception in StopUpdatingHeading(); Message: " + e.Message);
					#endif
					isStoppedOk = false;
				}
			} else {
				#if DEBUG
				SystemLogger.Log (SystemLogger.Module.PLATFORM, "StopUpdatingHeading() : HeadingAvailable = false");
				#endif
				isStoppedOk = true;
			}
			return isStoppedOk;
		}

		public override LocationCoordinate GetCoordinates ()
		{
			evaluateLocationCoordinates ();
			return locationCoordinate;
		}
		
		/// <summary>
		/// Setup the Location Manager settings (if needed)
		/// </summary>
		private void LocationManagerSetup() {
			if(locationManager != null && locationManager.Delegate == null) {
				
				SystemLogger.Log (SystemLogger.Module.PLATFORM, "LocationManagerSetup() : Setting Location Manager Delegate and settings");
				locationManager.Delegate = new UnityLocationManagerDelegate (unityLocation, unityHeading, geoDecoderAttributes);
				
				// **** Location settings **** 
				
				// The accuracy of the location data.
				locationManager.DesiredAccuracy = CLLocation.AccuracyBest; 
				// AccurracyBestForNavigation == Use the highest possible accuracy and combine it with additional sensor data. 
				// This level of accuracy is intended for use in navigation applications that require precise position information at all times 
				// and are intended to be used only while the device is plugged in.
				
				// The minimum distance (measured in meters) a device must move horizontally before an update event is generated.
				locationManager.DistanceFilter = 2;  // 2 meters 
				
				// **** Heading settings **** 
				
				// The minimum angular change (measured in degrees) required to generate new heading events.
				locationManager.HeadingFilter = 5;  //5 degree
				
				#if DEBUG
					SystemLogger.Log (SystemLogger.Module.PLATFORM, "** Location Manager - Desired Accuracy: " + locationManager.DesiredAccuracy);
					SystemLogger.Log (SystemLogger.Module.PLATFORM, "** Location Manager - Distance Filter (meters): " + locationManager.DistanceFilter);
					SystemLogger.Log (SystemLogger.Module.PLATFORM, "** Location Manager - Heading Filter (degrees): " + locationManager.HeadingFilter);
				#endif
			}
			
		}

		private void evaluateLocationCoordinates ()
		{
			if ( (CLLocationManager.LocationServicesEnabled) && (unityLocation != null) ) {
				locationCoordinate.XCoordinate = unityLocation.Latitude;
				locationCoordinate.YCoordinate = unityLocation.Longitude;
				locationCoordinate.ZCoordinate = unityLocation.Altitude;
				locationCoordinate.XDoP = (float)unityLocation.HorizontalAccuracy;
				locationCoordinate.YDoP = (float)unityLocation.VerticalAccuracy;
			} else {
				#if DEBUG
				SystemLogger.Log (SystemLogger.Module.PLATFORM, "GetCoordinates() not set to true LocationServicesEnabled");
				#endif
				locationCoordinate.XCoordinate = 0.0f;
				locationCoordinate.YCoordinate = 0.0f;
				locationCoordinate.ZCoordinate = 0.0f;
				locationCoordinate.XDoP = 0.0f;
				locationCoordinate.YDoP = 0.0f;
			}
		}

		public override float GetHeading ()
		{
			return GetHeading (NorthType.TrueNorth);
		}

		public override float GetHeading (NorthType type)
		{
			double heading = 0;
			if ( (CLLocationManager.HeadingAvailable) && (unityHeading != null) ){
				if (type == NorthType.TrueNorth) {
					heading = unityHeading.TrueHeading;
				}
				if (type == NorthType.MagneticNorth) {
					heading = unityHeading.MagneticHeading;
				}
			} else {
				#if DEBUG
				SystemLogger.Log (SystemLogger.Module.PLATFORM, "getGeoHeadingAttributes(): HeadingAvailable = false");
				#endif
			}
			#if DEBUG
			SystemLogger.Log (SystemLogger.Module.PLATFORM, "GetHeading(): Heading:" + heading);
			#endif
			
			return (float)heading;
		}

		public override void GetMap ()
		{
			using (var pool = new NSAutoreleasePool ()) {
				var thread = new Thread (ShowMap);
				thread.Start ();
			};
		}

		[Export("ShowMap")]
		private void ShowMap ()
		{
			UIApplication.SharedApplication.InvokeOnMainThread (delegate {
				StartUpdatingLocation();
				IPhoneUIViewController contentController = new IPhoneUIViewController ("Map", "Back");
				if (unityMapView == null) {
						unityMapView = new UnityMapView ();
				}
				coord = new CLLocationCoordinate2D (unityLocation.Latitude, unityLocation.Longitude);
				coordSpan = new MKCoordinateSpan (latitudeDeltaSpan, longitudeDeltaSpan);
				//0.5 means 0.5 degrees -> 55 km approximately; so it display an area of (55 Km * 55 Km) arround the current coordinates
				coordRegion = new MKCoordinateRegion (coord, coordSpan);
				setMapView ();
				setAnnotationPoints ();
				contentController.AddInnerView (unityMapView);
				IPhoneServiceLocator.CurrentDelegate.MainUIViewController ().PresentModalViewController (contentController, true);
				IPhoneServiceLocator.CurrentDelegate.SetMainUIViewControllerAsTopController(false);
			});
			
		}

		private void setMapView ()
		{
			unityMapView.SetRegion (coordRegion, true);
			unityMapView.SetCenterCoordinate (coord, true);
			setMapTypesInMap ();
		}

		private void setAnnotationPoints ()
		{
			cleanAllAnnotationsPins ();
			getAllAnnotationPins ();
			addAllAnnotationPinsToMap ();
			unityMapView.ShowsUserLocation = true;
		}

		private void cleanAllAnnotationsPins ()
		{
			//Cleaning the annotation points from the map and from annotationPins
			foreach (KeyValuePair<string, UnityAnnotation> pair in annotationPins) {
				unityMapView.RemoveAnnotation (pair.Value);
			}
			annotationPins.Clear ();
		}

		private void getAllAnnotationPins ()
		{
			//Getting all the POIs from the database and adding them to annotationPins
			const string selectFromPoi = "SELECT * FROM \"POI\"";
			const string selectFromLocCoordForPoi = "SELECT * FROM \"LOCATIONCOORDINATE\" WHERE POI_ID='";
			const string selectFromLocDescrForPoi = "SELECT * FROM \"LOCATIONDESCRIPTION\" WHERE POI_ID='";
			
			IResultSet resultSetPoi = mapDDBB.ExecuteSQLQuery (mapDDBB.GetDatabase (mapDDBBName), selectFromPoi);
			int numPois = resultSetPoi.GetRowCount ();
			for (int i = 0; i < numPois; i++) {
				string category = resultSetPoi.GetString ("CATEGORYMAIN");
				//It is the name
				string poiId = resultSetPoi.GetString ("ID");
				string fullSelectLocationCoordForPoiId = selectFromLocCoordForPoi + poiId + "'";
				string fullSelectLocationDescrForPoiId = selectFromLocDescrForPoi + poiId + "'";
				IResultSet resultSetLocationCoord = mapDDBB.ExecuteSQLQuery (mapDDBB.GetDatabase (mapDDBBName), fullSelectLocationCoordForPoiId);
				IResultSet resultSetLocationDescr = mapDDBB.ExecuteSQLQuery (mapDDBB.GetDatabase (mapDDBBName), fullSelectLocationDescrForPoiId);
				double latitude = resultSetLocationCoord.GetDouble ("LATITUDE");
				double longitude = resultSetLocationCoord.GetDouble ("LONGITUDE");
				string description = resultSetLocationDescr.GetString ("DESCRIPTION");
				resultSetPoi.MoveToNext ();
				CLLocationCoordinate2D latitudeLongitude = new CLLocationCoordinate2D (latitude, longitude);
				UnityAnnotation annotation = new UnityAnnotation (latitudeLongitude, category, description, MKPinAnnotationColor.Red);
				if (annotationPins.ContainsKey (poiId)) {
					annotationPins.Remove (poiId);
				}
				annotationPins.Add (poiId, annotation);
			}
		}

		private void addAllAnnotationPinsToMap ()
		{
			//Adding all the annotation points to the map
			foreach (KeyValuePair<string, UnityAnnotation> pair in annotationPins) {
				unityMapView.AddAnnotationObject (pair.Value);
			}
		}

		public override DeviceOrientation GetDeviceOrientation ()
		{
			// Device orientation notifications are not immediately available after notification starts;
			// however they are available after a very short time delay.
			
			// start generating orientation notifications.
			DeviceOrientation orientation = DeviceOrientation.Unknown;
			bool beginGeneratingNotifications = false;
			if (!UIDevice.CurrentDevice.GeneratesDeviceOrientationNotifications) {
				UIDevice.CurrentDevice.BeginGeneratingDeviceOrientationNotifications ();
				beginGeneratingNotifications = true;
			}
			
			switch (UIDevice.CurrentDevice.Orientation) {
			case UIDeviceOrientation.Portrait:
				orientation = DeviceOrientation.Portrait;
				break;
			case UIDeviceOrientation.PortraitUpsideDown:
				orientation = DeviceOrientation.PortraitUpsideDown;
				break;
			case UIDeviceOrientation.FaceDown:
				orientation = DeviceOrientation.FaceDown;
				break;
			case UIDeviceOrientation.FaceUp:
				orientation = DeviceOrientation.FaceUp;
				break;
			case UIDeviceOrientation.LandscapeLeft:
				orientation = DeviceOrientation.LandscapeLeft;
				break;
			case UIDeviceOrientation.LandscapeRight:
				orientation = DeviceOrientation.LandscapeRight;
				break;
			default:
				orientation = DeviceOrientation.Unknown;
				break;
			}
			
			// stop generating notifications
			if (beginGeneratingNotifications) {
				UIDevice.CurrentDevice.EndGeneratingDeviceOrientationNotifications ();
			}		
#if DEBUG
			SystemLogger.Log (SystemLogger.Module.PLATFORM, "Device orientation: " + orientation.ToString ());
#endif
			return orientation;
		}


		public override POI GetPOI (string id)
		{
			return getPoiFromId (id);
		}

		private POI getPoiFromId (string id)
		{
			POI poi = new POI ();
			LocationCoordinate locCoord = new LocationCoordinate ();
			LocationDescription locDesc = new LocationDescription ();
			LocationCategory locCat = new LocationCategory ();
			List<LocationCategory> listCategories = new List<LocationCategory> ();
			
			string selectFromPOI = "SELECT * FROM \"POI\" WHERE ID='" + id + "'";
			IResultSet resultSelectPoi = mapDDBB.ExecuteSQLQuery (mapDDBB.GetDatabase (mapDDBBName), selectFromPOI);
			
			if (resultSelectPoi.GetRowCount () > 0) {
				string selectLocationCoordinateForPoId = "SELECT * FROM \"LOCATIONCOORDINATE\" WHERE POI_ID='" + id + "'";
				string selectLocationDescriptionForPoiId = "SELECT * FROM \"LOCATIONDESCRIPTION\" WHERE POI_ID='" + id + "'";
				string selectSecondaryCategoryForPoiId = "SELECT * FROM \"SECONDARYCATEGORY\" WHERE POI_ID='" + id + "'";
				
				IResultSet resultSetSelectLocCoord = mapDDBB.ExecuteSQLQuery (mapDDBB.GetDatabase (mapDDBBName), selectLocationCoordinateForPoId);
				IResultSet resultSetSelectLocDescr = mapDDBB.ExecuteSQLQuery (mapDDBB.GetDatabase (mapDDBBName), selectLocationDescriptionForPoiId);
				IResultSet resultSetSelectSecondCat = mapDDBB.ExecuteSQLQuery (mapDDBB.GetDatabase (mapDDBBName), selectSecondaryCategoryForPoiId);
				
				double latitude = resultSetSelectLocCoord.GetDouble ("LATITUDE");
				double longitude = resultSetSelectLocCoord.GetDouble ("LONGITUDE");
				double altitude = resultSetSelectLocCoord.GetDouble ("ALTITUDE");
				float horAccur = resultSetSelectLocCoord.GetFloat ("HORIZONTALACCURACY");
				float verAccur = resultSetSelectLocCoord.GetFloat ("VERTICALACCURACY");
				
				string description = resultSetSelectLocDescr.GetString ("DESCRIPTION");
				string name = resultSetSelectLocDescr.GetString ("NAME");
				string catMainName = resultSetSelectLocDescr.GetString ("CATEGORYMAINNAME");
				
				poi.ID = id;
				
				locCoord.XCoordinate = latitude;
				locCoord.YCoordinate = longitude;
				locCoord.ZCoordinate = altitude;
				locCoord.XDoP = horAccur;
				locCoord.YDoP = verAccur;
				
				poi.Location = locCoord;
				
				locDesc.Description = description;
				locDesc.Name = name;
				locCat.Name = catMainName;
				locDesc.CategoryMain = locCat;
				
				int numRowsSecCat = resultSetSelectSecondCat.GetRowCount ();
				for (int i = 0; i < numRowsSecCat; i++) {
					string secondCatName = resultSetSelectSecondCat.GetString ("SECONDCATEGORYNAME");
					LocationCategory locCategory = new LocationCategory ();
					locCategory.Name = secondCatName;
					listCategories.Add (locCategory);
					resultSetSelectSecondCat.MoveToNext ();
					
				}
				
				locDesc.Categories = listCategories.ToArray ();
				poi.Description = locDesc;
				
				poi.Category = locCat;
				
			}  else {
				poi = null;
		}
			
		return poi;
		}

		public override POI[] GetPOIList (LocationCoordinate location, float radius, LocationCategory category)
		{
			return GetPOIList (location, radius, ".*", category);
		}

		public override POI[] GetPOIList (LocationCoordinate location, float radius)
		{
			LocationCategory dummyLocCat = new LocationCategory ();
			dummyLocCat.Name = ".*";
			return GetPOIList (location, radius, ".*", dummyLocCat);
		}

		public override POI[] GetPOIList (LocationCoordinate location, float radius, string queryText, LocationCategory category)
		{
			List<POI> poisInRadius = getPoisInRadius (location, radius);
			List<POI> poisMatchQuery = getPoisMatchQuery (poisInRadius, queryText);
			List<POI> poisFinal = getPoisMatchCategory (poisMatchQuery, category);
			return poisFinal.ToArray ();
		}

		public override POI[] GetPOIList (LocationCoordinate location, float radius, string queryText)
		{
			LocationCategory dummyLocCat = new LocationCategory();
			dummyLocCat.Name = ".*";
			return GetPOIList(location, radius, queryText, dummyLocCat);
		}

		private List<POI> getPoisInRadius (LocationCoordinate location, float radius)
		{
			List<POI> pois = new List<POI> ();
			string selectFromLocationCoordinate = "SELECT * FROM \"LOCATIONCOORDINATE\"";
			IResultSet resultSetFromLocationCoordinate = mapDDBB.ExecuteSQLQuery (mapDDBB.GetDatabase (mapDDBBName), selectFromLocationCoordinate);
			int numRows = resultSetFromLocationCoordinate.GetRowCount ();
			for (int i = 0; i < numRows; i++) {
				double latitude = resultSetFromLocationCoordinate.GetDouble ("LATITUDE");
				double longitude = resultSetFromLocationCoordinate.GetDouble ("LONGITUDE");
				string poiId = resultSetFromLocationCoordinate.GetString ("POI_ID");
				double distance = GeoUtils.GetInstance().evaluateDistanceSphericalLawOfCosines (location.XCoordinate, location.YCoordinate, latitude, longitude);
				#if DEBUG
				SystemLogger.Log (SystemLogger.Module.PLATFORM, "getPoisInRadius: lat1:" + location.XCoordinate + " lon1:" + location.YCoordinate + " lat2:" + latitude + " lon2:" + longitude + " Distance:" + distance + " Radius:" + radius);
				#endif
				if (distance <= radius) {
					POI poi = getPoiFromId (poiId);
					if (poi != null) {
						pois.Add (poi);
						#if DEBUG
						SystemLogger.Log (SystemLogger.Module.PLATFORM, "getPoisInRadius: Added POI_ID:" + poi.ID + " Latitude:" + latitude + " Longitude:" + longitude + " because:" + distance + "<=" + radius);
						#endif
					}
				}
				resultSetFromLocationCoordinate.MoveToNext ();
			}
			
			return pois;
		}

		private List<POI> getPoisMatchCategory (List<POI> inPois, LocationCategory locCat)
		{
			List<POI> pois = new List<POI> ();
			foreach (POI item in inPois) {
				bool isMatch;
				if (locCat.Name != null) {
					Regex pattern = new Regex (locCat.Name, RegexOptions.IgnoreCase);
					isMatch = pattern.IsMatch (item.Category.Name);
					if (isMatch) {
						pois.Add (item);
						#if DEBUG
						SystemLogger.Log (SystemLogger.Module.PLATFORM, "getPoisMatchCategory: Added: POI_ID:" + item.ID + "Category:" + item.Category.Name);
						#endif
					}
				}
			}
			return pois;
			
		}

		private List<POI> getPoisMatchQuery (List<POI> inPois, string queryString)
		{
			List<POI> pois = new List<POI> ();
			foreach (POI item in inPois) {
				bool isMatch;
				if (queryString != null) {
					int idx = queryString.IndexOf (@"=");
					string name = queryString.Substring (idx + 1);
					Regex pattern = new Regex (name, RegexOptions.IgnoreCase);
					string descr = item.Description.Name.ToString ();
					isMatch = pattern.IsMatch (descr);
					if (isMatch) {
						pois.Add (item);
						#if DEBUG
						SystemLogger.Log (SystemLogger.Module.PLATFORM, "getPoisMatchQuery: Added: POI_ID:" + item.ID + " Latitude:" + item.Location.XCoordinate + " Longitude:" + item.Location.YCoordinate + " Description:" + item.Description.Description.ToString ());
						#endif
					}
				}
			}
			return pois;
		}

		public override float GetVelocity ()
		{
			double velocity = ( (CLLocationManager.LocationServicesEnabled) && (unityLocation != null) ) ? unityLocation.Speed : 0.0f;
			#if DEBUG
			SystemLogger.Log (SystemLogger.Module.PLATFORM, "Velocity: " + velocity);
			#endif
			return (float)velocity;
		}

		public override bool RemovePOI (string id)
		{
			// Deleting every entry in every table because SQLite, by default, does not applies foreign keys behavior.
			// The foreign key behavior can be set creating triggers when the database and tables are created for the first time;
			// this can be programme in the next release.
			string deleteFromPOI = "DELETE FROM \"POI\" WHERE ID='" + id + "'";
			string deleteFromLocationCoordinate = "DELETE FROM \"LOCATIONCOORDINATE\" WHERE POI_ID='" + id + "'";
			string deleteFromLocationDescription = "DELETE FROM \"LOCATIONDESCRIPTION\" WHERE POI_ID='" + id + "'";
			string deleteFromSecondaryCategory = "DELETE FROM \"SECONDARYCATEGORY\" WHERE POI_ID='" + id + "'";
			
			bool isDeleteFromSecondaryCategoryOk = mapDDBB.ExecuteSQLStatement (mapDDBB.GetDatabase (mapDDBBName), deleteFromSecondaryCategory);
			bool isDeleteFromLocationDescriptionOk = mapDDBB.ExecuteSQLStatement (mapDDBB.GetDatabase (mapDDBBName), deleteFromLocationDescription);
			bool isDeleteFromLocationCoordinateOk = mapDDBB.ExecuteSQLStatement (mapDDBB.GetDatabase (mapDDBBName), deleteFromLocationCoordinate);
			bool isDeleteFromPoiOk = mapDDBB.ExecuteSQLStatement (mapDDBB.GetDatabase (mapDDBBName), deleteFromPOI);
			
			if (annotationPins.ContainsKey (id)) {
				UnityAnnotation annotationToRemove = annotationPins[id];
				unityMapView.RemoveAnnotation (annotationToRemove);
				annotationPins.Remove (id);
			}
			
			#if DEBUG
			SystemLogger.Log (SystemLogger.Module.PLATFORM, "Is OK deleting from SECONDARYCATEGORY the POI_ID " + id + ":" + isDeleteFromSecondaryCategoryOk);
			SystemLogger.Log (SystemLogger.Module.PLATFORM, "Is OK deleting from LOCATIONDESCRIPTION the POI_ID " + id + ":" + isDeleteFromLocationDescriptionOk);
			SystemLogger.Log (SystemLogger.Module.PLATFORM, "Is OK deleting from LOCATIONCOORDINATE the POI_ID " + id + ":" + isDeleteFromLocationCoordinateOk);
			SystemLogger.Log (SystemLogger.Module.PLATFORM, "Is OK deleting from POI the POI_ID " + id + ":" + isDeleteFromPoiOk);
			#endif
			return (isDeleteFromSecondaryCategoryOk && isDeleteFromLocationDescriptionOk && isDeleteFromLocationCoordinateOk && isDeleteFromPoiOk);
		}

		public override void SetMapSettings (float scale, float boundingBox)
		{
			latitudeDeltaSpan = scale;
			longitudeDeltaSpan = boundingBox;
		}

		public override bool UpdatePOI (POI poi)
		{
			#if DEBUG
			SystemLogger.Log (SystemLogger.Module.PLATFORM, "Before update POI table");
			#endif
			bool isPoiOk = updatePoiTable (poi);
			bool isLocCoordOk = updateLocationCoordinateTable (poi);
			bool isLocDescrOK = updateLocationDescriptionTable (poi);
			bool isSecondCatOK = updateSecondaryCategoryTable (poi);
			return (isPoiOk && isLocCoordOk && isLocDescrOK && isSecondCatOK);
		}

		private bool updatePoiTable (POI poi)
		{
			const string insertInPoiStatement = "INSERT INTO \"POI\" VALUES('";
			const string updateInPoiStatement = "UPDATE \"POI\" SET CATEGORYMAIN='";
			const string selectPoiFromPoiId = "SELECT * FROM \"POI\" WHERE ID='";
			string poiId = poi.ID;
			string category = poi.Description.Name;
			string fullInsertPoiStatement = insertInPoiStatement + poiId + "'," + "'" + category + "')";
			string fullUpdatePoiStatement = updateInPoiStatement + category + "' WHERE ID='" + poiId + "'";
			string fullSelectPoiFromPoiId = selectPoiFromPoiId + poiId + "'";
			#if DEBUG
			SystemLogger.Log (SystemLogger.Module.PLATFORM, "updatePoiTable INSERT statement: " + fullInsertPoiStatement);
			SystemLogger.Log (SystemLogger.Module.PLATFORM, "updatePoiTable UPDATE statement: " + fullUpdatePoiStatement);
			SystemLogger.Log (SystemLogger.Module.PLATFORM, "updatePoiTable SELECT statement: " + fullSelectPoiFromPoiId);
			#endif
			bool isSqlStatementOk;
			IResultSet resultSetSelect = mapDDBB.ExecuteSQLQuery (mapDDBB.GetDatabase (mapDDBBName), fullSelectPoiFromPoiId);
			int selectCount = resultSetSelect.GetRowCount ();
			if (selectCount > 0) {
				isSqlStatementOk = mapDDBB.ExecuteSQLStatement (mapDDBB.GetDatabase (mapDDBBName), fullUpdatePoiStatement);
				#if DEBUG
				SystemLogger.Log (SystemLogger.Module.PLATFORM, "updatePoiTable: Result of UPDATE POI table:" + isSqlStatementOk);
				#endif
			} else {
				isSqlStatementOk = mapDDBB.ExecuteSQLStatement (mapDDBB.GetDatabase (mapDDBBName), fullInsertPoiStatement);
				#if DEBUG
				SystemLogger.Log (SystemLogger.Module.PLATFORM, "updatePoiTable: Result of INSERT POI table:" + isSqlStatementOk);
				#endif
			}
			#if DEBUG
			SystemLogger.Log (SystemLogger.Module.PLATFORM, "updatePoiTable: Is INSERT/UPDATE into POI OK:" + isSqlStatementOk);
			IResultSet resultSet = mapDDBB.ExecuteSQLQuery (mapDDBB.GetDatabase (mapDDBBName), "SELECT * FROM \"POI\"");
			int numRows = resultSet.GetRowCount ();
			for (int i = 0; i < numRows; i++) {
				string resultId = resultSet.GetString ("ID");
				string resultText = resultSet.GetString ("CATEGORYMAIN");
				resultSet.MoveToNext ();
				SystemLogger.Log (SystemLogger.Module.PLATFORM, "updatePoiTable: ID in POI table:" + resultId);
				SystemLogger.Log (SystemLogger.Module.PLATFORM, "updatePoiTable: CATEGORYMAIN in POI table:" + resultText);
			}
			#endif
			return isSqlStatementOk;
		}

		private bool updateLocationCoordinateTable (POI poi)
		{
			const string insertInLocationCoordinateStatement = "INSERT INTO \"LOCATIONCOORDINATE\" VALUES(";
			const string updateInLocationCoordinateStatement = "UPDATE \"LOCATIONCOORDINATE\" SET LATITUDE=";
			const string selectFromLocationCoordinateForPoi = "SELECT * FROM \"LOCATIONCOORDINATE\" WHERE POI_ID='";
			string poiId = poi.ID;
			string latitude = poi.Location.XCoordinateString();
			string longitude = poi.Location.YCoordinateString();
			string altitude = poi.Location.ZCoordinateString();
			string horAccur = poi.Location.XDoPString();
			string verAccur = poi.Location.YDoPString();
			
			string fullInsertLocationCoordinateStatement = insertInLocationCoordinateStatement + latitude + "," + longitude + "," + altitude + "," + horAccur + "," + verAccur + ",'" + poiId + "')";
			string fullUpdateLocationCoordinateStatement = updateInLocationCoordinateStatement + latitude + ", LONGITUDE=" + longitude + ", ALTITUDE=" + altitude + ", HORIZONTALACCURACY=" + horAccur + ", VERTICALACCURACY=" + verAccur + " WHERE POI_ID='" + poiId + "'";
			string fullSelectLocationCoordinateForPoi = selectFromLocationCoordinateForPoi + poiId + "'";
			#if DEBUG
			SystemLogger.Log (SystemLogger.Module.PLATFORM, "updateLocationCoordinateTable INSERT statement: " + fullInsertLocationCoordinateStatement);
			SystemLogger.Log (SystemLogger.Module.PLATFORM, "updateLocationCoordinateTable UPDATE statement: " + fullUpdateLocationCoordinateStatement);
			SystemLogger.Log (SystemLogger.Module.PLATFORM, "updateLocationCoordinateTable SELECT statement: " + fullSelectLocationCoordinateForPoi);
			#endif
			IResultSet resultSetSelect = mapDDBB.ExecuteSQLQuery (mapDDBB.GetDatabase (mapDDBBName), fullSelectLocationCoordinateForPoi);
			int selectCount = resultSetSelect.GetRowCount ();
			bool isSqlStatementOk;
			if (selectCount > 0) {
				isSqlStatementOk = mapDDBB.ExecuteSQLStatement (mapDDBB.GetDatabase (mapDDBBName), fullUpdateLocationCoordinateStatement);
				#if DEBUG
				SystemLogger.Log (SystemLogger.Module.PLATFORM, "updateLocationCoordinateTable: Result of UPDATE LOCATIONCOORDINATE table:" + isSqlStatementOk);
				#endif				
			} else {
				isSqlStatementOk = mapDDBB.ExecuteSQLStatement (mapDDBB.GetDatabase (mapDDBBName), fullInsertLocationCoordinateStatement);
				#if DEBUG
				SystemLogger.Log (SystemLogger.Module.PLATFORM, "updateLocationCoordinateTable: Result of UPDATE LOCATIONCOORDINATE table:" + isSqlStatementOk);
				#endif
			}
			#if DEBUG
			SystemLogger.Log (SystemLogger.Module.PLATFORM, "Is INSERT/UPDATE into LOCATIONCOORDINATE OK:" + isSqlStatementOk);
			const string selectFromLocationCoordinate = "SELECT * FROM \"LOCATIONCOORDINATE\"";
			IResultSet resultSet = mapDDBB.ExecuteSQLQuery (mapDDBB.GetDatabase (mapDDBBName), selectFromLocationCoordinate);
			int numRows = resultSet.GetRowCount ();
			for (int i = 0; i < numRows; i++) {
				string resultId = resultSet.GetString ("POI_ID");
				double resultLatitude = resultSet.GetDouble ("LATITUDE");
				double resultLongitude = resultSet.GetDouble ("LONGITUDE");
				double resultAltitude = resultSet.GetDouble ("ALTITUDE");
				float resultHorAccur = resultSet.GetFloat ("HORIZONTALACCURACY");
				float resultVerAccur = resultSet.GetFloat ("VERTICALACCURACY");
				resultSet.MoveToNext ();
				SystemLogger.Log (SystemLogger.Module.PLATFORM, "POI_ID in POI table:" + resultId);
				SystemLogger.Log (SystemLogger.Module.PLATFORM, "LATITUDE in POI table:" + resultLatitude);
				SystemLogger.Log (SystemLogger.Module.PLATFORM, "LONGITUDE in POI table:" + resultLongitude);
				SystemLogger.Log (SystemLogger.Module.PLATFORM, "ALTITUDE in POI table:" + resultAltitude);
				SystemLogger.Log (SystemLogger.Module.PLATFORM, "HORIZONTALACCURACY in POI table:" + resultHorAccur);
				SystemLogger.Log (SystemLogger.Module.PLATFORM, "VERTICALACCURACY in POI table:" + resultVerAccur);
			}
			#endif
			return isSqlStatementOk;
		}

		private bool updateLocationDescriptionTable (POI poi)
		{
			const string insertInLocationDescriptionStatement = "INSERT INTO \"LOCATIONDESCRIPTION\" VALUES('";
			const string updateInLocationDescriptionStatement = "UPDATE \"LOCATIONDESCRIPTION\" SET DESCRIPTION='";
			const string selectLocationDescriptionForPoi = "SELECT * FROM \"LOCATIONDESCRIPTION\" WHERE POI_ID='";
			string poiId = poi.ID;
			string description = poi.Description.Description;
			string name = poi.Description.Name;
			string categoryMainName = poi.Description.CategoryMain.Name;
			string fullInsertLocationDescriptionStatement = insertInLocationDescriptionStatement + description + "'," + "'" + name + "'," + "'" + categoryMainName.ToString () + "','" + poiId + "')";
			string fullUpdateInLocationDescriptionStatement = updateInLocationDescriptionStatement + description + "', NAME='" + name + "', CATEGORYMAINNAME='" + categoryMainName + "' WHERE POI_ID='" + poiId + "'";
			string fullSelectLocationDescriptionForPoi = selectLocationDescriptionForPoi + poiId + "'";
			#if DEBUG
			SystemLogger.Log (SystemLogger.Module.PLATFORM, "updateLocationDescriptionTable INSERT statement: " + fullInsertLocationDescriptionStatement);
			SystemLogger.Log (SystemLogger.Module.PLATFORM, "updateLocationDescriptionTable UPDATE statement: " + fullUpdateInLocationDescriptionStatement);
			SystemLogger.Log (SystemLogger.Module.PLATFORM, "updateLocationDescriptionTable SELECT statement: " + fullSelectLocationDescriptionForPoi);
			#endif
			IResultSet resultSetSelect = mapDDBB.ExecuteSQLQuery (mapDDBB.GetDatabase (mapDDBBName), fullSelectLocationDescriptionForPoi);
			int selectCount = resultSetSelect.GetRowCount ();
			bool isSqlStatementOk;
			if (selectCount > 0) {
				isSqlStatementOk = mapDDBB.ExecuteSQLStatement (mapDDBB.GetDatabase (mapDDBBName), fullUpdateInLocationDescriptionStatement);
				#if DEBUG
				SystemLogger.Log (SystemLogger.Module.PLATFORM, "updateLocationDescriptionTable: UPDATING:" + isSqlStatementOk);
				#endif
			} else {
				isSqlStatementOk = mapDDBB.ExecuteSQLStatement (mapDDBB.GetDatabase (mapDDBBName), fullInsertLocationDescriptionStatement);
				#if DEBUG
				SystemLogger.Log (SystemLogger.Module.PLATFORM, "updateLocationDescriptionTable: INSERTING:" + isSqlStatementOk);
				#endif
			}
			#if DEBUG
			SystemLogger.Log (SystemLogger.Module.PLATFORM, "Is INSERT/UPDATE into LOCATIONDESCRIPTION OK:" + isSqlStatementOk);
			const string selectFromLocationDescription = "SELECT * FROM \"LOCATIONDESCRIPTION\"";
			IResultSet resultSet = mapDDBB.ExecuteSQLQuery (mapDDBB.GetDatabase (mapDDBBName), selectFromLocationDescription);
			int numRows = resultSet.GetRowCount ();
			for (int i = 0; i < numRows; i++) {
				string resultId = resultSet.GetString ("POI_ID");
				string resultDescription = resultSet.GetString ("DESCRIPTION");
				string resultName = resultSet.GetString ("NAME");
				string resultCatMainName = resultSet.GetString ("CATEGORYMAINNAME");
				resultSet.MoveToNext ();
				SystemLogger.Log (SystemLogger.Module.PLATFORM, "POI_ID in LOCATIONDESCRIPTION table:" + resultId);
				SystemLogger.Log (SystemLogger.Module.PLATFORM, "DESCRIPTION in LOCATIONDESCRIPTION table:" + resultDescription);
				SystemLogger.Log (SystemLogger.Module.PLATFORM, "NAME in LOCATIONDESCRIPTION table:" + resultName);
				SystemLogger.Log (SystemLogger.Module.PLATFORM, "CATEGORYMAINNAME in LOCATIONDESCRIPTION table:" + resultCatMainName);
			}
			#endif
			return isSqlStatementOk;
		}

		private bool updateSecondaryCategoryTable (POI poi)
		{
			const string insertInSecondaryCategoryStatement = "INSERT INTO \"SECONDARYCATEGORY\" VALUES('";
			const string deleteInSecondaryCategoryStatement = "DELETE FROM \"SECONDARYCATEGORY\" WHERE POI_ID='";
			const string selectSecondaryCategoryEntriesForPoiId = "SELECT * FROM SECONDARYCATEGORY WHERE POI_ID='";
			
			string poiId = poi.ID;
			LocationCategory[] categories = poi.Description.Categories;
			string fullSelectSecondaryCategoryEntriesForPoid = selectSecondaryCategoryEntriesForPoiId + poiId + "'";
			string fullDeleteSecondaryCategoryEntriesForPoid = deleteInSecondaryCategoryStatement + poiId + "'";
			#if DEBUG
			SystemLogger.Log (SystemLogger.Module.PLATFORM, "updateSecondaryCategoryTable: SELECT statement: " + fullSelectSecondaryCategoryEntriesForPoid);
			SystemLogger.Log (SystemLogger.Module.PLATFORM, "updateSecondaryCategoryTable: DELETE statement: " + fullDeleteSecondaryCategoryEntriesForPoid);
			#endif
			IResultSet resultSetSelect = mapDDBB.ExecuteSQLQuery (mapDDBB.GetDatabase (mapDDBBName), fullSelectSecondaryCategoryEntriesForPoid);
			int countPoi = resultSetSelect.GetRowCount ();
			if (countPoi > 0) { 
				mapDDBB.ExecuteSQLQuery (mapDDBB.GetDatabase (mapDDBBName), fullDeleteSecondaryCategoryEntriesForPoid);
			}
			int numElements = categories.Length;
			bool[] results = new bool[numElements];
			int j = 0;
			foreach (LocationCategory item in categories) {
				string category = item.Name;
				string fullInsertSecondCategoryStatement = insertInSecondaryCategoryStatement + category + "'," + "'" + poiId + "')";
				#if DEBUG
				SystemLogger.Log (SystemLogger.Module.PLATFORM, "updateSecondaryCategoryTable: INSERT statement: " + fullInsertSecondCategoryStatement);
				#endif
				bool isSqlStatementOk = mapDDBB.ExecuteSQLStatement (mapDDBB.GetDatabase (mapDDBBName), fullInsertSecondCategoryStatement);
				results[j] = isSqlStatementOk;
				#if DEBUG
				SystemLogger.Log (SystemLogger.Module.PLATFORM, "updateSecondaryCategoryTable: Is insert into SECONDARYCATEGORY OK:" + isSqlStatementOk);
				SystemLogger.Log (SystemLogger.Module.PLATFORM, "updateSecondaryCategoryTable: results[" + j + "]:" + results[j]);
				#endif
				++j;
			}
			#if DEBUG
			const string selectSecondaryCategory = "SELECT * FROM \"SECONDARYCATEGORY\"";
			IResultSet resultSet = mapDDBB.ExecuteSQLQuery (mapDDBB.GetDatabase (mapDDBBName), selectSecondaryCategory);
			int numRows = resultSet.GetRowCount ();
			for (int i = 0; i < numRows; i++) {
				string resultId = resultSet.GetString ("POI_ID");
				string resultSecCategName = resultSet.GetString ("SECONDCATEGORYNAME");
				resultSet.MoveToNext ();
				SystemLogger.Log (SystemLogger.Module.PLATFORM, "POI_ID in SECONDARYCATEGORY table:" + resultId);
				SystemLogger.Log (SystemLogger.Module.PLATFORM, "SECONDCATEGORYNAME in SECONDARYCATEGORY table:" + resultSecCategName);
			}
			#endif
			bool areAllInsertOK = true;
			for (int i = 0; i < numElements; i++)
			{
				areAllInsertOK = areAllInsertOK && results[i];
			}
			return areAllInsertOK;
		}

		public override GeoDecoderAttributes GetGeoDecoder ()
		{
			#if DEBUG
			SystemLogger.Log (SystemLogger.Module.PLATFORM, "Latitude: " + locationManager.Location.Coordinate.Latitude.ToString ());
			SystemLogger.Log (SystemLogger.Module.PLATFORM, "Longitude: " + locationManager.Location.Coordinate.Longitude.ToString ());
			SystemLogger.Log (SystemLogger.Module.PLATFORM, "Additional City Info: " + geoDecoderAttributes.AdditionalCityLevelInfo);
			SystemLogger.Log (SystemLogger.Module.PLATFORM, "Additional Street Info: " + geoDecoderAttributes.AdditionalStreetLevelInfo);	
			SystemLogger.Log (SystemLogger.Module.PLATFORM, "Country: " + geoDecoderAttributes.Country);
			SystemLogger.Log (SystemLogger.Module.PLATFORM, "Country Code: " + geoDecoderAttributes.CountryCode);				
			SystemLogger.Log (SystemLogger.Module.PLATFORM, "Locality: " + geoDecoderAttributes.Locality);		
			SystemLogger.Log (SystemLogger.Module.PLATFORM, "Postal code: " + geoDecoderAttributes.PostalCode);		
			SystemLogger.Log (SystemLogger.Module.PLATFORM, "Street address: " + geoDecoderAttributes.StreetAddress);	
			SystemLogger.Log (SystemLogger.Module.PLATFORM, "Subadministrative area: " + geoDecoderAttributes.SubAdministrativeArea);		
			#endif
			return geoDecoderAttributes;
		}
		
		public override bool StartProximitySensor ()
		{
			bool result = false;
			UIDevice.CurrentDevice.ProximityMonitoringEnabled = true;
			if ( UIDevice.CurrentDevice.ProximityMonitoringEnabled == true ) {
				result = true;
			}
#if DEBUG
			SystemLogger.Log (SystemLogger.Module.PLATFORM, "StartProximitySensor: " + result);
#endif
			return result;
		}
		
		public override bool StopProximitySensor ()
		{
			bool result = false;
			UIDevice.CurrentDevice.ProximityMonitoringEnabled = false;
			if ( UIDevice.CurrentDevice.ProximityMonitoringEnabled == false ) {
				result = true;
			}
#if DEBUG
			SystemLogger.Log (SystemLogger.Module.PLATFORM, "StopProximitySensor: " + result);
#endif
			return result;
		}
		
		public override bool IsGPSEnabled ()
		{
			bool result = false;
			
			try {
				result = CLLocationManager.LocationServicesEnabled;
			} catch (Exception ex) {
#if DEBUG
			SystemLogger.Log (SystemLogger.Module.PLATFORM, "Unhandle exception checking location services availability: " + ex.Message);
#endif
			}
			return result;
		}
		
	}
}
