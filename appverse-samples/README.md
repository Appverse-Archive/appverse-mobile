# Appverse Application Samples

This module contains application samples build using the Appverse platform.

You will find here a bunch of application projects ready to be compiled and executed for all supported platforms (iOS, Android, etc), in the following project configurations:

* Monotouch C# Project Samples (iOS application)
* Eclipse Project Samples (Android application)
* NetBeans Project Samples (Android application)

For more information, please refer to <http://appverse.github.com>.
This software is licensed under APL 2.0 <http://appverse.org/legal/appverse-license/>.


## What you need to configure per application?

Each mobile application needs to configure some parameters and build properties.
Appverse provides you with some application project samples with all these properties already configured.
But when you create your own application project, you should check and reconfigure them in order to attend your own application requirements.

### Monotouch C# iOS Application 

**How to embed your Web resources**
* In order to add the web resources for an application, you first need to add them to the project in MonoDevelop. To accomplish this perform the following steps:
	* Copy your web resources into the project's location, under the following folder structure: "WebResources">"www"
	* Right-click on the project folder (or Ctrl-Click) and select *Add* > *Add Exiting folder...*, displaying the *Add folder* dialog window.
	* Select the "WebResources" folder and then select all the needed resources in the path.
	* Mark all resources as <b>Content</b> in the <b>Build Action</b> selector at the *Add folder* dialog window.
	* Once included, if new resources are added to the project don't forget to mark them as *Content*. Right-click on each field to see its *Properties*.
* If your application also needs some custom API configuration, repeat the previous steps with the application config files ("app" folder). See the examples for further information.

**Appverse and Third-party Libraries**:
* The **Monotouch compiling/runtime dependencies** should be placed the _"lib"_ project folder.
* You should compile and copy the **Appverse** libraries, from Core C# (**appverse-modile/appverse-core**) and Platform iOS (**appverse-mobile/appverse-platfrom-ios**) projects to the _"libs"_ project folder.
	* AppverseCoreIOS.dll
	* AppversePlatformIOS.dll
* Other *third-party* needed libraries You could find them at the **appverse-mobile/appverse-runtime-ios** project):
	* GoogleAnalytics.dll (analytics)
	* SharpZipLib.dll  (security)
	* BouncyCastle.Crypto.dll  (security)

**General Settings**
* **Bundle Name**: the short display name of the bundle (the name of the final binary application).
	* Configure it at *Info.plist* file using the property *CFBundleName*, or in the *Project Options*>*IPhone Application* settings > *Advanced* tab > *Bundle Name* property.

* **Bundle Display Name**: the actual name of the bundle (the display name of the application). 
	* Configure it at *Info.plist* file using the property *CFBundleDisplayName*, or in the *Project Options* > IPhone Application* settings > *Summary* tab > *Application name* property.
	
* **Bundle Identifier**: an identifier string that uniquely identifies the bundle. Each distinct app or bundle on the system must have a unique bundle ID. The system uses this string to identify your app in many ways. The string should be in reverse DNS format using only the Roman alphabet in upper and lower case (A-Z, a-z), the dot (.), and the hyphen (-).
	* Configure it at *Info.plist* file using the property *CFBundleIdentifier*, or in the *Project Options* > IPhone Application* settings > *Summary* tab > *Identifier* property.

* **Bundle Version**: specifies the build version number of the bundle, which identifies an iteration (released or unreleased) of the bundle. The build version number should be a string comprised of three non-negative, period-separated integers with the first integer being greater than zero. The string should only contain numeric (0-9) and period (.) characters. Leading zeros are truncated from each integer and will be ignored (that is, 1.02.3 is equavalent to 1.2.3).
	* Configure it at *Info.plist* file using the property *CFBundleVersion*, or in the *Project Options* > *IPhone Application* settings > *Summary* tab > *Version* property.

* **Bundle Executable**: the name of the bundle's executable file.
	* Configure it at *Info.plist* file using the property *CFBundleExecutable*, or in the *Project Options* > *IPhone Application* settings > *Advanced* tab > *Executable File* property.
	
