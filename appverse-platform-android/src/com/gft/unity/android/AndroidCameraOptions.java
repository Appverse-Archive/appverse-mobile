package com.gft.unity.android;


import android.os.Parcel;
import android.os.Parcelable;
import android.util.Log;

public class AndroidCameraOptions extends com.gft.unity.core.media.camera.CameraOptions implements Parcelable {

	@Override
	public int describeContents() {
		// TODO Auto-generated method stub
		return 0;
	}

	@Override
	public void writeToParcel(Parcel dest, int flags) {
		dest.writeStringArray(new String[] {String.valueOf(this.getImageScaleFactor()),
				String.valueOf(this.getUseFrontCamera()),
				String.valueOf(this.getUseCustomCameraOverlay()),
				String.valueOf(this.getGuidelinesMargins()),
				String.valueOf(this.getGuidelinesColorHexadecimal()),
				this.getScanButtonColorHexadecimal(),
				this.getScanButtonPressedColorHexadecimal(),
				String.valueOf(this.getScanButtonWidth()),
				String.valueOf(this.getScanButtonHeight()),
				String.valueOf(this.getScanButtonMarginBottom()),
				this.getScanButtonIconColorHexadecimal(),
				String.valueOf(this.getScanButtonIconWidth()),
				String.valueOf(this.getScanButtonIconHeight()),
				this.getDescriptionLabelText(),
				this.getDescriptionLabelColorHexadecimal(),
				String.valueOf(this.getDescriptionLabelMarginBottom()),
				String.valueOf(this.getDescriptionLabelMarginLeftRight()),
				String.valueOf(this.getDescriptionLabelHeight()),
				this.getDescriptionLabelFontFamilyName(),
				String.valueOf(this.getDescriptionLabelFontSize()),
				this.getCancelButtonText(),
				this.getCancelButtonColorHexadecimal(),
				String.valueOf(this.getCancelButtonWidth()),
				String.valueOf(this.getCancelButtonHeight()),
				this.getCancelButtonFontFamilyName(),
				String.valueOf(this.getCancelButtonFontSize())
                });
		
	}
	
	public static final Parcelable.Creator CREATOR = new Parcelable.Creator() {
        public AndroidCameraOptions createFromParcel(Parcel in) {
            return new AndroidCameraOptions(in); 
        }

        public AndroidCameraOptions[] newArray(int size) {
            return new AndroidCameraOptions[size];
        }
    };
    
