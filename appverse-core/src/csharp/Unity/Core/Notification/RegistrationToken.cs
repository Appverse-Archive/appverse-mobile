using System;

namespace Unity.Core.Notification
{
	public class RegitrationToken
	{
		public RegitrationToken ()
		{
		}

		public byte[] Binary { get; set; }
		
		public int BinaryLength {
			get {
				if (Binary != null) {
					return Binary.Length;
				} else {
					return 0;
				}
			}
		}

		public string StringRepresentation { get; set; }

	}
}

