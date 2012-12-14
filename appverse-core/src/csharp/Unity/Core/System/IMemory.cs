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
	public interface IMemory
	{

		/// <summary>
		/// Helper method to return available memory types.
		/// </summary>
		/// <returns>List of supported memory types.</returns>
		MemoryType[] GetMemoryTypes ();

		/// <summary>
		/// Helper method to return supported memory usage types.
		/// </summary>
		/// <returns>List of supported memory usage types.</returns>
		MemoryUse[] GetMemoryUses ();

		/// <summary>
		/// Installed memory types.
		/// </summary>
		/// <returns>List of installed memory types.</returns>
		MemoryType[] GetMemoryAvailableTypes ();

		/// <summary>
		/// Provides a global map of the memory status for all storage types installed.
		/// </summary>
		/// <returns>MemoryStatus</returns>
		MemoryStatus GetMemoryStatus ();

		/// <summary>
		/// Provides a map of the memory for the given storage type.
		/// </summary>
		/// <param name="type">Type of memory.</param>
		/// <returns>MemoryStatus</returns>
		MemoryStatus GetMemoryStatus (MemoryType type);

		/// <summary>
		/// Provides memory available for the given use.
		/// </summary>
		/// <param name="use">Type of usage.</param>
		/// <returns>Memory available in bytes.</returns>
		long GetMemoryAvailable (MemoryUse use);

		/// <summary>
		/// Provides memory available for the given use and of the given type.
		/// </summary>
		/// <param name="use">Type of usage.</param>
		/// <param name="type">Type of storage.</param>
		/// <returns>Memory available in bytes.</returns>
		long GetMemoryAvailable (MemoryUse use, MemoryType type);


	}//end IMemory

}//end namespace System