    public AndroidCameraOptions(Parcel in){
		String[] data = new String[26];
		int i = 0;
	    try {
			in.readStringArray(data);
			Log.d("AndroidCameraOptions", "[float] setImageScaleFactor index: "+i+" data: "+data[i]);
			this.setImageScaleFactor(Float.valueOf(data[i++]));
			Log.d("AndroidCameraOptions", "[boolean] setUseFrontCamera index: "+i+" data: "+data[i]);
			this.setUseFrontCamera(Boolean.parseBoolean(data[i++]));
			Log.d("AndroidCameraOptions", "[boolean] setUseCustomCameraOverlay index: "+i+" data: "+data[i]);
			this.setUseCustomCameraOverlay(Boolean.parseBoolean(data[i++]));
			Log.d("AndroidCameraOptions", "[int] setGuidelinesMargins index: "+i+" data: "+data[i]);
			this.setGuidelinesMargins(Integer.valueOf(data[i++]));
			Log.d("AndroidCameraOptions", "setGuidelinesColorHexadecimal index: "+i+" data: "+data[i]);
			this.setGuidelinesColorHexadecimal(data[i++]);
			Log.d("AndroidCameraOptions", "setScanButtonColorHexadecimal index: "+i+" data: "+data[i]);
			this.setScanButtonColorHexadecimal(data[i++]);
			Log.d("AndroidCameraOptions", "setScanButtonPressedColorHexadecimal index: "+i+" data: "+data[i]);
			this.setScanButtonPressedColorHexadecimal(data[i++]);
			Log.d("AndroidCameraOptions", "[int] setScanButtonWidth index: "+i+" data: "+data[i]);
			this.setScanButtonWidth(Integer.valueOf(data[i++]));
			Log.d("AndroidCameraOptions", "[int] setScanButtonHeight index: "+i+" data: "+data[i]);
			this.setScanButtonHeight(Integer.valueOf(data[i++]));
			Log.d("AndroidCameraOptions", "[int] setScanButtonMarginBottom index: "+i+" data: "+data[i]);
			this.setScanButtonMarginBottom(Integer.valueOf(data[i++]));
			Log.d("AndroidCameraOptions", "setScanButtonIconColorHexadecimal index: "+i+" data: "+data[i]);
			this.setScanButtonIconColorHexadecimal(data[i++]);
			Log.d("AndroidCameraOptions", "[int] setScanButtonIconWidth index: "+i+" data: "+data[i]);
			this.setScanButtonIconWidth(Integer.valueOf(data[i++]));
			Log.d("AndroidCameraOptions", "[int] setScanButtonIconHeight index: "+i+" data: "+data[i]);
			this.setScanButtonIconHeight(Integer.valueOf(data[i++]));
			Log.d("AndroidCameraOptions", "setDescriptionLabelText index: "+i+" data: "+data[i]);
			this.setDescriptionLabelText(data[i++]);
			Log.d("AndroidCameraOptions", "setDescriptionLabelColorHexadecimal index: "+i+" data: "+data[i]);
			this.setDescriptionLabelColorHexadecimal(data[i++]);
			Log.d("AndroidCameraOptions", "[int] setDescriptionLabelMarginBottom index: "+i+" data: "+data[i]);
			this.setDescriptionLabelMarginBottom(Integer.valueOf(data[i++]));
			Log.d("AndroidCameraOptions", "[int] setDescriptionLabelMarginLeftRight index: "+i+" data: "+data[i]);
			this.setDescriptionLabelMarginLeftRight(Integer.valueOf(data[i++]));
			Log.d("AndroidCameraOptions", "[int] setDescriptionLabelHeight index: "+i+" data: "+data[i]);
			this.setDescriptionLabelHeight(Integer.valueOf(data[i++]));
			Log.d("AndroidCameraOptions", "setDescriptionLabelFontFamilyName index: "+i+" data: "+data[i]);			
			this.setDescriptionLabelFontFamilyName(data[i++]);
			Log.d("AndroidCameraOptions", "[int] setDescriptionLabelFontSize index: "+i+" data: "+data[i]);
			this.setDescriptionLabelFontSize(Integer.valueOf(data[i++]));
			Log.d("AndroidCameraOptions", "[int] setCancelButtonText index: "+i+" data: "+data[i]);
			this.setCancelButtonText(data[i++]);
			Log.d("AndroidCameraOptions", "setCancelButtonColorHexadecimal index: "+i+" data: "+data[i]);
			this.setCancelButtonColorHexadecimal(data[i++]);
			Log.d("AndroidCameraOptions", "[int] setCancelButtonWidth index: "+i+" data: "+data[i]);
			this.setCancelButtonWidth(Integer.valueOf(data[i++]));
			Log.d("AndroidCameraOptions", "[int] setCancelButtonHeight index: "+i+" data: "+data[i]);
			this.setCancelButtonHeight(Integer.valueOf(data[i++]));
			Log.d("AndroidCameraOptions", "[int] setCancelButtonFontFamilyName index: "+i+" data: "+data[i]);						
			this.setCancelButtonFontFamilyName(data[i++]);
			Log.d("AndroidCameraOptions", "[int] setCancelButtonFontSize index: "+i+" data: "+data[i]);
			this.setCancelButtonFontSize(Integer.valueOf(data[i++]));
		} catch (Exception e) {
			// TODO Auto-generated catch block
			Log.e("AndroidCameraOptions", "Fail constructor Parcel in index: "+i+" data: "+data[i],e);
		}	
	};
    
    public AndroidCameraOptions(){
    	super();
    };
    