* **Minimun OS Version**: indicates the minimum version of OS X required for this app to run in a device. This string must be of the form n.n.n where n is a number. The first number is the major version number of the system. The second and third numbers are minor revision numbers.
	* Configure it at *Info.plist* file using the property *MinimumOSVersion*, or in the *Project Options* > *IPhone Application* settings > *Summary* tab > *Deployment Target* property.
	* Default *Appverse* value is 4.3
	
* **Targeted Device Family**: an array that specifies the underlying hardware types on which this app is designed to run.
	* Configure it at *Info.plist* file using the property *UIDeviceFamily*, or in the *Project Options* > *IPhone Application* settings > *Advanced* tab > *Targeted Device Family* array property.
 	* Default *Appverse* value is: iPhone/iPod touch + iPad. In the *Summary* tab you should select the *Universal* option on the *Devices* selection list.
	
* **Supported Device Orientations**: an array that specifies the orientations that the app supports. The system uses this information (along with the current device orientation) to choose the initial orientation in which to launch your app. The value for this key is an array of strings: *UIInterfaceOrientationPortrait*, *UIInterfaceOrientationPortraitUpsideDown*, *UIInterfaceOrientationLandscapeLeft* and *UIInterfaceOrientationLandscapeRight*.
	* Configure it at *Info.plist* file using the property *UISupportedInterfaceOrientations*, or in the *Project Options* > *IPhone Application* settings > *Summary* tab > *iPhone / iPod Deployment Info* section > *Supported Device Orientations* selectable graphical buttons.
	* Default *Appverse* value for a iPhone and iPod thouch is just the *UIInterfaceOrientationPortrait* orientation.

* **Supported Device Orientations for iPad**: an array that specifies the orientations that the app supports in the iPad devices. The system uses this information (along with the current device orientation) to choose the initial orientation in which to launch your app. The value for this key is an array of strings: *UIInterfaceOrientationPortrait*, *UIInterfaceOrientationPortraitUpsideDown*, *UIInterfaceOrientationLandscapeLeft* and *UIInterfaceOrientationLandscapeRight*.
	* Configure it at *Info.plist* file using the property *UISupportedInterfaceOrientations~ipad*, or in the *Project Options* > *IPhone Application* settings > *Summary* tab > *iPad Deployment Info* section > *Supported Device Orientations* selectable graphical buttons.
	* Default *Appverse* value for a iPad is: *UIInterfaceOrientationPortrait*, *UIInterfaceOrientationLandscapeLeft* and *UIInterfaceOrientationLandscapeRight* orientations.

* **Application icons**: an array identifying the icon files for the bundle. It is recommended that you always create icon files using the PNG format.
	* Include them at *Info.plist* file using the property *CFBundleIconFiles*, or in the *Project Options* > *IPhone Application* settings > *Summary* tab > *Universal Icons* section > *App Icons* selectable graphical buttons.
	* iPhone icon: the main icon for applications running on iPhone; 57x57 pixels.
	* iPhone Retina icon: the high resolution version of the main icon for applications running on iPhone; 114x114 pixels.
	* iPad icon: the main icon for applications running on iPad; 72x72 pixels.
	* iPad Retina icon: the high resolution version of the main icon for applications running on iPad; 144x144 pixels.

* **Prerendered Icon**: specifies whether the app's icon already includes a shine effect. Select "true" if you don't want that the iOS applies a shine effect to the app icon.
	* Configure it at *Info.plist* file using the property *UIPrerenderedIcon*, or in the *Project Options* > *IPhone Application* settings > *Summary* tab > *Universal Icons* section > *App Icons* > *Prerendered* checkbox.
	* Default value is:true.

