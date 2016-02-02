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

import java.util.ArrayList;
import java.util.List;

import android.content.ContentValues;
import android.database.Cursor;
import android.database.SQLException;
import android.database.sqlite.SQLiteDatabase;
import android.database.sqlite.SQLiteOpenHelper;

import com.gft.unity.core.geo.LocationCategory;
import com.gft.unity.core.geo.LocationCoordinate;
import com.gft.unity.core.geo.LocationDescription;
import com.gft.unity.core.geo.POI;
import com.gft.unity.core.system.SystemLogger;
import com.gft.unity.core.system.SystemLogger.Module;

public class AndroidPOIDatabase extends SQLiteOpenHelper {

	private static final SystemLogger LOG = AndroidSystemLogger.getInstance();

	private static final int DATABASE_VERSION = 1;
	private static final String DATABASE_NAME = "unitypoidb";
	private static final String POI_TABLE_NAME = "poi";
	private static final String SECONDARY_TABLE_NAME = "secondary";
	public static final String _ID = "_id";
	public static final String KEY_POI_ID = "poi_id";
	public static final String KEY_NAME = "name";
	public static final String KEY_DESCRIPTION = "description";
	public static final String KEY_MAIN_CATEGORY = "main_category";
	public static final String KEY_LATITUDE = "latitude";
	public static final String KEY_LONGITUDE = "longitude";
	public static final String KEY_ALTITUDE = "altitude";
	public static final String KEY_ACCURACY = "accuracy";
	public static final String KEY_SEC_CATEGORY = "sec_category";

	private SQLiteDatabase db;

	public AndroidPOIDatabase() {
		super(AndroidServiceLocator.getContext(), DATABASE_NAME, null,
				DATABASE_VERSION);
	}

	@Override
	public void onCreate(SQLiteDatabase db) {
		// Creates the POI table
		db.execSQL("CREATE TABLE " + POI_TABLE_NAME + " (" + _ID
				+ " INTEGER PRIMARY KEY AUTOINCREMENT, " + KEY_POI_ID
				+ " TEXT, " + KEY_NAME + " TEXT," + KEY_DESCRIPTION + " TEXT,"
				+ KEY_MAIN_CATEGORY + " TEXT," + KEY_LATITUDE + " REAL,"
				+ KEY_LONGITUDE + " REAL," + KEY_ALTITUDE + " REAL,"
				+ KEY_ACCURACY + " REAL);");

		// Creates Secondary Category Table
		db.execSQL("CREATE TABLE " + SECONDARY_TABLE_NAME + " (" + _ID
				+ " INTEGER REFERENCES " + POI_TABLE_NAME + "(" + _ID
				+ ") ON DELETE CASCADE, " + KEY_POI_ID + " TEXT, "
				+ KEY_SEC_CATEGORY + " TEXT);");
	}

	@Override
	public void onOpen(SQLiteDatabase db) {
		super.onOpen(db);
		if (!db.isReadOnly()) {
			// Enable foreign key constraints
			db.execSQL("PRAGMA foreign_keys=ON;");
		}
	}

	@Override
	public void onUpgrade(SQLiteDatabase arg0, int arg1, int arg2) {
		// nothing to do here
	}

	private void open() {
		if (db == null || !db.isOpen() || db.isReadOnly()) {
			db = getWritableDatabase();
		}
	}

	public boolean exists(POI poi) {

		if (poi == null || poi.getID() == null || poi.getID().equals("")) {
			return false;
		}
		try {
			open();
			Cursor c = db.query(POI_TABLE_NAME, new String[] { KEY_POI_ID },
					KEY_POI_ID + " = '" + poi.getID() + "'", null, null, null,
					null);
			return c.getCount() > 0;
		} finally {
			close();
		}
	}

