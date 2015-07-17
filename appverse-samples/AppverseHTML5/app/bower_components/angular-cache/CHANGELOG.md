##### 2.3.7 - 16 June 2014

###### Backwards compatible bug fixes
- Angular 1.2.18 with $http/localStorage #116

##### 2.3.6 - 15 June 2014

###### Backwards compatible bug fixes
- $http w/ cache is trying to store a promise, which dies on JSON.stringify #115

##### 2.3.5 - 15 June 2014

###### Backwards compatible bug fixes
- Page refresh retouches cache expiry times with defaults #114

##### 2.3.4 - 01 May 2014

###### Backwards compatible bug fixes
- Fix module definition and load sequencing #111

##### 2.3.3 - 24 February 2014

###### Backwards compatible bug fixes
- *sigh* Fixed #102 (regression from #100)

##### 2.3.2 - 23 February 2014

###### Backwards compatible bug fixes
- Fixed #100 (regression from #89)

##### 2.3.1 - 19 February 2014

###### Backwards compatible bug fixes
- Fixed #89

##### 2.3.0 - 09 January 2014
- Caches can now be disabled #82
- The `options` object (`$angularCacheFactory()`, `AngularCache#setOptions()`, and `$angularCacheFactoryProvider.setCacheDefaults()`) now accepts a `disabled` field, which can be set to `true` and defaults to `false`.
- `$angularCacheFactory.enableAll()` will enable any disabled caches.
- `$angularCacheFactory.disableAll()` will disable all caches.
- A disabled cache will operate as normal, except `AngularCache#get()` and `AngularCache#put()` will both immediately return `undefined` instead of performing their normal functions.

###### Backwards compatible API changes
- `removeExpired()` now returns an object (or array) of the removed items.

###### Backwards compatible bug fixes
- `removeExpired()` now removes _all_ expired items.

##### 2.2.0 - 15 December 2013

###### Backwards compatible API changes
- `removeExpired()` now returns an object (or array) of the removed items.

###### Backwards compatible bug fixes
- `removeExpired()` now removes _all_ expired items.

##### 2.1.1 - 20 November 2013

###### Backwards compatible bug fixes
- Allow number keys, but stringify them #76
- Fix "Uncaught TypeError: Cannot read property 'maxAge' of null" #77 (thanks @evngeny-o)

##### 2.1.0 - 03 November 2013

###### Backwards compatible API changes
- Modify .get(key, options) to accept multiple keys #71 (thanks @roryf)

###### Other
- Run tests against multiple versions of Angular.js #72
- Add banner to dist/angular-cache.min.js #68

##### 2.0.0 - 30 October 2013
- Not all methods of AngularCache and $angularCacheFactory are in README #61
- Fix demo to work with 2.0.0-rc.1 #62
- Using Bower to install this package, the dist filenames change per version? #63

##### 2.0.0-rc.1 - 14 October 2013

###### Breaking API changes
- Swapped `aggressiveDelete` option for `deleteOnExpire` option. #30, #47
- Changed `$angularCacheFactory.info()` to return an object similar to `AngularCache.info()` #45
- Namespaced angular-cache module under `jmdobry` so it is now "jmdobry.angular-cache". #42
- Substituted `storageImpl` and `sessionStorageImpl` options for just `storageImpl` option.

###### Backwards compatible API changes
- Added `recycleFreq` to specify how frequently to check for expired items (no more $timeout). #28, #57
- Added ability to set global cache defaults in $angularCacheFactoryProvider. #55

###### Backwards compatible bug fixes
- cacheFlushInterval doesn't clear web storage when storageMode is used. #52
- AngularCache#info(key) should return 'undefined' if the key isn't in the cache #53
- Fixed timespan issues in README.md. #59

###### Other
- Refactored angular-cache `setOptions()` internals to be less convoluted and to have better validation. #46
- Re-wrote documentation to be clearer and more organized. #56
- Fixed documentation where time spans were incorrectly labeled. #59

##### 1.2.0 - 20 September 2013

###### Backwards compatible API changes
- Added AngularCache#info(key) #43

###### Backwards compatible bug fixes
- Fixed #39, #44, #49, #50

##### 1.1.0 - 03 September 2013

###### Backwards compatible API changes
- Added `onExpire` callback hook #27
- Added `$angularCacheFactory.removeAll()` and `$angularCacheFactory.clearAll()` convenience methods #37, #38

###### Backwards compatible bug fixes
- Fixed #36

##### 1.0.0 - 25 August 2013
- Closed #31 (Improved documentation)
- Closed #32

##### 1.0.0-rc.1 - 21 August 2013
- Added localStorage feature #26, #29

##### 0.9.1 - 03 August 2013
- Fixed #25

##### 0.9.0 - 03 August 2013
- Added a changelog #13
- Added documentation for installing with bower
- Added ability to set option `aggressiveDelete` when creating cache and when adding items
- Cleaned up README.md
- Switched the demo to use Bootstrap 3

##### 0.8.2 - 09 July 2013
- Added CONTRIBUTING.md #22
- Cleaned up meta data in bower.json and package.json

##### 0.8.1 - 09 July 2013
- Added .jshintrc
- Cleaned up the docs a bit
- `bower.json` now uses `src/angular-cache.js` instead of the versioned output files #21
- From now on the tags for the project will be named using [semver](http://semver.org/)

##### 0.8.0 - 08 July 2013
- Added `AngularCache.setOptions()`, the ability to dynamically change the configuration of a cache #20
- Added `AngularCache.keys()`, which returns an array of the keys in a cache #19
- Added `AngularCache.keySet()`, which returns a hash of the keys in a cache #19

##### 0.7.2 - June 2013
- Added `angular-cache` to bower registry #7
- Created a working demo #9 #17
- Fixed the size not being reset to 0 when the cache clears itself #14 #16
- Added `$angularCacheFactory.keys()`, which returns an array of the keys (the names of the caches) in $angularCacheFactory #18
- Added `$angularCacheFactory.keySet()`, which returns a hash of the keys (the names of the caches) in $angularCacheFactory #18

##### 0.6.1 - June 2013
- Got the project building on TravisCI
- Renamed the project to `angular-cache` #5

##### 0.5.0 - June 2013
- Added a roadmap to README.md #4
- Clarify usage documentation #3
- Wrote unit tests #2

##### 0.4.0 - May 2013
- Added Grunt build tasks #1
