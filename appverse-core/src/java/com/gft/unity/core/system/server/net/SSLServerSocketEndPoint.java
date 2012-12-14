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
package com.gft.unity.core.system.server.net;

import com.gft.unity.core.system.log.Logger;
import com.gft.unity.core.system.log.Logger.LogCategory;
import java.io.File;
import java.io.FileInputStream;
import java.io.IOException;
import java.io.InputStream;
import java.net.ServerSocket;
import java.net.Socket;
import java.security.GeneralSecurityException;
import java.security.KeyStore;
import java.security.Principal;
import java.security.PrivateKey;
import java.security.cert.X509Certificate;
import javax.net.ssl.KeyManager;
import javax.net.ssl.KeyManagerFactory;
import javax.net.ssl.SSLContext;
import javax.net.ssl.SSLServerSocket;
import javax.net.ssl.TrustManager;
import javax.net.ssl.TrustManagerFactory;
import javax.net.ssl.X509KeyManager;

/**
 * This EndPoint provides SSL sockets for HTTP protocol. This extends the
 * ServerSocket to create SSLSockets for HTTP.
 *
 * <table class="inner"> <tr class="header"> <td>Parameter Name</td>
 * <td>Explanation</td> <td>Default Value</td> <td>Required</td> </tr> <tr
 * class="row"> <td>keystore</td> <td>The keystore of the certificates used by
 * this socket. This contains both the certificate used to authenticate the
 * server as well as authenticate clients.</td> <td>None</td> <td>Yes</td> </tr>
 * <tr class="altrow"> <td>storepass</td> <td>Password for the keystore.</td>
 * <td>None</td> <td>Yes</td> </tr> <tr class="row"> <td>keypass</td>
 * <td>Password for the key with the given alias.</td> <td>Defaults to
 * storepass</td> <td>No, but its a good idea</td> </tr> <tr class="altrow">
 * <td>alias</td> <td>The name of the certificate in the keystore used for
 * server authentication.</td> <td>sslkey</td> <td>No</td> </tr> <tr
 * class="row"> <td>ciphers</td> <td>A comma separated list of cipher suites to
 * use to encrypt the SSL socket. </td> <td></td> <td>No</td> </tr> <tr
 * class="atlrow"> <td>protocols</td> <td>A comma separated list of protocols to
 * use in negotiating the SSL socket. </td> <td></td> <td>No</td> </tr> <tr
 * class="row"> <td>clientauth</td> <td>Setting this to true will require
 * clients authenticate to the server.</td> <td>false</td> <td>No</td> </tr>
 * </table>
 */
public class SSLServerSocketEndPoint extends ServerSocketEndPoint {

    private static final String LOGGER_MODULE = "WebServer.SSLServerSocketEndPoint";
    private static final Logger LOGGER = Logger.getInstance(LogCategory.CORE,
            LOGGER_MODULE);
    private static final ConfigOption KEYSTORE_OPTION = new ConfigOption(
            "keystore", true, "The keystore used by the SSL server.");
    private static final ConfigOption STOREPASS_OPTION = new ConfigOption(
            "storepass", true, "The keystore password.");
    private static final ConfigOption KEYPASS_OPTION = new ConfigOption(
            "keypass", false, "The password for the key in the keystore.");
    private static final ConfigOption ALIAS_OPTION = new ConfigOption("alias",
            "sslkey", "The alias to the key used by the SSL server.");
    private static final ConfigOption CIPHERS_OPTION = new ConfigOption(
            "ciphers", false,
            "Comma separated list of ciphers to use for the SSL server.");
    private static final ConfigOption PROTOCOLS_OPTION = new ConfigOption(
            "protocols", false,
            "Comma separated list of protocols for SSL server.");
    private static final ConfigOption CLIENT_AUTH_OPTION = new ConfigOption(
            "clientauth", "false",
            "Require client authentication during SSL handshake.");

