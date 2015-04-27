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
package com.gft.appverse.android.scanner;

public enum BarCodeType {
    /// <summary>Aztec 2D barcode format.</summary>
    AZTEC(1),

    /// <summary>CODABAR 1D format.</summary>
    CODABAR(2),

    /// <summary>Code 39 1D format.</summary>
    CODE_39(4),

    /// <summary>Code 93 1D format.</summary>
    CODE_93(8),

    /// <summary>Code 128 1D format.</summary>
    CODE_128(16),

    /// <summary>Data Matrix 2D barcode format.</summary>
    DATA_MATRIX(32),

    /// <summary>EAN-8 1D format.</summary>
    EAN_8(64),

    /// <summary>EAN-13 1D format.</summary>
    EAN_13(128),

    /// <summary>ITF (Interleaved Two of Five) 1D format.</summary>
    ITF(256),

    /// <summary>MaxiCode 2D barcode format.</summary>
    MAXICODE(512),

    /// <summary>PDF417 format.</summary>
    PDF_417(1024),

    /// <summary>QR Code 2D barcode format.</summary>
    QR_CODE(2048),

    /// <summary>RSS 14</summary>
    RSS_14(4096),

    /// <summary>RSS EXPANDED</summary>
    RSS_EXPANDED(8192),

    /// <summary>UPC-A 1D format.</summary>
    UPC_A(16384),

    /// <summary>UPC-E 1D format.</summary>
    UPC_E(32768),

    /// <summary>UPC/EAN extension format. Not a stand-alone format.</summary>
    UPC_EAN_EXTENSION(65536),

    /// <summary>MSI</summary>
    MSI(131072),

    /// <summary>Plessey</summary>
    PLESSEY(262144),

    /// DEFAULT
    DEFAULT(524288);
    
    private final long id;
    BarCodeType(long id) { 
        this.id = id;
    }
    public long getValue() { 
        return id; 
    }
}