* **Spotlight & Settings icons**: an array identifying the icon files for the bundle. It is recommended that you always create icon files using the PNG format.
	* Include them at *Info.plist* file using the property *CFBundleIconFiles*, or in the *Project Options* > *IPhone Application* settings > *Summary* tab > *Universal Icons* section > *Spotlight & Settings  Icons* selectable graphical buttons.
	* iPhone Spotlight and iPad Settings: the icon displayed in the Spotlight search results on iPhone and in the Settings application on iPad; 29x29 pixels.
	* iPhone Retina Spotlight and iPad Retina Settings: the high resolution version of the icon displayed in the Spotlight search results on iPhone and in the Settings application on iPad; 58x58 pixels.
	* iPad Spotlight: the icon displayed in the Spotlight search results on iPad; 50x50 pixels.
	* iPad Retina Spotlight: the high resolution version of the icon displayed in the Spotlight search results on iPad; 100x100 pixels.

* **Launch Images**: the images to be used as the application launch image. The images should be included on the project with the appropriate name and dimensions.
	* Attach them at the *Project Options* > *IPhone Application* settings > *Summary* tab > *Universal Icons* section > *iPhone Launch Images* and *iPad Launch Images*.
	* iPhone: the launch image displayed for applications running on iPhone ( *Default.png* ); 320x480 pixels.
	* iPhone Retina: the high resolution version of the launch image displayed for applications running on iPhone ( *Default@2x.png* ); 640x960 pixels.
	* iPhone 5: the high resolution version of the launch image displayed for applications running on iPhone 5 ( *Default-568h@2x.png* ); 640x1136 pixels.
	* iPad portrait: the launch image displayed for applications running on iPad in portrait mode ( *Default-Portrait.png* ); 768x1004 pixels.
	* iPad landscape: the launch image displayed for applications running on iPad in landscape mode ( *Default-Landscape.png* ); 1024x748 pixels.
	* iPad Retina portrait: the high resolution version of the launch image displayed for applications running on iPad in portrait mode ( *Default-Portrait@2x.png* ); 1536x2008 pixels.
	* iPad Retina landscape: the high resolution version of the launch image displayed for applications running on iPad in landscape mode ( *Default-Landscape@2x.png* ); 2048x1496 pixels.
	
**Signing**

In order to properly sign the application for device deployment, you should provide the signing information at the *Project Options* > *IPhone Bundle Signing* settings:

* **Signing Identity**: the name of the Apple certificate that will be used to sign the application.
* **Provisioning profile** (optional): the provisioning profile file to be embedded inside the application bundle. This file usually contains information about the allowed devices that could install the application and which certificates could sign it. It is used for *Over the Air* device deployments.
* Custom **entitlements** and **resource rules**.

### Eclipse Android Application

Before you start with your first Android app as an Eclipse project, you will need to check that you have your development environment set up.
You will need to install the following:

