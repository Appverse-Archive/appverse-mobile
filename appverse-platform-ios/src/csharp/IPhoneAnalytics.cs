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
using Unity.Core.Analytics;
using GoogleAnalytics;
using Unity.Core.System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Unity.Platform.IPhone
{
	public class IPhoneAnalytics: AbstractAnalytics 
	{
		//Using Google Analytics SDK v3.01 Implemented 13-Nov-2013
		private GAITracker _tracker = null;
		
		private GAITracker SharedTracker(string webPropertyID) {

			if(this._tracker == null) {
				UIApplication.SharedApplication.InvokeOnMainThread (delegate {
					this._tracker = GAI.SharedInstance.TrackerWithTrackingId(webPropertyID);
					SystemLogger.Log(SystemLogger.Module.PLATFORM, "*** GANTracker.SharedTracker instance");
					if(this._tracker == null) {
						SystemLogger.Log(SystemLogger.Module.PLATFORM, "*** Instance returned by GANTracker is NULL. Please check your assembly linking");
					}
				});
			}
		
			return GAI.SharedInstance.DefaultTracker;
		}
		
		#region implemented abstract members of Unity.Core.Analytics.AbstractAnalytics
		public override bool StartTracking (string webPropertyID)
		{
			
			SystemLogger.Log(SystemLogger.Module.PLATFORM, "Starting tracking for account: " + webPropertyID);
			
			if (SharedTracker(webPropertyID) != null) {
				//int time=10;
	            try {
					UIApplication.SharedApplication.InvokeOnMainThread (delegate {
						try {
		                	/* deprecated - changes to compile with iOS 7 
		                	SharedTracker().StartTracker(webPropertyID, 1, null);
		                	SharedTracker().Dispatch();
		                	*/
							this._tracker.Set(GAIFields.SessionControl, "start");

							SystemLogger.Log(SystemLogger.Module.PLATFORM, "Tracking STARTED for account: " + webPropertyID);
						} catch (Exception e) {
	                		SystemLogger.Log(SystemLogger.Module.PLATFORM, "Error starting tracker", e);
	            		}
					});
					return true;
	              
	            } catch (Exception e) {
	                SystemLogger.Log(SystemLogger.Module.PLATFORM, "Couldn't start Analytics session", e);
	                return false;
	            }
		    }else
		    	return false;
			
		}

		public override bool StopTracking ()
		{
			
			SystemLogger.Log(SystemLogger.Module.PLATFORM, "Stopping Tracking");
			
			if (this._tracker != null) {
				UIApplication.SharedApplication.InvokeOnMainThread (delegate {
					try {
						/* deprecated - changes to compile with iOS 7 
            			this._tracker.StopTracker();
						this._tracker.Dispatch();
						*/
						this._tracker.Set(GAIFields.SessionControl, "end");
						this._tracker.Dispose();
						this._tracker = null;

						SystemLogger.Log(SystemLogger.Module.PLATFORM, "Tracking STOPPED");

					} catch (Exception e) {
	                	SystemLogger.Log(SystemLogger.Module.PLATFORM, "Error stopping tracker", e);
	            	}
				});
            	return true;
        	}else
				return false;
		}

		public override bool TrackEvent (string sGroup, string action, string label, int iValue)
		{
			SystemLogger.Log(SystemLogger.Module.PLATFORM, "Tracking event [" + sGroup + "-" + action + "-" + label + "-" + iValue);
			if (this._tracker != null) {
				try {
					UIApplication.SharedApplication.InvokeOnMainThread (delegate {
						try {
							/* deprecated - changes to compile with iOS 7 
							NSError error;
							this._tracker.TrackEvent(sGroup, action, label, iValue, out error);
	                		this._tracker.Dispatch();
	                		*/
							this._tracker.Send(GAIDictionaryBuilder.CreateEventWithCategory(sGroup,action,label,iValue).Build());
							GAI.SharedInstance.Dispatch();
							/*bool success = SharedTracker().TrackEvent(sGroup,action, label, iValue);
							if(success)  {
								SystemLogger.Log(SystemLogger.Module.PLATFORM, "Event TRACKED [" + sGroup + "-" + action + "-" + label + "-" + iValue);
							} else {
								SystemLogger.Log(SystemLogger.Module.PLATFORM, "Error tracking event (without further error information)");
							}*/
						} catch (Exception e) {
	                		SystemLogger.Log(SystemLogger.Module.PLATFORM, "Error tracking event", e);
	            		}
					});
	                return true;
	            } catch (Exception e) {
	                SystemLogger.Log(SystemLogger.Module.PLATFORM, "Couldn't track analytics event", e);
	                return false;
	            }
			}else
				return false;
		}

		public override bool TrackPageView (string relativeUrl)
		{
			SystemLogger.Log(SystemLogger.Module.PLATFORM, "Tracking Page View: " + relativeUrl);
			if (this._tracker != null) {
				
	            try {
					UIApplication.SharedApplication.InvokeOnMainThread (delegate {
						try {
							/* deprecated - changes to compile with iOS 7 
							NSError error;
							bool success = this._tracker.TrackPageView(relativeUrl, out error);
	               	 		this._tracker.Dispatch();
							if(!success){ 
								SystemLogger.Log(SystemLogger.Module.PLATFORM, "error:" + error.ToString());
							} else {
								SystemLogger.Log(SystemLogger.Module.PLATFORM, "Page View: " + relativeUrl);
							}

							bool success = SharedTracker().TrackView(relativeUrl);
							if(success){ 
								SystemLogger.Log(SystemLogger.Module.PLATFORM, "Page View TRACKED: " + relativeUrl);
							} else {
								SystemLogger.Log(SystemLogger.Module.PLATFORM, "Error tracking page view (without further error information)");
							}*/
							this._tracker.Set(GAIFields.ScreenName,relativeUrl);
							this._tracker.Send(GAIDictionaryBuilder.CreateAppView().Build());
						} catch (Exception e) {
	                		SystemLogger.Log(SystemLogger.Module.PLATFORM, "Error tracking page view", e);
	            		}
					});
					return true;
	            } catch (Exception e) {
	                SystemLogger.Log(SystemLogger.Module.PLATFORM, "Couldn't track analytics pageview [" + relativeUrl + "]", e);
	                return false;
	            }
        	}else
        		return false;
		}
		#endregion
	}
}

