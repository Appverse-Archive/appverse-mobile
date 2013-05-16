# Appverse Re-Sign Utility

## Introduction

The following script allows you to re-sign an iOS IPA file that has been previously signed for adhoc/development distribution. 

### Requirements

Ensure that your environment has been configured to allow you to use codesign from command line in your OSX environment. You will require the following:

- Mac OS 10.5+
- The distribution certificate issued by Apple.
- Apple _applewwcdr_ intermediate certificate must be trusted.
- Xcode *latest* version for your OS (please, update if needed).
- A provisioning file for your distribution (from Apple Developer Portal)
- An entitlements.plist file for distribution.
- The appverse-resign.sh script.

Apple's *codesign* tool needs *codesign_allocate* (contained within Xcode itself) in order to perform the re-signing process and validating the result correctly. To ensure this is done, execute the following commands:

```
sudo mv /usr/bin/codesign_allocate /usr/bin/codesign_allocate_old
sudo ln -s /Applications/Xcode.app/Contents/Developer/usr/bin/codesign_allocate /usr/bin
```

Executing */usr/bin/codesign_allocate* should report:

```
Usage: /usr/bin/codesign_allocate -i input [-a <arch> <size>]... [-A <cputype> <cpusubtype> <size>]... -o output
```

Once this is in place, you should be able to re-sign applications with the appverse-resign tool without any issues.


## Re-signing




### Re-signing the IPA

Execute the script with the following parameters:

```
./appverse-resign.sh <IPA_SOURCE> "<CERTIFICATE_NAME>" -p <MOBILE_PROVISIONING_FILE> -e <ENTITLEMENTS_FILE> <IPA_DESTINATION>
```
Below is an example for the application "CODE_n", where we have a **dev**elopment version and we want to create the **dist**ribution version:

```
./appverse-resign.sh CODE_n_dev.ipa "iPhone Distribution: My Company Signature Ltd" -p CODE_n_dist.mobileprovision -e entitlements-dist.plist CODE_n_dist.ipa
```

**NOTE**: Remember to **chmod +x appverse-resign.sh** to make the script executable.

### Example entitlements.plist file

You should change the entitlements.plist file with your seed and bundle indentifier. Below is an example with our identifier:

```
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
<dict>
        <key>get-task-allow</key>
        <false/>
        <key>application-identifier</key>
        <string>HAAF68S9SE.com.adaptiveware.coden</string>
</dict>
</plist>

```
