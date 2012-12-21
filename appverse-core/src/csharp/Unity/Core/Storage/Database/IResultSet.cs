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
using System.Collections.Generic;
using System.Text;

namespace Unity.Core.Storage.Database
{
	public interface IResultSet
	{
		/// <summary>
		/// Returns the number of rows in the current result set.
		/// </summary>
		/// <returns>Number of rows.</returns>
		int GetRowCount ();

		/// <summary>
		/// Returns the number of total columns in the current result set.
		/// </summary>
		/// <returns>Number of columns.</returns>
		int GetColumnCount ();

		/// <summary>
		/// Returns the column name for the given column index.
		/// </summary>
		/// <param name="columnIndex">Column index.</param>
		/// <returns>Column name.</returns>
		string GetColumnName (int columnIndex);

		/// <summary>
		/// Returns the column name for the given column name.
		/// </summary>
		/// <param name="columnName">Column name.</param>
		/// <returns>Zero-based index, or -1 if that column name is not returned by this result set.</returns>
		int GetColumnIndex (string columnName);

		/// <summary>
		/// Returns the column type for the given column index.
		/// </summary>
		/// <param name="columnIndex">Column index.</param>
		/// <returns>Column type.</returns>
		Type GetColumnType (int columnIndex);

		/// <summary>
		/// Returns the column name for the given column name.
		/// </summary>
		/// <param name="columnName">Column name.</param>
		/// <returns>Column type.</returns>
		Type GetColumnType (string columnName);

		/// <summary>
		/// Returns the list of column names for the current result set.
		/// </summary>
		/// <returns>Column names list.</returns>
		string[] GetColumnNames ();

		/// <summary>
		/// Returns the value of the requested column as a byte array.
		/// </summary>
		/// <param name="columnIndex">Column index.</param>
		/// <returns>Byte array.</returns>
		byte[] GetBytes (int columnIndex);

		/// <summary>
		/// Returns the value of the requested column as a byte array.
		/// </summary>
		/// <param name="columnName">Column name.</param>
		/// <returns>Byte array.</returns>
		byte[] GetBytes (string columnName);

		/// <summary>
		/// Returns the value of the requested column as a string.
		/// </summary>
		/// <param name="columnIndex">Column index.</param>
		/// <returns>String.</returns>
		string GetString (int columnIndex);

		/// <summary>
		/// Returns the value of the requested column as a string.
		/// </summary>
		/// <param name="columnName">Column name.</param>
		/// <returns>String.</returns>
		string GetString (string columnName);

		/// <summary>
		/// Returns the value of the requested column as an integer.
		/// </summary>
		/// <param name="columnIndex">Column index.</param>
		/// <returns>Integer.</returns>
		int GetInt (int columnIndex);

		/// <summary>
		/// Returns the value of the requested column as an integer.
		/// </summary>
		/// <param name="columnName">Column name.</param>
		/// <returns>Integer.</returns>
		int GetInt (string columnName);

		/// <summary>
		/// Returns the value of the requested column as a long.
		/// </summary>
		/// <param name="columnIndex">Column index.</param>
		/// <returns>Long.</returns>
		long GetLong (int columnIndex);

		/// <summary>
		/// Returns the value of the requested column as a long.
		/// </summary>
		/// <param name="columnName">Column name.</param>
		/// <returns>Long.</returns>
		long GetLong (string columnName);

		/// <summary>
		/// Returns the value of the requested column as a double.
		/// </summary>
		/// <param name="columnIndex">Column index.</param>
		/// <returns>Double.</returns>
		double GetDouble (int columnIndex);

		/// <summary>
		/// Returns the value of the requested column as a double.
		/// </summary>
		/// <param name="columnName">Column name.</param>
		/// <returns>Double.</returns>
		double GetDouble (string columnName);

		/// <summary>
		/// Returns the value of the requested column as a float.
		/// </summary>
		/// <param name="columnIndex">Column index.</param>
		/// <returns>Float.</returns>
		float GetFloat (int columnIndex);

		/// <summary>
		/// Returns the value of the requested column as a float.
		/// </summary>
		/// <param name="columnName">Column name.</param>
		/// <returns>Float.</returns>
		float GetFloat (string columnName);

		/// <summary>
		/// Returns the value of the requested column as a short.
		/// </summary>
		/// <param name="columnIndex">Column index.</param>
		/// <returns>Short.</returns>
		short GetShort (int columnIndex);

		/// <summary>
		/// Returns the value of the requested column as a short.
		/// </summary>
		/// <param name="columnName">Column name.</param>
		/// <returns>Short.</returns>
		short GetShort (string columnName);
        
		/// <summary>
		/// Returns the current position of the cursor in the current row set.
		/// </summary>
		/// <returns>Current cursor position.</returns>
		int GetCurrentPosition ();

		/// <summary>
		/// Moves cursor by a relative offset, forward or backward, from the current position in the row set.
		/// </summary>
		/// <param name="offset">Relative amount to move cursor by.</param>
		/// <returns>True on success.</returns>
		bool Move (int offset);

		/// <summary>
		/// Moves cursor to the given position in the row set.
		/// </summary>
		/// <param name="position">Position to move cursor to.</param>
		/// <returns>True on success.</returns>
		bool MoveToPosition (int position);

		/// <summary>
		/// Moves cursor to the first row.
		/// </summary>
		/// <returns>True on success.</returns>
		bool MoveToFirst ();

		/// <summary>
		/// Moves cursor to the last row.
		/// </summary>
		/// <returns>True on success.</returns>
		bool MoveToLast ();

		/// <summary>
		/// Moves cursor to the next position from its current position in the current row set.
		/// </summary>
		/// <returns>True on success.</returns>
		bool MoveToNext ();

		/// <summary>
		/// Moves cursor to the previous position from its current position in the current row set.
		/// </summary>
		/// <returns>True on success.</returns>
		bool MoveToPrevious ();

		/// <summary>
		/// Closes the current result set.
		/// </summary>
		void Close ();

		/// <summary>
		/// Returns true if current result set is closed.
		/// </summary>
		/// <returns>True if closed.</returns>
		bool IsClosed ();

		/// <summary>
		/// Returns true if the value in the indicated column is null.
		/// </summary>
		/// <param name="columnIndex">Column index.</param>
		/// <returns>True if value is NULL for given column.</returns>
		bool IsNull (int columnIndex);
    
	}
}
