jquery-mobile-map-js
=================

A lightweight helper library for use with jQuery mobile and the ArcGIS API for JavaScript. The library automatically recenters the map after phone orientation changes or view transitions. It also correctly reinflates the map where the map is hidden and then made visible again. Use this with single and multiple view jQuery mapping apps.

Without this library, a map may perform inconsistently. 

##How to use?

		var helper = new jQueryHelper(/* esri.map */ map);
		
##Features

* jQueryHelper.js - This library takes over the life cycle of the map when users navigate to a child view. 
* Three samples - index.html is a single page mobile app, index_twopage.html demonstrates a two page mobile app. And, index_twopage_query.html shows how to use the library with feature layers.
* Helper methods for handling more complex user interface situations that require manipulation of height and width.
* Debounces the map when the window is resized.
* Custom jQuery event "helper-map-loaded" - notifies you when map has been reinflated so that you can restart services or redraw features if required.


##Dependencies
* ArcGIS API for JavaScript
* jQuery and jQuery Mobile

## Resources

* [ArcGIS Developers](http://developers.arcgis.com)
* [ArcGIS REST Services](http://resources.arcgis.com/en/help/arcgis-rest-api/)
* [twitter@esri](http://twitter.com/esri)

## Issues

Find a bug or want to request a new feature?  Please let us know by submitting an issue.

## Contributing

Esri welcomes contributions from anyone and everyone. Your feedback can make the difference between an okay project and an amazingly awesome project. Please see our [guidelines for contributing](https://github.com/esri/contributing).


## Licensing
Copyright 2014 Esri

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

   http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.

A copy of the license is available in the repository's [license.txt]( license.txt) file.

[](Esri Tags: ArcGIS Web Mapping jQuery Mobile)
[](Esri Language: JavaScript)


