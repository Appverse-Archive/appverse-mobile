
var testDatabaseName = "myDatabase.db";
var testDatabase = new Object();
testDatabase.Name = testDatabaseName;
testDatabase.NewOption = true;
testDatabase.CompressOption = true;
var testDatabaseTable = "myTable";
var testDatabaseTableColumnDefs = new Array();
testDatabaseTableColumnDefs[0] = "key INTEGER PRIMARY KEY";
testDatabaseTableColumnDefs[1] = "data TEXT";
var testSQLQuery = "SELECT * from myTable";
var testSQLStatement = "INSERT INTO myTable (key,data) VALUES (1,{0})";
var testSQLStatementReplace1= "'this is my test data 1'";
var testSQLStatements = new Array();
testSQLStatements[0] = "DELETE from myTable WHERE key=1";
testSQLStatements[1] = "INSERT INTO myTable (key,data) VALUES (2,'test 2')";
var testSQLTransactionRollback = true;

//********** UI COMPONENTS

//********** DATABASE TESTCASES
var TestCase_Database = [Appverse.Database,
	[['CreateDatabase','{"param1":' +  JSON.stringify(testDatabaseName) + '}'],
	['CreateTable','{"param1":' +  JSON.stringify(testDatabase) + ',"param2":' + JSON.stringify(testDatabaseTable) + ',"param3":[' + JSON.stringify(testDatabaseTableColumnDefs[0]) + ',' + JSON.stringify(testDatabaseTableColumnDefs[1])  +']}'],
	['DeleteDatabase','{"param1":' +  JSON.stringify(testDatabase) + '}'],
	['DeleteTable','{"param1":' +  JSON.stringify(testDatabase) + ',"param2":' + JSON.stringify(testDatabaseTable) + '}'],
	['ExecuteSQLQuery','{"param1":' +  JSON.stringify(testDatabase) + ',"param2":' + JSON.stringify(testSQLQuery) + '}'],
	['ExecuteSQLStatement','{"param1":' +  JSON.stringify(testDatabase) + ',"param2":' + JSON.stringify(testSQLStatement) + ',"param3":['+JSON.stringify(testSQLStatementReplace1) +']}'],
	['ExecuteSQLTransaction','{"param1":' +  JSON.stringify(testDatabase) 
		+ ',"param2":[' + JSON.stringify(testSQLStatements[0]) + ',' + JSON.stringify(testSQLStatements[1]) 
		+ '],"param3":' + JSON.stringify(testSQLTransactionRollback) +'}'],
	['Exists','{"param1":' +  JSON.stringify(testDatabase) + ',"param2":' + JSON.stringify(testDatabaseTable) + '}'],
	['ExistsDatabase','{"param1":' +  JSON.stringify(testDatabaseName) + '}'],
	['GetDatabase','{"param1":' +  JSON.stringify(testDatabaseName) + '}'],
	['GetDatabaseList',''],
	['GetTableNames','{"param1":' +  JSON.stringify(testDatabase) + '}']]];
	
//********** HANDLING CALLBACKS
