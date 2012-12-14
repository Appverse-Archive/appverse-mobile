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
using System;
using System.Text;
using System.Net.Sockets;
//using System.Security.Cryptography;
using System.IO;
using Unity.Core.System.Server.Cryptography;

namespace Unity.Core.System.Server.Net
{
	public delegate void ConnectionRead (ClientInfo ci, String text);
	public delegate void ConnectionClosed (ClientInfo ci);
	public delegate void ConnectionReadBytes (ClientInfo ci, byte[] bytes, int len);
	public delegate void ConnectionReadMessage (ClientInfo ci, uint code, byte[] bytes, int len);
	public delegate void ConnectionReadPartialMessage (ClientInfo ci, uint code, byte[] bytes, int start, int read, int sofar, int totallength);
	public delegate void ConnectionNotify (ClientInfo ci);

	public class ClientInfo
	{
		internal Server server = null;
		private Socket sock;
		private String buffer;
		public event ConnectionRead OnRead;
		public event ConnectionClosed OnClose;
		public event ConnectionReadBytes OnReadBytes;
		public event ConnectionReadMessage OnReadMessage;
		public event ConnectionReadPartialMessage OnPartialMessage;
		//public event ConnectionNotify OnReady;
		public MessageType MessageType;
		private ClientDirection dir;
		int id;
		bool alreadyclosed = false;
		public static int NextID = 100;
		//private ClientThread t;
		public object Data = null;

		// Encryption info
		EncryptionType encType;
		int encRead = 0, encStage, encExpected;
		internal bool encComplete;
		internal byte[] encKey;
		//internal RSAParameters encParams;

		public EncryptionType EncryptionType {
			get { return encType; }
			set {
				if (encStage != 0)
					throw new ArgumentException ("Key exchange has already begun");
				encType = value;
				encComplete = encType == EncryptionType.None;
				encExpected = -1;
			}
		}
		public bool EncryptionReady {
			get { return encComplete; }
		}
		/*
		internal ICryptoTransform encryptor, decryptor;
		public ICryptoTransform Encryptor {
			get { return encryptor; }
		}
		public ICryptoTransform Decryptor {
			get { return decryptor; }
		}
		*/
		private string delim;
		public const int BUFSIZE = 1024;
		byte[] buf = new byte[BUFSIZE];
		ByteBuilder bytes = new ByteBuilder (10);

		byte[] msgheader = new byte[8];
		byte headerread = 0;
		bool wantingChecksum = true;

		public string Delimiter {
			get { return delim; }
			set { delim = value; }
		}

		public ClientDirection Direction {
			get { return dir; }
		}
		public Socket Socket {
			get { return sock; }
		}
		public Server Server {
			get { return server; }
		}
		public int ID {
			get { return id; }
		}

		public bool Closed {
			get { return !sock.Connected; }
		}

		public ClientInfo (Socket cl, bool StartNow) : this(cl, null, null, ClientDirection.Both, StartNow, EncryptionType.None)
		{
		}
		//public ClientInfo(Socket cl, ConnectionRead read, ConnectionReadBytes readevt, ClientDirection d) : this(cl, read, readevt, d, true, EncryptionType.None) {}
		public ClientInfo (Socket cl, ConnectionRead read, ConnectionReadBytes readevt, ClientDirection d, bool StartNow) : this(cl, read, readevt, d, StartNow, EncryptionType.None)
		{
		}
		public ClientInfo (Socket cl, ConnectionRead read, ConnectionReadBytes readevt, ClientDirection d, bool StartNow, EncryptionType encryptionType)
		{
			sock = cl;
			sock.SendTimeout=20000; // 20 seconds timeout
			sock.LingerState = new LingerOption(false,0);
			sock.NoDelay = true;
			// NOT WORKING IN EMULATOR sock.ExclusiveAddressUse = true;

			buffer = "";
			OnReadBytes = readevt;
			encType = encryptionType;
			encStage = 0;
			encComplete = encType == EncryptionType.None;
			OnRead = read;
			MessageType = MessageType.EndMarker;
			dir = d;
			delim = "\n";
			id = NextID;
			// Assign each client an application-unique ID
			unchecked {
				NextID++;
			}
			//t = new ClientThread(this);
			if (StartNow)
				BeginReceive ();
		}

