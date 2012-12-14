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
using System;
using System.Data;
using System.Collections.Generic;
using Unity.Core.System;

namespace Unity.Core.Storage.Database
{
	public class ResultSet : IResultSet
	{

		private DataRowCollection Rows;
		private DataColumnCollection Columns;
		private int CurrentPosition = -1;
		private DataRow CurrentRow = null;
		private bool Closed = false;
		
		/** Public Properties **/
		public int RowCount { get; set; }

		public int ColumnCount { get; set; }

		public string[] ColumnNames { get; set; }

		public ResultSetRow[] RowsList { get; set; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="ds">
		/// A <see cref="DataSet"/>
		/// </param>
		public ResultSet (DataSet ds)
		{
			if (ds != null && ds.Tables != null && ds.Tables.Count > 0) {
				DataTable dt = ds.Tables [0]; // only one table is returned with query results.
				Rows = dt.Rows;
				Columns = dt.Columns;
			}
			
			// Expose values as public properties to access them on json objects.
			PopulatingPublicProperties ();
		}
		
		public void PopulatingPublicProperties ()
		{
			this.RowCount = GetRowCount ();
			this.ColumnCount = GetColumnCount ();
			this.ColumnNames = GetColumnNames ();
			
			List<ResultSetRow> list = new List<ResultSetRow> ();
			
			if (!IsClosed () && this.Rows != null) {
				// Build ResultSetRow List.
				while (MoveToNext()) {
					ResultSetRow resultSetRow = new ResultSetRow (CurrentRow, Columns);
					list.Add (resultSetRow);
				}
				MoveToFirst (); // reset current position
			}
			
			this.RowsList = list.ToArray ();
			
		}

        #region Miembros de IResultSet

		/// <summary>
		/// Returns row count.
		/// </summary>
		/// <returns>Row count.</returns>
		public int GetRowCount ()
		{
			int count = 0;
			if (Rows != null) {
				return Rows.Count;
			}

			return count;
		}

		/// <summary>
		/// Returns column count.
		/// </summary>
		/// <returns>Column count.</returns>
		public int GetColumnCount ()
		{
			int count = 0;
			if (Columns != null) {
				return Columns.Count;
			}

			return count;
		}
		
		/// <summary>
		/// Returns column name for the given column index.
		/// </summary>
		/// <param name="columnIndex">Column index (zero based index).</param>
		/// <returns>Column name.</returns>
		public string GetColumnName (int columnIndex)
		{
			string columnName = null;
			if (Columns != null) {
				columnName = Columns [columnIndex].ColumnName;
			}

			return columnName;
		}

		/// <summary>
		/// Returns column index for the given column name.
		/// </summary>
		/// <param name="columnName">Column Name</param>
		/// <returns>Column index (zero based index, or -1 if not found con columns list)</returns>
		public int GetColumnIndex (string columnName)
		{
			int columnIndex = -1;
			if (Columns != null) {
				columnIndex = Columns.IndexOf (Columns [columnName]);
			}

			return columnIndex;
		}

		/// <summary>
		/// Return column type for the given column index.
		/// </summary>
		/// <param name="columnIndex">Column index.</param>
		/// <returns>Column data type.</returns>
		public Type GetColumnType (int columnIndex)
		{
			Type dataType = null;

			if (Columns != null) {
				dataType = Columns [columnIndex].DataType;
			}
			return dataType;
		}

		/// <summary>
		/// Return column type for the given column name.
		/// </summary>
		/// <param name="columnName">Column name.</param>
		/// <returns>Column data type.</returns>
		public Type GetColumnType (string columnName)
		{
			Type dataType = null;

			if (Columns != null) {
				dataType = Columns [columnName].DataType;
			}
			return dataType;
		}

		/// <summary>
		/// Returns all column names.
		/// </summary>
		/// <returns>Column names list.</returns>
		public string[] GetColumnNames ()
		{
			List<string> names = new List<string> ();

			if (Columns != null) {
				foreach (DataColumn dataColumn in Columns) {
					names.Add (dataColumn.ColumnName);
				}
			}

			return names.ToArray ();
		}

		/// <summary>
		/// Returns the value of the requested column as an object.
		/// </summary>
		/// <param name="columnIndex">Column index.</param>
		/// <returns>Column Value, or null if not found.</returns>
		public Object GetValue (int columnIndex)
		{
			Object result = null;

			if (CurrentRow != null) {
				if (CurrentRow [Columns [columnIndex]] != null) {
					try {
						result = (byte[])(CurrentRow [Columns [columnIndex]]);
					} catch (Exception e) {
						SystemLogger.Log (SystemLogger.Module.CORE, "Error getting byte[] value for column index [" + columnIndex + "].", e);
					}
				}
			}

			return result;
		}

		/// <summary>
		/// Returns the value of the requested column as an object.
		/// </summary>
		/// <param name="columnName">Column name.</param>
		/// <returns>Column value.</returns>
		public Object GetValue (string columnName)
		{
			throw new NotImplementedException ();
		}


		/// <summary>
		/// Returns the value of the requested column as a byte array.
		/// </summary>
		/// <param name="columnIndex">Column index.</param>
		/// <returns>Byte array, or null if not found.</returns>
		public byte[] GetBytes (int columnIndex)
		{
			byte[] result = null;

			if (CurrentRow != null) {
				if (CurrentRow [Columns [columnIndex]] != null) {
					try {
						result = (byte[])(CurrentRow [Columns [columnIndex]]);
					} catch (Exception e) {
						SystemLogger.Log (SystemLogger.Module.CORE, "Error getting byte[] value for column index [" + columnIndex + "].", e);
					}
				}
			}

			return result;
		}

		/// <summary>
		/// Returns the value of the requested column as a byte array.
		/// </summary>
		/// <param name="columnName">Column name.</param>
		/// <returns>Byte array, or null if not found.</returns>
		public byte[] GetBytes (string columnName)
		{
			byte[] result = null;

			if (CurrentRow != null) {
				if (CurrentRow [Columns [columnName]] != null) {
					try {
						result = (byte[])(CurrentRow [Columns [columnName]]);
					} catch (Exception e) {
						SystemLogger.Log (SystemLogger.Module.CORE, "Error getting byte[] value for column name [" + columnName + "].", e);
					}
				}
			}

			return result;
		}

		/// <summary>
		/// Returns the value of the requested column as a string.
		/// </summary>
		/// <param name="columnIndex">Column index.</param>
		/// <returns>String, or null if not found.</returns>
		public string GetString (int columnIndex)
		{
			string result = null;

			if (CurrentRow != null) {
				if (CurrentRow [Columns [columnIndex]] != null) {
					result = CurrentRow [Columns [columnIndex]].ToString ();
				}
			}

			return result;
		}

		/// <summary>
		/// Returns the value of the requested column as a string.
		/// </summary>
		/// <param name="columnName">Column name.</param>
		/// <returns>String, or null if not found.</returns>
		public string GetString (string columnName)
		{
			string result = null;

			if (CurrentRow != null) {
				if (CurrentRow [Columns [columnName]] != null) {
					result = CurrentRow [Columns [columnName]].ToString ();
				}
			}

			return result;
		}

		/// <summary>
		/// Returns the value of the requested column as an integer.
		/// </summary>
		/// <param name="columnIndex">Column index.</param>
		/// <returns>Integer (zero is returned on error casting to int type).</returns>
		public int GetInt (int columnIndex)
		{
			int result = 0;

			if (CurrentRow != null) {
				try {
					result = Int32.Parse (CurrentRow [Columns [columnIndex]].ToString ());
				} catch (Exception e) {
					SystemLogger.Log (SystemLogger.Module.CORE, "Error getting int value for column index [" + columnIndex + "].", e);
				}
			}
			return result;
		}

		/// <summary>
		/// Returns the value of the requested column as an integer.
		/// </summary>
		/// <param name="columnName">Column name.</param>
		/// <returns>Integer (zero is returned on error casting to int type).</returns>
		public int GetInt (string columnName)
		{
			int result = 0;

			if (CurrentRow != null) {
				try {
					result = Int32.Parse (CurrentRow [Columns [columnName]].ToString ());
				} catch (Exception e) {
					SystemLogger.Log (SystemLogger.Module.CORE, "Error getting int value for column name [" + columnName + "].", e);
				}
			}
			return result;
		}

		/// <summary>
		/// Returns the value of the requested column as a long.
		/// </summary>
		/// <param name="columnIndex">Column index.</param>
		/// <returns>Long (zero is returned on error casting to long type).</returns>
		public long GetLong (int columnIndex)
		{
			long result = 0;

			if (CurrentRow != null) {
				try {
					result = Int64.Parse (CurrentRow [Columns [columnIndex]].ToString ());
				} catch (Exception e) {
					SystemLogger.Log (SystemLogger.Module.CORE, "Error getting long value for column index [" + columnIndex + "].", e);
				}
			}
			return result;
		}

		/// <summary>
		/// Returns the value of the requested column as a long.
		/// </summary>
		/// <param name="columnName">Column name.</param>
		/// <returns>Long (zero is returned on error casting to long type).</returns>
		public long GetLong (string columnName)
		{
			long result = 0;

			if (CurrentRow != null) {
				try {
					result = Int64.Parse (CurrentRow [Columns [columnName]].ToString ());
				} catch (Exception e) {
					SystemLogger.Log (SystemLogger.Module.CORE, "Error getting int value for column name [" + columnName + "].", e);
				}
			}
			return result;
		}

		/// <summary>
		/// Returns the value of the requested column as a double.
		/// </summary>
		/// <param name="columnIndex">Column index.</param>
		/// <returns>Double (zero is returned on error casting to double type).</returns>
		public double GetDouble (int columnIndex)
		{
			double result = 0;

			if (CurrentRow != null) {
				try {
					result = Double.Parse (CurrentRow [Columns [columnIndex]].ToString ());
				} catch (Exception e) {
					SystemLogger.Log (SystemLogger.Module.CORE, "Error getting long value for column index [" + columnIndex + "]: ", e);
				}
			}
			return result;
		}

		/// <summary>
		/// Returns the value of the requested column as a double.
		/// </summary>
		/// <param name="columnName">Column name.</param>
		/// <returns>Double (zero is returned on error casting to double type).</returns>
		public double GetDouble (string columnName)
		{
			double result = 0;

			if (CurrentRow != null) {
				try {
					result = Double.Parse (CurrentRow [Columns [columnName]].ToString ());
				} catch (Exception e) {
					SystemLogger.Log (SystemLogger.Module.CORE, "Error getting int value for column name [" + columnName + "].", e);
				}
			}
			return result;
		}

		/// <summary>
		/// Returns the value of the requested column as a float.
		/// </summary>
		/// <param name="columnIndex">Column index.</param>
		/// <returns>Float (zero is returned on error casting to float type).</returns>
		public float GetFloat (int columnIndex)
		{
			float result = 0;

			if (CurrentRow != null) {
				try {
					result = float.Parse (CurrentRow [Columns [columnIndex]].ToString ());
				} catch (Exception e) {
					SystemLogger.Log (SystemLogger.Module.CORE, "Error getting long value for column index [" + columnIndex + "].", e);
				}
			}
			return result;
		}

		/// <summary>
		/// Returns the value of the requested column as a float.
		/// </summary>
		/// <param name="columnName">Column name.</param>
		/// <returns>Float (zero is returned on error casting to float type).</returns>
		public float GetFloat (string columnName)
		{
			float result = 0;

			if (CurrentRow != null) {
				try {
					result = float.Parse (CurrentRow [Columns [columnName]].ToString ());
				} catch (Exception e) {
					SystemLogger.Log (SystemLogger.Module.CORE, "Error getting int value for column name [" + columnName + "].", e);
				}
			}
			return result;
		}

		/// <summary>
		/// Returns the value of the requested column as a short.
		/// </summary>
		/// <param name="columnIndex">Column index.</param>
		/// <returns>Short (zero is returned on error casting to short type).</returns>
		public short GetShort (int columnIndex)
		{
			short result = 0;

			if (CurrentRow != null) {
				try {
					result = short.Parse (CurrentRow [Columns [columnIndex]].ToString ());
				} catch (Exception e) {
					SystemLogger.Log (SystemLogger.Module.CORE, "Error getting long value for column index [" + columnIndex + "].", e);
				}
			}
			return result;
		}

		/// <summary>
		/// Returns the value of the requested column as a short.
		/// </summary>
		/// <param name="columnName">Column name.</param>
		/// <returns>Short (zero is returned on error casting to short type).</returns>
		public short GetShort (string columnName)
		{
			short result = 0;

			if (CurrentRow != null) {
				try {
					result = short.Parse (CurrentRow [Columns [columnName]].ToString ());
				} catch (Exception e) {
					SystemLogger.Log (SystemLogger.Module.CORE, "Error getting int value for column name [" + columnName + "].", e);
				}
			}
			return result;
		}

		/// <summary>
		/// Returns the current position of the cursor in the current row set.
		/// </summary>
		/// <returns>Current cursor position.</returns>
		public int GetCurrentPosition ()
		{
			return CurrentPosition;
		}

		/// <summary>
		/// Moves cursor by a relative offset, forward or backward, from the current position in the row set.
		/// </summary>
		/// <param name="offset">Relative amount to move cursor by.</param>
		/// <returns>True on success.</returns>
		public bool Move (int offset)
		{
			bool result = false;

			try {
				if (Rows != null) {
					int nextPosition = CurrentPosition + offset;
					if ((nextPosition >= 0) && (nextPosition < Rows.Count)) {
						CurrentRow = Rows [nextPosition];
						CurrentPosition = nextPosition;
						result = true;
					}
				}
			} catch (Exception e) {
				SystemLogger.Log (SystemLogger.Module.CORE, "Error moving cursor to relative position offset [" + offset + "].", e);
			}

			return result;
		}

		/// <summary>
		/// Moves cursor to the given position in the row set.
		/// </summary>
		/// <param name="position">Position to move cursor to.</param>
		/// <returns>True on success.</returns>
		public bool MoveToPosition (int position)
		{
			bool result = false;

			try {
				if (Rows != null) {
					if ((position >= 0) && (position < Rows.Count)) {
						CurrentRow = Rows [position];
						CurrentPosition = position;
						result = true;
					}
				}
			} catch (Exception e) {
				SystemLogger.Log (SystemLogger.Module.CORE, "Error moving cursor to absolute position [" + position + "].", e);
			}

			return result;
		}

		/// <summary>
		/// Moves cursor to the first row.
		/// </summary>
		/// <returns>True on success.</returns>
		public bool MoveToFirst ()
		{
			bool result = false;

			try {
				if (Rows != null && Rows.Count > 0) {
					CurrentPosition = 0;
					CurrentRow = Rows [CurrentPosition];
					result = true;
				}
			} catch (Exception e) {
				SystemLogger.Log (SystemLogger.Module.CORE, "Error moving cursor to first position.", e);
			}

			return result;
		}

		/// <summary>
		/// Moves cursor to the last row.
		/// </summary>
		/// <returns>True on success.</returns>
		public bool MoveToLast ()
		{
			bool result = false;

			try {
				if (Rows != null && Rows.Count > 0) {
					CurrentRow = Rows [Rows.Count - 1];
					CurrentPosition = Rows.Count - 1;
					result = true;
				}
			} catch (Exception e) {
				SystemLogger.Log (SystemLogger.Module.CORE, "Error moving cursor to last position.", e);
			}

			return result;
		}

		/// <summary>
		/// Moves cursor to the next position from its current position in the current row set.
		/// </summary>
		/// <returns>True on success.</returns>
		public bool MoveToNext ()
		{
			bool result = false;

			try {
				if (Rows != null) {
					int nextPosition = CurrentPosition + 1;
					if ((nextPosition >= 0) && (nextPosition < Rows.Count)) {
						CurrentRow = Rows [nextPosition];
						CurrentPosition = nextPosition;
						result = true;
					}
				}
			} catch (Exception e) {
				SystemLogger.Log (SystemLogger.Module.CORE, "Error moving cursor to next position.", e);
			}

			return result;
		}

		/// <summary>
		/// Moves cursor to the previous position from its current position in the current row set.
		/// </summary>
		/// <returns>True on success.</returns>
		public bool MoveToPrevious ()
		{
			bool result = false;

			try {
				if (Rows != null) {
					int previousPosition = CurrentPosition - 1;
					if ((previousPosition >= 0) && (previousPosition < Rows.Count)) {
						CurrentRow = Rows [previousPosition];
						CurrentPosition = previousPosition;
						result = true;
					}
				}
			} catch (Exception e) {
				SystemLogger.Log (SystemLogger.Module.CORE, "Error moving cursor to next position.", e);
			}

			return result;
		}

		/// <summary>
		/// Closes the current result set.
		/// </summary>
		public void Close ()
		{
			Closed = true;
			CurrentPosition = -1;
			CurrentRow = null;
		}

		/// <summary>
		/// Returns true if current result set is closed.
		/// </summary>
		/// <returns>True if closed.</returns>
		public bool IsClosed ()
		{
			return Closed;
		}

		/// <summary>
		/// Returns true if the value in the indicated column is null.
		/// </summary>
		/// <param name="columnIndex">Column index.</param>
		/// <returns>True if value is NULL for given column.</returns>
		public bool IsNull (int columnIndex)
		{
			bool result = false;

			if (CurrentRow != null) {
				if (CurrentRow [Columns [columnIndex]] == null) {
					result = true;
				}
			}

			return result;
		}

        #endregion
		
		
	}
}
