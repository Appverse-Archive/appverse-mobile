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
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Text;
using Unity.Core.Storage.Database;
using System.Data.Common;
using System.IO;

namespace Unity.Platform.Windows
{
    public class WindowsDatabase : AbstractDatabase
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
                SQLiteConnection connection = new SQLiteConnection(
                    "Data Source=" + Path.Combine(GetBasePath(),name) +
                    ";Version=" + DEFAULT_DATABASE_VERSION +
                    ";New=" + DEFAULT_DATABASE_OPTION_NEW +
                    ";Compress=" + DEFAULT_DATABASE_OPTION_COMPRESS + ";");

                connection.Open();
                StoreConnection(name, connection);
                StoreDatabase(name, database);
            }
            catch (Exception e)
            {
#if DEBUG
                Console.WriteLine("Exception creating database: " + e.Message);
#endif
                database = null;
            }

            return database;
        }

        /// <summary>
        /// Executes SQL statement, on the given database.
        /// </summary>
        /// <param name="db">Database reference.</param>
        /// <param name="statement">Statement to be executed.</param>
        /// <param name="statementParams">Replacement parameters to be applied, by order, on the statement string.</param>
        /// <returns></returns>
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
                        SQLiteDataAdapter adapter = new SQLiteDataAdapter(SQL, (SQLiteConnection)connection);
                        adapter.Fill(new DataSet());
                        result = true;
                    }
                    catch (Exception e)
                    {
#if DEBUG
                        Console.WriteLine("Exception replacement strings on statement: " + e.Message);
#endif
                    }
                }
                else
                {
#if DEBUG
                    Console.WriteLine("SQLiteConnection not found for database name: " + db.Name);
#endif
                }
            }
            else
            {
#if DEBUG
                Console.WriteLine("Provided database is null");
#endif
            }
            return result;
        }

        /// <summary>
        /// Executes the given SQL statements, on the given database, inside a transaction.
        /// </summary>
        /// <param name="db">Database reference.</param>
        /// <param name="statements">Statements to be executed.</param>
        /// <param name="rollbackFlag">True if rollback on statement failure.</param>
        /// <returns>True on successful transaction.</returns>
        public override bool ExecuteSQLTransaction(Database db, string[] statements, bool rollbackFlag)
        {
            bool result = false;
            if (db != null)
            {
                SQLiteConnection connection = (SQLiteConnection)GetConnection(db.Name);
                if (connection != null)
                {
                    // Start a local transaction.
                    SQLiteTransaction transaction = connection.BeginTransaction();
                    // Enlist a command in the current transaction.
                    SQLiteCommand command = connection.CreateCommand();
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
#if DEBUG
                        // Handle the exception if the transaction fails to commit.
                        Console.WriteLine(ex.Message);
#endif
                        try
                        {
                            if (rollbackFlag)
                            {
                                // Attempt to roll back the transaction.
                                transaction.Rollback();
                            }
                            else
                            {
#if DEBUG
                                // No rollback specified.
                                Console.WriteLine("No rollback specified.");
#endif
                                // Commit the transaction (sucess statements, previous to exception, are committed)
                                transaction.Commit();
                            }
                        }
                        catch (Exception exRollback)
                        {
#if DEBUG
                            // Throws an InvalidOperationException if the connection 
                            // is closed or the transaction has already been rolled 
                            // back on the server.
                            Console.WriteLine(exRollback.Message);
#endif
                        }
                    }
                }
                else
                {
#if DEBUG
                    Console.WriteLine("SQLiteConnection not found for database name: " + db.Name);
#endif
                }
            }
            else
            {
#if DEBUG
                Console.WriteLine("Provided database is null");
#endif
            }
            return result;
        }

        /// <summary>
        /// Executes a SQL Query that should return data.
        /// </summary>
        /// <param name="db">Database reference.</param>
        /// <param name="queryText">Query to be executed.</param>
        /// <param name="queryParams">Values to be replaced on query string.</param>
        /// <returns>Result set.</returns>
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
                        SQLiteDataAdapter adapter = new SQLiteDataAdapter(queryString, (SQLiteConnection)connection);
                        adapter.Fill(ds);
                        resultSet = new ResultSet(ds);
                    }
                    catch (Exception e)
                    {
#if DEBUG
                        Console.WriteLine("Exception replacement strings on statement: " + e.Message);
#endif
                    }
                }
                else
                {
#if DEBUG
                    Console.WriteLine("SQLiteConnection not found for database name: " + db.Name);
#endif
                }
            }
            else
            {
#if DEBUG
                Console.WriteLine("Provided database is null");
#endif
            }
            return resultSet;
        }
       
    }
}
