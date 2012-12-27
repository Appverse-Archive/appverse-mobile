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

* <b>How to embedded your Web resources</b>
	* In order to add the web resources for an application, you first need to add them to the project in MonoDevelop. To accomplish this perform the following steps:
		* Copy your web resources into the project's location, under the following folder structure: "WebResources">"www"
		* Right-click on the project folder (or Ctrl-Click) and select *Add* > *Add Exiting folder...*, displaying the *Add folder* dialog window.
		* Select the "WebResources" folder and then select all the needed resources in the path.
		* Mark all resources as <b>Content</b> in the <b>Build Action</b> selector at the *Add folder* dialog window.
		* Once included, if new resources are added to the project don't forget to mark them as *Content*. Right-click on each field to see its *Properties*.
	* If your application also needs some custom API configuration, repeat the previous steps with the application config files ("app" folder). See the examples for further information.

* <b>General Settings</b>
 * <b>*Bundle Name*</b>: the short display name of the bundle (the name of the final binary application).
		* Configure it at *Info.plist* file using the property *CFBundleName*, or in the *Project Options*>*IPhone Application* settings > *Advanced* tab > *Bundle Name* property.

 * <b>*Bundle Display Name*</b>: the actual name of the bundle (the display name of the application). 
		* Configure it at *Info.plist* file using the property *CFBundleDisplayName*, or in the *Project Options* > IPhone Application* settings > *Summary* tab > *Application name* property.
	
 * <b>*Bundle Identifier*</b>: an identifier string that uniquely identifies the bundle. Each distinct app or bundle on the system must have a unique bundle ID. The system uses this string to identify your app in many ways. The string should be in reverse DNS format using only the Roman alphabet in upper and lower case (A-Z, a-z), the dot (.), and the hyphen (-).
		* Configure it at *Info.plist* file using the property *CFBundleIdentifier*, or in the *Project Options* > IPhone Application* settings > *Summary* tab > *Identifier* property.
	
 * <b>*Bundle Version*</b>: specifies the build version number of the bundle, which identifies an iteration (released or unreleased) of the bundle. The build version number should be a string comprised of three non-negative, period-separated integers with the first integer being greater than zero. The string should only contain numeric (0-9) and period (.) characters. Leading zeros are truncated from each integer and will be ignored (that is, 1.02.3 is equavalent to 1.2.3).
		* Configure it at *Info.plist* file using the property *CFBundleVersion*, or in the *Project Options* > *IPhone Application* settings > *Summary* tab > *Version* property.

 * <b>*Bundle Executable*</b>: the name of the bundle's executable file.
		* Configure it at *Info.plist* file using the property *CFBundleExecutable*, or in the *Project Options* > *IPhone Application* settings > *Advanced* tab > *Executable File* property.
	
 * <b>*Minimun OS Version*</b>: indicates the minimum version of OS X required for this app to run in a device. This string must be of the form n.n.n where n is a number. The first number is the major version number of the system. The second and third numbers are minor revision numbers.
		* Configure it at *Info.plist* file using the property *MinimumOSVersion*, or in the *Project Options* > *IPhone Application* settings > *Summary* tab > *Deployment Target* property.

		* Default *Appverse* value is 4.3
	
 * <b>*Targeted Device Family*</b>: an array that specifies the underlying hardware types on which this app is designed to run.
		* Configure it at *Info.plist* file using the property *UIDeviceFamily*, or in the *Project Options* > *IPhone Application* settings > *Advanced* tab > *Targeted Device Family* array property.

	 	* Default *Appverse* value is: iPhone/iPod touch + iPad. In the *Summary* tab you should select the *Universal* option on the *Devices* selection list.
	
 * <b>*Supported Device Orientations*</b>: an array that specifies the orientations that the app supports. The system uses this information (along with the current device orientation) to choose the initial orientation in which to launch your app. The value for this key is an array of strings: *UIInterfaceOrientationPortrait*, *UIInterfaceOrientationPortraitUpsideDown*, *UIInterfaceOrientationLandscapeLeft* and *UIInterfaceOrientationLandscapeRight*.
		* Configure it at *Info.plist* file using the property *UISupportedInterfaceOrientations*, or in the *Project Options* > *IPhone Application* settings > *Summary* tab > *iPhone / iPod Deployment Info* section > *Supported Device Orientations* selectable graphical buttons.

		* Default *Appverse* value for a iPhone and iPod thouch is just the *UIInterfaceOrientationPortrait* orientation.

 * <b>*Supported Device Orientations for iPad*</b>: an array that specifies the orientations that the app supports in the iPad devices. The system uses this information (along with the current device orientation) to choose the initial orientation in which to launch your app. The value for this key is an array of strings: *UIInterfaceOrientationPortrait*, *UIInterfaceOrientationPortraitUpsideDown*, *UIInterfaceOrientationLandscapeLeft* and *UIInterfaceOrientationLandscapeRight*.
		* Configure it at *Info.plist* file using the property *UISupportedInterfaceOrientations~ipad*, or in the *Project Options* > *IPhone Application* settings > *Summary* tab > *iPad Deployment Info* section > *Supported Device Orientations* selectable graphical buttons.
			 
		* Default *Appverse* value for a iPad is: *UIInterfaceOrientationPortrait*, *UIInterfaceOrientationLandscapeLeft* and *UIInterfaceOrientationLandscapeRight* orientations.

 * <b>*Application icons*</b>: an array identifying the icon files for the bundle. It is recommended that you always create icon files using the PNG format.
		* Include them at *Info.plist* file using the property *CFBundleIconFiles*, or in the *Project Options* > *IPhone Application* settings > *Summary* tab > *Universal Icons* section > *App Icons* selectable graphical buttons.

		* iPhone icon: the main icon for applications running on iPhone; 57x57 pixels.

		* iPhone Retina icon: the high resolution version of the main icon for applications running on iPhone; 114x114 pixels.

	 	*iPad icon: the main icon for applications running on iPad; 72x72 pixels.

		* iPad Retina icon: the high resolution version of the main icon for applications running on iPad; 144x144 pixels.

 * <b>*Prerendered Icon*</b>: specifies whether the app's icon already includes a shine effect. Select "true" if you don't want that the iOS applies a shine effect to the app icon.
		* Configure it at *Info.plist* file using the property *UIPrerenderedIcon*, or in the *Project Options* > *IPhone Application* settings > *Summary* tab > *Universal Icons* section > *App Icons* > *Prerendered* checkbox.

		* Default value is:true.

 * <b>*Spotlight & Settings icons*</b>: an array identifying the icon files for the bundle. It is recommended that you always create icon files using the PNG format.
		* Include them at *Info.plist* file using the property *CFBundleIconFiles*, or in the *Project Options* > *IPhone Application* settings > *Summary* tab > *Universal Icons* section > *Spotlight & Settings  Icons* selectable graphical buttons.

		* iPhone Spotlight and iPad Settings: the icon displayed in the Spotlight search results on iPhone and in the Settings application on iPad; 29x29 pixels.

		* iPhone Retina Spotlight and iPad Retina Settings: the high resolution version of the icon displayed in the Spotlight search results on iPhone and in the Settings application on iPad; 58x58 pixels.

		* iPad Spotlight: the icon displayed in the Spotlight search results on iPad; 50x50 pixels.

		* iPad Retina Spotlight: the high resolution version of the icon displayed in the Spotlight search results on iPad; 100x100 pixels.

 * <b>*Launch Images*</b>: the images to be used as the application launch image. The images should be included on the project with the appropriate name and dimensions.
		* Attach them at the *Project Options* > *IPhone Application* settings > *Summary* tab > *Universal Icons* section > *iPhone Launch Images* and *iPad Launch Images*.

		* iPhone: the launch image displayed for applications running on iPhone ( *Default.png* ); 320x480 pixels.

		* iPhone Retina: the high resolution version of the launch image displayed for applications running on iPhone ( *Default@2x.png* ); 640x960 pixels.

		* iPhone 5: the high resolution version of the launch image displayed for applications running on iPhone 5 ( *Default-568h@2x.png* ); 640x1136 pixels.

		* iPad portrait: the launch image displayed for applications running on iPad in portrait mode ( *Default-Portrait.png* ); 768x1004 pixels.

		* iPad landscape: the launch image displayed for applications running on iPad in landscape mode ( *Default-Landscape.png* ); 1024x748 pixels.

		* iPad Retina portrait: the high resolution version of the launch image displayed for applications running on iPad in portrait mode ( *Default-Portrait@2x.png* ); 1536x2008 pixels.

		* iPad Retina landscape: the high resolution version of the launch image displayed for applications running on iPad in landscape mode ( *Default-Landscape@2x.png* ); 2048x1496 pixels.
	
* <b>Signing</b>

	In order to properly sign the application for device deployment, you should provide the signing information at the *Project Options* > *IPhone Bundle Signing* settings:

	* <b>Signing Identity</b>: the name of the Apple certificate that will be used to sign the application.

	* <b>Provisioning profile</b> (optional): the provisioning profile file to be embedded inside the application bundle. This file usually contains information about the allowed devices that could install the application and which certificates could sign it. It is used for *Over the Air* device deployments.

	* Custom <b>entitlements</b> and <b>resource rules</b>.

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
 
