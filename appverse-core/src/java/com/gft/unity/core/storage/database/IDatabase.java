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
package com.gft.unity.core.storage.database;

public interface IDatabase {

    /**
     * Creates database
     * <CODE>db</CODE> on default path.
     *
     * @param name Database file name.
     * @return Created database reference, or <CODE>null</CODE> if it cannot be
     * created.
     */
    public Database CreateDatabase(String name);

    /**
     * Deletes database
     * <CODE>db</CODE> on default path.
     *
     * @param db Database reference.
     *
     * @return <CODE>true</CODE> on successful deletion, <CODE>false</CODE>
     * otherwise.
     */
    public boolean DeleteDatabase(Database db);

    /**
     * Creates table
     * <CODE>tableName</CODE> inside database
     * <CODE>db</CODE>.
     *
     * @param db Database reference.
     * @param tableName Table name.
     * @param columnDefinitions String array with column definitions (SQLite
     * syntax language).
     * @return <CODE>true</CODE> on successful creation, <CODE>false</CODE>
     * otherwise.
     */
    public boolean CreateTable(Database db, String tableName,
            String[] columnDefinitions);

    /**
     * Deletes table
     * <CODE>tableName</CODE> from database
     * <CODE>db</CODE>.
     *
     * @param db Database reference.
     * @param tableName Table name.
     * @return <CODE>true</CODE> on successful deletion, <CODE>false</CODE>
     * otherwise.
     */
    public boolean DeleteTable(Database db, String tableName);

    /**
     * Executes SQL query
     * <CODE>queryText</CODE> against database
     * <CODE>db</CODE>.
     *
     * @param db Database reference.
     * @param queryText Query to be executed (SQLite syntax language).
     * @return Result set.
     */
    public IResultSet ExecuteSQLQuery(Database db, String queryText);

    /**
     * Executes SQL query
     * <CODE>queryText</CODE> against database
     * <CODE>db</CODE>. Parameters
     * <CODE>queryParams</CODE> are replaced in the SQL query
     * <CODE>queryText</CODE>.
     *
     * @param db Database reference.
     * @param queryText Query to be executed (SQLite syntax language).
     * @param queryParams Replacement parameters to be applied to query
     * statement.
     * @return Result set.
     */
    public IResultSet ExecuteSQLQuery(Database db, String queryText,
            String[] queryParams);

    /**
     * Executes SQL statement
     * <CODE>statement</CODE> into database
     * <CODE>db</CODE>.
     *
     * @param db Database reference.
     * @param statement Statement to be executed (SQLite syntax language).
     * @return <CODE>true</CODE> on successful execution, <CODE>false</CODE>
     * otherwise.
     */
    public boolean ExecuteSQLStatement(Database db, String statement);

    /**
     * Executes SQL statement
     * <CODE>statement</CODE> into database
     * <CODE>db</CODE>. Parameters
     * <CODE>statementParams</CODE> are replaced in the SQL statement
     * <CODE>statement</CODE>.
     *
     * @param db Database reference.
     * @param statement Statement to be executed (SQLite syntax language).
     * @param statementParams Replacement parameters to be applied to statement.
     * @return <CODE>true</CODE> on successful execution, <CODE>false</CODE>
     * otherwise.
     */
    public boolean ExecuteSQLStatement(Database db, String statement,
            String[] statementParams);

    /**
     * Executes SQL statements
     * <CODE>statements</CODE> inside database
     * <CODE>db</CODE>. All statements are executed in the same transaction.
     *
     * @param db Database reference.
     * @param statements Statements to be executed during transaction (SQLite
     * syntax language).
     * @param rollbackFlag Indicates if rollback should be performed when any
     * statement execution fails.
     * @return <CODE>true</CODE> on successful execution, <CODE>false</CODE>
     * otherwise.
     */
    public boolean ExecuteSQLTransaction(Database db, String[] statements,
            boolean rollbackFlag);

    /**
     * Checks if database
     * <CODE>db</CODE> exists.
     *
     * @param db Database reference.
     * @return <CODE>true</CODE> if database exists, <CODE>false</CODE>
     * otherwise.
     */
    public boolean Exists(Database db);

    /**
     * Checks if table
     * <CODE>tableName</CODE> exists on database
     * <CODE>db</CODE> .
     *
     * @param db Database reference.
     * @param tableName Table name.
     * @return <CODE>true</CODE> if table exists, <CODE>false</CODE> otherwise.
     */
    public boolean Exists(Database db, String tableName);

    /**
     * Checks if database
     * <CODE>dbName</CODE> exists.
     *
     * @param dbName Database name.
     * @return <CODE>true</CODE> if database exists, <CODE>false</CODE>
     * otherwise.
     */
    public boolean ExistsDatabase(String dbName);

    /**
     * Gets database
     * <CODE>dbName</CODE>.
     *
     * @param dbName Database name.
     * @return Database reference, or null if database does not exist.
     */
    public Database GetDatabase(String dbName);

    /**
     * Gets stored databases.
     *
     * @return List of Databases.
     */
    public Database[] GetDatabaseList();

    /**
     * Gets tables names from database
     * <CODE>db</CODE>.
     *
     * @param db Database reference.
     * @return List of tables names.
     */
    public String[] GetTableNames(Database db);
}
