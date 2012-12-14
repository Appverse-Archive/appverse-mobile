/*
 Copyright (c) 2012 GFT Appverse, S.L., Sociedad Unipersonal.

 This Source  Code Form  is subject to the  terms of  the Appverse Public License 
 Version 2.0  (“APL v2.0”).  If a copy of  the APL  was not  distributed with this 
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
package com.gft.unity.core.storage.database;

import java.lang.reflect.Type;

public interface IResultSet {

    /**
     * Returns the number of rows in the current result set.
     *
     * @return Number of rows.
     */
    public int GetRowCount();

    /**
     * Returns the number of total columns in the current result set.
     *
     * @return Number of columns.
     */
    public int GetColumnCount();

    /**
     * Returns the column name for the given column index.
     *
     * @param columnIndex Column index.
     * @return Column name.
     */
    public String GetColumnName(int columnIndex);

    /**
     * Returns the column name for the given column name.
     *
     * @param columnName Column name.
     * @return Zero-based index, or -1 if that column name is not returned by
     * this result set.
     */
    public int GetColumnIndex(String columnName);

    /**
     * Returns the column type for the given column index.
     *
     * @param columnIndex Column index.
     * @return Column type.
     */
    public Type GetColumnType(int columnIndex);

    /**
     * Returns the column name for the given column name.
     *
     * @param columnName Column name.
     * @return Column type.
     */
    public Type GetColumnType(String columnName);

    /**
     * Returns the list of column names for the current result set.
     *
     * @return Column names list.
     */
    public String[] GetColumnNames();

    /**
     * Returns the value of the requested column as a byte array.
     *
     * @param columnIndex Column index.
     * @return Byte array.
     */
    public byte[] GetBytes(int columnIndex);

    /**
     * Returns the value of the requested column as a byte array.
     *
     * @param columnName Column name
     * @return Byte array.
     */
    public byte[] GetBytes(String columnName);

    /**
     * Returns the value of the requested column as a String.
     *
     * @param columnIndex Column index.
     * @return String.
     */
    public String GetString(int columnIndex);

    /**
     * Returns the value of the requested column as a String.
     *
     * @param columnName Column name.
     * @return String.
     */
    public String GetString(String columnName);

    /**
     * Returns the value of the requested column as an integer.
     *
     * @param columnIndex Column index.
     * @return Integer.
     */
    public int GetInt(int columnIndex);

    /**
     * Returns the value of the requested column as an integer.
     *
     * @param columnName Column name.
     * @return Integer.
     */
    public int GetInt(String columnName);

    /**
     * Returns the value of the requested column as a long.
     *
     * @param columnIndex Column index.
     * @return Long.
     */
    public long GetLong(int columnIndex);

    /**
     * Returns the value of the requested column as a long.
     *
     * @param columnName Column name.
     * @return Long.
     */
    public long GetLong(String columnName);

    /**
     * Returns the value of the requested column as a double.
     *
     * @param columnIndex Column index.
     * @return Double.
     */
    public double GetDouble(int columnIndex);

    /**
     * Returns the value of the requested column as a double.
     *
     * @param columnName Column name.
     * @return Double.
     */
    public double GetDouble(String columnName);

    /**
     * Returns the value of the requested column as a float.
     *
     * @param columnIndex Column index.
     * @return Float.
     */
    public float GetFloat(int columnIndex);

    /**
     * Returns the value of the requested column as a float.
     *
     * @param columnName Column name.
     * @return Float.
     */
    public float GetFloat(String columnName);

    /**
     * Returns the value of the requested column as a short.
     *
     * @param columnIndex Column index.
     * @return Short.
     */
    public short GetShort(int columnIndex);

    /**
     * Returns the value of the requested column as a short.
     *
     * @param columnName Column name.
     * @return Short.
     */
    public short GetShort(String columnName);

    /**
     * Returns the current position of the cursor in the current row set.
     *
     * @return Current cursor position.
     */
    public int GetCurrentPosition();

    /**
     * Moves cursor by a relative offset, forward or backward, from the current
     * position in the row set.
     *
     * @param offset Relative amount to move cursor by.
     * @return <CODE>true</CODE> on success.
     */
    public boolean Move(int offset);

    /**
     * Moves cursor to the given position in the row set.
     *
     * @param position Position to move cursor to.
     * @return True on success.
     */
    public boolean MoveToPosition(int position);

    /**
     * Moves cursor to the first row.
     *
     * @return <CODE>true</CODE> on success.
     */
    public boolean MoveToFirst();

    /**
     * Moves cursor to the last row.
     *
     * @return <CODE>true</CODE> on success.
     */
    public boolean MoveToLast();

    /**
     * Moves cursor to the next position from its current position in the
     * current row set.
     *
     * @return <CODE>true</CODE> on success.
     */
    public boolean MoveToNext();

    /**
     * Moves cursor to the previous position from its current position in the
     * current row set.
     *
     * @return <CODE>true</CODE> on success.
     */
    public boolean MoveToPrevious();

    /**
     * Closes the current result set.
     */
    public void Close();

    /**
     * Returns true if current result set is closed.
     *
     * @return <CODE>true</CODE> if closed.
     */
    public boolean IsClosed();

    /**
     * Returns true if the value in the indicated column is null.
     *
     * @param columnIndex Column index.
     * @return <CODE>true</CODE> if value is <CODE>null</CODE> for given column.
     */
    public boolean IsNull(int columnIndex);
}
