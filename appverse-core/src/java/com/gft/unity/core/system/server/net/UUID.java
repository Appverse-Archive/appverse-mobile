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
package com.gft.unity.core.system.server.net;

import java.security.SecureRandom;
import java.util.StringTokenizer;

public class UUID implements Comparable<UUID> {

    private static SecureRandom randomGenerator = new SecureRandom();
    private long high;
    private long low;

    public UUID(long high, long low) {
        this.high = high;
        this.low = low;
    }

    public static UUID createUUID() {

        byte[] uuid = new byte[16];
        randomGenerator.nextBytes(uuid);
        uuid[6] &= 0xf;
        uuid[6] |= 0x40;
        uuid[8] &= 0x3f;
        uuid[8] |= 0x80;
        uuid[10] |= 0x80;
        return new UUID(copyToLong(uuid, 0), copyToLong(uuid, 8));
    }

    @Override
    public int compareTo(UUID uuid) {

        if (this.high < uuid.high) {
            return -1;
        } else if (this.high == uuid.high) {
            if (this.low < uuid.low) {
                return -1;
            } else if (this.low == uuid.low) {
                return 0;
            } else {
                return 1;
            }
        } else {
            return 1;
        }
    }

    private static long copyToLong(byte[] uuid, int start) {
        long num = 0L;
        for (int i = start; i < (start + 8); i++) {
            num = num << 8 | (long) (uuid[i] & 0xff);
        }
        return num;
    }

    @Override
    public int hashCode() {
        return (int) (high >> 32 ^ high ^ low >> 32 ^ low);
    }

    @Override
    public boolean equals(Object obj) {
        if (obj == this) {
            return true;
        }

        if (obj instanceof UUID) {
            UUID uuid = (UUID) obj;

            return (uuid.high == this.high) && (uuid.low == this.low);
        }

        return false;
    }

    private static String digits(long num, int i) {
        long shifted = 1L << i * 4;
        return Long.toHexString(shifted | num & shifted - 1L).substring(1);
    }

    @Override
    public String toString() {
        return digits(high >> 32, 8) + "-" + digits(high >> 16, 4) + "-"
                + digits(high, 4) + "-" + digits(low >> 48, 4) + "-"
                + digits(low, 12);
    }

    public static UUID parse(String uuidStr) {
        StringTokenizer token = new StringTokenizer(uuidStr, "-");
        long upper = Long.parseLong(token.nextToken(), 16) << 32;
        upper = upper | (Long.parseLong(token.nextToken(), 16) << 16);
        upper = upper | (Long.parseLong(token.nextToken(), 16));

        long lower = Long.parseLong(token.nextToken(), 16) << 48;
        lower = lower | Long.parseLong(token.nextToken(), 16);

        return new UUID(upper, lower);
    }

    public static void main(String[] args) {
        UUID uuid = UUID.createUUID();
        System.out.println("UUID: (" + uuid + ")");
        System.out.println("UUID: (" + UUID.parse(uuid.toString()) + ")");

        UUID uuid2 = UUID.createUUID();
        System.out.println("UUID: (" + uuid2 + ")");
        System.out.println("UUID: (" + UUID.parse(uuid2.toString()) + ")");

        UUID uuid3 = UUID.createUUID();
        System.out.println("UUID: (" + uuid3 + ")");
        System.out.println("UUID: (" + UUID.parse(uuid3.toString()) + ")");
    }
}
