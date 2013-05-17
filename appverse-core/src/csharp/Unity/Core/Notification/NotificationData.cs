using System;

namespace Unity.Core.Notification
{
	public class NotificationData
	{
		public NotificationData ()
		{
			Sound = "default"; // default alert sound
			Badge = -1;  // -1 value to indicate that the current badge should not be changed, 0 will indicate to remove the current badge icon.
		}

		public string AlertMessage { get;set; }

		public int Badge { get;set; }

		public string Sound { get;set; }

		public string CustomDataJsonString { get;set; } 

	}
}

