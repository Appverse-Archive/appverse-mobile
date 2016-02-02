config = {
    VERSION_APP: Appverse&&Appverse.version?Appverse.version:"5.1",//
    VERSION_DISCLAIMER: " &copy; 2013 GFT IT Consulting, S.L.U.",
    CONNECTIVITY_HOST: "unity.gft.com",
    menuOptions: [
        {name: 'Network', icon: 'resources/images/network_icon.png', api: TestCase_Network, platformAvailable: true, iconCls:'wireless'},
        {name: 'Display', icon: 'resources/images/sys_display_icon.png', api: TestCase_System_Display, platformAvailable: true, iconCls:'screens'},
        {name: 'HumanInteraction', icon: 'resources/images/sys_humaninteract_icon.png', api: TestCase_System_HumanInteraction, platformAvailable: true, iconCls:'user'},
        {name: 'Memory', icon: 'resources/images/sys_memory_icon.png', api: TestCase_System_Memory, platformAvailable: true, iconCls:'compose'},
        {name: 'OperatingSystem', icon: 'resources/images/sys_os_icon.png', api: TestCase_System_OS, platformAvailable: true, iconCls:'minus'},
        {name: 'Power', icon: 'resources/images/sys_power_icon.png', api: TestCase_System_Power, platformAvailable: true, iconCls:'battery'},
        {name: 'Processor', icon: 'resources/images/sys_processor_icon.png', api: TestCase_System_Processor, platformAvailable: true, iconCls:'power'},
        {name: 'Database', icon: 'resources/images/database_icon.png', api: TestCase_Database, platformAvailable: true, iconCls:'chart'},
        {name: 'FileSystem', icon: 'resources/images/filesystem_icon.png', api: TestCase_Filesystem, platformAvailable: true, iconCls:'news'},
        {name: 'Notification', icon: 'resources/images/notification_icon.png', api: TestCase_Notification, platformAvailable: true, iconCls:'flag'},
        {name: 'IOServices', icon: 'resources/images/io_services_icon.png', api: TestCase_IOServices, platformAvailable: true, iconCls:'network'},
        {name: 'Geolocation', icon: 'resources/images/geo_icon.png', api: TestCase_Geolocation, platformAvailable: true, iconCls:'locate'},
        {name: 'Media', icon: 'resources/images/media_icon.png', api: TestCase_Media, platformAvailable: true, iconCls:'video'},
        {name: 'Messaging', icon: 'resources/images/messaging_icon.png', api: TestCase_Messaging, platformAvailable: true, iconCls:'mail'},
        {name: 'Contacts', icon: 'resources/images/contacts_icon.png', api: TestCase_Pim_Contacts, platformAvailable: true, iconCls:'team'},
        {name: 'Calendar', icon: 'resources/images/calendar_icon.png', api: TestCase_Pim_Calendar, platformAvailable: true, iconCls:'calendar'},
        {name: 'Telephony', icon: 'resources/images/phone_icon.png', api: TestCase_Telephony, platformAvailable: true, iconCls:'bell'},
        {name: 'Internationalization', icon: 'resources/images/i18n_icon.png', api: TestCase_i18n, platformAvailable: true, iconCls:'browser'},
        {name: 'Analytics', icon: 'resources/images/analytics_icon.png', api: TestCase_Analytics, platformAvailable: (!!Appverse.Analytics), iconCls:'chart2'},
        {name: 'Security', icon: 'resources/images/security_icon.png', api: TestCase_Security, platformAvailable: true, iconCls:'lock'},
        {name: 'Scanner', icon: 'resources/images/scanner_icon.png', api: TestCase_Scanner, platformAvailable: (!!Appverse.Scanner), iconCls:'print'},
        {name: 'Webtrekk', icon: 'resources/images/webtrekk_icon.png', api: TestCase_Webtrekk, platformAvailable: (!!Appverse.Webtrekk), iconCls:'chart2'},
        {name: 'AppLoader', icon: 'resources/images/app_loader_icon.png', api: TestCase_AppLoader, platformAvailable: true, iconCls:'download'},
        {name: 'NFC', icon: 'resources/images/nfc_icon.png', api: TestCase_NFC, platformAvailable: (!Appverse.is.iOS && !!Appverse.NFC), iconCls:'rss'},
        {name: 'Beacon', icon: 'resources/images/beacon_icon.png', api: TestCase_Beacon, platformAvailable: (!!Appverse.Beacon), iconCls:'bell'},
        {name: 'PushNotifications', icon: 'resources/images/notification_icon.png', api: TestCase_PushNotification, platformAvailable: (!!Appverse.PushNotifications), iconCls:'flag'},
        {name: 'EndPoint Test', icon: '', api: null, platformAvailable: true, iconCls:'shuffle'},
		{name: 'AppsFlyer', icon: 'resources/images/analytics_icon.png', api: TestCase_AppsFlyer, platformAvailable: (!Appverse.is.Windows && !!Appverse.AppsFlyer), iconCls:'chart2'},
		{name: 'Adform', icon: 'resources/images/analytics_icon.png', api: TestCase_Adform, platformAvailable: (!Appverse.is.Windows && !!Appverse.Adform), iconCls:'chart2'}
    ]
}


