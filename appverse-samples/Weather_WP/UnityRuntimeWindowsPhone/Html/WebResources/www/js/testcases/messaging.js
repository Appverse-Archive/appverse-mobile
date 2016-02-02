var testMessageNumber = "+34666111111";
var testMessageText = "Message sent from Appverse Showcase";

var testMessageEmail = new Object();
testMessageEmail.Subject = "Appverse Showcase Email Sample";
var testFromAddress = new Object();
testFromAddress.Address = "appverse@gft.com";
testFromAddress.CommonName = "ddbc";
testFromAddress.Firstname = "David";
testFromAddress.Surname = "Barranco";
var testToAddress = new Object();
testToAddress.Address = "appverse@gft.com";
testToAddress.CommonName = "csfe";
testToAddress.Firstname = "Carles";
testToAddress.Surname = "Farre";
var testRecipientsArray = new Array();
testRecipientsArray[0] = testFromAddress;
testRecipientsArray[1] = testToAddress;
testMessageEmail.FromAddress = testFromAddress;
testMessageEmail.ToRecipients = testRecipientsArray;
testMessageEmail.BccRecipients = testRecipientsArray;
testMessageEmail.CcRecipients = testRecipientsArray;
testMessageEmail.MessageBodyMimeType = "text/plain";
testMessageEmail.MessageBody = "This is the message body for the Appverse Showcase Emailing Sample.";


// share contact test
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
        //console.log("TEST S2B "+i+" "+str.length)
    }
    // return an array of bytes
    return re;
}

function toUTF8Array(str) {
    var utf8= unescape(encodeURIComponent(str));
    var arr= new Array(utf8.length);
    for (var i= 0; i<utf8.length; i++)
        arr[i]= utf8.charCodeAt(i);
    return arr;
}

var vCard = "BEGIN:VCARD\nVERSION:3.0\nN:Appverse\nFN:Unityversal\nEMAIL;TYPE=INTERNET,PREF:unityversal@gmail.com\nREV:1394725590\nEND:VCARD";
var byteVCard = stringToBytes(vCard); //stringToBytes(vCard);
console.log("byteVCard length: " + byteVCard.length);

var vCardAttachment =
{ MimeType: 'text/x-vcard', FileName: 'vCard.vcf', Data: byteVCard, DataSize: byteVCard.length, ReferenceUrl: "" };

var vCardSubject = "Test VCard";
var vCardBody = "Hello from Appverse Mobile, this is my VCard";

var emailData = {
    Subject: vCardSubject,
    Attachment: [vCardAttachment],
    MessageBody: vCardBody
};

//********** UI COMPONENTS



//********** MESSAGING TESTCASES
var TestCase_Messaging = [Appverse.Messaging,
	[['SendMessageSMS','{"param1":' + JSON.stringify(testMessageNumber) +',"param2":' + JSON.stringify(testMessageText) + '}'],
	['SendMessageMMS','{"param1":' + JSON.stringify(testMessageNumber) +',"param2":' + JSON.stringify(testMessageText) + ',"param3":null}'],
	['SendEmail','{"param1":' + JSON.stringify(testMessageEmail) + '}'],
	['SendEmail#Share VCard','{"param1":' + JSON.stringify(emailData) + '}']]];

	
//********** HANDLING CALLBACKS



