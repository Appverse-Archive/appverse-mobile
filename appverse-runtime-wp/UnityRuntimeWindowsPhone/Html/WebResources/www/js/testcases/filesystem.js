
var testRemoteFileUrl = "http://dl.dropbox.com/u/12811767/test.pdf";
var testLocalFilename = testRemoteFileUrl.substring(testRemoteFileUrl.lastIndexOf('/')+1);

var testResourcesFileSourcePath = "Contents/tobecopied.txt";
var testResourcesFileDestinationPath = "tobecopied.txt";

var testDirectoryName = "myDirectory";
var testFileName = "myFile.txt";
var testDirectory = new Object();
testDirectory.FullName = "development";

var testDirectory2 = new Object();
testDirectory2.FullName = "development/myDirectory";
var testFile = new Object();
testFile.FullName="development/myDirectory/myFile.txt";
var testAppendFlag = true;
var testStringWriteFile = "---- a test added to file ----";


function stringToBytes ( str ) {
  var ch, st, re = [];
  for (var i = 0; i < str.length; i++ ) {
    ch = str.charCodeAt(i);  // get char 
    st = [];                 // set up "stack"
    do {
      st.push( ch & 0xFF );  // push byte to stack
      ch = ch >> 8;          // shift value down by 1 byte
    }  
    while ( ch );
    // add stack contents to result
    // done because chars have "wrong" endianness
    re = re.concat( st.reverse() );
  }
  // return an array of bytes
  return re;
}

var testStringWriteFileByteArray = stringToBytes(testStringWriteFile);

//********** UI COMPONENTS

//********** FILESYSTEM TESTCASES
var TestCase_Filesystem = [Appverse.FileSystem,
	[['CopyFromRemote','{"param1":' + JSON.stringify(testRemoteFileUrl) + ',"param2":' + JSON.stringify(testLocalFilename) + '}'],
	['CopyFromResources','{"param1":' + JSON.stringify(testResourcesFileSourcePath) + ',"param2":' + JSON.stringify(testResourcesFileDestinationPath) + '}'],
	['CreateDirectory','{"param1":' + JSON.stringify(testDirectoryName) + ',"param2":' + JSON.stringify(testDirectory) + '}'],
	['CreateFile','{"param1":' + JSON.stringify(testFileName) + ',"param2":' + JSON.stringify(testDirectory2) + '}'],
	['DeleteDirectory','{"param1":' + JSON.stringify(testDirectory2) + '}'],
	['DeleteFile','{"param1":' + JSON.stringify(testFile) +'}'],
	['ExistsDirectory','{"param1":' + JSON.stringify(testDirectory2) + '}'],
	['ExistsFile','{"param1":' + JSON.stringify(testFile) +'}'],
	['GetDirectoryRoot',''],
	['ListDirectories','{"param1":' + JSON.stringify(testDirectory) + '}'],
	['ListFiles','{"param1":' + JSON.stringify(testDirectory) + '}'],
	['ReadFile','{"param1":' + JSON.stringify(testFile) +'}'],
	['WriteFile','{"param1":' + JSON.stringify(testFile) +',"param2":' + JSON.stringify(testStringWriteFileByteArray) + ', "param3":' + JSON.stringify(testAppendFlag) + '}']]];
	
//********** HANDLING CALLBACKS
