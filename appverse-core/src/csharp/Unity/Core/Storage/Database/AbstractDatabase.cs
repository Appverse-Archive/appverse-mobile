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
using System.Data;
using System.Data.Common;
using System.IO;
using System.Text;
using Unity.Core.System;
	
namespace Unity.Core.Storage.Database
{
	public abstract class AbstractDatabase : IDatabase
	{
		public static string DEFAULT_DATABASE_PATH = Environment.GetFolderPath (Environment.SpecialFolder.Personal); // path must exists, if not exists, it is created when instantiating this class.
		public static string DEFAULT_DATABASE_DIR = "sqlite";
		public static string DEFAULT_DATABASE_VERSION = "3";            // sqlite3
		public static bool DEFAULT_DATABASE_OPTION_NEW = true;
		public static bool DEFAULT_DATABASE_OPTION_COMPRESS = true;

		private Dictionary<string, Database> Databases { get; set; }

		private Dictionary<string, DbConnection> Connections;

		/// <summary>
		/// Adds the given connection to the dictionary.
		/// </summary>
		/// <param name="key">The key to apply when adding to dictionary for the given connection.</param>
		/// <param name="connection">Connection to be added.</param>
		protected void StoreConnection (string key, DbConnection connection)
		{
			if (Connections == null) {
				Connections = new Dictionary<string, DbConnection> ();
			}
			Connections [key] = connection;
		}

		/// <summary>
		/// Deletes the connection that matches the given key on the inner dictionary.
		/// </summary>
		/// <param name="key">The key to apply when deleting connection.</param>
		protected void RemoveConnection (string key)
		{
			if (Connections != null && Connections.ContainsKey (key)) {
				Connections.Remove (key);
			}
		}

		/// <summary>
		/// Returns stored connection by the given key.
		/// </summary>
		/// <param name="key">Dictionary key.</param>
		/// <returns>Stored connection, or null if not stored.</returns>
		protected DbConnection GetConnection (string key)
		{
			DbConnection connection = null;
			if (Connections != null && Connections.ContainsKey (key)) {
				connection = Connections [key];
			}

			return connection;
		}


		/// <summary>
		/// Constructor.
		/// Gets all information for already stored database files.
		/// </summary>
		public AbstractDatabase ()
		{
            
			string basePath = GetBasePath ();
			
			// Initialices dictionaries from file path.
			if (!Directory.Exists (basePath)) {
				Directory.CreateDirectory (basePath);
			}

			string[] filePaths = Directory.GetFiles (basePath);
			
			foreach (string filePath in filePaths) {
				string fileName = Path.GetFileName (filePath);
				CreateDatabase (fileName);
			}
		}

		protected string GetBasePath ()
		{
			return Path.Combine (DEFAULT_DATABASE_PATH, DEFAULT_DATABASE_DIR);
			;
		}
		
		/// <summary>
		/// Deletes database file if exists.
		/// No exception is thrown when fiel does not exists.
		/// </summary>
		/// <param name="name"></param>
		protected void DeleteDatabaseFile (string name)
		{
			string basePath = GetBasePath ();
			File.Delete (Path.Combine (basePath, name));
		}

		/// <summary>
		/// Adds the given database to the dictionary.
		/// </summary>
		/// <param name="key">The key to apply when adding to dictionary for the given database.</param>
		/// <param name="connection">Database to be added.</param>
		protected void StoreDatabase (string key, Database database)
		{
			if (Databases == null) {
				Databases = new Dictionary<string, Database> ();
			}

			Databases [key] = database;
		}

		/// <summary>
		/// Deletes the database that matches the given key on the inner dictionary.
		/// </summary>
		/// <param name="key">The key to apply when deleting database.</param>
		protected void RemoveDatabase (string key)
		{
			if (Databases != null && Databases.ContainsKey (key)) {
				Databases.Remove (key);
			}
		}


        #region Miembros de IDatabase

		public abstract Database CreateDatabase (string name);

