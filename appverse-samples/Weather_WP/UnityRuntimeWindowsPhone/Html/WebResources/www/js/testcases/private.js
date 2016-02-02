privateSamples = {};

///USPG CASES

var ID = {
    ApplicationId: 'SendMoney',
    MerchantId: 'P2P',
    AppVersion: '1.0',
    Username: '008p2p@gmail.com',
    Params: []
};

var password = 'Password.8!';

var payment = {
    Amount: '500',
    Currency: 'EUR',
    PaymentID: '067e6162-3b6f-4ae2-a171-2470b63dff00',
    Pin: '88880',
    Description: 'test',
    Params: [{
            'Key': 'merchId',
            'Value': '6776767'
        },
        {
            'Key': 'storeId',
            'Value': '87260'
        }]
};


//////////

USPGSample = {}
USPGSample.step1 = function () {
    Appverse.USPG.Activate(ID, password);
}
USPGSample.step2 = function () {
    Appverse.USPG.Login(password);
}
USPGSample.step3 = function () {
    Appverse.USPG.CheckState(ID, 'test', 'test');
}
USPGSample.step4 = function () {
    Appverse.USPG.CheckStatus(payment);
}
USPGSample.step5 = function () {
    Appverse.USPG.PaymentAuth(payment);
}

privateSamples.USPG = USPGSample;


/******************************************************************/

KCSample = {}
KCSample.obj = {};
KCSample.obj.Value = "a";
KCSample.obj.Key = "test";
KCSample.obj.Encryption = true;


KCSample.step1 = function () {
    Appverse.Security.StoreKeyValuePair(KCSample.obj);
}

//Appverse.OnKeyValuePairsStoreCompleted = function () {console.log(arguments);}

KCSample.step2 = function () {
    Appverse.Security.GetStoredKeyValuePair(KCSample.obj);
}

//Appverse.OnKeyValuePairsFound = function () {console.log(arguments);}

KCSample.step3 = function () {
    Appverse.Security.RemoveStoredKeyValuePair(KCSample.obj.Key)
}

privateSamples.keychaing = KCSample;

/******************************************************************/

/// Infocert cases

InfocertSample = {};
// test page: http://www.stp-service.it/test/test.html
// username: stp1 // password: o9f7G2slaq

InfocertSample.step1 = function(token,actcode) {
    Appverse.InfoCert.Initialize(token,actcode);
}

InfocertSample.step2 = function() {
    Appverse.InfoCert.GetDeviceDataId();
}

InfocertSample.step3 = function(signature) {
    Appverse.InfoCert.TransactOTPSignature(signature);
}

InfocertSample.step4 = function() {
    Appverse.InfoCert.ChangePin();
}


privateSamples.infocert = InfocertSample;
