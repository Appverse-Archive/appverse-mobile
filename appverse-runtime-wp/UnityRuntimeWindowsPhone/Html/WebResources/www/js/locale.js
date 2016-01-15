//********************************** 
//********************************** 
//********************************** 
//**** LOCALIZATION CONTROLLER ****
//********************************** 
//********************************** 
//********************************** 

        var localizationController = {
    language: null,
    defaultLanguage: "en",
    supportedLanguages: {
        en: {
            Title: 'Appverse Showcase',
            Back: "Back",
            Navigation: "Navigation",
            Search: "Search",
            Cancel: "Cancel",
            ChangeLanguage: "Change Language",
            Loading: "Loading...",
            NotImplemented: "Function not yet implemented",
            Version: "Current app version: ",
            InvocationError: "Cannot convert invocation parameters properly.",
            Network: "Network",
            Display: "Display",
            HumanInteraction: "Human Interaction",
            Memory: "Memory",
            OperatingSystem: "Operating System",
            Power: "Power",
            Processor: "Processor",
            Database: "Database",
            FileSystem: "FileSystem",
            Notification: "Notification",
            IOServices: "I/O Services",
            GeoLocation: "GeoLocation",
            Media: "Media",
            Messaging: "Messaging",
            Contacts: "Contacts",
            Calendar: "Calendar",
            Telephony: "Telephony",
            Internationalization: "i18n",
            Analytics: "Analytics",
            AppLoader: "Application Loader",
            Submit: "Submit",
            FormInstructions: "Please enter the information above to run the test case and click 'Submit'.",
            ServiceFormInstructions: "Please enter the Service Definition above and click 'Next' arrow.",
            RequestFormInstructions: "Please enter the Request Definition above and click 'Next' arrow.",
            GoInstructions: "Please push the button to proceed.",
            Service: "Service",
            Method: "Method",
            Parameters: "Parameters",
            Result: "Result",
            NoRequiredParams: "<no params required for this method>",
            PlayAudio: "Play Audio",
            PlayVideo: "Play Video",
            Security: "Security",
            Webtrekk: "Webtrekk",
			NFC: "NFC Payment",
			Scanner: "Scanner",
			PushNotifications: "Push Notifications"
        },
        es: {
            Title: 'Appverse Showcase',
            Back: "Atrás",
            Navigation: "Navegación",
            Search: "Buscar",
            Cancel: "Cancelar",
            ChangeLanguage: "Cambiar idioma",
            Loading: "Cargando...",
            NotImplemented: "Funcionalidad no implementada",
            Version: "Version actual de la applicacion: ",
            InvocationError: "Error al invocar el metodo con los parametros definidos.",
            Network: "Red",
            Display: "Pantalla",
            HumanInteraction: "Interacción Usuario",
            Memory: "Memoria",
            OperatingSystem: "Sistema Operativo",
            Power: "Batería",
            Processor: "Procesador",
            Database: "Base de Datos",
            FileSystem: "Directorio de Sistema",
            Notification: "Notificación",
            IOServices: "Servicios E/S",
            GeoLocation: "GeoLocalización",
            Media: "Media",
            Messaging: "Mensajes",
            Contacts: "Contactos",
            Calendar: "Calendario",
            Telephony: "Teléfono",
            Internationalization: "i18n",
            Analytics: "Análisis",
            AppLoader: "Descarga Aplicaciones",
            Submit: "Enviar",
            FormInstructions: "Introduzca arriba la información necesaria para ejecutar el test y presione 'Enviar'.",
            ServiceFormInstructions: "Introduzca la definicion del servicio y presione la flecha de siguiente.",
            RequestFormInstructions: "Introduzca la definicion de la peticion y presione la flecha de siguiente.",
            GoInstructions: "Pulse el boton para proceder.",
            Service: "Servicio",
            Method: "Método",
            Parameters: "Parámetros",
            Result: "Resultado",
            NoRequiredParams: "<este método no requiere parámetros>",
            PlayAudio: "Reproducir Audio",
            PlayVideo: "Reproducir Video",
            Security: "Seguridad",
            Webtrekk: "Webtrekk",
			NFC: "Pago NFC",
			Scanner: "Escáner",
			PushNotifications: "Notificationes Push"
        }
    }
};

localizationController.localizedUIString = function(c) {
    if (localizationController.language == null) {
        localizationController.language = localizationController.defaultLanguage;
    }
    var b = localizationController.supportedLanguages[localizationController.language];
    if (!b) {
        return c
    }
    var a = b[c];
    if (!a) {
        return c
    }
    return a
};

var getBrowserLanguage = function() {

    var language = null;
    var a = null;

    if (navigator && navigator.language && navigator.language.length >= 2) {
        a = navigator.language.substring(0, 2);
    }

    if (a && localizationController.supportedLanguages[a]) {
        language = a; // supported language found
    } else {
        language = null; // no supported language found
    }

    return language;
}