		public void BeginReceive ()
		{
			sock.BeginReceive (buf, 0, BUFSIZE, 0, new AsyncCallback (ReadCallback), this);
		}
		
		public void Send (String text)
		{
			byte[] ba = Encoding.UTF8.GetBytes (text);
			Send (ba);
		}

		public void SendMessage (uint code, byte[] bytes)
		{
			SendMessage (code, bytes, 0, bytes.Length);
		}
		
		public void SendMessage (uint code, byte[] bytes, byte paramType)
		{
			SendMessage (code, bytes, paramType, bytes.Length);
		}
		
		public void SendMessage (uint code, byte[] bytes, byte paramType, int len)
		{
			if (paramType > 0) {
				ByteBuilder b = new ByteBuilder (3);
				b.AddParameter (bytes, paramType);
				bytes = b.Read (0, b.Length);
				len = bytes.Length;
			}
			
			
			lock (sock) {
				byte checksum = 0;
				byte[] ba;
				switch (MessageType) {
				case MessageType.CodeAndLength:
					
					Send (ba = UintToBytes (code));
					
					for (int i = 0; i < 4; i++) checksum += ba[i];
					Send (ba = IntToBytes (len));
					
					for (int i = 0; i < 4; i++) checksum += ba[i];
					if (encType != EncryptionType.None) {
						Send (new byte[] { checksum });
					}
					break;
				case MessageType.Length:
					Send (ba = IntToBytes (len));
					for (int i = 0; i < 4; i++) {
						checksum += ba[i];
					}
					if (encType != EncryptionType.None) {
						Send (new byte[] { checksum });
					}
					break;
				}
				Send (bytes, len);
				
				if (encType != EncryptionType.None) {
					checksum = 0;
					for (int i = 0; i < len; i++) {
						checksum += bytes[i];
					}
					Send (new byte[] { checksum });
				}
			}
			
		}
		public void Send (byte[] bytes)
		{
			Send (bytes, bytes.Length);
		}
		public void Send (byte[] bytes, int len)
		{
			/*
			if (!encComplete)
				throw new IOException ("Key exchange is not yet completed");
			if (encType != EncryptionType.None) {
				byte[] outbytes = new byte[len];
				Encryptor.TransformBlock (bytes, 0, len, outbytes, 0);
				bytes = outbytes;
			}
			*/

			sock.Send (bytes, len, SocketFlags.None);

		}
		/*
		public bool MessageWaiting ()
		{
			FillBuffer (sock);
			return buffer.IndexOf (delim) >= 0;
		}
	   */
		
		public String Read ()
		{
			int p = buffer.IndexOf (delim);
			if (p >= 0) {
				String res = buffer.Substring (0, p);
				buffer = buffer.Substring (p + delim.Length);
				return res;
			} else
				return "";
		}
		
		private void FillBuffer (Socket sock)
		{
			byte[] buf = new byte[256];
			int read;
			while (sock.Available != 0) {
				read = sock.Receive (buf);
				if (OnReadBytes != null)
					OnReadBytes (this, buf, read);
				buffer += Encoding.UTF8.GetString (buf, 0, read);
			}
		}

		void ReadCallback (IAsyncResult ar)
		{
			try {
				int read = sock.EndReceive (ar);
				//SystemLogger.Log(SystemLogger.Module .CORE, "Socket "+ID+" read "+read+" bytes");
				if (read > 0) {
					DoRead (buf, read);
					BeginReceive ();
				} else {
					SystemLogger.Log (SystemLogger.Module.CORE, ID + " zero byte read closure");
					Close ();
				}
			} catch (SocketException e) {
				SystemLogger.Log (SystemLogger.Module.CORE, ID + " socket exception closure: " + e);
				Close ();
			} catch (ObjectDisposedException) {
				SystemLogger.Log (SystemLogger.Module.CORE, ID + " disposed exception closure");
				Close ();
			}
		}

