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
package com.gft.unity.core.storage.filesystem;

public abstract class AbstractFileSystem implements IFileSystem {

    public AbstractFileSystem() {
    }

    @Override
    public abstract boolean CopyFromResources(String fromPath, String toPath);

    @Override
    public abstract boolean CopyFromRemote(String url, String toPath);

    @Override
    public abstract DirectoryData CreateDirectory(String directoryName,
            DirectoryData baseDirectory);

    @Override
    public abstract DirectoryData CreateDirectory(String directoryName);

    @Override
    public abstract FileData CreateFile(String fileName, DirectoryData baseDirectory);

    @Override
    public abstract FileData CreateFile(String fileName);

    @Override
    public abstract boolean DeleteDirectory(DirectoryData directory);

    @Override
    public abstract boolean DeleteFile(FileData file);

    @Override
    public abstract boolean ExistsDirectory(DirectoryData directory);

    @Override
    public abstract boolean ExistsFile(FileData file);

    @Override
    public abstract DirectoryData GetDirectoryRoot();

    @Override
    public abstract DirectoryData[] ListDirectories(DirectoryData directory);

    @Override
    public abstract DirectoryData[] ListDirectories();

    @Override
    public abstract FileData[] ListFiles(DirectoryData directory);

    @Override
    public abstract byte[] ReadFile(FileData file);

    @Override
    public abstract boolean WriteFile(FileData file, byte[] contents, boolean append);
}