	public POI getPOI(String id) {

		try {
			if (id == null || id == "") {
				return null;
			}
			try {
				open();
				Cursor poisCursor = db.rawQuery("SELECT * FROM "
						+ POI_TABLE_NAME + " WHERE " + KEY_POI_ID + " = '" + id
						+ "'", null);
				Cursor categoriesCursor = db.rawQuery("SELECT "
						+ KEY_SEC_CATEGORY + " FROM " + SECONDARY_TABLE_NAME
						+ " WHERE " + KEY_POI_ID + " = '" + id + "'", null);
				List<POI> pois = cursorToPOI(poisCursor, categoriesCursor);
				if (pois.size() > 0) {
					return pois.get(0);
				} else {
					return null;
				}
			} finally {
				close();
			}
		} catch (SQLException e) {
			LOG.Log(Module.PLATFORM, "Unable to get POI [" + id + "]", e);
			return null;
		}
	}

	public POI getPOI(POI poi) {
		return getPOI(poi.getID());
	}

	public boolean updatePOI(POI poi) {

		try {
			if (poi.getID() == null || poi.getID() == "") {
				return false;
			}
			try {
				open();
				boolean poiUpdated = db.update(POI_TABLE_NAME,
						POIToContentValues(poi),
						KEY_POI_ID + " = '" + poi.getID() + "'", null) > 0;
				db.delete(SECONDARY_TABLE_NAME,
						KEY_POI_ID + " = '" + poi.getID() + "'", null);
				if (poi.getDescription() != null
						&& poi.getDescription().getCategories() != null) {
					for (LocationCategory cat : poi.getDescription()
							.getCategories()) {
						ContentValues values = new ContentValues();
						values.put(KEY_POI_ID, poi.getID());
						values.put(KEY_SEC_CATEGORY, cat.getName());
						db.insert(SECONDARY_TABLE_NAME, null, values);
						poiUpdated = true;
					}
				}
				return poiUpdated;
			} finally {
				close();
			}
		} catch (SQLException e) {
			LOG.Log(Module.PLATFORM, "Unable to update POI [" + poi.getID()
					+ "]", e);
			return false;
		}
	}

	public boolean removePOI(POI poi) {
		return removePOI(poi.getID());
	}

	public boolean removePOI(String id) {
		try {
			if (id == null || id == "") {
				return false;
			}
			try {
				open();
				db.delete(POI_TABLE_NAME, KEY_POI_ID + " = '" + id + "'", null);
				return true;
			} finally {
				close();
			}
		} catch (SQLException e) {
			LOG.Log(Module.PLATFORM, "Unable to demove POI [" + id + "]", e);
			return false;
		}
	}

	public boolean insertPOI(POI poi) {

		try {
			try {
				ContentValues values = POIToContentValues(poi);
				if (values != null) {
					open();
					db.insert(POI_TABLE_NAME, null, values);
					return true;
				}
			} finally {
				close();
			}
			return true;
		} catch (SQLException e) {
			LOG.Log(Module.PLATFORM, "Unable to insert POI [" + poi.getID()
					+ "]", e);
			return false;
		}
	}

	private ContentValues POIToContentValues(POI poi) {

		if (poi == null) {
			return null;
		}
		ContentValues values = new ContentValues();
		if (poi.getID() != null || poi.getID() != "") {
			values.put(KEY_POI_ID, poi.getID());
		}
		values.put(KEY_NAME, poi.getDescription().getName());
		values.put(KEY_DESCRIPTION, poi.getDescription().getDescription());
		values.put(KEY_MAIN_CATEGORY, poi.getDescription().getCategoryMain()
				.getName());
		values.put(KEY_LONGITUDE, poi.getLocation().getXCoordinateString());
		values.put(KEY_LATITUDE, poi.getLocation().getYCoordinateString());
		values.put(KEY_ALTITUDE, poi.getLocation().getZCoordinate());
		values.put(KEY_ACCURACY, poi.getLocation().getXDoP());

		return values;
	}

	public List<POI> getAllPOIs() {
		List<POI> pois = new ArrayList<POI>();
		
		try {
			try {
				open();
				Cursor p = db.rawQuery("SELECT * FROM " + POI_TABLE_NAME, null);
				Cursor c = db.rawQuery("SELECT * FROM " + SECONDARY_TABLE_NAME,
						null);
				return cursorToPOI(p, c);
			} finally {
				close();
			}
		} catch (SQLException e) {
			LOG.Log(Module.PLATFORM, "Unable to get all POIs ", e);
			return pois;
		}
	}

