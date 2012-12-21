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

/**
 * This class is used by handlers or endpoints to tell the server which options
 * are used to configure the object.
 */
public class ConfigOption {

    private String propertyName;
    private String defaultValue;
    private boolean isRequired;
    private String helpString;

    /**
     * This is used to create an optional property that defaults to null if
     * unspecified.
     *
     * @param propertyName the name of the property.
     * @param helpString the help string shown to the user.
     */
    public ConfigOption(String propertyName, String helpString) {
        this(propertyName, false, helpString);
    }

    /**
     * This is used to create an optional property that has a supplied default
     * value.
     *
     * @param propertyName the name of the property.
     * @param defaultValue the default value used if this property is
     * unspecified.
     * @param helpString the help string shown to the user.
     */
    public ConfigOption(String propertyName, String defaultValue,
            String helpString) {
        this.propertyName = propertyName;
        this.defaultValue = defaultValue;
        this.isRequired = false;
        this.helpString = helpString;
    }

    /**
     * This is used to create a required property. There is no default supplied
     * in the case where required is true. If you specify it as false it's the
     * same as an optional property with no default.
     *
     * @param propertyName the name of the property.
     * @param required Used to specify a required property. True for required,
     * false for optional.
     * @param helpString the help string shown to the user if nothing is
     * specified.
     */
    public ConfigOption(String propertyName, boolean required, String helpString) {
        this.propertyName = propertyName;
        this.defaultValue = null;
        this.isRequired = required;
        this.helpString = helpString;
    }

    /**
     * This is used to fetch the value of the property. It's returned as a
     * String. It will return the default property if it's not specified.
     *
     * @param server the Server object used by the system.
     * @param name the name of the handler or endpoint instance.
     * @return the value of the property or the default value if it was supplied
     * in the constructor.
     */
    public String getProperty(Server server, String name) {
        String key = propertyName;
        if (name != null) {
            key = name + "." + key;
        }
        String value = server.getProperty(key, defaultValue);
        if (isRequired && value == null) {
            throw new IllegalArgumentException(key + " is a required argument.");
        }
        return value;
    }

    /**
     * This is used to fetch the value of the property as a Boolean. It will
     * return the default property if it's not specified.
     *
     * @param server the Server object used by the system.
     * @param name the name of the handler or endpoint instance.
     * @return a Boolean value of the property or the default value if it was
     * supplied in the constructor.
     */
    public Boolean getBoolean(Server server, String name) {
        return new Boolean(getProperty(server, name));
    }

    /**
     * This is used to fetch the value of the property as an Integer. It will
     * return the default property if it's not specified.
     *
     * @param server the Server object used by the system.
     * @param name the name of the handler or endpoint instance.
     * @return an Integer value of the property or the default value if it was
     * supplied in the constructor.
     */
    public Integer getInteger(Server server, String name) {
        return new Integer(getProperty(server, name));
    }

    /**
     * This method is used to return a user viewable help string in case an
     * object was misconfigured.
     *
     * @return the user viewable string for misconfigured properties.
     */
    public String toHelp() {
        return propertyName + "\t" + (isRequired ? "Required" : "Optional")
                + "\t" + defaultValue + "\t" + helpString;
    }

    /**
     * The name of the configuration property that this ConfigOption was
     * constructed with.
     *
     * @return the propery's name.
     */
    public String getName() {
        return propertyName;
    }
}
