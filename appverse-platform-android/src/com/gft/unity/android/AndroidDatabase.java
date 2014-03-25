/*
 Copyright (c) 2012 GFT Appverse, S.L., Sociedad Unipersonal.

 This Source  Code Form  is subject to the  terms of  the Appverse Public License 
 Version 2.0  ("APL v2.0").  If a copy of  the APL  was not  distributed with this 
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
package com.gft.unity.android;

import java.text.MessageFormat;
import java.util.ArrayList;
import java.util.HashSet;
import java.util.Set;

import android.content.Context;
import android.database.Cursor;
import android.database.sqlite.SQLiteDatabase;
import android.database.sqlite.SQLiteException;

import com.gft.unity.core.storage.database.AbstractDatabase;
import com.gft.unity.core.storage.database.Database;
import com.gft.unity.core.storage.database.IResultSet;
import com.gft.unity.core.system.log.Logger;
import com.gft.unity.core.system.log.Logger.LogCategory;

public class AndroidDatabase extends AbstractDatabase {

	private static final String LOGGER_MODULE = "IDatabase";
	private static final Logger LOGGER = Logger.getInstance(LogCategory.PLATFORM,
			LOGGER_MODULE);

	private static final Set<String> SYSTEM_TABLES = new HashSet<String>();

	static {
		SYSTEM_TABLES.add("android_metadata");
	}

	@Override
	public Database CreateDatabase(String dbName) {
		Database db = null;

		LOGGER.logOperationBegin("CreateDatabase", new String[] { "dbName" },
				new Object[] { dbName });

		SQLiteDatabase sqlDB = null;
		try {
			Context context = AndroidServiceLocator.getContext();
			sqlDB = context.openOrCreateDatabase(dbName, Context.MODE_PRIVATE,
					null);
			if (!sqlDB.isOpen()) {
				return null;
			}
			db = new Database(dbName, DEFAULT_DATABASE_OPTION_COMPRESS,
					DEFAULT_DATABASE_OPTION_NEW);
		} catch (Exception ex) {
			LOGGER.logError("CreateDatabase", "Error", ex);
		} finally {
			closeDatabase(sqlDB);
			LOGGER.logOperationEnd("CreateDatabase", db);
		}

		return db;
	}

	@Override
	public boolean DeleteDatabase(Database db) {
		boolean result = false;

		LOGGER.logOperationBegin("DeleteDatabase", new String[] { "db" },
				new Object[] { db });

		try {
			Context context = AndroidServiceLocator.getContext();
			result = context.deleteDatabase(db.getName());
		} catch (Exception ex) {
			LOGGER.logError("DeleteDatabase", "Error", ex);
		} finally {
			LOGGER.logOperationEnd("DeleteDatabase", result);
		}

		return result;
	}

	@Override
	public boolean CreateTable(Database db, String tableName,
			String[] columnDefinitions) {
		boolean result = false;

		LOGGER.logOperationBegin("CreateTable", new String[] { "db",
				"tableName", "columnDefinitions" }, new Object[] { db,
				tableName, columnDefinitions });

		SQLiteDatabase sqlDB = null;
		try {
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < columnDefinitions.length; i++) {
				if (i != 0) {
					sb.append(",");
				}
				sb.append(columnDefinitions[i]);
			}
			String columns = sb.toString();
			columns = columns.replace("\"", "");

			sqlDB = openDatabase(db);
			if(sqlDB!=null) {
				String sql = "CREATE TABLE IF NOT EXISTS " + tableName + " ("
						+ columns + ")";
				sqlDB.execSQL(sql);
				result = true;
			} 			
		} catch (Exception ex) {
			LOGGER.logError("CreateTable", "Error", ex);
		} finally {
			closeDatabase(sqlDB);
			LOGGER.logOperationEnd("CreateTable", result);
		}

		return result;
	}

	@Override
	public boolean DeleteTable(Database db, String tableName) {
		boolean result = false;

		LOGGER.logOperationBegin("DeleteTable", new String[] { "db",
				"tableName" }, new Object[] { db, tableName });

		SQLiteDatabase sqlDB = null;
		try {
			sqlDB = openDatabase(db);
			if(sqlDB!=null) {
				sqlDB.execSQL("DROP TABLE " + tableName);
				result = true;
			}
		} catch (Exception ex) {
			LOGGER.logError("DeleteTable", "Error", ex);
		} finally {
			closeDatabase(sqlDB);
			LOGGER.logOperationEnd("DeleteTable", result);
		}

		return result;
	}

	@Override
	public IResultSet ExecuteSQLQuery(Database db, String queryText,
			String[] queryParams) {
		AndroidResultSet result = null;

		LOGGER.logOperationBegin("ExecuteSQLQuery", new String[] { "db",
				"queryText", "queryParams" }, new Object[] { db, queryText,
				queryParams });

		String sql = getFormattedSQL(queryText, queryParams);

		SQLiteDatabase sqlDB = null;
		Cursor cursor = null;
		try {
			sqlDB = openDatabase(db);
			if(sqlDB!=null) {
				cursor = sqlDB.rawQuery(sql, null);
				result = new AndroidResultSet(cursor);
			}
		} catch (Exception ex) {
			LOGGER.logError("ExecuteSQLQuery", "Error", ex);
			return null;
		} finally {
			if (cursor != null) {
				cursor.close();
			}
			closeDatabase(sqlDB);
			LOGGER.logOperationEnd("ExecuteSQLQuery", result);
		}

		return result;
	}

	@Override
	public IResultSet ExecuteSQLQuery(Database db, String queryText) {
		return ExecuteSQLQuery(db, queryText, null);
	}

	@Override
	public boolean ExecuteSQLStatement(Database db, String statement,
			String[] statementParams) {
		boolean result = false;

		LOGGER.logOperationBegin("ExecuteSQLStatement", new String[] { "db",
				"statement", "statementParams" }, new Object[] { db, statement,
				statementParams });

		SQLiteDatabase sqlDB = null;
		try {
			sqlDB = openDatabase(db);
			if(sqlDB != null) {
				if ((statementParams != null) && (statementParams.length > 0)) {
					statement = getFormattedSQL(statement, statementParams);
				}
				sqlDB.execSQL(statement);
				result = true;
			}
		} catch (Exception ex) {
			LOGGER.logError("ExecuteSQLStatement", "Error", ex);
		} finally {
			closeDatabase(sqlDB);
			LOGGER.logOperationEnd("ExecuteSQLStatement", result);
		}

		return result;
	}

	@Override
	public boolean ExecuteSQLStatement(Database db, String statement) {
		return ExecuteSQLStatement(db, statement, null);
	}

	@Override
	public boolean ExecuteSQLTransaction(Database db, String[] statements,
			boolean rollbackFlag) {
		boolean result = false;

		LOGGER.logOperationBegin("ExecuteSQLTransaction", new String[] { "db",
				"statements", "rollbackFlag" }, new Object[] { db, statements,
				rollbackFlag });

		boolean rollback = false;
		SQLiteDatabase sqlDB = null;
		try {
			sqlDB = openDatabase(db);
			if(sqlDB != null) {
				sqlDB.beginTransaction();
				for (String statement : statements) {
					try {
						sqlDB.execSQL(statement);
					} catch (Exception ex) {
						LOGGER.logWarning("ExecuteSQLTransaction",
								"ExecuteSQLTransaction error executing sql statement ["
										+ statement + "]", ex);
						if (rollbackFlag) {
							LOGGER.logInfo("ExecuteSQLTransaction",
									"Transaction rolled back");
							rollback = true;
							break;
						}
					}
				}
				if (!rollback) {
					sqlDB.setTransactionSuccessful();
					result = true;
				}
			}
		} catch (Exception ex) {
			LOGGER.logError("ExecuteSQLTransaction", "Error", ex);
		} finally {
			if (sqlDB != null) {
				sqlDB.endTransaction();
			}
			LOGGER.logInfo("ExecuteSQLTransaction", "SQL Transaction finished");
			closeDatabase(sqlDB);
			LOGGER.logOperationEnd("ExecuteSQLTransaction", result);
		}

		return result;
	}

	@Override
	public boolean Exists(Database db) {
		return ExistsDatabase(db.getName());
	}

	@Override
	public boolean Exists(Database db, String tableName) {
		boolean result = false;

		LOGGER.logOperationBegin("Exists", new String[] { "db", "tableName" },
				new Object[] { db, tableName });

		try {
			String[] tables = GetTableNames(db);
			for (String table : tables) {
				if (table.equals(tableName)) {
					result = true;
					break;
				}
			}
		} catch (Exception ex) {
			LOGGER.logError("Exists", "Error", ex);
		} finally {
			LOGGER.logOperationEnd("Exists", result);
		}

		return result;
	}

	@Override
	public boolean ExistsDatabase(String dbName) {
		boolean result = false;

		LOGGER.logOperationBegin("ExistsDatabase", new String[] { "dbName" },
				new Object[] { dbName });

		try {
			Database[] dbs = GetDatabaseList();
			for (Database db : dbs) {
				if (db.getName().equals(dbName)) {
					result = true;
					break;
				}
			}
		} catch (Exception ex) {
			LOGGER.logError("ExistsDatabase", "Error", ex);
		} finally {
			LOGGER.logOperationEnd("ExistsDatabase", result);
		}

		return result;
	}

	@Override
	public Database GetDatabase(String dbName) {
		Database result = null;

		LOGGER.logOperationBegin("GetDatabase", new String[] { "dbName" },
				new Object[] { dbName });

		try {
			if (ExistsDatabase(dbName)) {
				result = new Database(dbName, DEFAULT_DATABASE_OPTION_COMPRESS,
						DEFAULT_DATABASE_OPTION_NEW);
			}
		} catch (Exception ex) {
			LOGGER.logError("GetDatabase", "Error", ex);
		} finally {
			LOGGER.logOperationEnd("GetDatabase", result);
		}

		return result;
	}

	@Override
	public Database[] GetDatabaseList() {
		Database[] result = null;

		LOGGER.logOperationBegin("GetDatabaseList", Logger.EMPTY_PARAMS,
				Logger.EMPTY_VALUES);

		try {
			String[] databaseNames = AndroidServiceLocator.getContext()
					.databaseList();
			ArrayList<Database> databases = new ArrayList<Database>();
			for (String databaseName : databaseNames) {
				Database database = new Database(databaseName,
						DEFAULT_DATABASE_OPTION_NEW,
						DEFAULT_DATABASE_OPTION_COMPRESS);
				databases.add(database);
			}

			result = databases.toArray(new Database[databases.size()]);
		} catch (Exception ex) {
			LOGGER.logError("GetDatabaseList", "Error", ex);
		} finally {
			LOGGER.logOperationEnd("GetDatabaseList", result);
		}

		return result;
	}

	@Override
	public String[] GetTableNames(Database db) {
		String[] result = null;

		LOGGER.logOperationBegin("GetTableNames", new String[] { "db" },
				new Object[] { db });

		try {

			if (Exists(db)) {
				IResultSet tables = ExecuteSQLQuery(db,
						"SELECT tbl_name FROM sqlite_master");
				if (tables != null) {
					tables.MoveToFirst();
					ArrayList<String> tableNames = new ArrayList<String>();
					while (tables.GetCurrentPosition() < tables.GetRowCount()) {
						String tableName = tables.GetString("tbl_name");
						if (!SYSTEM_TABLES.contains(tableName)) {
							tableNames.add(tableName);
						}
						tables.MoveToNext();
					}
					tables.Close();
					result = tableNames.toArray(new String[tableNames.size()]);
				}
			}
		} catch (Exception ex) {
			LOGGER.logError("GetTableNames", "Error", ex);
		} finally {
			LOGGER.logOperationEnd("GetTableNames", result);
		}

		return result;
	}

	private SQLiteDatabase openDatabase(Database db) throws SQLiteException {
		Context context = AndroidServiceLocator.getContext();
		if(db!=null) {
			return SQLiteDatabase.openDatabase(context
					.getDatabasePath(db.getName()).getAbsolutePath(), null,
					SQLiteDatabase.OPEN_READWRITE);
		}
		
		LOGGER.logError("openDatabase()", "Given database object is null. Please, check code to provide appropiated database object.");
		return null;
	}

	private void closeDatabase(SQLiteDatabase sqlDB) throws SQLiteException {

		if ((sqlDB != null) && (sqlDB.isOpen())) {
			try {
				sqlDB.close();
			} catch (Exception ex) {
			}
		}
	}

	private String getFormattedSQL(String sql, String[] params) {
		String result;

		if (params != null) {
			sql = sql.replaceAll("'", "\"");
			for (int i = 0; i < params.length; i++) {
				if (params[i] != null) {
					params[i] = params[i].replace("\"", "");
				}
			}
			result = MessageFormat.format(sql, (Object[]) params);
		} else {
			result = sql;
		}

		return result;
	}

}
