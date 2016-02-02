var imgP = null,
    latestPickedImageUrl = null;
var testMediaFilePath = "res/sample.m4a";
//var testMediaFilePath = "res/Avatar.mov";
//var testMediaFileStream = "http://media.radiosrichinmoy.org/radio/46/46-18-1.m4a";
var testMediaFileStream = "http://trailers.apple.com/movies/fox/avatar/avatar-aug27a_480p.mov";
var testMediaPosition = 10;

var testMediaCameraOptions = {
    "ImageScaleFactor": 2,
    "UseFrontCamera": false,
    "UseCustomCameraOverlay": true,
    "GuidelinesColorHexadecimal": "#CACACA",
    "ScanButtonColorHexadecimal": "#F28400",
    "ScanButtonPressedColorHexadecimal": "#B95800",
    "ScanButtonMarginBottom": 20,
    "GuidelinesMargins": 15,
    "CancelButtonText": "Annulla",
    "DescriptionLabelText": "Inquadra tutto il documento\nnei segni sullo schermo",
    "DescriptionLabelFontFamilyName": "Ubuntu",
    "CancelButtonFontFamilyName": "Ubuntu"
};



//********** UI COMPONENTS

mediaPanelConfig = {
    scroll: 'both',
    width: 250,
    height: 250,
    floating: true,
    centered: true,
    modal: true
};

mediaPanel = null;


mediaButtons = {
    xtype: 'container',
    layout: 'hbox',
    cls: 'mediaButtons',
    docked: 'top',
    items: [{
        xtype: 'spacer'
    }, {
        xtype: 'button',
        iconCls: 'play',
        text: localizationController.localizedUIString("PlayAudio"),
        handler: function () {
            Appverse.Media.Play('res/sample.m4a');
        }
        }, {
        xtype: 'spacer'
    }, {
        xtype: 'button',
        iconCls: 'video',
        text: localizationController.localizedUIString("PlayVideo"),
        handler: function () {
            Appverse.Media.Play('res/Avatar.mov');
        }
        }, {
        xtype: 'spacer'
    }, {
        xtype: 'button',
        iconCls: 'wireless',
        text: localizationController.localizedUIString("PlayStream"),
        handler: function () {
            Appverse.Media.PlayStream('http://builder.gft.com/appstore/showcase/sample.m4a');
        }
        }, {
        xtype: 'spacer'
    }]
};

openMedia = function () {

    if (mediaPanel) {
        mediaPanel.show('pop');
        mediaPanel.scroller.scrollTo({
            x: 0,
            y: 0
        });
    }
};



//********** MEDIA TESTCASES
var TestCase_Media = [Appverse.Media,
    [['GetMetadata', '{"param1":' + JSON.stringify(testMediaFilePath) + '}'],
        ['GetCurrentMedia', ''],
        ['GetSnapshot', ''],
        ['GetState', ''],
        ['Play', '{"param1":' + JSON.stringify(testMediaFilePath) + '}'],
        ['PlayStream', '{"param1":' + JSON.stringify(testMediaFileStream) + '}'],
        ['Pause', ''],
        ['SeekPosition', '{"param1":' + JSON.stringify(testMediaPosition) + '}'],
        ['Stop', ''],
        ['TakeSnapshot', ''],
        ['TakeSnapshotWithOptions', '{"param1":' + JSON.stringify(testMediaCameraOptions) + '}']], mediaButtons];

//********** HANDLING CALLBACKS

Appverse.Media.onFinishedPickingImage = function (mediaMetadata) {
    console.log("onFinishedPickingImage: " + JSON.stringify(mediaMetadata));
    Showcase.app.getController('Main').console(feedObj("Appverse.Media.onFinishedPickingImage ", "Appverse.Media.onFinishedPickingImage", mediaMetadata));

    try {
        //Ext.Msg.alert("Image picked!");
        if (!mediaMetadata) {
            Showcase.app.getController('Main').toast("Image picking cancelled!");
            return;
        }
        Showcase.app.getController('Main').toast("Image picked!");
        // remove previous picked image, if exists
        if (latestPickedImageUrl) {
            var file = new Object();
            file.FullName = latestPickedImageUrl;
            Appverse.FileSystem.DeleteFile(file);
        }
        //formPanel = Ext.ComponentQuery.query('formpanel')[0];
        //var record = formPanel.getRecord();
        var resultJsonString = null;
        if (mediaMetadata != null) {
            resultJsonString = JSON.stringify(mediaMetadata);
        }

        //Ext.ComponentQuery.query('panel[cls="resultPanel"]')[0].setHtml('<img src="' + Appverse.DOCUMENTS_RESOURCE_URI + mediaMetadata.ReferenceUrl + '"/>');
        //imgP = Ext.create('Ext.Container',{html:'<img src="' + Appverse.DOCUMENTS_RESOURCE_URI + mediaMetadata.ReferenceUrl + '"/>'});
        imgP = Ext.create('Ext.Container', {
            cls: 'imgP',
            floating: true,
            modal: true,
            hidden: true,
            centered: true,
            scrollable: true,
            width: 300,
            height: 400,
            hideOnMaskTap: true,
            html: '<img width="300" height="400" src="' + Appverse.DOCUMENTS_RESOURCE_URI + mediaMetadata.ReferenceUrl + '"/>'
        });
        Ext.Viewport.add(imgP);
        imgP.show();

        latestPickedImageUrl = mediaMetadata.ReferenceUrl;

        //mediaPanel = Ext.create('Ext.Panel',mediaPanelConfig);

        //record.setResult(resultJsonString, true);
        //formPanel.load(record);
        //resultP.add(imgP);
    } catch (e) {
        Ext.Msg.alert(e);
    }
};


