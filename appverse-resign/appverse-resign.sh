# Please let us know about any improvements you make to this script!
# ./sign source identity -p "path/to/profile" -e "path/to/entitlements" target
 
if [ $# -lt 3 ]; then
	echo "usage: $0 source identity [-p provisioning] [-e entitlements] target" >&2
	exit 1
fi
 
ORIGINAL_FILE="$1"
CERTIFICATE="$2"
NEW_PROVISION=
ENTITLEMENTS=
 
OPTIND=3
while getopts p:e: opt; do
	case $opt in
		p)
			NEW_PROVISION="$OPTARG"
			echo "Specified provisioning profile: $NEW_PROVISION" >&2
			;;
		e)
			ENTITLEMENTS="$OPTARG"
			echo "Specified signing entitlements: $ENTITLEMENTS" >&2
			;;
		\?)
			echo "Invalid option: -$OPTARG" >&2
			exit 1
			;;
		:)
			echo "Option -$OPTARG requires an argument." >&2
			exit 1
			;;
	esac
done
 
shift $((OPTIND-1))
 
NEW_FILE="$1"
 
# Check if the supplied file is an ipa or an app file
if [ "${ORIGINAL_FILE#*.}" = "ipa" ]
	then
		# Unzip the old ipa quietly
		unzip -q "$ORIGINAL_FILE" -d temp
elif [ "${ORIGINAL_FILE#*.}" = "app" ]
	then
		# Copy the app file into an ipa-like structure
		mkdir -p "temp/Payload"
		cp -Rf "$ORIGINAL_FILE" "temp/Payload/$ORIGINAL_FILE"
else
	echo "Error: Only can resign .app files and .ipa files." >&2
	exit
fi
 
# Set the app name
# The app name is the only file within the Payload directory
APP_NAME=$(ls temp/Payload/)
echo "APP_NAME=$APP_NAME" >&2
 
# Replace the embedded mobile provisioning profile
if [ "$NEW_PROVISION" != "" ]; then
	echo "Adding the new provision: $NEW_PROVISION"
	cp "$NEW_PROVISION" "temp/Payload/$APP_NAME/embedded.mobileprovision"
fi
 
# Resign the application
echo "Resigning application using certificate: $CERTIFICATE" >&2
if [ "$ENTITLEMENTS" != "" ]; then
	echo "Using Entitlements: $ENTITLEMENTS" >&2
	/usr/bin/codesign -f -s "$CERTIFICATE" --entitlements="$ENTITLEMENTS" "temp/Payload/$APP_NAME"
else
	/usr/bin/codesign -f -s "$CERTIFICATE" "temp/Payload/$APP_NAME"
fi
 
# Repackage quietly
echo "Repackaging as $NEW_FILE"
 
# Zip up the contents of the temp folder
# Navigate to the temporary directory (sending the output to null)
# Zip all the contents, saving the zip file in the above directory
# Navigate back to the orignating directory (sending the output to null)
pushd temp > /dev/null
zip -qry ../temp.ipa *
popd > /dev/null
 
# Move the resulting ipa to the target destination
mv temp.ipa "$NEW_FILE"
 
# Remove the temp directory
rm -rf "temp"
