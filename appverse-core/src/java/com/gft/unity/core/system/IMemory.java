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
package com.gft.unity.core.system;

public interface IMemory {

    /**
     * Provides memory available for the given use.
     *
     * @param use Type of usage.
     * @return Memory available in bytes.
     */
    public long GetMemoryAvailable(MemoryUse use);

    /**
     * Provides memory available for the given use and of the given type.
     *
     * @param use Type of usage.
     * @param type Type of storage.
     * @return Memory available in bytes.
     */
    public long GetMemoryAvailable(MemoryUse use, MemoryType type);

    /**
     * Installed memory types.
     *
     * @return List of installed memory types.
     */
    public MemoryType[] GetMemoryAvailableTypes();

    /**
     * Provides a global map of the memory status for all storage types
     * installed.
     *
     * @return MemoryStatus.
     */
    public MemoryStatus GetMemoryStatus();

    /**
     * Provides a map of the memory for the given storage type.
     *
     * @param type Type of memory.
     * @return MemoryStatus.
     */
    public MemoryStatus GetMemoryStatus(MemoryType type);

    /**
     * Provides the supported memory types.
     *
     * @return List of supported memory types.
     */
    public MemoryType[] GetMemoryTypes();

    /**
     * Provides the supported memory usage types.
     *
     * @return List of supported memory usage types.
     */
    public MemoryUse[] GetMemoryUses();
}
