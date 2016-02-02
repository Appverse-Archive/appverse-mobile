Utility = function() {
};

Utility.prototype.createDateTimeObjectFromDate = function (jsDate) {
	
	if(jsDate) {
		return this.createDateTimeObjectFromValues(
			jsDate.getDate(), 
			jsDate.getMonth() +1, 
			jsDate.getFullYear(), 
			jsDate.getHours(), 
			jsDate.getMinutes(), 
			jsDate.getSeconds());
	}
	
	return null;
}

Utility.prototype.createDateTimeObjectFromValues = function (day,month,year,hour,minute,seconds) {
	var dateTimeObject = new Object();
	if(day && month && year) { // day, month and year are REQUIRED
		dateTimeObject.Year = year;
		dateTimeObject.Month = month;
		dateTimeObject.Day = day;
		dateTimeObject.Hour = (hour?hour:0);
		dateTimeObject.Minute = (minute?minute:0);
		dateTimeObject.Second = (seconds?seconds:0);
	}
	return dateTimeObject;
}

Utility.prototype.printDateTimeObject = function (dateTimeObject) {
	if(dateTimeObject)
		return dateTimeObject.Day + "-" + dateTimeObject.Month + "-" + dateTimeObject.Year + " " + dateTimeObject.Hour + ":" + dateTimeObject.Minute + ":" + dateTimeObject.Second;
	return "undefined";
}

ShowcaseUtils = new Utility();


var getOffsetRect = function getOffsetRect(elem) {
    var box = elem.getBoundingClientRect();

    var body = document.body;
    var docElem = document.documentElement;

    var scrollTop = window.pageYOffset || docElem.scrollTop || body.scrollTop;
    var scrollLeft = window.pageXOffset || docElem.scrollLeft || body.scrollLeft;

    var clientTop = docElem.clientTop;
    var clientLeft = docElem.clientLeft;


    var top = box.top + scrollTop - clientTop;
    var left = box.left + scrollLeft - clientLeft;
    var bottom = top + (box.bottom - box.top);
    var right = left + (box.right - box.left);

    return {
        top: Math.round(top),
        left: Math.round(left),
        bottom: Math.round(bottom),
        right: Math.round(right),
    }
}

var getOffset = function getOffset(elem) {
    if (elem) {
        if (elem.getBoundingClientRect) {
            return getOffsetRect(elem);
        } 
    } 
    return null;
}