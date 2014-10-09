# Appverse Mobile Security

Appverse Mobile Framework has vastly improved its security in 4.7 version and above.

Previously you can check in runtime if you were running in a Jailbroken/Rooted device (Appverse.Security.IsDeviceModified), this method has been improved by detecting now even bypass methods like Snoop-It or RootCloak.

Moreover now you can even forbid the use of your app if the device is Jalbroken/Rooted, preventing all the traffic could be ever done and  thus protecting all sensitive data. In order to do that you have to:

> **Android**: Add token to the \res\values\strings.xml in your Android project: 
```
<string name="Appverse_BlockRooted">true</string>
```


> **iOS**: In the Info.plist change the value of your Appverse_BlockRooted key:
```
<key>Appverse_BlockRooted</key>
	<true/> 
```

In addition the framework is protected against Unintended Data Leakage, Malintentioned Path Traversal, Port Binding or Javascript Injection. 

## More Information

* **[About this project](README.md)**
* **About licenses & groups**: <http://appverse.github.com>
* **About The Appverse Project**: <http://appverse.org>

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