		/// <summary>
		/// Deletes the given database.
		/// </summary>
		/// <param name="db">Database reference to be deleted.</param>
		/// <returns>True on successful deletion.</returns>
		public bool DeleteDatabase (Database db)
		{
			bool result = false;
			if (db != null) {
				DbConnection connection = GetConnection (db.Name);
				if (connection != null) {
					connection.Close ();
					RemoveConnection (db.Name);
					RemoveDatabase (db.Name);
					DeleteDatabaseFile (db.Name);
					connection = null;
					result = true;
				}
			}

			return result;
		}

		/// <summary>
		/// Returns the database that matches the given name.
		/// </summary>
		/// <param name="dbName">Database name.</param>
		/// <returns>Found database, or null if not found.</returns>
		public Database GetDatabase (string dbName)
		{
			Database database = null;

			if (Databases != null && Databases.ContainsKey (dbName)) {
				database = Databases [dbName];
			}
			return database;
		}

		/// <summary>
		/// Returns a list of databases stored.
		/// </summary>
		/// <returns>List of databases, or an empty list if no database is stored.</returns>
		public Database[] GetDatabaseList ()
		{
			List<Database> databaselist = new List<Database> ();

			if (Databases != null) {
				foreach (KeyValuePair<string, Database> pair in Databases) {
					databaselist.Add (pair.Value);
				}
			}

			return databaselist.ToArray ();
		}


		/// <summary>
		/// Checks whether the given database exists or not.
		/// </summary>
		/// <param name="db">Database to be checked.</param>
		/// <returns>True if database exists.</returns>
		public bool Exists (Database db)
		{
			return ExistsDatabase (db.Name);
		}

		/// <summary>
		/// Returns true if the database - given its name - exists.
		/// </summary>
		/// <param name="dbName">Database name.</param>
		/// <returns>True if database exists.</returns>
		public bool ExistsDatabase (string dbName)
		{ 
			return (Databases != null && Databases.ContainsKey (dbName)); 
		}

		/// <summary>
		/// Creates a table inside the given database, using the provided columns definitions.
		/// </summary>
		/// <param name="db">Database reference.</param>
		/// <param name="tableName">New table name.</param>
		/// <param name="columnDefinitions">Columns definitions (sqlite syntax).</param>
		/// <returns>True if successful table creation.</returns>
		public bool CreateTable (Database db, string tableName, string[] columnDefinitions)
		{
			bool result = false;
			if (db != null) {
				try {
					DbConnection connection = GetConnection (db.Name);
					if (connection != null) {
						StringBuilder buffer = new StringBuilder ();
						string columnDefs = "";
						foreach (string columnDef in columnDefinitions) {
							buffer.Append ((buffer.Length > 0 ? "," + columnDef : columnDef));
						}
						if (buffer.Length > 0) {
							columnDefs = "(" + buffer.ToString () + ")";
						}

						string SQL = "CREATE TABLE " + tableName + columnDefs;
						SystemLogger.Log (SystemLogger.Module .CORE, "SQL to execute: " + SQL);
						
						DbCommand command = connection.CreateCommand ();
						command.CommandText = SQL;
						command.CommandType = CommandType.Text;
						command.ExecuteNonQuery ();
						result = true;
					} else {
						SystemLogger.Log (SystemLogger.Module .CORE, "SQLiteConnection not found for database name: " + db.Name);
					}
				} catch (Exception e) {
					SystemLogger.Log (SystemLogger.Module .CORE, "Exception creating table[" + tableName + "] on database[" + db.Name + "]", e);
				}
			} else {
				SystemLogger.Log (SystemLogger.Module .CORE, "Provided database is null");
			}

			return result;
		}