    @Override
    public void initialize(String name, Server server) throws IOException {
        super.initialize(name, server);

        try {
            File keystoreFile = new File(KEYSTORE_OPTION.getProperty(server,
                    endpointName));
            String storepass = STOREPASS_OPTION.getProperty(server,
                    endpointName);
            String keypass = KEYPASS_OPTION.getProperty(server, endpointName);
            keypass = keypass == null ? storepass : keypass;
            KeyStore keystore = loadKeystoreFromFile(keystoreFile,
                    storepass.toCharArray());

            SSLContext context = SSLContext.getInstance("SSL");
            context.init(
                    getKeyManagers(keystore, keypass.toCharArray(),
                    ALIAS_OPTION.getProperty(server, endpointName)),
                    getTrustManagers(keystore), null);
            factory = context.getServerSocketFactory();
        } catch (GeneralSecurityException e) {
            LOGGER.logError("initialize",
                    "Security Exception while initializing.", e);
            throw (IOException) new IOException().initCause(e);
        }
    }

    @Override
    protected String getProtocol() {
        return "https";
    }

    @Override
    protected ServerSocket createSocket(int port) throws IOException {
        ServerSocket serverSocket = super.createSocket(port);

        String cipherSuites = CIPHERS_OPTION.getProperty(server, getName());
        if (cipherSuites != null) {
            ((SSLServerSocket) serverSocket)
                    .setEnabledCipherSuites(cipherSuites.split(","));
        }

        String protocols = PROTOCOLS_OPTION.getProperty(server, getName());
        if (protocols != null) {
            ((SSLServerSocket) serverSocket).setEnabledProtocols(protocols
                    .split(","));
        }

        boolean clientAuth = CLIENT_AUTH_OPTION.getBoolean(server, getName())
                .booleanValue();
        if (clientAuth) {
            ((SSLServerSocket) serverSocket).setNeedClientAuth(true);
        }

        return serverSocket;
    }

    private KeyStore loadKeystoreFromFile(File file, char[] password)
            throws IOException, GeneralSecurityException {
        KeyStore keystore = KeyStore.getInstance("JKS");

        InputStream stream = new FileInputStream(file);
        keystore.load(stream, password);
        stream.close();

        return keystore;
    }

    private TrustManager[] getTrustManagers(KeyStore keystore)
            throws GeneralSecurityException {

        TrustManagerFactory factory = TrustManagerFactory
                .getInstance("SunX509");
        factory.init(keystore);

        return factory.getTrustManagers();
    }

    private KeyManager[] getKeyManagers(KeyStore keystore, char[] pwd,
            String alias) throws GeneralSecurityException {
        KeyManagerFactory factory = KeyManagerFactory.getInstance("SunX509");
        factory.init(keystore, pwd);
        KeyManager[] kms = factory.getKeyManagers();
        if (alias != null) {
            for (int i = 0; i < kms.length; i++) {
                if (kms[i] instanceof X509KeyManager) {
                    kms[i] = new AliasForcingKeyManager(
                            (X509KeyManager) kms[i], alias);
                }
            }
        }

        return kms;
    }

    private class AliasForcingKeyManager implements X509KeyManager {

        X509KeyManager baseKM = null;
        String alias = null;

        public AliasForcingKeyManager(X509KeyManager keyManager, String alias) {
            baseKM = keyManager;
            this.alias = alias;
        }

        @Override
        public String chooseClientAlias(String[] keyType, Principal[] issuers,
                Socket socket) {
            return baseKM.chooseClientAlias(keyType, issuers, socket);
        }

        @Override
        public String chooseServerAlias(String keyType, Principal[] issuers,
                Socket socket) {
            String[] validAliases = baseKM.getServerAliases(keyType, issuers);
            if (validAliases != null) {
                for (int j = 0; j < validAliases.length; j++) {
                    if (validAliases[j].equals(alias)) {
                        return alias;
                    }
                }
            }
            // use default if we can't find the alias.
            return baseKM.chooseServerAlias(keyType, issuers, socket);
        }

        @Override
        public X509Certificate[] getCertificateChain(String alias) {
            return baseKM.getCertificateChain(alias);
        }

        @Override
        public String[] getClientAliases(String keyType, Principal[] issuers) {
            return baseKM.getClientAliases(keyType, issuers);
        }

        @Override
        public PrivateKey getPrivateKey(String alias) {
            return baseKM.getPrivateKey(alias);
        }

        @Override
        public String[] getServerAliases(String keyType, Principal[] issuers) {
            return baseKM.getServerAliases(keyType, issuers);
        }
    }
}
