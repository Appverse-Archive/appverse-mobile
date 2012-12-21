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
package com.gft.unity.core.geo;

import com.gft.unity.core.FormatUtils;

public class LocationCoordinate {

    private double xCoordinate;
    private double yCoordinate;
    private double zCoordinate;
    private float XDoP;
    private float YDoP;

    public LocationCoordinate() {
    }

    public float getXDoP() {
        return XDoP;
    }

    public void setXDoP(float XDoP) {
        this.XDoP = XDoP;
    }

    public float getYDoP() {
        return YDoP;
    }

    public void setYDoP(float YDoP) {
        this.YDoP = YDoP;
    }

    public double getXCoordinate() {
        return xCoordinate;
    }

    public void setXCoordinate(double xCoordinate) {
        this.xCoordinate = xCoordinate;
    }

    public double getYCoordinate() {
        return yCoordinate;
    }

    public void setYCoordinate(double yCoordinate) {
        this.yCoordinate = yCoordinate;
    }

    public double getZCoordinate() {
        return zCoordinate;
    }

    public void setZCoordinate(double zCoordinate) {
        this.zCoordinate = zCoordinate;
    }

    public String getXDoPString() {
        return FormatUtils.formatDecimalNumber(this.XDoP);
    }

    public String getYDoPString() {
        return FormatUtils.formatDecimalNumber(this.YDoP);
    }

    public String getXCoordinateString() {
        return FormatUtils.formatDecimalNumber(this.xCoordinate);
    }

    public String getYCoordinateString() {
        return FormatUtils.formatDecimalNumber(this.yCoordinate);
    }

    public String getZCoordinateString() {
        return FormatUtils.formatDecimalNumber(this.zCoordinate);
    }

    @Override
    public String toString() {
        StringBuilder builder = new StringBuilder();
        builder.append("LocationCoordinate [xCoordinate=");
        builder.append(xCoordinate);
        builder.append(", yCoordinate=");
        builder.append(yCoordinate);
        builder.append(", zCoordinate=");
        builder.append(zCoordinate);
        builder.append(", XDoP=");
        builder.append(XDoP);
        builder.append(", YDoP=");
        builder.append(YDoP);
        builder.append("]");
        return builder.toString();
    }
}
