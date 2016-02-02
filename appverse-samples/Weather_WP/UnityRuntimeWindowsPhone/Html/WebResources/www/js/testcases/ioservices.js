var testServiceName = "iWorkspaceSrv";

var testServiceName = "iWorkspaceSrv";
var testServiceType = Appverse.IO.SERVICETYPE_XMLRPC_JSON;
var testJSONServiceRequestContent = '{method:contactService.getContactDetail,id:1,params:[MAPS]}';
var testJSONServiceRequest = new Object();
testJSONServiceRequest.Session = new Object();
testJSONServiceRequest.Content = testJSONServiceRequestContent;
/*
// request contenttype attribute will override service type
testJSONServiceRequest.ContentType = "x-gwt-rpc";

// example to add headers to the request
var requestHeaders = new Array();
var rHeader1 = new Object();
rHeader1.Name = "X-GWT-Permutation";
rHeader1.Value = "permutation-value";
var rHeader2 = new Object();
rHeader2.Name = "X-GWT-Module_Base";
rHeader2.Value = "module-base-value";
requestHeaders[0] = rHeader1;
requestHeaders[1] = rHeader2;
testJSONServiceRequest.Headers = requestHeaders;


// example to add cookies to the request
var requestCookies = new Array();
var cookie1 = new Object();
cookie1.Name = "mycookie1";
cookie1.Value = "maps";
var cookie2 = new Object();
cookie2.Name = "mycookie2";
cookie2.Value = "12345";
requestCookies[0] = cookie1;
requestCookies[1] = cookie2;
testJSONServiceRequest.Session.Cookies = requestCookies;
*/

var testService = new Object();
testService.Name = "iWorkspaceSrv";
testService.Type = Appverse.IO.SERVICETYPE_XMLRPC_JSON;
var testServiceEndPoint = new Object();
testServiceEndPoint.Scheme = "/iworkspacesrv/scheme";
testServiceEndPoint.Host = "https://workspace.gft.com";
testServiceEndPoint.Path = "/iworkspacesrv/jsonrpc";
testService.Endpoint = testServiceEndPoint;
testService.RequestMethod = 0; // POST method


var testServiceOctect = {};
testServiceOctect.Name = "getBinary";
testServiceOctect.Endpoint = {};
testServiceOctect.Endpoint.Host = "http://builder.gft.com/appstore/apploader-test/test-1.1.0.zip";
testServiceOctect.Endpoint.Port = 0;
testServiceOctect.Endpoint.Path = "";
testServiceOctect.RequestMethod = 0;
testServiceOctect.Type = Appverse.IO.SERVICETYPE_OCTET_BINARY;

var testRequestContentOctet = '';
var testRequestOctet = {};
testRequestOctet.Session = {};
testRequestOctet.Content = testRequestContentOctet;

var testStorePath = "tmp-showcase.zip";

//var testServiceWithObject = '{"Endpoint": {"ProxyUrl": null, "Port": 0, "Scheme": "/iworkspacesrv/scheme", "Host": "https://workspace.gft.com", "Path": "/iworkspacesrv/jsonrpc" }, "Name": "iWorkspaceSrv", "RequestMethod": 0,  "Type": 4}';


var myRequest = new Object();
myRequest.Session = new Object();
myRequest.Content = '{"language":"es","appVersion":"2"}';

var myService = new Object();
myService.Name = "appverseBank";
myService.Type = Appverse.IO.SERVICETYPE_XMLRPC_JSON;
var myServiceEndPoint = new Object();
myServiceEndPoint.Host = "https://appversebank.gftlabs.com/org-appverse-app-banking";
myServiceEndPoint.Path = "/rest/login/es/prelogin";
myService.Endpoint = myServiceEndPoint;
myService.RequestMethod = 0; // POST method

var myServiceName = "appverseBank";
var myServiceType = Appverse.IO.SERVICETYPE_XMLRPC_JSON;

var opencpuSN = "opencpu";

