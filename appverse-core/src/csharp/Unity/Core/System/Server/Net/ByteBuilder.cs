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
using System;
using System.Collections.Generic;
using System.Text;

namespace Unity.Core.System.Server.Net
{
	public class ByteBuilder
	{
		byte[][] data;
		int packsize, used;

		public int Length {
			get {
				int len = 0;
				for (int i = 0; i < used; i++)
					len += data [i].Length;
				return len;
			}
		}

		public byte this [int i] {
			get { return Read (i, 1) [0]; }
		}

		public ByteBuilder () : this(10)
		{
		}

		public ByteBuilder (int packsize)
		{
			this.packsize = packsize;
			used = 0;
			data = new byte[packsize][];
		}

		public ByteBuilder (byte[] data)
		{
			packsize = 1;
			used = 1;
			this.data = new byte[][] { data };
		}

		public ByteBuilder (byte[] data, int len) : this(data, 0, len)
		{
		}

		public ByteBuilder (byte[] data, int from, int len)
            : this(1)
		{
			Add (data, from, len);
		}

		public void Add (byte[] moredata)
		{
			Add (moredata, 0, moredata.Length);
		}

		public void Add (byte[] moredata, int from, int len)
		{
			//SystemLogger.Log(SystemLogger.Module .CORE, "Getting "+from+" to "+(from+len-1)+" of "+moredata.Length);
			if (used < packsize) {
				data [used] = new byte[len];
				for (int j = from; j < from + len; j++)
					data [used] [j - from] = moredata [j];
				used++;
			} else {
				// Compress the existing items into the first array
				byte[] newdata = new byte[Length + len];
				int np = 0;
				for (int i = 0; i < used; i++)
					for (int j = 0; j < data[i].Length; j++)
						newdata [np++] = data [i] [j];
				for (int j = from; j < from + len; j++)
					newdata [np++] = moredata [j];
				data [0] = newdata;
				for (int i = 1; i < used; i++)
					data [i] = null;
				used = 1;
			}
		}

		public byte[] Read (int from, int len)
		{
			if (len == 0)
				return new byte[0];
			byte[] res = new byte[len];
			int done = 0, start = 0;

			for (int i = 0; i < used; i++) {
				if ((start + data [i].Length) <= from) {
					start += data [i].Length;
					continue;
				}
				// Now we're in the data block
				for (int j = 0; j < data[i].Length; j++) {
					if ((j + start) < from)
						continue;
					res [done++] = data [i] [j];
					if (done == len)
						return res;
				}
			}

			throw new ArgumentException ("Datapoints " + from + " and " + (from + len) + " must be less than " + Length);
		}

		public void Clear ()
		{
			used = 0;
			for (int i = 0; i < used; i++)
				data [i] = null;
		}

		public Parameter GetParameter (ref int index)
		{
			Parameter res = new Parameter ();
			res.Type = Read (index++, 1) [0];
			byte[] lenba = Read (index, 4);
			index += 4;
			int len = ClientInfo.GetInt (lenba, 0, 4);
			res.content = Read (index, len);
			index += len;
			return res;
		}

		public void AddParameter (Parameter param)
		{
			AddParameter (param.content, param.Type);
		}

		public void AddParameter (byte[] content, byte Type)
		{
			Add (new byte[] { Type });
			Add (ClientInfo.IntToBytes (content.Length));
			Add (content);
		}

		public static String FormatParameter (Parameter p)
		{
			switch (p.Type) {
			case ParameterType.Int:
				int[] ia = ClientInfo.GetIntArray (p.content);
				StringBuilder sb = new StringBuilder ();
				foreach (int i in ia)
					sb.Append (i + " ");
				return sb.ToString ();
			case ParameterType.Uint:
				ia = ClientInfo.GetIntArray (p.content);
				sb = new StringBuilder ();
				foreach (int i in ia)
					sb.Append (i.ToString ("X8") + " ");
				return sb.ToString ();
			case ParameterType.String:
				return Encoding.UTF8.GetString (p.content);
			case ParameterType.StringArray:
				string[] sa = ClientInfo.GetStringArray (p.content, Encoding.UTF8);
				sb = new StringBuilder ();
				foreach (string s in sa)
					sb.Append (s + "; ");
				return sb.ToString ();
			case ParameterType.Byte:
				sb = new StringBuilder ();
				foreach (int b in p.content)
					sb.Append (b.ToString ("X2") + " ");
				return sb.ToString ();
			default:
				return "??";
			}
		}
	}
}
