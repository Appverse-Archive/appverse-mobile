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
using Mono.Data.Sqlite;
using System;
using System.Data;
using System.Data.Common;
using System.IO;
using Unity.Core.Storage.Database;
using Unity.Core.System;


namespace Unity.Platform.IPhone
{
    public class IPhoneDatabase : AbstractDatabase
    {
        /// <summary>
        /// Creates and opens a database.
        /// </summary>
        /// <param name="name">Database name.</param>
        /// <returns>Created database, or null if cannot be created.</returns>
        public override Database CreateDatabase(string name)
        {
            
            Database database = null;
            try
            {
                database = new Database(name, DEFAULT_DATABASE_OPTION_NEW, DEFAULT_DATABASE_OPTION_COMPRESS);
                string dbPath = Path.Combine(GetBasePath(), name);
				
				// Checks if a file already exists, if not creates the file.
				bool exists = File.Exists (dbPath);  
		        if (!exists)  
				{
					SqliteConnection.CreateFile (dbPath);  
				}
				
                SqliteConnection connection = new SqliteConnection(
                    "Data Source=" + dbPath +
                    ",version=" + DEFAULT_DATABASE_VERSION);

                connection.Open();
                StoreConnection(name, connection);
                StoreDatabase(name, database);
              
            }
            catch (Exception e)
            {
				SystemLogger.Log(SystemLogger.Module.PLATFORM, "Exception creating database.", e);
                database = null;
            }

            return database;
            
        }

		/// <summary>
		///  Executing SQL Statement 
		///  (using Mono/monotouch apis for SqliteConnection and SqliteDataAdapter classes).
		/// </summary>
		/// <param name="db">
		/// A <see cref="Database"/>
		/// </param>
		/// <param name="statement">
		/// A <see cref="System.String"/>
		/// </param>
		/// <param name="statementParams">
		/// A <see cref="System.String[]"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.Boolean"/>
		/// </returns>
		public override bool ExecuteSQLStatement(Database db, string statement, string[] statementParams)
        {
            bool result = false;
            if (db != null)
            {
                DbConnection connection = GetConnection(db.Name);
                if (connection != null)
                {
                    string SQL = statement;
                    try
                    {
                        if (statementParams != null)
                        {
                            SQL = String.Format(SQL, statementParams);
                        }
						
                        SqliteDataAdapter adapter = new SqliteDataAdapter(SQL, (SqliteConnection)connection);
                        adapter.Fill(new DataSet());
                        result = true;
                    }
                    catch (Exception e)
                    {
						SystemLogger.Log(SystemLogger.Module.PLATFORM, "Exception replacement strings on statement.", e);
                    }
                }
                else
                {
					SystemLogger.Log(SystemLogger.Module.PLATFORM, "SQLiteConnection not found for database name: " + db.Name);
                }
            }
            else
            {
				SystemLogger.Log(SystemLogger.Module.PLATFORM, "Provided database is null");
            }
            return result;
        }

		/// <summary>
		/// Executing SQL Transaction 
		/// (using Mono/monotouch apis for SqliteConnection, SqliteTransaction  and SqliteCommand classes).
		/// </summary>
		/// <param name="db">
		/// A <see cref="Database"/>
		/// </param>
		/// <param name="statements">
		/// A <see cref="System.String[]"/>
		/// </param>
		/// <param name="rollbackFlag">
		/// A <see cref="System.Boolean"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.Boolean"/>
		/// </returns>
        public override bool ExecuteSQLTransaction(Database db, string[] statements, bool rollbackFlag)
        {
            bool result = false;
            if (db != null)
            {
                SqliteConnection connection = (SqliteConnection)GetConnection(db.Name);
                if (connection != null)
                {
                    // Start a local transaction.
					SqliteTransaction transaction = connection.BeginTransaction();
                    // Enlist a command in the current transaction.
					SqliteCommand command = connection.CreateCommand();
                    command.Transaction = transaction;

                    try
                    {
                        // Execute separate commands.
                        foreach (string statement in statements)
                        {
                            command.CommandText = statement;
                            command.ExecuteNonQuery();
                        }

                        // Commit the transaction.
                        transaction.Commit();
                        result = true;
                    }
                    catch (Exception ex)
                    {	
						// Handle the exception if the transaction fails to commit.
                        SystemLogger.Log(SystemLogger.Module.PLATFORM, "SQL Transaction fails to commit.", ex);
                        try
                        {
                            if (rollbackFlag)
                            {
                                // Attempt to roll back the transaction.
                                transaction.Rollback();
                            }
                            else
                            {
								// No rollback specified.
                                SystemLogger.Log(SystemLogger.Module.PLATFORM, "No rollback specified.");
								
                                // Commit the transaction (sucess statements, previous to exception, are committed)
                                transaction.Commit();
                            }
                        }
                        catch (Exception exRollback)
                        {
                            // Throws an InvalidOperationException if the connection 
                            // is closed or the transaction has already been rolled 
                            // back on the server.
                            SystemLogger.Log(SystemLogger.Module.PLATFORM, "Error when doing Rollback on SQL Transaction.", exRollback);
                        }
                    }
                }
                else
                {
					SystemLogger.Log(SystemLogger.Module.PLATFORM, "SQLiteConnection not found for database name: " + db.Name);
                }
            }
            else
            {
				SystemLogger.Log(SystemLogger.Module.PLATFORM, "Provided database is null");
            }
            return result;
        }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="db">
		/// A <see cref="Database"/>
		/// </param>
		/// <param name="queryText">
		/// A <see cref="System.String"/>
		/// </param>
		/// <param name="queryParams">
		/// A <see cref="System.String[]"/>
		/// </param>
		/// <returns>
		/// A <see cref="IResultSet"/>
		/// </returns>
        public override IResultSet ExecuteSQLQuery(Database db, string queryText, string[] queryParams)
        {
            IResultSet resultSet = null;

            if (db != null)
            {
                DbConnection connection = GetConnection(db.Name);
                if (connection != null)
                {
                    string queryString = queryText;
                    try
                    {
                        if (queryParams != null)
                        {
                            queryString = String.Format(queryString, queryParams);
                        }
                        DataSet ds = new DataSet();
                        SqliteDataAdapter adapter = new SqliteDataAdapter(queryString, (SqliteConnection)connection);
                        adapter.Fill(ds);
                        resultSet = new ResultSet(ds);
                    }
                    catch (Exception e)
                    {
						SystemLogger.Log(SystemLogger.Module.PLATFORM, "Exception replacement strings on statement.", e);
                    }
                }
                else
                {
					SystemLogger.Log(SystemLogger.Module.PLATFORM, "SQLiteConnection not found for database name: " + db.Name);
                }
            }
            else
            {
				SystemLogger.Log(SystemLogger.Module.PLATFORM, "Provided database is null");
            }
            return resultSet;
        }
    }
}
