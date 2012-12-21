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

public class GeoDecoderAttributes {

    private String additionalCityLevelInfo;
    private String additionalStreetLevelInfo;
    private String administrativeArea;
    private String country;
    private String countryCode;
    private String locality;
    private String postalCode;
    private String streetAddress;
    private String subAdministrativeArea;

    public GeoDecoderAttributes() {
    }

    public String getAdditionalCityLevelInfo() {
        return additionalCityLevelInfo;
    }

    public void setAdditionalCityLevelInfo(String additionalCityLevelInfo) {
        this.additionalCityLevelInfo = additionalCityLevelInfo;
    }

    public String getAdditionalStreetLevelInfo() {
        return additionalStreetLevelInfo;
    }

    public void setAdditionalStreetLevelInfo(String additionalStreetLevelInfo) {
        this.additionalStreetLevelInfo = additionalStreetLevelInfo;
    }

    public String getAdministrativeArea() {
        return administrativeArea;
    }

    public void setAdministrativeArea(String administrativeArea) {
        this.administrativeArea = administrativeArea;
    }

    public String getCountry() {
        return country;
    }

    public void setCountry(String country) {
        this.country = country;
    }

    public String getCountryCode() {
        return countryCode;
    }

    public void setCountryCode(String countryCode) {
        this.countryCode = countryCode;
    }

    public String getLocality() {
        return locality;
    }

    public void setLocality(String locality) {
        this.locality = locality;
    }

    public String getPostalCode() {
        return postalCode;
    }

    public void setPostalCode(String postalCode) {
        this.postalCode = postalCode;
    }

    public String getStreetAddress() {
        return streetAddress;
    }

    public void setStreetAddress(String streetAddress) {
        this.streetAddress = streetAddress;
    }

    public String getSubAdministrativeArea() {
        return subAdministrativeArea;
    }

    public void setSubAdministrativeArea(String subAdministrativeArea) {
        this.subAdministrativeArea = subAdministrativeArea;
    }

    @Override
    public String toString() {
        StringBuilder builder = new StringBuilder();
        builder.append("GeoDecoderAttributes [additionalCityLevelInfo=");
        builder.append(additionalCityLevelInfo);
        builder.append(", additionalStreetLevelInfo=");
        builder.append(additionalStreetLevelInfo);
        builder.append(", administrativeArea=");
        builder.append(administrativeArea);
        builder.append(", country=");
        builder.append(country);
        builder.append(", countryCode=");
        builder.append(countryCode);
        builder.append(", locality=");
        builder.append(locality);
        builder.append(", postalCode=");
        builder.append(postalCode);
        builder.append(", streetAddress=");
        builder.append(streetAddress);
        builder.append(", subAdministrativeArea=");
        builder.append(subAdministrativeArea);
        builder.append("]");
        return builder.toString();
    }
}
