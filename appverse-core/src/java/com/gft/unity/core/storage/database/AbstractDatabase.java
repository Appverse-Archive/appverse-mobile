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

public abstract class AbstractDatabase implements IDatabase {

    protected static boolean DEFAULT_DATABASE_OPTION_COMPRESS = true;
    protected static boolean DEFAULT_DATABASE_OPTION_NEW = true;

    public AbstractDatabase() {
    }

    @Override
    public abstract Database CreateDatabase(String name);

    @Override
    public abstract boolean DeleteDatabase(Database db);

    @Override
    public abstract boolean CreateTable(Database db, String tableName,
            String[] columnDefinitions);

    @Override
    public abstract boolean DeleteTable(Database db, String tableName);

    @Override
    public abstract IResultSet ExecuteSQLQuery(Database db, String queryText);

    @Override
    public abstract IResultSet ExecuteSQLQuery(Database db, String queryText,
            String[] queryParams);

    @Override
    public abstract boolean ExecuteSQLStatement(Database db, String statement);

    @Override
    public abstract boolean ExecuteSQLStatement(Database db, String statement,
            String[] statementParams);

    @Override
    public abstract boolean ExecuteSQLTransaction(Database db,
            String[] statements, boolean rollbackFlag);

    @Override
    public abstract boolean Exists(Database db);

    @Override
    public abstract boolean Exists(Database db, String tableName);

    @Override
    public abstract boolean ExistsDatabase(String dbName);

    @Override
    public abstract Database GetDatabase(String dbName);

    @Override
    public abstract Database[] GetDatabaseList();

    @Override
    public abstract String[] GetTableNames(Database db);
}
