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
package com.gft.unity.core.beacon;

public class Beacon {

    
    private static final double DISTANCE_THRESHOLD_WTF = 0.0;
    private static final double DISTANCE_THRESHOLD_IMMEDIATE = 0.5;
    private static final double DISTANCE_THRESHOLD_NEAR = 3.0;
    private String address;
    private String name;
    private String UUID;
    private DistanceType distance;
    private double meters;
    private int major;
    private int minor;
    private long timestamp;

    public Beacon() {
    }

    public Beacon(String address, String name, String uuid, double meters, int major, int minor, long timestamp) {
        this.address = address;
        this.name = name;
        this.UUID = uuid;
        this.distance = getDistance();
        this.meters = meters;
        this.major = major;
        this.minor = minor;
        this.timestamp = timestamp;
    }

    
    public String getAddress() {
        return address;
    }

    public void setAddress(String address) {
        this.address = address;
    }

    public String getName() {
        return name;
    }

    public void setName(String name) {
        this.name = name;
    }

    public String getUUID() {
        return UUID;
    }

    public void setUUID(String uuid) {
        this.UUID = uuid;
    }

    public final DistanceType getDistance() {
        if (meters < DISTANCE_THRESHOLD_WTF) {
            return DistanceType.UNKNOWN;
        }
        if (meters < DISTANCE_THRESHOLD_IMMEDIATE) {
            return DistanceType.IMMEDIATE;
        }
        if (meters < DISTANCE_THRESHOLD_NEAR) {
            return DistanceType.NEAR;
        }
        return DistanceType.FAR;


    }

    public void setDistance(DistanceType distance) {
        this.distance = distance;
    }

    public double getMeters() {
        return meters;
    }

    public void setMeters(double meters) {
        this.meters = meters;
    }

    public int getMajor() {
        return major;
    }

    public void setMajor(int major) {
        this.major = major;
    }

    public int getMinor() {
        return minor;
    }

    public void setMinor(int minor) {
        this.minor = minor;
    }

    public long getTimestamp() {
        return timestamp;
    }

    public void setTimestamp(long timestamp) {
        this.timestamp = timestamp;
    }

    @Override
    public String toString() {
        StringBuilder builder = new StringBuilder();
        builder.append("Beacon [address=");
        builder.append(address);
        builder.append(", name=");
        builder.append(name);
        builder.append(", uuid=");
        builder.append(UUID);
        builder.append(", distance=");
        builder.append(distance);
        builder.append(",meters=");
        builder.append(meters);
        builder.append(", major=");
        builder.append(major);
        builder.append(", minor=");
        builder.append(minor);
        builder.append(", minor=");
        builder.append(timestamp);
        builder.append("]");
        return builder.toString();
    }
    
    public enum DistanceType {
        IMMEDIATE,
        NEAR,
        FAR,
        UNKNOWN,
    }
}
