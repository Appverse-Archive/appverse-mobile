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
namespace Unity.Core.System
{
	public interface IDisplay
	{


		/// <summary>
		/// Provides the current orientation of the primary display - the primary display is 1.
		/// </summary>
		/// <returns>Display orientation.</returns>
		DisplayOrientation GetOrientationCurrent ();

		/// <summary>
		/// Provides the current orientation of the given display number, 1 being the primary display.
		/// </summary>
		/// <param name="displayNumber">Screen identifier.</param>
		/// <returns>Display orientation.</returns>
		DisplayOrientation GetOrientation (int displayNumber);

		/// <summary>
		/// Provides the list of supported orientations for the device.
		/// </summary>
		/// <returns>List of supported orientations.</returns>
		DisplayOrientation[] GetOrientationSupported ();

		/// <summary>
		/// Provides the list of supported orientations for the given display number.
		/// </summary>
		/// <param name="displayNumber">Screen identifier.</param>
		/// <returns>List of supported orientations.</returns>
		DisplayOrientation[] GetOrientationSupported (int displayNumber);

		/// <summary>
		/// Provides the number of screens connected to the device. Display 1 is the primary.
		/// </summary>
		/// <returns>The number of displays connected to the device.</returns>
		int GetDisplays ();

		/// <summary>
		/// Provides information about the primary display.
		/// </summary>
		/// <returns>Display information.</returns>
		DisplayInfo GetDisplayInfo ();

		/// <summary>
		/// Provides information about the display number.
		/// </summary>
		/// <param name="displayNumber">The display number.</param>
		/// <returns>Display information.</returns>
		DisplayInfo GetDisplayInfo (int displayNumber);
		
		/// <summary>
		/// Sets whether the current application should autorotate or not.
		/// If value is set to 'false', application's orientation will be set to the given orientation.
		/// </summary>
		/// <param name="lockOrientation">
		/// A <see cref="System.Boolean"/> value indicating whether the application view should autorotate; 'true' to remain on the specified orientation
		/// </param>
		/// <param name="orientation">
		/// A <see cref="DisplayOrientation"/> the orientation enum constant that the application should be locked, if lock is false this value is ignored
		/// </param>
		void LockOrientation (bool lockOrientation, DisplayOrientation orientation);
		
		/// <summary>
		/// Indicates whether the current application if currently configured to autorotate or not.
		/// </summary>
		/// <returns>
		/// A <see cref="System.Boolean"/> If returned value is 'false', application remains on the default screen mode (portrait, in the iOS default case)
		/// </returns>
		bool IsOrientationLocked ();
		
		/// <summary>
		/// Getter for the current locked orientation
		/// </summary>
		/// <returns>
		/// A <see cref="DisplayOrientation"/> the locked orientation
		/// </returns>
		DisplayOrientation GetLockedOrientation ();
		
		/// <summary>
		/// Shows the splash screen.
		/// </summary>
		/// <returns>
		/// The splash screen.
		/// </returns>
		bool ShowSplashScreen ();
		
		/// <summary>
		/// Dismisses the splash screen.
		/// </summary>
		/// <returns>
		/// The splash screen.
		/// </returns>
		bool DismissSplashScreen ();
   
		
	}//end IDisplay

}//end namespace System