//// Scanner TestCases

/* QR Code */
var testAutoHandleQR = true;
var testQRCode = new Object();
testQRCode.Text = "TEL:123456789";
var QrContent;
if (Appverse.Scanner) {
    testQRCode.BarcodeType = Appverse.Scanner.BARCODETYPE_QR;
    testQRCode.QRType = Appverse.Scanner.QRTYPE_TEL;

    QrContent = {

        /**
         * The media text included inside the QR Code.
         * @type {String}
         */
        Text: 'test',

        /**
         * The media QR Code type.
         * <br/>Possible values: {@link Appverse.Scanner#QRTYPE_ADDRESSBOOK QRTYPE_ADDRESSBOOK},
         * {@link Appverse.Scanner#QRTYPE_EMAIL_ADDRESS QRTYPE_EMAIL_ADDRESS},
         * {@link Appverse.Scanner#QRTYPE_TEXT QRTYPE_TEXT},
         * {@link Appverse.Scanner#QRTYPE_GEO QRTYPE_GEO},
         * {@link Appverse.Scanner#QRTYPE_TEL QRTYPE_TEL},
         * {@link Appverse.Scanner#QRTYPE_SMS QRTYPE_SMS},
         * {@link Appverse.Scanner#QRTYPE_CALENDAR QRTYPE_CALENDAR},
         * {@link Appverse.Scanner#QRTYPE_WIFI QRTYPE_WIFI},
         * {@link Appverse.Scanner#QRTYPE_ISBN QRTYPE_ISBN},
         * {@link Appverse.Scanner#QRTYPE_PRODUCT QRTYPE_PRODUCT} & {@link Appverse.Scanner#QRTYPE_URI QRTYPE_URI}
         * @type {int}
         */
        QRType: Appverse.Scanner.QRTYPE_TEXT,

        /**
         * The Barcode type.
         * @type {int}
         */
        BarcodeType: Appverse.Scanner.BARCODETYPE_QR,

        /**
         * The Barcode Contact.
         * @type {Appverse.Scanner.VCard}
         */
        Contact: {},

        /**
         * The Barcode Coordinate.
         * @type {Appverse.Scanner.Coordinate}
         */
        Coord: {},

        /**
         * The Barcode size.
         * @type {int}
         */
        Size: 0
    };
}

var TestCase_Scanner = [Appverse.Scanner,
		[['DetectQRCode', '{"param1":' + JSON.stringify(testAutoHandleQR) + '}'],
		['HandleQRCode', '{"param1":' + JSON.stringify(testQRCode) + '}'],
        ['GenerateQRCode', '{"param1":' + JSON.stringify(QrContent) + '}']]];

//********** HANDLING QR CODES

if (Appverse.Scanner) {




    Appverse.Scanner.onQRCodeDetected = function (QRCodeContent) {
        testQRCode = QRCodeContent;
        console.log("onQRCodeDetected");
        console.log(JSON.stringify(QRCodeContent));

        var resultJsonString = null;
        if (QRCodeContent != null) {
            try {
                resultJsonString = JSON.stringify(QRCodeContent);
            } catch (e) {
                Ext.Msg.alert(e);
            }
        }
        testCase.setResult(resultJsonString);
        Ext.ComponentQuery.query('test')[0].setRecord(testCase);
        Showcase.app.getController('Main').console(feedObj("Appverse.Scanner.onQRCodeDetected", "Appverse.Scanner.onQRCodeDetected", resultJsonString));
        Showcase.app.getController('Main').showResult();

    };

    Appverse.Scanner.onGeneratedQR = function (QRData) {
        console.log(arguments);
        Ext.Viewport.add({
            xtype: 'container',
            centered: true,
            modal: true,
            html: '<img src="' + Appverse.DOCUMENTS_RESOURCE_URI + QRData.ReferenceUrl + '">',
            autoDestroy: true,
            height: '60%',
            widht: '60%',
            hideOnMaskTap: true
        });
    };


}
