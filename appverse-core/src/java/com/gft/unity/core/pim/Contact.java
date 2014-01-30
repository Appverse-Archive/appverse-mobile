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
package com.gft.unity.core.pim;

import java.util.Arrays;

public class Contact extends ContactLite {

    private ContactAddress[] addresses;
    private String company;
    private String department;
    private String jobTitle;
    private String notes;
    private byte[] photo;
    private String photoBase64Encoded;
    private RelationshipType relationship;
    private String[] webSites;

    public Contact() {
    }

    public ContactAddress[] getAddresses() {
        return addresses;
    }

    public void setAddresses(ContactAddress[] addresses) {
        this.addresses = addresses;
    }

    public String getCompany() {
        return company;
    }

    public void setCompany(String company) {
        this.company = company;
    }

    public String getDepartment() {
        return department;
    }

    public void setDepartment(String department) {
        this.department = department;
    }

    public String getJobTitle() {
        return jobTitle;
    }

    public void setJobTitle(String jobTitle) {
        this.jobTitle = jobTitle;
    }

    public String getNotes() {
        return notes;
    }

    public void setNotes(String notes) {
        this.notes = notes;
    }

    public byte[] getPhoto() {
        return photo;
    }

    public void setPhoto(byte[] photo) {
        this.photo = photo;
    }

    public String getPhotoBase64Encoded() {
        return photoBase64Encoded;
    }

    public void setPhotoBase64Encoded(String photoBase64Encoded) {
        this.photoBase64Encoded = photoBase64Encoded;
    }

    public RelationshipType getRelationship() {
        return relationship;
    }

    public void setRelationship(RelationshipType relationship) {
        this.relationship = relationship;
    }

    public String[] getWebSites() {
        return webSites;
    }

    public void setWebSites(String[] webSites) {
        this.webSites = webSites;
    }

    @Override
    public String toString() {
        StringBuilder builder = new StringBuilder();
        builder.append("Contact [addresses=");
        builder.append(Arrays.toString(addresses));
        builder.append(", company=");
        builder.append(company);
        builder.append(", department=");
        builder.append(department);
        builder.append(", displayName=");
        builder.append(this.getDisplayName());
        builder.append(", emails=");
        builder.append(Arrays.toString(this.getEmails()));
        builder.append(", firstname=");
        builder.append(this.getFirstname());
        builder.append(", group=");
        builder.append(this.getGroup());
        builder.append(", ID=");
        builder.append(this.getID());
        builder.append(", jobTitle=");
        builder.append(jobTitle);
        builder.append(", lastname=");
        builder.append(this.getLastname());
        builder.append(", name=");
        builder.append(this.getName());
        builder.append(", notes=");
        builder.append(notes);
        builder.append(", phones=");
        builder.append(Arrays.toString(this.getPhones()));
        builder.append(", photo=");
        builder.append(Arrays.toString(photo));
        builder.append(", photoBase64Encoded=");
        builder.append(photoBase64Encoded);
        builder.append(", relationship=");
        builder.append(relationship);
        builder.append(", webSites=");
        builder.append(Arrays.toString(webSites));
        builder.append("]");
        return builder.toString();
    }
}
