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

public interface IMap {

    /**
     * Shows Map on screen.
     */
    public void GetMap();

    /**
     * Gets POI by given id.
     *
     * @param id POI identifier.
     * @return POI.
     */
    public POI GetPOI(String id);

    /**
     * Provides the list of POIs for the current location, given a radius
     * (bounding box).
     *
     *
     * @param location Map location point to search nearest POIs.
     * @param radius The radius around location to search POIs in.
     * @return Points of Interest for location, ordered by distance.
     */
    public POI[] GetPOIList(LocationCoordinate location, float radius);

    /**
     * Provides the list of POIs for the current location, given a radius
     * (bounding box), that match given query.
     *
     * @param location Map location point to search nearest POIs.
     * @param radius The radius around location to search POIs in.
     * @param queryText The query to search POIs.
     * @return Points of Interest for location, ordered by distance.
     */
    public POI[] GetPOIList(LocationCoordinate location, float radius,
            String queryText);

    /**
     * Provides the list of POIs for the current location, given a radius
     * (bounding box), that match given query and category.
     *
     * @param location Map location point to search nearest POIs.
     * @param radius The radius around location to search POIs in.
     * @param queryText The query to search POIs.
     * @param category The category that should map listed POIs.
     * @return Points of Interest for location, ordered by distance.
     */
    public POI[] GetPOIList(LocationCoordinate location, float radius,
            String queryText, LocationCategory category);

    /**
     * Provides the list of POIs for the current location, given a radius
     * (bounding box), that match given category.
     *
     * @param location Map location point to search nearest POIs.
     * @param radius The radius around location to search POIs in.
     * @param category The category that should map listed POIs.
     * @return Points of Interest for location, ordered by distance.
     */
    public POI[] GetPOIList(LocationCoordinate location, float radius,
            LocationCategory category);

    /**
     * Removes a POI from map given its id.
     *
     * @param id POI identifier.
     * @return <CODE>true</CODE> on success, <CODE>false</CODE> otherwise.
     */
    public boolean RemovePOI(String id);

    /**
     * Specifies current map scale and bounding box radius.
     *
     * @param scale Map scale.
     * @param boundingBox Map bounding box.
     */
    public void SetMapSettings(float scale, float boundingBox);

    /**
     * Updates a POI.
     *
     * @param poi POI.
     * @return <CODE>true</CODE> on success, <CODE>false</CODE> otherwise.
     */
    public boolean UpdatePOI(POI poi);
}
