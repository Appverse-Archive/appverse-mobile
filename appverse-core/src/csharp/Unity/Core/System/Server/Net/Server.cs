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
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using Unity.Core.System.Server.Cryptography;
using System.Threading;

#if MONOTOUCH
using MonoTouch.Foundation;
#endif

namespace Unity.Core.System.Server.Net
{
	public class Server
	{
		ArrayList clients = new ArrayList ();
		Socket ss;

		public event ClientEvent Connect;
		public event ClientEvent ClientReady;

		public IEnumerable Clients {
			get { return clients; }
		}

		public Socket ServerSocket {
			get { return ss; }
		}

		public ClientInfo this [int id] {
			get {
				foreach (ClientInfo ci in Clients)
					if (ci.ID == id)
						return ci;
				return null;
			}
		}

		private EncryptionType encType;

		public EncryptionType DefaultEncryptionType {
			get { return encType; }
			set { encType = value; }
		}

		public int Port {
			get { return ((IPEndPoint)ss.LocalEndPoint).Port; }
		}

		public Server (int port) : this(port, null)
		{
		}

		public Server (int port, ClientEvent connDel)
		{
			Connect = connDel;
			
			ss = new Socket (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			ss.SendBufferSize = 1024;
			ss.ReceiveBufferSize = 256;
			ss.Bind (new IPEndPoint (IPAddress.Loopback, port));
			ss.Listen (25);
			
			
			// Start the accept process. When a connection is accepted, the callback
			// must do this again to accept another connection
			ss.BeginAccept (new AsyncCallback (AcceptCallback), ss);
			
			
			
		}

		internal void ClientClosed (ClientInfo ci)
		{
			clients.Remove (ci);
		}

		public void Broadcast (byte[] bytes)
		{
			foreach (ClientInfo ci in clients)
				ci.Send (bytes);
		}

		public void BroadcastMessage (uint code, byte[] bytes)
		{
			BroadcastMessage (code, bytes, 0);
		}

		public void BroadcastMessage (uint code, byte[] bytes, byte paramType)
		{
			foreach (ClientInfo ci in clients)
				ci.SendMessage (code, bytes, paramType);
		}


		// ASYNC CALLBACK CODE
		void AcceptCallback (IAsyncResult ar)
		{
			try {
				Socket server = (Socket)ar.AsyncState;
				Socket cs = server.EndAccept (ar);
				
				// Start the thing listening again
				server.BeginAccept (new AsyncCallback (AcceptCallback), server);
				
				ClientInfo c = new ClientInfo (cs, null, null, ClientDirection.Both, false);
				c.server = this;
				// Allow the new client to be rejected by the application
				if (Connect != null) {
					if (!Connect (this, c)) {
						// Rejected
						cs.Close ();
						return;
					}
				}
				// Initiate key exchange
				c.EncryptionType = encType;
				switch (encType) {
				case EncryptionType.None:
					KeyExchangeComplete (c);
					break;
				/*
				case EncryptionType.ServerKey:
					c.encKey = GetSymmetricKey ();
					byte[] key = ClientInfo.GetLengthEncodedVector (c.encKey);
					cs.Send (key);
					c.MakeEncoders ();
					KeyExchangeComplete (c);
					break;
				case EncryptionType.ServerRSAClientKey:
					RSACryptoServiceProvider rsa = new RSACryptoServiceProvider ();
					RSAParameters p = rsa.ExportParameters (true);
					cs.Send (ClientInfo.GetLengthEncodedVector (p.Modulus));
					cs.Send (ClientInfo.GetLengthEncodedVector (p.Exponent));
					c.encParams = p;
					break;
				*/
				default:
					throw new ArgumentException ("Unknown or unsupported encryption type " + encType);
				}
				clients.Add (c);
				
				
#if MONOTOUCH
				GetHandlerThread(c).Start();
#else 
				c.BeginReceive ();
#endif
				
			} catch (ObjectDisposedException) {
			} catch (SocketException) {
			} catch (Exception e) {
				SystemLogger.Log (SystemLogger.Module .CORE, null, e);
			}
		}

		public virtual HandlerThread GetHandlerThread (ClientInfo info)
		{
			return new HandlerThread (info);
		}
		
		protected virtual byte[] GetSymmetricKey ()
		{
			return EncryptionUtils.GetRandomBytes (24, false);
		}

		internal void KeyExchangeComplete (ClientInfo ci)
		{
			// Key exchange is complete on this client. Client ready
			// handlers may still force a close of the connection
			if (ClientReady != null)
			if (!ClientReady (this, ci))
				ci.Close ();
		}

		~Server ()
		{
			Close ();
		}
		
		public void Close ()
		{
			ArrayList cl2 = new ArrayList ();
			foreach (ClientInfo c in clients)
				cl2.Add (c);
			foreach (ClientInfo c in cl2)
				c.Close ();
			
			ss.Close ();
		}
	}
}
