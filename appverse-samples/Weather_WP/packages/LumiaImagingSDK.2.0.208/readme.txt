
NuGet has successfully installed the SDK into your project!

Finalizing the installation
===========================
  
  - Certain versions of Visual Studio may not find the references to the 
    Lumia Imaging SDK that were added to your project by NuGet. To fix things, 
	simply close your project and reopen it.
  
  - Make sure that your project doesn't have "Any CPU" as an "Active solution 
    platform". You can find instructions on how to do this here: 
	http://dev.windows.com/en-us/featured/lumia


New Users
=========

If this is your first time with the Lumia Imaging SDK, welcome, we are glad to have you with us!
To get you started off on the right foot, take a quick peek at our documentation:

http://dev.windows.com/en-us/featured/lumia

New in SDK 2.0
==============

Subnamespaces
The Lumia Imaging SDK types have now been split into the following namespaces:
•	Lumia.Imaging
•	Lumia.Imaging.Adjustments
•	Lumia.Imaging.Artistic
•	Lumia.Imaging.Custom
•	Lumia.Imaging.Compositing
•	Lumia.Imaging.Transforms
This is a breaking change; additional using files may be needed in your source files.

New Filters
•	GaussianNoiseFilter
•	HueSaturationLightnessFilter
•	SaturationLightnessFilter
•	RgbMixerFilter
•	ScaleFilter
•	SharpnessFilter was redesigned, now taking a wider ranged input to allow for greater precision
•	VibranceFilter
•	RedEyeRemovalFilter

Additions to existing filters
•	New blend functions:
	o	Lineardodge (alias to blend mode “add”)
	o	Linearlight
	o	Vividlight
	o	SignedDifference
•	GrayscaleFilter with RGB color factors as input parameters
•	OilyFilter
	o	OilBrushSize

New Effects
•	BlendEffect
	o	Same functionality as BlendFilter, but optimized internally depending on use case
•	CachingEffect
	o	Creates a full bitmap version of the source graph, and caches that until the user calls Invalidate(). Helps the user to be explicit about avoiding costly re-rendering.
•	EffectGroupAdapter
•	CustomImageSourceAdapter
	o	DelegatingImageSource deprecated.
•	CustomEffectAdapter
	o	DelegatingEffect deprecated.


Additions to existing Effects
•	ImageAligner now has two new properties
	o	UseColorCorrection
	o	AlignQuality, takes AlignQuality
•	LensBlurEffect
	o 	DefocusStrategy: Allows the user to have more control over the sampling when blurring pixels near the edge of the focus area.
	o 	FocusEdgeSoftening: Allows the user to have more control on how the edge between the focus and blurred area is treated.
	o 	KernelMapType: Allows the developer more flexibility when composing the kernel map. Specifically the new option is LensBlurKernelMapType.Continuous which allows for continues indexes instead of the standard black and white style mask.
	o 	PointLightStrength: The property existed before on each ILensBlurKernel, now it has been moved to the Effect level so the same property applies for all kernels.


Interfaces
•	IActiveFrame
	o	An interface for getting/setting the active frame of an animated image.
	o	Implemented by BufferImageSource, BufferProviderImageSource and StorageFileImageSource
	o	IReadableBitmapProvider


*NEW* Conceptual Decorators
•	AnimationFrame, similar to previous FrameDescriptor. Used to set properties for individual frames, e.g when encoding animated GIF

ImageSources
•	ExtractObjectImageSource
	o	An image source that represents an extracted sub-image from the
•	BitmapProviderImageSource
	o	An image source created from a provider of a bitmap

Enums
•	AlignQuality
	o	Low/Medium/High
•	OutputColorSpacing
	o	Grayscale colorspacing added

Other
•	AutoFixAnalyzer
	o	Analysis an image a produces one curve for the LightnessFilter and one for the SaturationFilter which, when applied, will improve the visual quality of the image.
•	BufferFactory
	o	CreateFromBuffer
•	MaskAnnotationType
•	Curve
	o	Several utility functions added
•	RampedRange
•	ArgbColorCurves
•	CurveMinMaxPair
•	ObjectExtractor
	o	Extracts objects from an image using a mask image


Deprecated – to be removed in next major release
•	BlendFilter – replace with BlendEffect
•	ILockableMemory – deprecated with remove attribute
•	BufferFactory
	o	CreateBuffer – deprecated with remove attribute
•	ImageProvideInfo
	o	ImageSize {set} – deprecated with remove attribute

Copyright (c) 2012-2014, Microsoft
All rights reserved.


Copyright (c) 1992-2008 The University of Tennessee.  All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are
met:

- Redistributions of source code must retain the above copyright
  notice, this list of conditions and the following disclaimer.

- Redistributions in binary form must reproduce the above copyright
  notice, this list of conditions and the following disclaimer listed
  in this license in the documentation and/or other materials
  provided with the distribution.

- Neither the name of the copyright holders nor the names of its
  contributors may be used to endorse or promote products derived from
  this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
"AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.


----


Copyright 1990 - 1998, 2000 by AT&T, Lucent Technologies and Bellcore.

Permission to use, copy, modify, and distribute this software
and its documentation for any purpose and without fee is hereby
granted, provided that the above copyright notice appear in all
copies and that both that the copyright notice and this
permission notice and warranty disclaimer appear in supporting
documentation, and that the names of AT&T, Bell Laboratories,
Lucent or Bellcore or any of their entities not be used in
advertising or publicity pertaining to distribution of the
software without specific, written prior permission.

AT&T, Lucent and Bellcore disclaim all warranties with regard to
this software, including all implied warranties of
merchantability and fitness.  In no event shall AT&T, Lucent or
Bellcore be liable for any special, indirect or consequential
damages or any damages whatsoever resulting from loss of use,
data or profits, whether in an action of contract, negligence or
other tortious action, arising out of or in connection with the
use or performance of this software.