	private List<POI> cursorToPOI(Cursor poisCursor, Cursor categoriesCursor) {
		List<POI> pois = new ArrayList<POI>();
		
		if (poisCursor != null && poisCursor.getCount() > 0) {
			poisCursor.moveToFirst();
			for (; !poisCursor.isAfterLast(); poisCursor.moveToNext()) {
				POI poi = new POI();
				// ID
				poi.setID(poisCursor.getString(poisCursor
						.getColumnIndex(KEY_POI_ID)));
				// Main Category
				LocationCategory category = new LocationCategory();
				category.setName(poisCursor.getString(poisCursor
						.getColumnIndex(KEY_MAIN_CATEGORY)));
				poi.setCategory(category);
				// LocationCoordinate
				LocationCoordinate coord = new LocationCoordinate();
				coord.setXCoordinate(poisCursor.getDouble(poisCursor
						.getColumnIndex(KEY_LONGITUDE)));
				coord.setYCoordinate(poisCursor.getDouble(poisCursor
						.getColumnIndex(KEY_LATITUDE)));
				coord.setZCoordinate(poisCursor.getDouble(poisCursor
						.getColumnIndex(KEY_ALTITUDE)));
				coord.setXDoP(poisCursor.getFloat(poisCursor
						.getColumnIndex(KEY_ACCURACY)));
				coord.setYDoP(poisCursor.getFloat(poisCursor
						.getColumnIndex(KEY_ACCURACY)));
				poi.setLocation(coord);
				// LocationDescription
				LocationDescription description = new LocationDescription();
				description.setCategoryMain(category);// TODO again?
				description.setName(poisCursor.getString(poisCursor
						.getColumnIndex(KEY_NAME)));
				description.setDescription(poisCursor.getString(poisCursor
						.getColumnIndex(KEY_DESCRIPTION)));
				if (categoriesCursor != null && categoriesCursor.getCount() > 0) {
					categoriesCursor.moveToFirst();
					List<LocationCategory> cats = new ArrayList<LocationCategory>(
							categoriesCursor.getCount());
					for (; !poisCursor.isAfterLast(); poisCursor.moveToNext()) {
						LocationCategory cat = new LocationCategory();
						if (poi.getID().equals(
								categoriesCursor.getString(categoriesCursor
										.getColumnIndex(KEY_POI_ID)))) {
							cat.setName(categoriesCursor
									.getString(categoriesCursor
											.getColumnIndex(KEY_SEC_CATEGORY)));
							cats.add(cat);
						}
					}
					description.setCategories(cats
							.toArray(new LocationCategory[cats.size()]));
				} else {
					description.setCategories(new LocationCategory[] {});
				}
				poi.setDescription(description);
				pois.add(poi);

			}
		}
		
		return pois;
	}

	public List<POI> getPOIList(LocationCoordinate location, float radius) {
		
		List<POI> pois = getAllPOIs();
		for (POI poi : pois) {
			if (!isPOIInRadius(poi, location, radius)) {
				pois.remove(poi);
			}
		}
		
		return pois;
	}

	public boolean isPOIInRadius(POI poi, LocationCoordinate location,
			float radius) {
		// average radius of the Earth in meters
		final double R = 6371000.0f;

		// latitude1: is the given location (bounding box center) latitude (x
		// coordinate)
		double latitude1 = Math.toRadians(location.getYCoordinate());
		// latitude2: is the POI latitude (the x coordinate of the POI that we
		// want to evaluate to be inside the bounding box)
		double latitude2 = Math.toRadians(poi.getLocation().getYCoordinate());
		// longitude1: is the given location (bounding box center) longitude (y
		// coordinate)
		double longitude1 = Math.toRadians(location.getXCoordinate());
		// longitude2: is the POI longitude (the y coordinate of the POI that we
		// want to evaluate to be inside the bounding box)
		double longitude2 = Math.toRadians(poi.getLocation().getXCoordinate());
		// distance: is the distance in meters between those two points
		double distance = Math.acos(Math.sin(latitude1) * Math.sin(latitude2)
				+ Math.cos(latitude1) * Math.cos(latitude2)
				* Math.cos(longitude2 - longitude1))
				* R;

		return distance <= radius;
	}
}