    public AndroidCameraOptions(com.gft.unity.core.media.camera.CameraOptions options){
    	super();
    	
    	try {
			this.setImageScaleFactor(options.getImageScaleFactor());
			this.setUseFrontCamera(options.getUseFrontCamera());
			this.setUseCustomCameraOverlay(options.getUseCustomCameraOverlay());
			this.setGuidelinesMargins(options.getGuidelinesMargins());
			this.setGuidelinesColorHexadecimal(options.getGuidelinesColorHexadecimal());
			this.setScanButtonColorHexadecimal(options.getScanButtonColorHexadecimal());
			this.setScanButtonPressedColorHexadecimal(options.getScanButtonPressedColorHexadecimal());
			this.setScanButtonWidth(options.getScanButtonWidth());
			this.setScanButtonHeight(options.getScanButtonHeight());
			this.setScanButtonMarginBottom(options.getScanButtonMarginBottom());
			this.setScanButtonIconColorHexadecimal(options.getScanButtonIconColorHexadecimal());
			this.setScanButtonIconWidth(options.getScanButtonIconWidth());
			this.setScanButtonIconHeight(options.getScanButtonIconHeight());
			this.setDescriptionLabelText(options.getDescriptionLabelText());
			this.setDescriptionLabelColorHexadecimal(options.getDescriptionLabelColorHexadecimal());
			this.setDescriptionLabelMarginBottom(options.getDescriptionLabelMarginBottom());
			this.setDescriptionLabelMarginLeftRight(options.getDescriptionLabelMarginLeftRight());
			this.setDescriptionLabelHeight(options.getDescriptionLabelHeight());
			this.setDescriptionLabelFontFamilyName(options.getDescriptionLabelFontFamilyName());
			this.setDescriptionLabelFontSize(options.getDescriptionLabelFontSize());
			this.setCancelButtonText(options.getCancelButtonText());
			this.setCancelButtonColorHexadecimal(options.getCancelButtonColorHexadecimal());
			this.setCancelButtonWidth(options.getCancelButtonWidth());
			this.setCancelButtonHeight(options.getCancelButtonHeight());
			this.setCancelButtonFontFamilyName(options.getCancelButtonFontFamilyName());
			this.setCancelButtonFontSize(options.getCancelButtonFontSize());
		} catch (Exception e) {
			// TODO Auto-generated catch block
			Log.e("AndroidCameraOptions", "Fail constructor CameraOptions",e);
		}
    }

	@Override
	public String toString() {
		// TODO Auto-generated method stub
		StringBuilder builder = new StringBuilder();
        builder.append("getImageScaleFactor: "+String.valueOf(this.getImageScaleFactor())+", ");
		builder.append("getUseFrontCamera: "+String.valueOf(this.getUseFrontCamera())+", ");
		builder.append("getUseCustomCameraOverlay: "+String.valueOf(this.getUseCustomCameraOverlay())+", ");
		builder.append("getGuidelinesMargins: "+String.valueOf(this.getGuidelinesMargins())+", ");
		builder.append("getGuidelinesColorHexadecimal: "+String.valueOf(this.getGuidelinesColorHexadecimal())+", ");
		builder.append("getScanButtonColorHexadecimal: "+this.getScanButtonColorHexadecimal()+", ");
		builder.append("getScanButtonPressedColorHexadecimal: "+this.getScanButtonPressedColorHexadecimal()+", ");
		builder.append("getScanButtonWidth: "+String.valueOf(this.getScanButtonWidth())+", ");
		builder.append("getScanButtonHeight: "+String.valueOf(this.getScanButtonHeight())+", ");
		builder.append("getScanButtonMarginBottom: "+String.valueOf(this.getScanButtonMarginBottom())+", ");
		builder.append("getScanButtonIconColorHexadecimal: "+this.getScanButtonIconColorHexadecimal()+", ");
		builder.append("getScanButtonIconWidth: "+String.valueOf(this.getScanButtonIconWidth())+", ");
		builder.append("getScanButtonIconHeight: "+String.valueOf(this.getScanButtonIconHeight())+", ");
		builder.append("getDescriptionLabelText: "+this.getDescriptionLabelText()+", ");
		builder.append("getDescriptionLabelColorHexadecimal: "+this.getDescriptionLabelColorHexadecimal()+", ");
		builder.append("getDescriptionLabelMarginBottom: "+String.valueOf(this.getDescriptionLabelMarginBottom())+", ");
		builder.append("getDescriptionLabelMarginLeftRight: "+String.valueOf(this.getDescriptionLabelMarginLeftRight())+", ");
		builder.append("getDescriptionLabelHeight: "+String.valueOf(this.getDescriptionLabelHeight())+", ");
		builder.append("getDescriptionLabelFontFamilyName: "+this.getDescriptionLabelFontFamilyName()+", ");
		builder.append("getDescriptionLabelFontSize: "+String.valueOf(this.getDescriptionLabelFontSize())+", ");
		builder.append("getCancelButtonText: "+this.getCancelButtonText()+", ");
		builder.append("getCancelButtonColorHexadecimal: "+this.getCancelButtonColorHexadecimal()+", ");
		builder.append("getCancelButtonWidth: "+String.valueOf(this.getCancelButtonWidth())+", ");
		builder.append("getCancelButtonHeight: "+String.valueOf(this.getCancelButtonHeight())+", ");
		builder.append("getCancelButtonFontFamilyName: "+this.getCancelButtonFontFamilyName()+", ");
		builder.append("getCancelButtonFontSize: "+String.valueOf(this.getCancelButtonFontSize())+", ");
        return builder.toString();
	};
	
    
    

}