		internal void DoRead (byte[] buf, int read)
		{
			if (read > 0) {
				if (OnRead != null) {
					// Simple text mode
					buffer += Encoding.UTF8.GetString (buf, 0, read);
					while (buffer.IndexOf (delim) >= 0)
						OnRead (this, Read ());
				}
			}
			ReadInternal (buf, read, false);
		}

		/*
		public static void LogBytes (byte[] buf, int len)
		{
			byte[] ba = new byte[len];
			Array.Copy (buf, ba, len);
			SystemLogger.Log (SystemLogger.Module.CORE, ByteBuilder.FormatParameter (new Parameter (ba, ParameterType.Byte)));
		}
		*/
	
		void ReadInternal (byte[] buf, int read, bool alreadyEncrypted)
		{
			if ((!alreadyEncrypted) && (encType != EncryptionType.None)) {
				if (encComplete) {
					//SystemLogger.Log (SystemLogger.Module.CORE, ID + " Received: ");
					//LogBytes (buf, read);
					//buf = decryptor.TransformFinalBlock (buf, 0, read);
					//SystemLogger.Log (SystemLogger.Module.CORE, ID + " Decrypted: ");
					//LogBytes (buf, read);
				} else {
					// Client side key exchange
					int ofs = 0;
					if (encExpected < 0) {
						encStage++;
						ofs++;
						read--;
						encExpected = buf[0];
						// length of key to come
						encKey = new byte[encExpected];
						encRead = 0;
					}
					if (read >= encExpected) {
						Array.Copy (buf, ofs, encKey, encRead, encExpected);
						int togo = read - encExpected;
						encExpected = -1;
						SystemLogger.Log (SystemLogger.Module.CORE, ID + " Read encryption key: " + ByteBuilder.FormatParameter (new Parameter (encKey, ParameterType.Byte)));
						/*
						if (server == null)
							ClientEncryptionTransferComplete ();
						else
							ServerEncryptionTransferComplete ();
						*/
						if (togo > 0) {
							byte[] newbuf = new byte[togo];
							Array.Copy (buf, read + ofs - togo, newbuf, 0, togo);
							ReadInternal (newbuf, togo, false);
						}
					} else {
						Array.Copy (buf, ofs, encKey, encRead, read);
						encExpected -= read;
						encRead += read;
					}
					return;
				}
			}
			
			if ((!alreadyEncrypted) && (OnReadBytes != null))
				OnReadBytes (this, buf, read);
			
			if ((OnReadMessage != null) && (MessageType != MessageType.Unmessaged)) {
				// Messaged mode
				int copied;
				uint code = 0;
				switch (MessageType) {
				case MessageType.CodeAndLength:
				case MessageType.Length:
					int length;
					if (MessageType == MessageType.Length) {
						copied = FillHeader (ref buf, 4, read);
						if (headerread < 4)
							break;
						length = GetInt (msgheader, 0, 4);
					} else {
						copied = FillHeader (ref buf, 8, read);
						if (headerread < 8)
							break;
						code = (uint)GetInt (msgheader, 0, 4);
						length = GetInt (msgheader, 4, 4);
					}
					if (read == copied)
						break;
					// If encryption is on, the next byte is a checksum of the header
					int ofs = 0;
					if (wantingChecksum && (encType != EncryptionType.None)) {
						byte checksum = buf[0];
						ofs++;
						wantingChecksum = false;
						byte headersum = 0;
						for (int i = 0; i < 8; i++)
							headersum += msgheader[i];
						if (checksum != headersum) {
							Close ();
							throw new IOException ("Header checksum failed! (was " + checksum + ", calculated " + headersum + ")");
						}
					}
					bytes.Add (buf, ofs, read - ofs - copied);
					if (encType != EncryptionType.None)
						length++;
					// checksum byte
					// Now we know we are reading into the body of the message
					SystemLogger.Log (SystemLogger.Module.CORE, ID + " Added " + (read - ofs - copied) + " bytes, have " + bytes.Length + " of " + length);
					
					if (OnPartialMessage != null)
						OnPartialMessage (this, code, buf, ofs, read - ofs - copied, bytes.Length, length);
					
					if (bytes.Length >= length) {
						// A message was received!
						headerread = 0;
						wantingChecksum = true;
						byte[] msg = bytes.Read (0, length);
						if (encType != EncryptionType.None) {
							byte checksum = msg[length - 1], msgsum = 0;
							for (int i = 0; i < length - 1; i++)
								msgsum += msg[i];
							if (checksum != msgsum) {
								Close ();
								throw new IOException ("Content checksum failed! (was " + checksum + ", calculated " + msgsum + ")");
							}
							OnReadMessage (this, code, msg, length - 1);
						} else
							OnReadMessage (this, code, msg, length);
						// Don't forget to put the rest through the mill
						int togo = bytes.Length - length;
						if (togo > 0) {
							byte[] whatsleft = bytes.Read (length, togo);
							bytes.Clear ();
							ReadInternal (whatsleft, whatsleft.Length, true);
						} else
							bytes.Clear ();
					}
					//if(OnStatus != null) OnStatus(this, bytes.Length, length);
					break;
				}
			}
		}
		
