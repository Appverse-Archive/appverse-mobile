/*
 Copyright (c) 2012 GFT Appverse, S.L., Sociedad Unipersonal.

 This Source  Code Form  is subject to the  terms of  the Appverse Public License 
 Version 2.0  (“APL v2.0”).  If a copy of  the APL  was not  distributed with this 
 file, You can obtain one at http://appverse.org/legal/appverse-license/.

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
 */
package com.gft.unity.core.media.camera;

/**
 *
 * @author maps
 */
public class CameraOptions {

    private float imageScaleFactor = 1;
    private boolean useFrontCamera = false;
    private boolean useCustomCameraOverlay = false;
    
    
    private String overlay;
    
    // guidelines customization
    private int guidelinesMargins;
    private String guidelinesColorHexadecimal;

    // scan button customization
    private String scanButtonColorHexadecimal;
    private String scanButtonPressedColorHexadecimal;
    private int scanButtonWidth;
    private int scanButtonHeight;
    private int scanButtonMarginBottom;
    private String scanButtonIconColorHexadecimal;
    private int scanButtonIconWidth;
    private int scanButtonIconHeight;

    // description label
    private String descriptionLabelText;
    private String descriptionLabelColorHexadecimal;
    private int descriptionLabelMarginBottom;
    private int descriptionLabelMarginLeftRight;
    private int descriptionLabelHeight;
    public String descriptionLabelFontFamilyName;
    public int descriptionLabelFontSize;

    // cancel button
    private String cancelButtonText;
    private String cancelButtonColorHexadecimal;
    private int cancelButtonWidth;
    private int cancelButtonHeight;
    public String cancelButtonFontFamilyName;
    public int cancelButtonFontSize;
    
    public CameraOptions() {
        imageScaleFactor = 1;	 // default scale factor is 1
        useFrontCamera = false;  // default is to use back camera
        useCustomCameraOverlay = false;  // default is to use native camera overlay
        guidelinesMargins = 10;  // default is 10 pixels margin

        guidelinesColorHexadecimal = "#999999";
        scanButtonColorHexadecimal = "#999999";
        scanButtonPressedColorHexadecimal = "#666666";

        scanButtonMarginBottom = 30; // default is 30 pixels margin-bottom

        scanButtonWidth = 75;
        scanButtonHeight = 35;

        scanButtonIconWidth = 28; 
        scanButtonIconHeight = 24;
        scanButtonIconColorHexadecimal = "#ffffff";  // white icon by default

        descriptionLabelText = "Place here your description";  // null to avoid description being shown
        descriptionLabelColorHexadecimal = "#ffffff"; 
        descriptionLabelMarginBottom = 60;
        descriptionLabelMarginLeftRight = 25;
        descriptionLabelHeight = 60;
        descriptionLabelFontFamilyName = "Helvetica"; 
        descriptionLabelFontSize = 20;

        cancelButtonText = "Cancel";
        cancelButtonColorHexadecimal = "#ffffff"; 
        cancelButtonWidth = 80;
        cancelButtonHeight = 35;
        cancelButtonFontFamilyName = "Helvetica";  // each platform should define its own default font (among the ones available)
        cancelButtonFontSize = 20;
    }

    public float getImageScaleFactor() {
        return imageScaleFactor;
    }

    public void setImageScaleFactor(float imageScaleFactor) {
        this.imageScaleFactor = imageScaleFactor;
    }

    public boolean getUseFrontCamera() {
        return useFrontCamera;
    }

    public void setUseFrontCamera(boolean useFrontCamera) {
        this.useFrontCamera = useFrontCamera;
    }

    public boolean getUseCustomCameraOverlay() {
        return useCustomCameraOverlay;
    }

    public void setUseCustomCameraOverlay(boolean useFrame) {
        this.useCustomCameraOverlay = useFrame;
    }
    
    public String getOverlay() {
		return overlay;
	}

	public void setOverlay(String overlay) {
		this.overlay = overlay;
	}

	public int getGuidelinesMargins() {
        return guidelinesMargins;
    }

    public void setGuidelinesMargins(int guidelinesMargins) {
        this.guidelinesMargins = guidelinesMargins;
    }

    public String getGuidelinesColorHexadecimal() {
        return guidelinesColorHexadecimal;
    }

    public void setGuidelinesColorHexadecimal(String guidelinesColorHexadecimal) {
        this.guidelinesColorHexadecimal = guidelinesColorHexadecimal;
    }

    public String getScanButtonColorHexadecimal() {
        return scanButtonColorHexadecimal;
    }

    public void setScanButtonColorHexadecimal(String scanButtonColorHexadecimal) {
        this.scanButtonColorHexadecimal = scanButtonColorHexadecimal;
    }

    public String getScanButtonPressedColorHexadecimal() {
        return scanButtonPressedColorHexadecimal;
    }

