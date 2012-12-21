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
package com.gft.unity.core.system.server.net;

import java.io.IOException;

/**
 * Interface for monitoring progress of sending {@link HttpResponse}. Class that
 * implement this interface can listen for all HttpResponses being sent to
 * clients. They also can veto or block certain responses during their
 * transmissions by throwing a java.lang.IOException in the
 * {@link #notify(HttpRequest, int, int ) notify} method. The
 * {@link Server#setResponseListener(ResponseListener ) setResponseListener}
 * method must be called in order to set the ResponseListener for a Server.
 */
public interface ResponseListener {

    /**
     * This method is called at the beginning of an HttpRequest.
     *
     * @param request the request that is being respond to.
     */
    public void startTransfer(HttpRequest request);

    /**
     * This method is called at regular intervals to notify the progress of a
     * HttpResponse transmission. The number of time this method is called
     * depends on how much data is being set to the client. It will be called at
     * least once.
     *
     * @param request the request that is being responded to.
     * @param bytesSent the amount of data sent so far in bytes. Always be zero
     * or greater.
     * @param totalLength the total length of the data being sent in bytes. Can
     * be -1 for an unknown total.
     * @throws IOException throws an IOException if a ResponseListener wishes to
     * interrupt the transmission.
     */
    public void notify(HttpRequest request, int bytesSent, int totalLength)
            throws IOException;

    /**
     * This method is called once the transmission of the HttpResponse has
     * concluded.
     *
     * @param request the request that is being respond to.
     * @param e the exception if there was one that ended the transmission. It
     * is null if it completed successfully.
     */
    public void endTransfer(HttpRequest request, Exception e);
}