var opencpuRequest = new Object();
opencpuRequest.Session = new Object();


var fidorLogin = "heide_7268@fidortecs.de:password";
var authentication = "Basic " + btoa(fidorLogin);
//fidor
var fidorClient = "42fbad52002311b5";
var fidorSecret = "14d69a5b7394f4c800e6a9ad34a0cf49";
var fidorServiceType = Appverse.IO.SERVICETYPE_REST_JSON;
var fidorRequest = function () {
    return {
        "Headers": [{
            "Name": "Authorization",
            "Value": authentication
        }, {
            "Name": "Fidor-Client-Id",
            "Value": fidorClient
        }],
        "Session": {}
    }
};
var fidorServiceName = "BasicLogin";


var sendRequestAppAuthorize = function (fidorCookie) {
    return {
        "Session": {
            "Cookies": [{
                "Name": "session_token",
                "Value": fidorCookie
            }]
        }
    }
};
var fidorAppAuthorizeServiceName = "appAuthorize";


var sendRequestAuthorize = function (fidorCookie) {
    var status = Math.floor(Math.random() * 9000) + 1000;
    return {
        "Session": {
            "Cookies": [{
                "Name": "session_token",
                "Value": fidorCookie
            }]
        },
        "ProtocolVersion": Appverse.IO.HTTP_PROTOCOL_VERSION_1_1,
        "Content": "?redirect_uri=" + encodeURIComponent(Appverse.MAIN_PAGE_RESOURCE_URI + "index.html") + "&client_id=" + fidorClient + "&state=" + status + "&response_type=code",
        "StopAutoRedirect": true
    }
};
var fidorOauthAuthorizeServiceName = "oauthAuthorize";


var sendRequestToken = function (fidorCookie, code) {
    return {
        "Session": {
            "Cookies": [{
                "Name": "session_token",
                "Value": fidorCookie
            }]
        },
        "ContentType": "application/x-www-form-urlencoded",
        "ProtocolVersion": Appverse.IO.HTTP_PROTOCOL_VERSION_1_1,
        "Content": 'client_id=' + fidorClient + '&client_secret=' + fidorSecret + '&grant_type=authorization_code&redirect_uri=' + encodeURIComponent(Appverse.MAIN_PAGE_RESOURCE_URI + "index.html") + "&code=" + code

    }
};

var fidorOauthTokenServiceName = "oauthToken";


var myRequestMultipart = new Object();
myRequestMultipart.Session = new Object();
myRequestMultipart.Content = 'userid=secondtest';
myRequestMultipart.Attachment = [];
myRequestMultipart.Attachment[0] = {
    "FormFieldKey": "scannedPhoto1",
    "ReferenceUrl": "assets/imagename.JPG",
    "MimeType": "image/jpeg"
};


var myServiceNameMultipart = "mockServerMultipart";
var myServiceTypeMultipart = Appverse.IO.SERVICETYPE_MULTIPART_FORM_DATA;



// I/O SERVICES TEST CASE

var TestCase_IOServices = [Appverse.IO, [
    ['GetService', '{"param1":' + JSON.stringify(testServiceName) + ',"param2":' + JSON.stringify(testServiceType) + '}'],
    ['GetServices', ''],
    ['InvokeService', '{"param1":' + JSON.stringify(testJSONServiceRequest) + ',"param2":' + JSON.stringify(testServiceName) + ',"param3":' + JSON.stringify(testServiceType) + '}'],
    ['InvokeService#MULTIPART', '{"param1":' + JSON.stringify(myRequestMultipart) + ',"param2":' + JSON.stringify(myServiceNameMultipart) + ',"param3":' + JSON.stringify(myServiceTypeMultipart) + '}'],
    ['InvokeService#AppverseBank', '{"param1":' + JSON.stringify(myRequest) + ',"param2":' + JSON.stringify(myServiceName) + ',"param3":' + JSON.stringify(myServiceType) + '}'],
    //['InvokeService#opencpu', '{"param1":' + JSON.stringify(opencpuRequest) + ',"param2":' + JSON.stringify(opencpuSN) + ',"param3":' + JSON.stringify(myServiceType) + '}'],
    ['InvokeServiceForBinary', '{"param1":' + JSON.stringify(testRequestOctet) + ',"param2":' + JSON.stringify(testServiceOctect) + ',"param3":' + JSON.stringify(testStorePath) + '}'],




]];
//"param2":' + JSON.stringify(testService)