    public void setScanButtonPressedColorHexadecimal(String scanButtonPressedColorHexadecimal) {
        this.scanButtonPressedColorHexadecimal = scanButtonPressedColorHexadecimal;
    }

    public int getScanButtonWidth() {
        return scanButtonWidth;
    }

    public void setScanButtonWidth(int scanButtonWidth) {
        this.scanButtonWidth = scanButtonWidth;
    }

    public int getScanButtonHeight() {
        return scanButtonHeight;
    }

    public void setScanButtonHeight(int scanButtonHeight) {
        this.scanButtonHeight = scanButtonHeight;
    }

    public int getScanButtonMarginBottom() {
        return scanButtonMarginBottom;
    }

    public void setScanButtonMarginBottom(int scanButtonMarginBottom) {
        this.scanButtonMarginBottom = scanButtonMarginBottom;
    }

    public String getScanButtonIconColorHexadecimal() {
        return scanButtonIconColorHexadecimal;
    }

    public void setScanButtonIconColorHexadecimal(String scanButtonIconColorHexadecimal) {
        this.scanButtonIconColorHexadecimal = scanButtonIconColorHexadecimal;
    }

    public int getScanButtonIconWidth() {
        return scanButtonIconWidth;
    }

    public void setScanButtonIconWidth(int scanButtonIconWidth) {
        this.scanButtonIconWidth = scanButtonIconWidth;
    }

    public int getScanButtonIconHeight() {
        return scanButtonIconHeight;
    }

    public void setScanButtonIconHeight(int scanButtonIconHeight) {
        this.scanButtonIconHeight = scanButtonIconHeight;
    }

    public String getDescriptionLabelText() {
        return descriptionLabelText;
    }

    public void setDescriptionLabelText(String descriptionLabelText) {
        this.descriptionLabelText = descriptionLabelText;
    }

    public String getDescriptionLabelColorHexadecimal() {
        return descriptionLabelColorHexadecimal;
    }

    public void setDescriptionLabelColorHexadecimal(String descriptionLabelColorHexadecimal) {
        this.descriptionLabelColorHexadecimal = descriptionLabelColorHexadecimal;
    }

    public int getDescriptionLabelMarginBottom() {
        return descriptionLabelMarginBottom;
    }

    public void setDescriptionLabelMarginBottom(int descriptionLabelMarginBottom) {
        this.descriptionLabelMarginBottom = descriptionLabelMarginBottom;
    }

    public int getDescriptionLabelMarginLeftRight() {
        return descriptionLabelMarginLeftRight;
    }

    public void setDescriptionLabelMarginLeftRight(int descriptionLabelMarginLeftRight) {
        this.descriptionLabelMarginLeftRight = descriptionLabelMarginLeftRight;
    }

    public int getDescriptionLabelHeight() {
        return descriptionLabelHeight;
    }

    public void setDescriptionLabelHeight(int descriptionLabelHeight) {
        this.descriptionLabelHeight = descriptionLabelHeight;
    }

    public String getCancelButtonText() {
        return cancelButtonText;
    }

    public void setCancelButtonText(String cancelButtonText) {
        this.cancelButtonText = cancelButtonText;
    }

    public String getCancelButtonColorHexadecimal() {
        return cancelButtonColorHexadecimal;
    }

    public void setCancelButtonColorHexadecimal(String cancelButtonColorHexadecimal) {
        this.cancelButtonColorHexadecimal = cancelButtonColorHexadecimal;
    }

    public int getCancelButtonWidth() {
        return cancelButtonWidth;
    }

    public void setCancelButtonWidth(int cancelButtonWidth) {
        this.cancelButtonWidth = cancelButtonWidth;
    }

    public int getCancelButtonHeight() {
        return cancelButtonHeight;
    }

    public void setCancelButtonHeight(int cancelButtonHeight) {
        this.cancelButtonHeight = cancelButtonHeight;
    }

    public String getDescriptionLabelFontFamilyName() {
        return descriptionLabelFontFamilyName;
    }

    public void setDescriptionLabelFontFamilyName(String DescriptionLabelFontFamilyName) {
        this.descriptionLabelFontFamilyName = DescriptionLabelFontFamilyName;
    }

    public int getDescriptionLabelFontSize() {
        return descriptionLabelFontSize;
    }

    public void setDescriptionLabelFontSize(int DescriptionLabelFontSize) {
        this.descriptionLabelFontSize = DescriptionLabelFontSize;
    }

    public String getCancelButtonFontFamilyName() {
        return cancelButtonFontFamilyName;
    }

    public void setCancelButtonFontFamilyName(String cancelButtonFontFamilyName) {
        this.cancelButtonFontFamilyName = cancelButtonFontFamilyName;
    }

    public int getCancelButtonFontSize() {
        return cancelButtonFontSize;
    }

    public void setCancelButtonFontSize(int cancelButtonFontSize) {
        this.cancelButtonFontSize = cancelButtonFontSize;
    }
    
    
    
    
}
