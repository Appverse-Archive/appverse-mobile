/*
 Copyright (c) 2012 GFT Appverse, S.L., Sociedad Unipersonal.

 This Source  Code Form  is subject to the  terms of  the Appverse Public License 
 Version 2.0  ("APL v2.0").  If a copy of  the APL  was not  distributed with this 
 file, You can obtain one at http://www.appverse.mobi/licenses/apl_v2.0.pdf.

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

import java.lang.reflect.Type;

import android.database.Cursor;
import android.database.CursorWindow;
import android.database.MatrixCursor;

import com.gft.unity.core.storage.database.IResultSet;
import com.gft.unity.core.storage.database.ResultSetRow;

// TODO proper handling of column types (populateMatrixCursor, GetColumnType and GetRowsList)
public class AndroidResultSet implements IResultSet {

	private MatrixCursor mxcursor;

	public AndroidResultSet(Cursor c) {
		populateMatrixCursor(c);
	}

	private void populateMatrixCursor(Cursor cursor) {
		String[] columnNames = cursor.getColumnNames();
		mxcursor = new MatrixCursor(columnNames, cursor.getCount());
		cursor.moveToFirst();
		while (!cursor.isAfterLast()) {
			Object[] row = new Object[columnNames.length];
			for (String columnName : columnNames) {
				int index = cursor.getColumnIndex(columnName);
				row[index] = cursor.getString(index);
			}
			cursor.moveToNext();
			mxcursor.addRow(row);
		}
		cursor.close();
	}

	@Override
	public void Close() {
		if (!mxcursor.isClosed()) {
			mxcursor.close();
		}
	}

	@Override
	public byte[] GetBytes(int columnIndex) {
		return mxcursor.getBlob(columnIndex);
	}

	@Override
	public byte[] GetBytes(String columnName) {
		return GetBytes(mxcursor.getColumnIndex(columnName));
	}

	@Override
	public int GetColumnCount() {
		return mxcursor.getColumnCount();
	}

	@Override
	public int GetColumnIndex(String columnName) {
		return mxcursor.getColumnIndex(columnName);
	}

	@Override
	public String GetColumnName(int columnIndex) {
		return mxcursor.getColumnName(columnIndex);
	}

	@Override
	public String[] GetColumnNames() {
		return mxcursor.getColumnNames();
	}

	@Override
	public Type GetColumnType(int columnIndex) {
		
		CursorWindow window = mxcursor.getWindow();
		if (window != null) {
			if (window.isBlob(0, columnIndex)) {
				return byte[].class;
			} else if (window.isLong(0, columnIndex)) {
				return long.class;
			} else if (window.isFloat(0, columnIndex)) {
				return float.class;
			} else if (window.isString(0, columnIndex)) {
				return String.class;
			}
		}

		return null;
	}

	@Override
	public Type GetColumnType(String columnName) {
		return GetColumnType(mxcursor.getColumnIndex(columnName));

	}

	@Override
	public int GetCurrentPosition() {
		return mxcursor.getPosition();
	}

	@Override
	public double GetDouble(int columnIndex) {
		return mxcursor.getDouble(columnIndex);
	}

	@Override
	public double GetDouble(String columnName) {
		return GetDouble(mxcursor.getColumnIndex(columnName));
	}

	@Override
	public float GetFloat(int columnIndex) {
		return mxcursor.getFloat(columnIndex);
	}

	@Override
	public float GetFloat(String columnName) {
		return GetFloat(mxcursor.getColumnIndex(columnName));
	}

	@Override
	public int GetInt(int columnIndex) {
		return mxcursor.getInt(columnIndex);
	}

	@Override
	public int GetInt(String columnName) {
		return GetInt(mxcursor.getColumnIndex(columnName));
	}

	@Override
	public long GetLong(int columnIndex) {
		return mxcursor.getLong(columnIndex);
	}

	@Override
	public long GetLong(String columnName) {
		return GetLong(mxcursor.getColumnIndex(columnName));
	}

	@Override
	public int GetRowCount() {
		return mxcursor.getCount();
	}

	@Override
	public short GetShort(int columnIndex) {
		return mxcursor.getShort(columnIndex);
	}

	@Override
	public short GetShort(String columnName) {
		return GetShort(mxcursor.getColumnIndex(columnName));
	}

	@Override
	public String GetString(int columnIndex) {
		return mxcursor.getString(columnIndex);
	}

	@Override
	public String GetString(String columnName) {
		return GetString(mxcursor.getColumnIndex(columnName));
	}

	@Override
	public boolean IsClosed() {
		return mxcursor.isClosed();
	}

	@Override
	public boolean IsNull(int columnIndex) {
		return mxcursor.isNull(columnIndex);
	}

	@Override
	public boolean Move(int offset) {
		try {
			mxcursor.move(offset);
			return true;
		} catch (Exception e) {
			return false;
		}
	}

	@Override
	public boolean MoveToFirst() {
		try {
			mxcursor.moveToFirst();
			return true;
		} catch (Exception e) {
			return false;
		}
	}

	@Override
	public boolean MoveToLast() {
		try {
			mxcursor.moveToLast();
			return true;
		} catch (Exception e) {
			return false;
		}
	}

	@Override
	public boolean MoveToNext() {
		try {
			mxcursor.moveToNext();
			return true;
		} catch (Exception e) {
			return false;
		}
	}

	@Override
	public boolean MoveToPosition(int position) {
		try {
			mxcursor.moveToPosition(position);
			return true;
		} catch (Exception e) {
			return false;
		}
	}

	@Override
	public boolean MoveToPrevious() {
		try {
			mxcursor.moveToPrevious();
			return true;
		} catch (Exception e) {
			return false;
		}
	}

	public ResultSetRow[] GetRowsList() {		
		ResultSetRow[] rows = new ResultSetRow[GetRowCount()];
		
		mxcursor.moveToFirst();
		int index = 0;
		while (!mxcursor.isAfterLast()) {
			ResultSetRow row = new ResultSetRow();
			Object[] columnsList = new Object[GetColumnCount()];
			String[] columnNames = mxcursor.getColumnNames();
			for (String columnName : columnNames) {
				int colIndex = mxcursor.getColumnIndex(columnName);
				Type t = GetColumnType(colIndex);
				if (String.class.equals(t)) {
					columnsList[colIndex] = (mxcursor.getString(colIndex));
				} else if (long.class.equals(t)) {
					columnsList[colIndex] = (mxcursor.getLong(colIndex));
				} else if (byte[].class.equals(t)) {
					columnsList[colIndex] = (mxcursor.getBlob(colIndex));
				} else if (float.class.equals(t)) {
					columnsList[colIndex] = (mxcursor.getFloat(colIndex));
				} else { // t == null
					columnsList[colIndex] = (mxcursor.getString(colIndex));
				}
			}

			row.setColumnsList(columnsList);
			rows[index] = row;
			mxcursor.moveToNext();
			index++;
		}
		
		return rows;
	}
}
