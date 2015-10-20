/*
 Copyright (c) 2012 GFT Appverse, S.L., Sociedad Unipersonal.

 This Source  Code Form  is subject to the  terms of  the Appverse Public License 
 Version 2.0  (â€œAPL v2.0â€�).  If a copy of  the APL  was not  distributed with this 
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

public class MediaQRContent {
    private String Text;
    private BarCodeType BarcodeType;
    private QRType QRType;
    private int Size;
    private VCard Contact;
    private Coordinate Coord;
        

    public Coordinate getCoord() {
		return Coord;
	}

	public void setCoord(Coordinate coord) {
		Coord = coord;
	}

	public VCard getContact() {
		return Contact;
	}

	public void setContact(VCard contact) {
		this.Contact = contact;
	}

	public int getSize() {
		return Size;
	}

	public void setSize(int size) {
		this.Size = size;
	}

	public String getText() {
        return Text;
    }

    public void setText(String Text) {
        this.Text = Text;
    }

    public BarCodeType getBarcodeType() {
        return BarcodeType;
    }

    public void setBarcodeType(BarCodeType BarcodeType) {
        this.BarcodeType = BarcodeType;
    }

    public QRType getQRType() {
        return QRType;
    }

    public void setQRType(QRType QRType) {
        this.QRType = QRType;
    }  
    

    public MediaQRContent ()
    {
            Text = "";
            BarcodeType = BarCodeType.QR_CODE;
            QRType = QRType.TEXT;
    }
    public MediaQRContent (String text, BarCodeType barcodeType, QRType qrType)
    {
            Text = text;
            BarcodeType=barcodeType;
            QRType = qrType;
    }
}
