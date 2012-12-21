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
using System;
using System.Collections.Generic;
using System.Text;

namespace Unity.Core.System
{
	public enum InputCapability
	{
		Unknown,
		InternalTouchKeyboard,
		InternalKeyboard,
		ExternalKeyboard,
		InternalPointingDevice,
		ExternalPointingDevice,
		VoiceRecognition,
		MultiTouchGestures,
		MonoTouchGestures
	}

	public enum InputGesture
	{
		// TODO: provide gesture types.
	}

	public enum InputButton
	{
		// TODO: provide hardware buttons supported.
	}

	/// <summary>
	/// List of supported display orientations.
	/// </summary>
	public enum DisplayOrientation
	{
		Unknown,
		Portrait,
		Landscape
	}

	/// <summary>
	/// List of supported display types.
	/// </summary>
	public enum DisplayType
	{
		Unknown,
		Primary,
		External
	}

	/// <summary>
	/// List of possible colour depths.
	/// </summary>
	public enum DisplayBitDepth
	{
		Unknown,
		Bpp8,
		Bpp16,
		Bpp24,
		Bpp32
	}

	/// <summary>
	/// List of possible supported memory types.
	/// </summary>
	public enum MemoryType
	{
		Unknown,
		Main,
		Extended
	}

	/// <summary>
	/// List of supported memory usages.
	/// </summary>
	public enum MemoryUse
	{
		Application,
		Storage,
		Other
	}

	/// <summary>
	/// List of possible charge status.
	/// </summary>
	public enum PowerStatus
	{
		Unknown,
		FullyCharged,
		Charging,
		Discharging
	}

}