1. Download the Android SDK: [http://developer.android.com/sdk/index.html]
2. Install the ADT Plugin for Eclipse. See instructions here: [http://developer.android.com/tools/sdk/eclipse-adt.html] and [http://developer.android.com/sdk/installing/installing-adt.html]
3. Download the latest SDK Tools and platforms using the SDK Manager.

**System requirements**:

*Operating Systems*
* Windows XP (32-bit), Vista (32- or 64-bit), or Windows 7 (32- or 64-bit)
* Mac OS X 10.5.8 or later (x86 only)
* Linux (tested on Ubuntu Linux, Lucid Lynx)
	* GNU C Library (glibc) 2.7 or later is required.
	* On Ubuntu Linux, version 8.04 or later is required.
	* 64-bit distributions must be capable of running 32-bit applications.

*Eclipse IDE*
* Eclipse 3.6.2 (Helios) or greater
	> Note: Eclipse 3.5 (Galileo) is no longer supported with the latest version of ADT.
* Eclipse JDT plugin (included in most Eclipse IDE packages)
* JDK 6 (JRE alone is not sufficient)
* Android Development Tools plugin (recommended)
* Not compatible with GNU Compiler for Java (gcj)


**How to embed your Web resources**:
* In order to add the web resources for an application, you just need to add them to the _"assets"_ Eclipse project folder. They will be included automatically to the final native application on build time. 
* The folder structure is important:
	* You should place the application configuration files in the _"app"_ folder under the assets folder.
	* You should place your application web resources under the _"WebResources"_ >> _"www"_  folders; under the assets folder.

**Appverse and Third-party Libraries**:
* The **Android dependencies** in an Eclipse project are automatically included if you place the corresponding JAR libraries in the  _"libs"_ Eclipse project folder.
* You should compile and copy the **Appverse** libraries, from Core Java (**appverse-modile/appverse-core**) and Platform Android (**appverse-mobile/appverse-platfrom-android**) projects to the _"libs"_ project folder.
	* AppverseCoreJava.jar
	* AppversePlatformAndroid.jar
* Other *third-party* needed libraries You could find them at the **appverse-mobile/appverse-runtime-android** project):
	* libGoogleAnalytics.jar (analytics)
	* SpongyCastle_147.jar  (security)

**General Settings**

* **Pacakge Name**: a package identifier string (a full Java-language-stsyle package name) that uniquely identifies the application. Once published, you **cannot change the package name* fo ryour application.
	* Configure it at the *AndroidManifest.xml* file using the attribute *package* at the *<manifest>* root node, or open the file in the *Android Manifest Editor* >> *Manifest General Attributes* >> *Package* attribute.
	* For example: *org.appverse.mobile.samples.appversetwitter*
* **Version Code**: specifies the internal version number of this application. It is used to determine and compare recent versions.
	* Configure it at the *AndroidManifest.xml* file using the attribute *android:versionCode* at the *<manifest>* root node, or open the file in the *Android Manifest Editor* >> *Manifest General Attributes* >> *Version code* attribute.
* **Version Name**: specifies the version number of this application shown to users. Only display purposes.
	* Configure it at the *AndroidManifest.xml* file using the attribute *android:versionName* at the *<manifest>* root node, or open the file in the *Android Manifest Editor* >> *Manifest General Attributes* >> *Version name* attribute.
* **Min SDK Version**: an integer that specifies the *minimum API level* required for this application to run.
	* Configure it at the *AndroidManifest.xml* file using the attribute *android:minSdkVersion* at the *<uses-sdk>* node contained in the *<manifest>* root node, or open the file in the *Android Manifest Editor* >> *ManifestExtras* >> *Uses Sdk* extra >> *Min SDK version* attribute.
	* For an Appverse application, the min SDK version could be the API level 8 (Froyo); levels under 8 are not supported.
* **Target SDK Version**: an integer that specifies the *API level* this application targets. This attribute informs the system that you have tested against the target version and the system should not enable any compatibility behaviors to maintain your app's forward-compatibility with the target version.
	* Configure it at the *AndroidManifest.xml* file using the attribute *android:targetSdkVersion* at the *<uses-sdk>* node contained in the *<manifest>* root node, or open the file in the *Android Manifest Editor* >> *ManifestExtras* >> *Uses Sdk* extra >> *Target SDK version* attribute.
	* For an Appverse application, the target SDK version should be the API level 11 (Honeycomb); levels upper 8 are not already tested.
* **Display Name**: a user-readable label for the application, that acts as the default label for each of the application's components. It should be set as a reference to the a string resource.
	* Configure it at the *AndroidManifest.xml* file using the attribute *android:label* at the *<application>* node contained in the *<manifest>* root node, or open the file in the *Android Manifest Editor* >> *Application* tab >> *Application Attributes* section >> *Label* attribute.
	* For example: @string/app_name. And then the label text (for example, *AppverseTwitter*) should be added as a *Resource Element* in the file *res/values/strings.xml*. You could open this file using the *Android Common XMl Editor*.
* **Application Icon**: an icon resource for the application, that acts as the default icon for each of the application's components. It should be set as a reference to the a drawable resource.
	* Configure it at the *AndroidManifest.xml* file using the attribute *android:icon* at the *<application>* node contained in the *<manifest>* root node, or open the file in the *Android Manifest Editor* >> *Application* tab >> *Application Attributes* section >> *Icon* attribute.
	* For example: @drawable/icon. And then you should place the proper dimension png files at the following folders (the "icon" name could be changed as desired, but you should properly reference it on the *@drawable* link):
		*  the *icon.png* 72x72 pixels image at the *res/drawable-hdpi* folder.
		*  the *icon.png* 36x36 pixels image at the *res/drawable-ldpi* folder.
		*  the *icon.png* 48x48 pixels image at the *res/drawable-mdpi* folder.
		*  the *icon.png* 96x96 pixels image at the *res/drawable-xhdpi* folder.
* **Application Splashscreen**: this is a custom feature for the *Appverse applications*. You could configure 4 splash screen images (for the corresponding portrait and lanscape modes in smartphones and tablet devices). Just place them with the appropiate file name and dimensions:
	*  the *launch_portrait.png* 640x960 pixels image at the *res/drawable* folder; for the smartphone portrait launch mode.
	*  the *launch_lanscape.png* 1024x748 pixels image at the *res/drawable* folder; for the smartphone landscape launch mode.
	*  the *launch_portrait_tablet.png* 800x1232 pixels image at the *res/drawable* folder; for the tablet portrait launch mode.
	*  the *launch_lansacape_tablet.png* 1280x752 pixels image at the *res/drawable* folder; for the tablet landscape launch mode.


### NetBeans Android Application

Before you start with your first Android app as a NetBeans project, you will need to check that you have your development environment set up.
You will need to install the following:

1. Download the Android SDK: [http://developer.android.com/sdk/index.html]
2. Install the NBAndroid Plugin for NetBeans (optional). See instructions here: [http://kenai.com/projects/nbandroid/pages/Install]
3. Download the latest SDK Tools and platforms using the SDK Manager.

**System requirements**:

*Operating Systems*
* Windows XP (32-bit), Vista (32- or 64-bit), or Windows 7 (32- or 64-bit)
* Mac OS X 10.5.8 or later (x86 only)
* Linux (tested on Ubuntu Linux, Lucid Lynx)
	* GNU C Library (glibc) 2.7 or later is required.
	* On Ubuntu Linux, version 8.04 or later is required.
	* 64-bit distributions must be capable of running 32-bit applications.

*NetBeans IDE*
* JDK 6 (JRE alone is not sufficient)
* Apache Ant 1.8 or later
* Not compatible with Gnu Compiler for Java (gcj)


## License

    Copyright (c) 2012 GFT Appverse, S.L., Sociedad Unipersonal.

     This Source  Code Form  is subject to the  terms of  the Appverse Public License 
     Version 2.0  ("APL v2.0").  If a copy of  the APL  was not  distributed with this 
     file, You can obtain one at <http://appverse.org/legal/appverse-license/>.

     Redistribution and use in  source and binary forms, with or without modification, 
     are permitted provided that the  conditions  of the  AppVerse Public License v2.0 
     are met.

     THIS SOFTWARE IS PROVIDED BY THE  COPYRIGHT HOLDERS  AND CONTRIBUTORS "AS IS" AND
     ANY EXPRESS  OR IMPLIED WARRANTIES, INCLUDING, BUT  NOT LIMITED TO,   THE IMPLIED
     WARRANTIES   OF  MERCHANTABILITY   AND   FITNESS   FOR A PARTICULAR  PURPOSE  ARE
     DISCLAIMED. EXCEPT IN CASE OF WILLFUL MISCONDUCT OR GROSS NEGLIGENCE, IN NO EVENT
     SHALL THE  COPYRIGHT OWNER  OR  CONTRIBUTORS  BE LIABLE FOR ANY DIRECT, INDIRECT,
     INCIDENTAL,  SPECIAL,   EXEMPLARY,  OR CONSEQUENTIAL DAMAGES  (INCLUDING, BUT NOT
     LIMITED TO,  PROCUREMENT OF SUBSTITUTE  GOODS OR SERVICES;  LOSS OF USE, DATA, OR
     PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY,
     WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT(INCLUDING NEGLIGENCE OR OTHERWISE) 
     ARISING  IN  ANY WAY OUT  OF THE USE  OF THIS  SOFTWARE,  EVEN  IF ADVISED OF THE 
     POSSIBILITY OF SUCH DAMAGE.
 