		/// <summary>
		/// Deletes table - given the table name - on the given database.
		/// </summary>
		/// <param name="db">Database reference.</param>
		/// <param name="tableName">Table name.</param>
		/// <returns>True on successful deletion.</returns>
		public bool DeleteTable (Database db, string tableName)
		{
			bool result = false;
			if (db != null) {
				try {
					DbConnection connection = GetConnection (db.Name);
					if (connection != null) {
						string SQL = "DROP TABLE " + tableName;
						SystemLogger.Log (SystemLogger.Module .CORE, "SQL to execute: " + SQL);
						
						DbCommand command = connection.CreateCommand ();
						command.CommandText = SQL;
						command.CommandType = CommandType.Text;
						command.ExecuteNonQuery ();
						result = true;
					} else {
						SystemLogger.Log (SystemLogger.Module .CORE, "SQLiteConnection not found for database name: " + db.Name);
					}
				} catch (Exception e) {
					SystemLogger.Log (SystemLogger.Module .CORE, "Exception deleting table[" + tableName + "] on database[" + db.Name + "]", e);
				}
			} else {
				SystemLogger.Log (SystemLogger.Module .CORE, "Provided database is null");
			}

			return result;
		}

		/// <summary>
		/// Returns a list of table names for the given database. 
		/// </summary>
		/// <param name="db">
		/// A <see cref="Database"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.String[]"/>
		/// </returns>
		public string[] GetTableNames (Database db)
		{
			List<string> list = new List<string> ();

			if (db != null) {
				DbConnection connection = GetConnection (db.Name);
				if (connection != null) {
					DataTable dt = connection.GetSchema ("TABLES");
					foreach (DataRow row in dt.Rows) {
						foreach (DataColumn col in dt.Columns) {
							//Console.WriteLine("{0} = {1}", col.ColumnName, row[col]);
							if (col.ColumnName == "TABLE_NAME") {
								list.Add (row [col].ToString ());
							}
						}
					}
                  
				} else {
					SystemLogger.Log (SystemLogger.Module.PLATFORM, 
					                 "SQLiteConnection not found for database name: " + db.Name);
				}
			} else {
				SystemLogger.Log (SystemLogger.Module.PLATFORM, "Provided database is null");
			}
			return list.ToArray ();
		}

		/// <summary>
		/// Checks whether a table (given its name) exists on the given database.
		/// </summary>
		/// <param name="db">
		/// A <see cref="Database"/>
		/// </param>
		/// <param name="tableName">
		/// A <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.Boolean"/>
		/// </returns>
		public bool Exists (Database db, string tableName)
		{
			bool exists = false;

			if (db != null) {
				DbConnection connection = GetConnection (db.Name);
				if (connection != null) {
					DataTable dt = connection.GetSchema ("TABLES", new string[] {
						null,
						null,
						tableName,
						null
					});
					if (dt.Rows.Count > 0) {
						exists = true;
					}
				} else {
					SystemLogger.Log (SystemLogger.Module.PLATFORM, 
					                 "SQLiteConnection not found for database name: " + db.Name);
				}
			} else {
				SystemLogger.Log (SystemLogger.Module.PLATFORM, "Provided database is null");
			}
			return exists;
		}

		/// <summary>
		/// Executes an SQL statements without parameter replacements.
		/// </summary>
		/// <param name="db">Database reference.</param>
		/// <param name="statement">Statement to be executed.</param>
		/// <returns>True on successful execution.</returns>
		public bool ExecuteSQLStatement (Database db, string statement)
		{
			return ExecuteSQLStatement (db, statement, null);
		}

		public abstract bool ExecuteSQLStatement (Database db, string statement, string[] statementParams);

		public abstract bool ExecuteSQLTransaction (Database db, string[] statements, bool rollbackFlag);

		/// <summary>
		/// Executes a SQL Query that should return data (without query replacement values).
		/// </summary>
		/// <param name="db">Database reference.</param>
		/// <param name="queryText">Query to be executed.</param>
		/// <returns>Result set.</returns>
		public IResultSet ExecuteSQLQuery (Database db, string queryText)
		{
			return ExecuteSQLQuery (db, queryText, null);
		}

		public abstract IResultSet ExecuteSQLQuery (Database db, string queryText, string[] queryParams);

        #endregion
	}
}
