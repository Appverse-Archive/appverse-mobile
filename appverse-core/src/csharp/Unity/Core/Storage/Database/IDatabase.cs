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
#if WP8
using System.Threading.Tasks;
#endif
namespace Unity.Core.Storage.Database
{
    public interface IDatabase
    {
#if !WP8
		/// <summary>
		/// Creates database on default path.
		/// </summary>
		/// <param name="name">Database file name.</param>
		/// <returns>Database created, or null if it cannot be created.</returns>
		Database CreateDatabase (string name);

		/// <summary>
		/// Deletes database on default path.
		/// </summary>
		/// <param name="name">Database file name.</param>
		/// <returns>True on sucessful deletion.</returns>
		bool DeleteDatabase (Database db);

		/// <summary>
		/// Gets database by given name.
		/// </summary>
		/// <param name="dbName">Database name.</param>
		/// <returns>Database.</returns>
		Database GetDatabase (string dbName);

		/// <summary>
		/// Gets stored databases.
		/// </summary>
		/// <returns>List of Databases.</returns>
		Database[] GetDatabaseList ();

		/// <summary>
		/// Checks if database exists by database bean reference.
		/// </summary>
		/// <param name="db">Database reference.</param>
		/// <returns>True if database exists.</returns>
		bool Exists (Database db);

		/// <summary>
		/// Checks if database exists by given database name.
		/// </summary>
		/// <param name="dbName">Database name.</param>
		/// <returns>True if database exists.</returns>
		bool ExistsDatabase (string dbName);

		/// <summary>
		/// Creates table inside given database.
		/// </summary>
		/// <param name="db">Database to create table in.</param>
		/// <param name="tableName">Table name.</param>
		/// <param name="columnDefinitions">>String array with column definitions (sqlite syntax language).</param>
		/// <returns>True on sucessful creation.</returns>
		bool CreateTable (Database db, string tableName, string[] columnDefinitions);

		/// <summary>
		/// Deletes table from given database.
		/// </summary>
		/// <param name="db">Database reference.</param>
		/// <param name="tableName">Table name.</param>
		/// <returns>True on sucessful deletion.</returns>
		bool DeleteTable (Database db, string tableName);

		/// <summary>
		/// Gets tables names from given database.
		/// </summary>
		/// <param name="db">Database reference.</param>
		/// <returns>List of tables names.</returns>
		string[] GetTableNames (Database db);

		/// <summary>
		/// Checks if table exists on the given database.
		/// </summary>
		/// <param name="db">Database reference.</param>
		/// <param name="tableName">Table name.</param>
		/// <returns>True if table exists.</returns>
		bool Exists (Database db, string tableName);

		/// <summary>
		/// Executes an SQL statement into given database.
		/// </summary>
		/// <param name="db">Database reference.</param>
		/// <param name="statement">Statement to be executed.</param>
		/// <returns>True on successul execution.</returns>
		bool ExecuteSQLStatement (Database db, string statement);

		/// <summary>
		/// Executes an SQL statement into given database.
		/// </summary>
		/// <param name="db">Database reference.</param>
		/// <param name="statement">Statement to be executed.</param>
		/// <param name="statementParams">Replacement parameters to be applied to statement.</param>
		/// <returns>True on successul execution.</returns>
		bool ExecuteSQLStatement (Database db, string statement, string[] statementParams);

		/// <summary>
		/// Executes SQL transaction inside given database.
		/// </summary>
		/// <param name="db">Database reference.</param>
		/// <param name="statements">Statements to be executed during transaction (sqlite syntax language).</param>
		/// <param name="rollbackFlag">Indicates if rollback should be performed when any statement execution fails.</param>
		bool ExecuteSQLTransaction (Database db, string[] statements, bool rollbackFlag);

		/// <summary>
		/// Executes SQL query against given database.
		/// </summary>
		/// <param name="db">Database reference.</param>
		/// <param name="queryText">Query to be executed.</param>
		/// <returns>Result set.</returns>
		IResultSet ExecuteSQLQuery (Database db, string queryText);

		/// <summary>
		/// Executes SQL query against given database.
		/// </summary>
		/// <param name="db">Database reference.</param>
		/// <param name="queryText">Query to be executed.</param>
		/// <param name="queryParams">Replacement parameters to be applied to query statement.</param>
		/// <returns>Result set.</returns>
		IResultSet ExecuteSQLQuery (Database db, string queryText, string[] queryParams);
#else
#endif
    }//end IDatabase

}//end namespace Database