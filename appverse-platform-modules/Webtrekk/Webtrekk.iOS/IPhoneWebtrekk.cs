/*
 Copyright (c) 2012 GFT Appverse, S.L., Sociedad Unipersonal.

 This Source  Code Form  is subject to the  terms of  the Appverse Public License 
 Version 2.0  ("APL v2.0").  If a copy of  the APL  was not  distributed with this 
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
using Appverse.Core.Webtrekk;
using Foundation;
using UIKit;
using System.Threading;

namespace Appverse.Platform.IPhone
{
	public class IPhoneWebtrekk:AbstractWebtrekk
	{
		/*
		public Webtrekk _tracker = null;
		
		public IPhoneWebtrekk ()
		{
		}

		private Webtrekk.Webtrekk SharedTracker(string webServerUrl, string trackId) {
			if(this._tracker == null && webServerUrl!=String.Empty && trackId!=String.Empty) {
				UIApplication.SharedApplication.InvokeOnMainThread (delegate {
					try {
						this._tracker = Webtrekk.Webtrekk.webtrekkWithServerUrl(webServerUrl,trackId);
						SystemLogger.Log(SystemLogger.Module.PLATFORM, "Tracking STARTED for account: " + webServerUrl);
						if(this._tracker == null) {
							SystemLogger.Log(SystemLogger.Module.PLATFORM, "*** Instance returned by Webtrekk is NULL. Please check your assembly linking");
						}
					} catch (Exception e) {
						SystemLogger.Log(SystemLogger.Module.PLATFORM, "Error getting Webtrekk tracker with server url", e);
					}
				});
			}
			
			return this._tracker;
		}
		
		private Webtrekk.Webtrekk SharedTracker(string webServerUrl, string trackId, string samplingRate) {
			if(this._tracker == null && webServerUrl!=String.Empty && trackId!=String.Empty && samplingRate!=String.Empty) {
				UIApplication.SharedApplication.InvokeOnMainThread (delegate {
					try {
						this._tracker = Webtrekk.Webtrekk.webtrekkWithServerUrl(webServerUrl,trackId, samplingRate);
						SystemLogger.Log(SystemLogger.Module.PLATFORM, "Tracking STARTED for account: " + webServerUrl);
						if(this._tracker == null) {
							SystemLogger.Log(SystemLogger.Module.PLATFORM, "*** Instance returned by Webtrekk is NULL. Please check your assembly linking");
						}
					} catch (Exception e) {
						SystemLogger.Log(SystemLogger.Module.PLATFORM, "Error getting Webtrekk tracker with server url", e);
					}
				});
			}
			
			return this._tracker;
		}

		#region implemented abstract members of Unity.Core.Webtrekk.AbstractWebtrekk
		[Export("SetRequestIntervalThread")]
		private void SetRequestIntervalThread (object dataObject)
		{
			double intervalInSeconds = (double)dataObject;

			SystemLogger.Log (SystemLogger.Module.PLATFORM, "Changing Webtrekk Tracking request interval to: " + intervalInSeconds + " seconds.");
			if (this._tracker != null && intervalInSeconds> 0) {
				try {
					UIApplication.SharedApplication.InvokeOnMainThread (delegate {
						try {
							this._tracker.SetRequestInterval(intervalInSeconds);
							SystemLogger.Log(SystemLogger.Module.PLATFORM, "Tracking request interval set to : " + intervalInSeconds + " seconds.");
						} catch (Exception e) {
							SystemLogger.Log(SystemLogger.Module.PLATFORM, "Error setting request interval", e);
						}
					});	
				} catch (Exception e) {
					SystemLogger.Log(SystemLogger.Module.PLATFORM, "Couldn't change Webtrekk request interval", e);
				}
			}else {
				SystemLogger.Log(SystemLogger.Module.PLATFORM, "*** No Webtrekk tracker available, please execute StartTracking prior to set this interval setting");
			}
		}
		

		[Export("StartTrackingThread")]
		private void StartTrackingThread (object dataObject)
		{
			object[] data = (object[])dataObject;
			string webServerUrl = (string)data [0];
			string trackId = (string)data [1];
			string samplingRate = String.Empty;
			if (data.Length > 2)
				samplingRate = (string)data [2];

			try {
				if (samplingRate != String.Empty) {
					SystemLogger.Log (SystemLogger.Module.PLATFORM, "Starting tracking for account: " + webServerUrl + " Tracking Id: " + trackId + " Sampling Rate: " + samplingRate);
			
					if (SharedTracker (webServerUrl, trackId, samplingRate) != null) {
						UIApplication.SharedApplication.InvokeOnMainThread (delegate {
							try {
								this._tracker.StartSession ();
							} catch (Exception e) {
								SystemLogger.Log (SystemLogger.Module.PLATFORM, "Error starting session with sampling rate", e);
							}
						});
					}
				}else{
					SystemLogger.Log (SystemLogger.Module.PLATFORM, "Starting tracking for account: " + webServerUrl + " Tracking Id: " + trackId);
					if (SharedTracker(webServerUrl, trackId) != null) {
						UIApplication.SharedApplication.InvokeOnMainThread (delegate {
							try {
								this._tracker.StartSession();
							} catch (Exception e) {
								SystemLogger.Log(SystemLogger.Module.PLATFORM, "Error starting session", e);
							}
						});
					}
				}
			} catch (Exception e) {
				SystemLogger.Log (SystemLogger.Module.PLATFORM, "Couldn't start Webtrekk session", e);
			}
		}

		[Export("StopTrackingThread")]
		private void StopTrackingThread ()
		{
			SystemLogger.Log(SystemLogger.Module.PLATFORM, "Stopping Webtrekk Tracking");
			if (this._tracker != null) {
				UIApplication.SharedApplication.InvokeOnMainThread (delegate {
					try{
						this._tracker.EndSession();					
						SystemLogger.Log(SystemLogger.Module.PLATFORM, "Webtrekk Tracking STOPPED");					
						this._tracker.Dispose();
						this._tracker = null;
					} catch (Exception e) {
						SystemLogger.Log(SystemLogger.Module.PLATFORM, "Error stopping session", e);
					}
				});
			}
		}

		[Export("TrackClickThread")]
		private void TrackClickThread (object dataObject)
		{
			object[] data = (object[])dataObject;
			string clickId = (string)data[0]; 
			string contentId = (string)data[1];
			WebtrekkParametersCollection additionalParameters = null;
			if(data.Length>2)
				additionalParameters = (WebtrekkParametersCollection)data[2];

			if (this._tracker != null) {
				try {
					if(additionalParameters!=null){
						SystemLogger.Log(SystemLogger.Module.PLATFORM, "Webtrekk Tracking click [" + clickId + "-" + contentId + " - additional parameters]");
						NSDictionary dict = ParametersCollectionToDictionary(additionalParameters);

						UIApplication.SharedApplication.InvokeOnMainThread (delegate {
							try {
								this._tracker.TrackClick(clickId,contentId,dict);
								SystemLogger.Log(SystemLogger.Module.PLATFORM, "Webtrekk click TRACKED [" + clickId + "-" + contentId + " with additional parameters]");
							} catch (Exception e) {
								SystemLogger.Log(SystemLogger.Module.PLATFORM, "Error tracking click with additional parameters", e);
							}
						});
					}else{
						SystemLogger.Log(SystemLogger.Module.PLATFORM, "Webtrekk Tracking click [" + clickId + "-" + contentId + "]");
						UIApplication.SharedApplication.InvokeOnMainThread (delegate {
							try {
								this._tracker.TrackClick(clickId,contentId);
								SystemLogger.Log(SystemLogger.Module.PLATFORM, "Webtrekk click TRACKED [" + clickId + "-" + contentId + "]");
							} catch (Exception e) {
								SystemLogger.Log(SystemLogger.Module.PLATFORM, "Error tracking click", e);
							}
						});
					}
				} catch (Exception e) {
					SystemLogger.Log(SystemLogger.Module.PLATFORM, "Couldn't track Webtrekk click", e);
				}
			}
		}

		[Export("TrackContentThread")]
		private void TrackContentThread (object dataObject)
		{
			object[] data = (object[])dataObject;
			string contentId = (string)data[0];
			WebtrekkParametersCollection additionalParameters = null;
			if(data.Length>1)
				additionalParameters = (WebtrekkParametersCollection)data[1];

			if (this._tracker != null) {
				try {
					if(additionalParameters!= null)
					{
						SystemLogger.Log(SystemLogger.Module.PLATFORM, "Webtrekk Tracking content [" + contentId + " with additional parameters]");
						NSDictionary dict = ParametersCollectionToDictionary(additionalParameters);					
						UIApplication.SharedApplication.InvokeOnMainThread (delegate {
							try {
								this._tracker.TrackContent(contentId, dict);
								SystemLogger.Log(SystemLogger.Module.PLATFORM, "Webtrekk content TRACKED [" + contentId + "] with additional parameters");
							} catch (Exception e) {
								SystemLogger.Log(SystemLogger.Module.PLATFORM, "Error tracking content with additional parameters", e);
							}
						});
					}else{
						SystemLogger.Log(SystemLogger.Module.PLATFORM, "Webtrekk Tracking content [" + contentId + "]");
						UIApplication.SharedApplication.InvokeOnMainThread (delegate {
							try {
								this._tracker.TrackContent(contentId);
								SystemLogger.Log(SystemLogger.Module.PLATFORM, "Webtrekk content TRACKED [" + contentId + "]");
							} catch (Exception e) {
								SystemLogger.Log(SystemLogger.Module.PLATFORM, "Error tracking content", e);
							}
						});
					}
				} catch (Exception e) {
					SystemLogger.Log(SystemLogger.Module.PLATFORM, "Couldn't track Webtrekk content", e);
				}
			}
		}
#endregion
*/
		#region Thread Callers
		public override bool SetRequestInterval (double intervalInSeconds)
		{
			SystemLogger.Log(SystemLogger.Module.PLATFORM, "Feature not available. Webtrekk library is not compatible with Xamarin.iOS dll");
			return false;

			/*
			using (var pool = new NSAutoreleasePool ()) {
				Thread thread = new Thread (SetRequestIntervalThread);
				thread.Priority = ThreadPriority.AboveNormal;
				thread.Start ((object)intervalInSeconds);
			}
			return true;
			*/
		}

		public override bool StopTracking ()
		{
			SystemLogger.Log(SystemLogger.Module.PLATFORM, "Feature not available. Webtrekk library is not compatible with Xamarin.iOS dll");
			return false;

			/*
			using (var pool = new NSAutoreleasePool ()) {
				Thread thread = new Thread (StopTrackingThread);
				thread.Priority = ThreadPriority.AboveNormal;
				thread.Start ();
			}
			return true;
			*/
		}

		public override bool StartTracking (string webServerUrl, string trackId, string samplingRate)
		{
			SystemLogger.Log(SystemLogger.Module.PLATFORM, "Feature not available. Webtrekk library is not compatible with Xamarin.iOS dll");
			return false;

			/*
			using (var pool = new NSAutoreleasePool ()) {
				Thread thread = new Thread (StartTrackingThread);
				thread.Priority = ThreadPriority.AboveNormal;
				thread.Start (new object[]{webServerUrl, trackId, samplingRate});
			}
			return true;
			*/
		}

		public override bool StartTracking (string webServerUrl, string trackId)
		{
			SystemLogger.Log(SystemLogger.Module.PLATFORM, "Feature not available. Webtrekk library is not compatible with Xamarin.iOS dll");
			return false;

			/*
			using (var pool = new NSAutoreleasePool ()) {
				Thread thread = new Thread (StartTrackingThread);
				thread.Priority = ThreadPriority.AboveNormal;
				thread.Start (new object[]{webServerUrl, trackId});
			}
			return true;
			*/
		}

		public override bool TrackContent (string contentId, WebtrekkParametersCollection additionalParameters)
		{
			SystemLogger.Log(SystemLogger.Module.PLATFORM, "Feature not available. Webtrekk library is not compatible with Xamarin.iOS dll");
			return false;

			/*
			using (var pool = new NSAutoreleasePool ()) {
				Thread thread = new Thread (TrackContentThread);
				thread.Priority = ThreadPriority.AboveNormal;
				thread.Start (new object[]{contentId, additionalParameters});
			}
			return true;
			*/
		}

		public override bool TrackContent (string contentId)
		{
			SystemLogger.Log(SystemLogger.Module.PLATFORM, "Feature not available. Webtrekk library is not compatible with Xamarin.iOS dll");
			return false;

			/*
			using (var pool = new NSAutoreleasePool ()) {
				Thread thread = new Thread (TrackContentThread);
				thread.Priority = ThreadPriority.AboveNormal;
				thread.Start (new object[]{contentId});
			}
			return true;
			*/
		}

		public override bool TrackClick (string clickId, string contentId, WebtrekkParametersCollection additionalParameters)
		{
			SystemLogger.Log(SystemLogger.Module.PLATFORM, "Feature not available. Webtrekk library is not compatible with Xamarin.iOS dll");
			return false;

			/*
			using (var pool = new NSAutoreleasePool ()) {
				Thread thread = new Thread (TrackClickThread);
				thread.Priority = ThreadPriority.AboveNormal;
				thread.Start (new object[]{clickId, contentId, additionalParameters});
			}
			return true;
			*/
		}

		public override bool TrackClick (string clickId, string contentId)
		{
			SystemLogger.Log(SystemLogger.Module.PLATFORM, "Feature not available. Webtrekk library is not compatible with Xamarin.iOS dll");
			return false;

			/*
			using (var pool = new NSAutoreleasePool ()) {
				Thread thread = new Thread (TrackClickThread);
				thread.Priority = ThreadPriority.AboveNormal;
				thread.Start (new object[]{clickId, contentId});
			}
			return true;
			*/
		}
		#endregion
		/*
		private NSDictionary ParametersCollectionToDictionary (WebtrekkParametersCollection paramCollection)
		{
			try {
				NSDictionary dict = null;
				string[] keys = new string[paramCollection.AdditionalParameters.Length];
				string[] values = new string[paramCollection.AdditionalParameters.Length];
				for (int i = 0; i < paramCollection.AdditionalParameters.Length; i++) {
					keys [i] = paramCollection.AdditionalParameters [i].Name;
					values [i] = paramCollection.AdditionalParameters [i].Value;
				}
				dict = NSDictionary.FromObjectsAndKeys (values, keys);
				return dict;
			} catch (Exception e) {
				SystemLogger.Log(SystemLogger.Module.PLATFORM, "Error parsing Webtrekk Additional Parameters", e);
				return null;
			}
		}
		*/
	}
}