		int FillHeader (ref byte[] buf, int to, int read)
		{
			int copied = 0;
			if (headerread < to) {
				// First copy the header into the header variable.
				for (int i = 0; (i < read) && (headerread < to); i++,headerread++,copied++) {
					msgheader[headerread] = buf[i];
				}
			}
			if (copied > 0) {
				// Take the header bytes off the 'message' section
				byte[] newbuf = new byte[read - copied];
				for (int i = 0; i < newbuf.Length; i++)
					newbuf[i] = buf[i + copied];
				buf = newbuf;
			}
			return copied;
		}
		/*
		internal ICryptoTransform MakeEncryptor ()
		{
			return MakeCrypto (true);
		}
		internal ICryptoTransform MakeDecryptor ()
		{
			return MakeCrypto (false);
		}
		internal ICryptoTransform MakeCrypto (bool encrypt)
		{
			if (encrypt)
				return new SimpleEncryptor (encKey);
			else
				return new SimpleDecryptor (encKey);
		}
		*/
		/*
		void ServerEncryptionTransferComplete ()
		{
			switch (encType) {
			case EncryptionType.None:
				throw new ArgumentException ("Should not have key exchange for unencrypted connection!");
			case EncryptionType.ServerKey:
				throw new ArgumentException ("Should not have server-side key exchange for server keyed connection!");
			case EncryptionType.ServerRSAClientKey:
				// Symmetric key is in RSA-encoded encKey
				RSACryptoServiceProvider rsa = new RSACryptoServiceProvider ();
				rsa.ImportParameters (encParams);
				encKey = rsa.Decrypt (encKey, false);
				SystemLogger.Log (SystemLogger.Module.CORE, "Symmetric key is: ");
				//LogBytes (encKey, encKey.Length);
				
				MakeEncoders ();
				server.KeyExchangeComplete (this);
				break;
			}
		}
		*/
		
		/*
		void ClientEncryptionTransferComplete ()
		{
			// A part of the key exchange process has been completed, and the key is
			// in encKey
			switch (encType) {
			case EncryptionType.None:
				throw new ArgumentException ("Should not have key exchange for unencrypted connection!");
			case EncryptionType.ServerKey:
				// key for transfer is now in encKey, so all is good
				MakeEncoders ();
				break;
			case EncryptionType.ServerRSAClientKey:
				// Stage 1: modulus; Stage 2: exponent
				// When the exponent arrives, create a random DES key
				// and send it
				switch (encStage) {
				case 1:
					encParams.Modulus = encKey;
					break;
				case 2:
					encParams.Exponent = encKey;
					RSACryptoServiceProvider rsa = new RSACryptoServiceProvider ();
					rsa.ImportParameters (encParams);
					encKey = EncryptionUtils.GetRandomBytes (24, false);
					byte[] send = GetLengthEncodedVector (rsa.Encrypt (encKey, false));
					sock.Send (send);
					MakeEncoders ();
					break;
				}

				break;
			}
		}
		*/
		/*
		internal void MakeEncoders ()
		{
			encryptor = MakeEncryptor ();
			decryptor = MakeDecryptor ();
			if (OnReady != null)
				OnReady (this);
			encComplete = true;
		}
		*/
		public static byte[] GetLengthEncodedVector (byte[] @from)
		{
			int l = @from.Length;
			if (l > 255)
				throw new ArgumentException ("Cannot length encode more than 255");
			byte[] to = new byte[l + 1];
			to[0] = (byte)l;
			Array.Copy (@from, 0, to, 1, l);
			return to;
		}

