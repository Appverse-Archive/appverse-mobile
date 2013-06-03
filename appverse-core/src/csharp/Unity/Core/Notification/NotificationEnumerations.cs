using System;

namespace Unity.Core.Notification
{
	public enum RemoteNotificationType {
		NONE,
		BADGE,
		SOUND,
		ALERT,
		CONTENT_AVAILABILITY
	}

	public enum RepeatInterval {
		NO_REPEAT,
		HOURLY,
		DAILY,
		WEEKLY,
		MONTHLY,
		YEARLY
	}
}

