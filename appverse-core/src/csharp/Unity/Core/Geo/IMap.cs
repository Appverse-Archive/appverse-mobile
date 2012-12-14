/*
 Copyright (c) 2012 GFT Appverse, S.L., Sociedad Unipersonal.

 This Source  Code Form  is subject to the  terms of  the Appverse Public License 
 Version 2.0  (“APL v2.0”).  If a copy of  the APL  was not  distributed with this 
 file, You can obtain one at http://www.appverse.mobi/licenses/apl_v2.0.pdf.

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
namespace Unity.Core.Geo
{
	public interface IMap
	{

		/// <summary>
		/// List of POIs for the current location, given a radius (bounding box).
		/// </summary>
		/// <param name="location">Map location point to search nearest POIs.</param>
		/// <param name="radius">The radius around location to search POIs in.</param>
		/// <returns>Points of Interest for location, ordered by distance.</returns>
		POI[] GetPOIList (LocationCoordinate location, float radius);

		/// <summary>
		/// List of POIs for the current location, given a radius (bounding box), that match given query.
		/// </summary>
		/// <param name="location">Map location point to search nearest POIs.</param>
		/// <param name="radius">The radius around location to search POIs in.</param>
		/// <param name="queryText">The query to search POIs.</param>
		/// <returns>Points of Interest for location, ordered by distance.</returns>
		POI[] GetPOIList (LocationCoordinate location, float radius, string queryText);

		/// <summary>
		/// List of POIs for the current location, given a radius (bounding box), that match given query and category.
		/// </summary>
		/// <param name="location">Map location point to search nearest POIs.</param>
		/// <param name="radius">The radius around location to search POIs in.</param>
		/// <param name="queryText">The query to search POIs.</param>
		/// <param name="category">The category that should map listed POIs.</param>
		/// <returns>Points of Interest for location, ordered by distance.</returns>
		POI[] GetPOIList (LocationCoordinate location, float radius, string queryText, LocationCategory category);

		/// <summary>
		/// ist of POIs for the current location, given a radius (bounding box), that match given category.
		/// </summary>
		/// <param name="location">Map location point to search nearest POIs.</param>
		/// <param name="radius">The radius around location to search POIs in.</param>
		/// <param name="category">The category that should map listed POIs.</param>
		/// <returns>Points of Interest for location, ordered by distance.</returns>
		POI[] GetPOIList (LocationCoordinate location, float radius, LocationCategory category);
        
		/// <summary>
		/// Get POI by given id.
		/// </summary>
		/// <param name="id">POI identifier.</param>
		/// <returns>POI</returns>
		POI GetPOI (string id);

		/// <summary>
		/// Moves POI - given its id - to target location.
		/// </summary>
		/// <param name="id">POI identifier.</param>
		/// <param name="target">Target LocationCoordinate</param>
		/// <returns>true if the operation completes successfully</returns>
		bool UpdatePOI (POI poi);

		/// <summary>
		/// Removes POI from map given its id.
		/// </summary>
		/// <param name="id">POI identifier.</param>
		/// <returns>true if the operation completes successfully</returns>
		bool RemovePOI (string id);

		/// <summary>
		/// Shows Map on screen.
		/// </summary>
		void GetMap ();

		/// <summary>
		/// Specifies current map scale and bounding box radius.
		/// </summary>
		/// <param name="scale">Map scale.</param>
		/// <param name="boundingBox">Map bounding box.</param>
		void SetMapSettings (float scale, float boundingBox);
	}//end IMap

}//end namespace Geo