var fidorCookie;
var fidorCb1 = function (result, id) {

    if (result) {
        fidorCookie = result.Headers.filter(function (a) {
            return a.Name == "Fidor-Cookie"
        })[0].Value;
        /*if (activeAPI) {
            //Ext.Msg.alert('OK', "goto Step 2");
            var str = sendRequestAppAuthorize(fidorCookie);

            activeAPI[1].push([
                'InvokeService#fidorAppAuth', '{"param1":' + JSON.stringify(str) + ',"param2":' + JSON.stringify(fidorAppAuthorizeServiceName) + ',"param3":' + JSON.stringify(fidorServiceType) + ',"param4":"fidorCb2"}'
            ]);
            Showcase.app.getController('Main').setSelect(true);
        }else{*/
        step2(fidorCookie);
        //}
    }
}

var fidorCb2 = function (result, id) {
    if (result) {
        //Ext.Msg.alert('OK', "goto Step 3");
        /*if (activeAPI) {
            var str = sendRequestAuthorize(fidorCookie);
            activeAPI[1].push([
                'InvokeService#fidorOauthAuth', '{"param1":' + JSON.stringify(str) + ',"param2":' + JSON.stringify(fidorOauthAuthorizeServiceName) + ',"param3":' + JSON.stringify(fidorServiceType) + ',"param4":"fidorCb3"}'
            ]);
            Showcase.app.getController('Main').setSelect(true);
        }else{*/

        step3(fidorCookie);
        //}
    }
}

var fidorCb3 = function (result, id) {
    var code;
    if (result && result.Headers.filter(function (a) {
            return a.Name == "Status"
        })[0].Value.indexOf("302") != -1) {
        code = result.Headers.filter(function (a) {
            return a.Name == "Location"
        })[0].Value.split('=')[1].split('&')[0];
        console.log(code);
        //Ext.Msg.alert('OK', "goto Step 4");
        /*if (activeAPI) {
            var str = sendRequestToken(fidorCookie, code);
            activeAPI[1].push([
                'InvokeService#fidorOauthToken', '{"param1":' + JSON.stringify(str) + ',"param2":' + JSON.stringify(fidorOauthTokenServiceName) + ',"param3":' + JSON.stringify(fidorServiceType) + ',"param4":"fidorCb4"}'
            ]);
            Showcase.app.getController('Main').setSelect(true);
        }else{*/

        step4(fidorCookie, code);
        //}
    }
}
var fidorCb4 = function (result, id) {
    if (result) {
        access_token = JSON.parse(result.Content).access_token;
        Ext.Msg.alert('access_token', access_token);
        console.log(access_token);
        return access_token;
    }
    return null;
}


step1 = function () {
    Appverse.IO.InvokeService(fidorRequest(), fidorServiceName, fidorServiceType, 'fidorCb1', 'step1');
}
step2 = function (fidorCookie) {
    Appverse.IO.InvokeService(sendRequestAppAuthorize(fidorCookie), fidorAppAuthorizeServiceName, fidorServiceType, 'fidorCb2', 'step2');
}
step3 = function (fidorCookie) {
    Appverse.IO.InvokeService(sendRequestAuthorize(fidorCookie), fidorOauthAuthorizeServiceName, fidorServiceType, 'fidorCb3', 'step3');
}
step4 = function (fidorCookie, code) {
    Appverse.IO.InvokeService(sendRequestToken(fidorCookie, code), fidorOauthTokenServiceName, fidorServiceType, 'fidorCb4', 'step3');
}