		public static int GetInt (byte[] ba, int @from, int len)
		{
			int r = 0;
			for (int i = 0; i < len; i++)
				r += ba[@from + i] << ((len - i - 1) * 8);
			return r;
		}

		public static int[] GetIntArray (byte[] ba)
		{
			return GetIntArray (ba, 0, ba.Length);
		}
		public static int[] GetIntArray (byte[] ba, int @from, int len)
		{
			int[] res = new int[len / 4];
			for (int i = 0; i < res.Length; i++) {
				res[i] = GetInt (ba, @from + (i * 4), 4);
			}
			return res;
		}

		public static uint[] GetUintArray (byte[] ba)
		{
			uint[] res = new uint[ba.Length / 4];
			for (int i = 0; i < res.Length; i++) {
				res[i] = (uint)GetInt (ba, i * 4, 4);
			}
			return res;
		}

		public static byte[] IntToBytes (int val)
		{
			return UintToBytes ((uint)val);
		}
		public static byte[] UintToBytes (uint val)
		{
			byte[] res = new byte[4];
			for (int i = 3; i >= 0; i--) {
				res[i] = (byte)val;
				val >>= 8;
			}
			return res;
		}

		public static byte[] IntArrayToBytes (int[] val)
		{
			byte[] res = new byte[val.Length * 4];
			for (int i = 0; i < val.Length; i++) {
				byte[] vb = IntToBytes (val[i]);
				res[(i * 4)] = vb[0];
				res[(i * 4) + 1] = vb[1];
				res[(i * 4) + 2] = vb[2];
				res[(i * 4) + 3] = vb[3];
			}
			return res;
		}

		public static byte[] UintArrayToBytes (uint[] val)
		{
			byte[] res = new byte[val.Length * 4];
			for (uint i = 0; i < val.Length; i++) {
				byte[] vb = IntToBytes ((int)val[i]);
				res[(i * 4)] = vb[0];
				res[(i * 4) + 1] = vb[1];
				res[(i * 4) + 2] = vb[2];
				res[(i * 4) + 3] = vb[3];
			}
			return res;
		}

		public static byte[] StringArrayToBytes (string[] val, Encoding e)
		{
			byte[][] baa = new byte[val.Length][];
			int l = 0;
			for (int i = 0; i < val.Length; i++) {
				baa[i] = e.GetBytes (val[i]);
				l += 4 + baa[i].Length;
			}
			byte[] r = new byte[l + 4];
			IntToBytes (val.Length).CopyTo (r, 0);
			int ofs = 4;
			for (int i = 0; i < baa.Length; i++) {
				IntToBytes (baa[i].Length).CopyTo (r, ofs);
				ofs += 4;
				baa[i].CopyTo (r, ofs);
				ofs += baa[i].Length;
			}
			return r;
		}

		public static string[] GetStringArray (byte[] ba, Encoding e)
		{
			int l = GetInt (ba, 0, 4), ofs = 4;
			string[] r = new string[l];
			for (int i = 0; i < l; i++) {
				int thislen = GetInt (ba, ofs, 4);
				ofs += 4;
				r[i] = e.GetString (ba, ofs, thislen);
				ofs += thislen;
			}
			return r;
		}

		public void Close ()
		{
			if (!alreadyclosed) {
				if (server != null)
					server.ClientClosed (this);
				if (OnClose != null)
					OnClose (this);
				alreadyclosed = true;
				SystemLogger.Log (SystemLogger.Module.CORE, "**closed client** at " + DateTime.Now.Ticks);
				
			}
			sock.Close ();
		}
	}
	
	
	
}
