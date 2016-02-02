
// Geolocation

var testNorthType = 1; // magnetic(0), true(1)
var testMapScale = 1;
var testMapBoundingBox = 1;
var testRadius = 1;
var testLocation = new Object();
testLocation.XCoordinate = 50.46;
testLocation.YCoordinate = 50.0;
testLocation.ZCoordinate = 50.0;
testLocation.XDoP = 1.1;
testLocation.YDoP = 1.0;
var testQueryText = "?name=POI Name";
var testLocationCategory = new Object();
testLocationCategory.Name = "myCategory";

var testCategoryOther0 = new Object();
testCategoryOther0.Name = "My other category 0";
var testCategoryOther1 = new Object();
testCategoryOther1.Name = "My other category 1";

var testLocationCategoriesArray = new Array();
testLocationCategoriesArray[0] = testCategoryOther0;
testLocationCategoriesArray[1] = testCategoryOther1;

// Points of Interest

var testPOI = new Object();
testPOI.ID = "POI_1";
testPOI.Location = testLocation;
var POIDescription = new Object();
POIDescription.Description = "POI Description";
POIDescription.Name = "POI Name";
POIDescription.CategoryMain = testLocationCategory;
POIDescription.Categories = testLocationCategoriesArray;
testPOI.Description = POIDescription;
testPOI.Category = testLocationCategory;


// GEOLOCATION TEST CASES

var TestCase_Geolocation = [Appverse.Geo,
			[['GetAcceleration',''],
			['GetCoordinates',''],
			['GetHeading','{"param1":' + JSON.stringify(testNorthType) + '}'],
			['GetMap',''],
			['GetDeviceOrientation',''],
			['GetPOI','{"param1":' + JSON.stringify(testPOI.ID) + '}'],
			['GetPOIList','{"param1":' + JSON.stringify(testLocation) +',"param2":' + JSON.stringify(testRadius) + ',"param3":' + JSON.stringify(testQueryText) + ',"param4":' + JSON.stringify(testLocationCategory) +'}'],
			['GetVelocity',''],
			['IsGPSEnabled',''],
			['RemovePOI','{"param1":' + JSON.stringify(testPOI.ID) + '}'],
			['SetMapSettings','{"param1":' + JSON.stringify(testMapScale) +',"param2":' + JSON.stringify(testMapBoundingBox) + '}'],
			['UpdatePOI','{"param1":' + JSON.stringify(testPOI) + '}'],
			['StartUpdatingLocation',''],
			['StopUpdatingLocation',''],
			['StartUpdatingHeading',''],
			['StopUpdatingHeading',''],
			['GetGeoDecoder',''],
			['StartProximitySensor',''],
			['StopProximitySensor','']]
	];