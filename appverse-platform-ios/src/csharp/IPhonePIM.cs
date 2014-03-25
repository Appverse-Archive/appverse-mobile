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
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using MonoTouch.AddressBook;
using MonoTouch.AddressBookUI;
using MonoTouch.Foundation;
using Unity.Core.Pim;
using Unity.Core.System;
using System.Threading;
using MonoTouch.UIKit;
using MonoTouch.EventKit;
using MonoTouch.EventKitUI;
using System.Text;
using System.Linq;
using Unity.Core.Notification;

namespace Unity.Platform.IPhone
{
	public class IPhonePIM : AbstractPim
	{

		#region CONTACTS API
		public static ABPersonSortBy DEFAULT_CONTACTS_LIST_SORT = ABPersonSortBy.FirstName;
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="contactData">
		/// A <see cref="Contact"/>
		/// </param>
		/// <returns>
		/// A <see cref="Contact"/>
		/// </returns>
		public override Contact CreateContact (Contact contactData)
		{
			if(contactData!=null) {

				if (UIDevice.CurrentDevice.CheckSystemVersion (6, 0)) {
					RequestAccessToContacts(() => { LaunchCreateNewContact (contactData); } );
				} else {
					using (var pool = new NSAutoreleasePool ()) {
						var thread = new Thread (ShowCreateContactView);
						thread.Start (contactData);
					};
				}

			}
			// TODO return Contact object should return the inserted ID.
			return contactData;
		}
		
		[Export("ShowCreateContactView")]
		private void ShowCreateContactView (object contactObject)
		{
			UIApplication.SharedApplication.InvokeOnMainThread (delegate {
				Contact contact  = (Contact)contactObject;
				LaunchCreateNewContact(contact);
			});
		}

		/// <summary>
		/// A convenience method that requests access to the appropriate contacts ap and 
		/// shows an alert if access is not granted, otherwise executes the completion 
		/// method.
		/// </summary>
		protected void RequestAccessToContacts ( Action completion )
		{
			if(IPhoneServiceLocator.CurrentDelegate.AddressBook != null) {
				IPhoneServiceLocator.CurrentDelegate.AddressBook.RequestAccess ( (bool granted, NSError e) => {
					if (granted) {
						UIApplication.SharedApplication.InvokeOnMainThread ( () => { completion.Invoke(); } );
					} else {
						NotifyDeniedAccessToContacts();
					}
				} );
			} else {
				NotifyDeniedAccessToContacts();
			}
		}

		/// <summary>
		/// Notifies the denied access to contacts.
		/// </summary>
		private void NotifyDeniedAccessToContacts() {
			INotification notificationService = (INotification)IPhoneServiceLocator.GetInstance ().GetService ("notify");
			if (notificationService != null) {
				notificationService.StartNotifyAlert ("Access Denied", "User Denied Access to Contacts. Go to Privacy Settings to change it.", "OK");
			}
		}

		/// <summary>
		/// Launchs the create new contact.
		/// </summary>
		/// <param name='contact'>
		/// Contact.
		/// </param>
		/// 
		protected void LaunchCreateNewContact(Contact contact) {

			ABPerson person = new ABPerson();
			// Basic Info
			person.FirstName = contact.Name;
			person.MiddleName = contact.Firstname;
			person.LastName = contact.Lastname;
			person.Nickname = contact.DisplayName;
			person.Note = contact.Notes;
			
			// Work info
			person.Organization = contact.Company;
			person.JobTitle = contact.JobTitle;
			person.Department =	contact.Department;
			
			// Addresses
			person.SetAddresses(this.ConvertContactAddressesToNative(contact.Addresses));
			
			// Phones
			person.SetPhones(this.ConvertContactPhonesToNative(contact.Phones));
			
			// Emails
			person.SetEmails(this.ConvertContactEmailsToNative(contact.Emails));
			
			// Websites
			person.SetUrls(this.ConvertContactWebsitesToNative(contact.WebSites));
			
			// Photo
			if(contact.Photo==null && contact.PhotoBase64Encoded!=null) {
				contact.Photo = Convert.FromBase64String(contact.PhotoBase64Encoded);
			}
			if(contact.Photo!=null) {
				NSData imageData = this.ConvertContactBinaryDataToNative(contact.Photo);
				if(imageData!=null && imageData.Length>0) {
					person.Image = imageData;
				}
			}
			
			ABUnknownPersonViewController unknownPersonViewController = new ABUnknownPersonViewController();
			unknownPersonViewController.AddressBook = IPhoneServiceLocator.CurrentDelegate.AddressBook;
			unknownPersonViewController.AllowsActions = false;
			unknownPersonViewController.AllowsAddingToAddressBook = true;
			unknownPersonViewController.DisplayedPerson = person;
			unknownPersonViewController.PerformDefaultAction += HandlePerformDefaultAction;
			unknownPersonViewController.PersonCreated += HandleUnknownPersonCreated;
			unknownPersonViewController.Delegate = new UnknownPersonViewControllerDelegate();
			UIBarButtonItem backButtonItem = new UIBarButtonItem();
			backButtonItem.Title = "Cancel";
			backButtonItem.Clicked += HandleBackButtonItemClicked;
			unknownPersonViewController.NavigationItem.SetLeftBarButtonItem(backButtonItem,false);
			
			UINavigationController contactNavController = new UINavigationController();
			contactNavController.PushViewController(unknownPersonViewController,false);
			
			IPhoneServiceLocator.CurrentDelegate.MainUIViewController ().PresentModalViewController (contactNavController, true);
			IPhoneServiceLocator.CurrentDelegate.SetMainUIViewControllerAsTopController(false);
		}

		void HandleBackButtonItemClicked (object sender, EventArgs e)
		{
			UIApplication.SharedApplication.InvokeOnMainThread (delegate {
				IPhoneServiceLocator.CurrentDelegate.MainUIViewController ().DismissModalViewControllerAnimated(true);
			});
		}

		private class UnknownPersonViewControllerDelegate : ABUnknownPersonViewControllerDelegate {
			
			public override void DidResolveToPerson (ABUnknownPersonViewController personViewController, IntPtr person)
			{
				UIApplication.SharedApplication.InvokeOnMainThread (delegate {
					SystemLogger.Log(SystemLogger.Module.PLATFORM, "On UnknownPersonViewControllerDelegate::DidResolveToPerson");
					personViewController.DismissModalViewControllerAnimated(true);
					IPhoneServiceLocator.CurrentDelegate.MainUIViewController ().DismissModalViewControllerAnimated(true);
				});
			}

			public override bool ShouldPerformDefaultActionForPerson (ABUnknownPersonViewController personViewController, IntPtr person, int propertyId, int identifier)
			{
				UIApplication.SharedApplication.InvokeOnMainThread (delegate {
					SystemLogger.Log(SystemLogger.Module.PLATFORM, "On UnknownPersonViewControllerDelegate::ShouldPerformDefaultActionForPerson");
				});
				return true;
			}			
		}
		
		void HandleUnknownPersonCreated (object sender, ABUnknownPersonCreatedEventArgs e)
		{
			UIApplication.SharedApplication.InvokeOnMainThread (delegate {
				SystemLogger.Log(SystemLogger.Module.PLATFORM, "On HandleUnknownPersonCreated: " + e.Person);
				IPhoneServiceLocator.CurrentDelegate.MainUIViewController ().DismissModalViewControllerAnimated(true);
			});
		}

		void HandlePerformDefaultAction (object sender, ABPersonViewPerformDefaultActionEventArgs e)
		{
			UIApplication.SharedApplication.InvokeOnMainThread (delegate {
				SystemLogger.Log(SystemLogger.Module.PLATFORM, "On HandlePerformDefaultAction: " + e.Person);
				e.ShouldPerformDefaultAction = true;
			});
		}


		

		/// <summary>
		/// 
		/// </summary>
		/// <param name="id">
		/// A <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// A <see cref="Contact"/>
		/// </returns>
		public override Contact GetContact (string id)
		{

			ABAddressBook addressBook = IPhoneServiceLocator.CurrentDelegate.AddressBook;
			// Gets all people in the address book
			ABPerson[] people = addressBook.GetPeople();


			if(people != null) {
				// sort list by FirstName (default)
				Array.Sort(people, delegate(ABPerson person1, ABPerson person2) {
					return person1.CompareTo(person2, DEFAULT_CONTACTS_LIST_SORT);
				});

				ABPerson person = people.First (x => x.Id.ToString() == id);
			
				if(person!=null){
					Contact contact = new Contact();

					// Basic Info
					contact.ID = "" + person.Id;
					contact.Name = person.FirstName;
					contact.Firstname = person.MiddleName;
					contact.Lastname = person.LastName;
					contact.DisplayName = person.Nickname;
					contact.Notes = person.Note;

					// TODO how to get the group(s) this person belongs to
					contact.Group = person.Organization;

					// Work info
					contact.Company = person.Organization;
					contact.JobTitle = person.JobTitle;
					contact.Department = person.Department;

					// Addresses
					contact.Addresses = this.GetContactAdresses(person);

					// Phones
					contact.Phones = this.GetContactPhones(person);

					// Emails
					contact.Emails = this.GetContactEmails(person);

					// Websites
					contact.WebSites = this.GetContactWebsites(person);

					// Photo
					if(person.HasImage) {
						contact.Photo = this.GetContactBinaryPhoto(person.Image);
					}

					// Relationship
					// TODO contact.Relationship =

					return contact;
				}
			}

			return null;
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="query">
		/// A <see cref="ContactQuery"/>
		/// </param>
		public override void ListContacts (ContactQuery query)
		{
			SystemLogger.Log(SystemLogger.Module.PLATFORM, "Listing ALL contacts before permission request...");
			if (UIDevice.CurrentDevice.CheckSystemVersion (6, 0)) {
				RequestAccessToContacts(() => { LaunchListContacts (query); } );
			} else {
				LaunchListContacts (query);
			};


		}

		/// <summary>
		/// Launchs the create new contact.
		/// </summary>
		/// <param name='contact'>
		/// Contact.
		/// </param>
		/// 
		protected void LaunchListContacts (ContactQuery query){

			List<ContactLite> contactList = new List<ContactLite>();

			ABAddressBook addressBook = IPhoneServiceLocator.CurrentDelegate.AddressBook;

			// Gets all people in the address book
			ABPerson[] people = addressBook.GetPeople();

			if(people != null) {
				SystemLogger.Log(SystemLogger.Module.PLATFORM, "People found: " + people.Length);

				// sort list by FirstName (default)
				Array.Sort(people, delegate(ABPerson person1, ABPerson person2) {
					return person1.CompareTo(person2, DEFAULT_CONTACTS_LIST_SORT);
				});

			List<ABPerson> contacts = null;

			if(query == null || query.Value == null || query.Value.Trim().Equals("")) {
					SystemLogger.Log(SystemLogger.Module.PLATFORM, "Listing ALL contacts...");
					foreach (ABPerson person in people) {
						contactList.Add (ABPersonToContactLite(person));
					}


				} else {

					SystemLogger.Log(SystemLogger.Module.PLATFORM, "Listing contacts by query: " + query.ToString());
					string value = query.Value;
					SystemLogger.Log(SystemLogger.Module.PLATFORM, "Listing contacts by query with value: " + value);
					
					switch(query.Column){
						case ContactQueryColumn.ID:
							SystemLogger.Log (SystemLogger.Module.PLATFORM, "by ID ");
							contacts = people.ToList ().FindAll (p => p.Id.ToString () == value.ToString());
							break;

						case ContactQueryColumn.Name:
							SystemLogger.Log(SystemLogger.Module.PLATFORM, "by NAME in a list of " + people.Length + " people");
							switch(query.Condition){
								case ContactQueryCondition.Equals:
									SystemLogger.Log (SystemLogger.Module.PLATFORM, "EQUALS ");
									contacts = people.ToList ().FindAll (p => ((p.Nickname != null && p.Nickname.Equals (value))
										|| (p.LastName != null && p.LastName.Equals (value))
										|| (p.FirstName != null && p.FirstName.Equals (value))
										|| (p.MiddleName != null && p.MiddleName.Equals (value))));
									break;
								case ContactQueryCondition.StartsWith:
									SystemLogger.Log(SystemLogger.Module.PLATFORM, "STARTSWITH ");
									contacts = people.ToList ().FindAll (p => ((p.Nickname != null && p.Nickname.StartsWith (value))
										|| (p.LastName != null && p.LastName.StartsWith (value))
										|| (p.FirstName != null && p.FirstName.StartsWith (value))
										|| (p.MiddleName != null && p.MiddleName.StartsWith (value))));
									break;		
								case ContactQueryCondition.EndsWith:
									SystemLogger.Log(SystemLogger.Module.PLATFORM, "ENDSWITH ");
									contacts = people.ToList ().FindAll (p => ((p.Nickname != null && p.Nickname.EndsWith (value))
										|| (p.LastName != null && p.LastName.EndsWith (value))
										|| (p.FirstName != null && p.FirstName.EndsWith (value))
										|| (p.MiddleName != null && p.MiddleName.EndsWith (value))));
									break;
								case ContactQueryCondition.Contains:
									SystemLogger.Log (SystemLogger.Module.PLATFORM, "CONTAINS ");
									contacts = people.ToList ().FindAll (p => ((p.Nickname != null && p.Nickname.Contains (value))
										|| (p.LastName != null && p.LastName.Contains (value))
										|| (p.FirstName != null && p.FirstName.Contains (value))
										|| (p.MiddleName != null && p.MiddleName.Contains (value))));
									break;
							}
							break;				
					}
					if (contacts != null) 
					{
						foreach (ABPerson person in contacts) {

							contactList.Add (ABPersonToContactLite (person));

						}
					}
					
				}
			}
			SystemLogger.Log(SystemLogger.Module.PLATFORM, "contactList: " + contactList.Count);
			UIApplication.SharedApplication.InvokeOnMainThread (delegate {
				IPhoneUtils.GetInstance().FireUnityJavascriptEvent("Appverse.Pim.onListContactsEnd", contactList);
			});
			//return contactList.ToArray();
		
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="ABPerson">
		/// A <see cref="ABPerson"/>
		/// </param>
		/// <returns>
		/// A <see cref="ContactLite"/>
		/// </returns>
		protected ContactLite ABPersonToContactLite(ABPerson person)
		{
			ContactLite contact = new ContactLite ();

			// Basic Info
			contact.ID = "" + person.Id;
			contact.Name = person.FirstName;
			contact.Firstname = person.MiddleName;
			contact.Lastname = person.LastName;
			contact.DisplayName = person.Nickname;


			// TODO how to get the group(s) this person belongs to
			contact.Group = person.Organization;

			// Phones
			contact.Phones = this.GetContactPhones (person);

			// Emails
			contact.Emails = this.GetContactEmails (person);
			
			return contact;
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="addressBook">
		/// A <see cref="ABAddressBook"/>
		/// </param>
		/// <param name="requestedGroupName">
		/// A <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// A <see cref="ABPerson[]"/>
		/// </returns>
		private ABPerson[] GetContactsByGroup(ABAddressBook addressBook, string requestedGroupName) {
			ABPerson [] people = new ABPerson[0];
					
			ABGroup[] groups = addressBook.GetGroups();
			
			foreach(ABGroup abGroup in groups) {
				if(abGroup.Name == requestedGroupName) {
					ABRecord[] records = abGroup.GetMembers(DEFAULT_CONTACTS_LIST_SORT); //get list sorted by FirstName (default)
					people = new ABPerson[records.Length];
					for(int i=0; i< records.Length; i++) {
						ABRecord record = records[i];
						ABPerson person = addressBook.GetPerson(record.Id);
						if(person!=null) {
							people[i] = person;
						}
					}
					break;
				}
			}
			return people;
		}
		
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="people">
		/// A <see cref="ABPerson[]"/>
		/// </param>
		/// <param name="requestedName">
		/// A <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// A <see cref="ABPerson[]"/>
		/// </returns>
		private ABPerson[] FilterContactsByName(ABPerson[] people, string requestedName) {
			List<ABPerson> filteredPeopleList = new List<ABPerson>();
			foreach(ABPerson person in people) {
				// Check if requested name matches FistName,LastName,MiddleName or Nickname (ignoring case)
				if((person.FirstName!=null && person.FirstName.ToUpper().Contains(requestedName.ToUpper()))
				   ||(person.LastName!=null && person.LastName.ToUpper().Contains(requestedName.ToUpper()))
				    ||(person.MiddleName!=null && person.MiddleName.ToUpper().Contains(requestedName.ToUpper()))
				   		||(person.Nickname!=null && person.Nickname.ToUpper().Contains(requestedName.ToUpper()))) {
					filteredPeopleList.Add(person);
				}
			}
			return filteredPeopleList.ToArray();
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="data">
		/// A <see cref="NSData"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.Byte[]"/>
		/// </returns>
		private byte[] GetContactBinaryPhoto(NSData data) {
			byte[] buffer = null;
			try {
				buffer = new byte[data.Length];
				Marshal.Copy(data.Bytes, buffer,0,buffer.Length);	
			} catch (Exception e) {
				SystemLogger.Log(SystemLogger.Module.PLATFORM, "Exception getting photo binary data from contact", e);
			}
			
			return buffer;
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="buffer">
		/// A <see cref="System.Byte[]"/>
		/// </param>
		/// <returns>
		/// A <see cref="NSData"/>
		/// </returns>
		private NSData ConvertContactBinaryDataToNative(byte[] buffer) {
			try {
				if(buffer!=null) {
					//MemoryStream stream = new MemoryStream(buffer, 0, buffer.Length);
					//return NSData.FromStream(stream);
					return NSData.FromArray(buffer);
				}
			} catch (Exception e) {
				SystemLogger.Log(SystemLogger.Module.PLATFORM, "Exception setting photo binary data for contact", e);
			}	
			return null;
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="person">
		/// A <see cref="ABPerson"/>
		/// </param>
		/// <returns>
		/// A <see cref="ContactAddress[]"/>
		/// </returns>
		private ContactAddress[] GetContactAdresses(ABPerson person) {
			List<ContactAddress> contactAddressList = new List<ContactAddress>();
			
			if(person != null) {
				ABMultiValue<NSDictionary> addresses = person.GetAddresses();
				if(addresses!=null) {
					
					IEnumerator enumerator = addresses.GetEnumerator();
					while(enumerator.MoveNext()){
						object currentAddress = enumerator.Current;
						string label = ((ABMultiValueEntry<NSDictionary>)currentAddress).Label;
						NSDictionary addressDictionary = ((ABMultiValueEntry<NSDictionary>)currentAddress).Value;
						
						ContactAddress contactAddress = new ContactAddress();

						if(label == ABLabel.Home) {
							contactAddress.Type = DispositionType.HomeOffice;
						} else if(label == ABLabel.Work) {
							contactAddress.Type = DispositionType.Work;
						} else if (label == ABLabel.Other) {
							contactAddress.Type = DispositionType.Other;
						} 
						
						NSObject[] keys = addressDictionary.Keys;
						
						foreach(NSObject key in keys) {
							NSObject currentValue = addressDictionary.ObjectForKey(key);
							if(currentValue != null) {
								if(key.ToString() == ABPersonAddressKey.Street) {
									contactAddress.Address = currentValue.ToString();
									// contactAddress.AddressNumber = NOT PROVIDED BY API
								}
								
								if(key.ToString() == ABPersonAddressKey.City) {
									contactAddress.City = currentValue.ToString();
								}
								if(key.ToString() == ABPersonAddressKey.Country) {
									contactAddress.Country = currentValue.ToString();
								}
								if(key.ToString() == ABPersonAddressKey.Zip) {
									contactAddress.PostCode = currentValue.ToString();
								}
							}
						}
						
						contactAddressList.Add(contactAddress);
					}
				}
			}
			return contactAddressList.ToArray();
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="contactAddressList">
		/// A <see cref="ContactAddress[]"/>
		/// </param>
		/// <returns>
		/// A <see cref="ABMultiValue<NSDictionary>"/>
		/// </returns>
		private ABMultiValue<NSDictionary> ConvertContactAddressesToNative(ContactAddress[] contactAddressList) 
		{
			ABMutableDictionaryMultiValue multableMultilabel = new ABMutableDictionaryMultiValue();
		
			if(contactAddressList!=null) {
				foreach(ContactAddress contactAddress in contactAddressList) {
					
					NSString label = ABLabel.Other; // default is "Other"
					if(contactAddress.Type == DispositionType.HomeOffice) {
						label = ABLabel.Home;	
					} else if(contactAddress.Type == DispositionType.Work) {
						label = ABLabel.Work;	
					}
					
					NSMutableDictionary addressDictionary = new NSMutableDictionary();
					if(contactAddress.Address != null) {
						string address = contactAddress.Address;
						if(contactAddress.AddressNumber != null) {
							address += " " + contactAddress.AddressNumber;
						}
						addressDictionary.Add(ABPersonAddressKey.Street,new NSString(address));
					}
					
					if(contactAddress.City != null) {
						addressDictionary.Add(ABPersonAddressKey.City,new NSString(contactAddress.City));
					}
					
					if(contactAddress.Country != null) {
						addressDictionary.Add(ABPersonAddressKey.Country,new NSString(contactAddress.Country));
					}
					
					if(contactAddress.PostCode != null) {
						addressDictionary.Add(ABPersonAddressKey.Zip,new NSString(contactAddress.PostCode));
					}
					
					multableMultilabel.Add(addressDictionary, label);
				}
			}
				
			return multableMultilabel;
		}
			
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="person">
		/// A <see cref="ABPerson"/>
		/// </param>
		/// <returns>
		/// A <see cref="ContactPhone[]"/>
		/// </returns>
		private ContactPhone[] GetContactPhones(ABPerson person) {
			List<ContactPhone> contactPhoneList = new List<ContactPhone>();
			
			if(person != null) {
				ABMultiValue<string> phones = person.GetPhones();
				if(phones!=null) {
					
					IEnumerator enumerator = phones.GetEnumerator();
					while(enumerator.MoveNext()){
						object currentPhone = enumerator.Current;
						string label = ((ABMultiValueEntry<string>)currentPhone).Label;
						string phone = ((ABMultiValueEntry<string>)currentPhone).Value;
						
						ContactPhone contactPhone = new ContactPhone();
						contactPhone.Number = phone;
						
						if(label == ABLabel.Home) {
							contactPhone.Type = NumberType.FixedLine;
						} else if(label == ABLabel.Work) {
							contactPhone.Type = NumberType.Work;
						} else if(label == ABPersonPhoneLabel.HomeFax) { 
							contactPhone.Type = NumberType.HomeFax;
						} else if(label == ABPersonPhoneLabel.WorkFax) {
							contactPhone.Type = NumberType.WorkFax;
						} else if(label == ABPersonPhoneLabel.Mobile) {
							contactPhone.Type = NumberType.Mobile;
						} else if(label == ABPersonPhoneLabel.Pager) {
							contactPhone.Type = NumberType.Pager;
						} else if (label == ABPersonPhoneLabel.Main) {
							contactPhone.Type = NumberType.Other;
							contactPhone.IsPrimary = true;
						} else {
							contactPhone.Type = NumberType.Other;
						}
						
						contactPhoneList.Add(contactPhone);
					}
				}
			}
			return contactPhoneList.ToArray();
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="contactPhoneList">
		/// A <see cref="ContactPhone[]"/>
		/// </param>
		/// <returns>
		/// A <see cref="ABMultiValue<System.String>"/>
		/// </returns>
		private ABMultiValue<string> ConvertContactPhonesToNative(ContactPhone[] contactPhoneList) {
			ABMutableStringMultiValue multableMultilabel = new ABMutableStringMultiValue();
		
			if(contactPhoneList!=null) {
				foreach(ContactPhone contactPhone in contactPhoneList) {
					
					NSString label = ABLabel.Other; // default is "Other"
					if(contactPhone.Type == NumberType.Other && contactPhone.IsPrimary) {
						label = ABPersonPhoneLabel.Main;	
					} else if(contactPhone.Type == NumberType.Pager) {
						label = ABPersonPhoneLabel.Pager;	
					} else if(contactPhone.Type == NumberType.Mobile) {
						label = ABPersonPhoneLabel.Mobile;	
					} else if(contactPhone.Type == NumberType.WorkFax) {
						label = ABPersonPhoneLabel.WorkFax;	
					} else if(contactPhone.Type == NumberType.HomeFax) {
						label = ABPersonPhoneLabel.HomeFax;	
					} else if(contactPhone.Type == NumberType.Work) {
						label = ABLabel.Work;	
					} else if(contactPhone.Type == NumberType.FixedLine) {
						label = ABLabel.Home;	
					} 
					
					multableMultilabel.Add(contactPhone.Number, label);
				}
			}
				
			return multableMultilabel;
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="person">
		/// A <see cref="ABPerson"/>
		/// </param>
		/// <returns>
		/// A <see cref="ContactEmail[]"/>
		/// </returns>
		private ContactEmail[] GetContactEmails(ABPerson person) {
			List<ContactEmail> contactEmailList = new List<ContactEmail>();
			
			if(person != null) {
				ABMultiValue<string> emails = person.GetEmails();
				if(emails!=null) {
					
					IEnumerator enumerator = emails.GetEnumerator();
					while(enumerator.MoveNext()){
						object currentEmail = enumerator.Current;
						string label = ((ABMultiValueEntry<string>)currentEmail).Label;
						string email = ((ABMultiValueEntry<string>)currentEmail).Value;
						
						ContactEmail contactEmail = new ContactEmail();
						contactEmail.Address = email;
						
						// Following data not provided.
						//contactEmail.CommonName =
						//contactEmail.Firstname =
						//contactEmail.IsPrimary = 
						//contactEmail.Surname =
						
						if(label == ABLabel.Home) {
							contactEmail.Type = DispositionType.HomeOffice;
						} else if(label == ABLabel.Work) {
							contactEmail.Type = DispositionType.Work;
						} else {
							contactEmail.Type = DispositionType.Other;
						}
						
						contactEmailList.Add(contactEmail);
					}
				}
			}
			return contactEmailList.ToArray();
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="contactEmailList">
		/// A <see cref="ContactEmail[]"/>
		/// </param>
		/// <returns>
		/// A <see cref="ABMultiValue<System.String>"/>
		/// </returns>
		private ABMultiValue<string> ConvertContactEmailsToNative(ContactEmail[] contactEmailList) {
			ABMutableStringMultiValue multableMultilabel = new ABMutableStringMultiValue();
		
			if(contactEmailList!=null) {
				foreach(ContactEmail contactEmail in contactEmailList) {
					
					NSString label = ABLabel.Other; // default is "Other"
					if(contactEmail.Type == DispositionType.HomeOffice) {
						label = ABLabel.Home;	
					} else if(contactEmail.Type == DispositionType.Work) {
						label = ABLabel.Work;	
					}
					
					// Following data not provided on native API.
					//<< contactEmail.CommonName
					//<< contactEmail.Firstname
					//<< contactEmail.IsPrimary
					//<< contactEmail.Surname
					
					multableMultilabel.Add(contactEmail.Address, label);
				}
			}
				
			return multableMultilabel;
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="person">
		/// A <see cref="ABPerson"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.String[]"/>
		/// </returns>
		private string[] GetContactWebsites(ABPerson person) {
			List<string> contactUrlList = new List<string>();
			
			if(person != null) {
				ABMultiValue<string> urls = person.GetUrls();
				if(urls!=null) {
					
					IEnumerator enumerator = urls.GetEnumerator();
					while(enumerator.MoveNext()){
						object currentUrll = enumerator.Current;
						// string label = ((ABMultiValueEntry<string>)currentUrll).Label;
						string url = ((ABMultiValueEntry<string>)currentUrll).Value;
						contactUrlList.Add(url);
					}
				}
			}
			return contactUrlList.ToArray();
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="contactWebsiteList">
		/// A <see cref="System.String[]"/>
		/// </param>
		/// <returns>
		/// A <see cref="ABMultiValue<System.String>"/>
		/// </returns>
		private ABMultiValue<string> ConvertContactWebsitesToNative(string[] contactWebsiteList) {
			ABMutableStringMultiValue multableMultilabel = new ABMutableStringMultiValue();
		
			if(contactWebsiteList!=null) {
				foreach(string website in contactWebsiteList) {
					
					NSString label = ABLabel.Other; // default is "Other"
					
					multableMultilabel.Add(website, label);
				}
			}
				
			return multableMultilabel;
		}
		
		public override bool UpdateContact (string ID, Contact newContactData)
		{
			throw new System.NotImplementedException();
		}

		public override bool DeleteContact (Contact contact)
		{
			throw new System.NotImplementedException();
		}

#endregion

		#region CALENDAR API
	
		public override CalendarEntry CreateCalendarEntry (CalendarEntry entry)
		{
			if(entry!=null) {
				if (UIDevice.CurrentDevice.CheckSystemVersion (6, 0)) {
					RequestAccessToCalendar (EKEntityType.Event, () => { LaunchCreateNewEvent (entry); } );
				} else {
					using (var pool = new NSAutoreleasePool ()) {
						var thread = new Thread (ShowCreateCalendarEventView);
						thread.Start (entry);
					};
				}
			}
			
			// TODO return CalendarEntry object should return the inserted ID event.
			return entry;
		}

		[Export("ShowCreateCalendarEventView")]
		private void ShowCreateCalendarEventView (object entryObject)
		{
			UIApplication.SharedApplication.InvokeOnMainThread (delegate {
				CalendarEntry entry  = (CalendarEntry)entryObject;
				LaunchCreateNewEvent (entry);
			});
		}
		
		/// <summary>
		/// A convenience method that requests access to the appropriate calendar and 
		/// shows an alert if access is not granted, otherwise executes the completion 
		/// method.
		/// </summary>
		protected void RequestAccessToCalendar ( EKEntityType type, Action completion )
		{
			IPhoneServiceLocator.CurrentDelegate.EventStore.RequestAccess (type, 
			                                                               (bool granted, NSError e) => {
				if (granted) {
					UIApplication.SharedApplication.InvokeOnMainThread ( () => { completion.Invoke(); } );
				} else {
					INotification notificationService = (INotification)IPhoneServiceLocator.GetInstance ().GetService ("notify");
					if (notificationService != null) {
						notificationService.StartNotifyAlert ("Access Denied", "User Denied Access to Calendars/Reminders. Go to Privacy Settings to change it.", "OK");
					}
				}
			} );
		}
		
		/// <summary>
		/// Launchs the create new event controller.
		/// </summary>
		protected void LaunchCreateNewEvent (CalendarEntry entry)
		{
			EKEventStore store = IPhoneServiceLocator.CurrentDelegate.EventStore;
			EKCalendar calendar = store.DefaultCalendarForNewEvents;
			
			EKEvent calendarEvent = EKEvent.FromStore(store);
			// add event to the default calendar for the new events
			calendarEvent.Calendar = calendar;
			
			try {
				// add event details
				calendarEvent.Title = entry.Title;
				if(entry.Notes == null) {
					entry.Notes = "";
				}
				calendarEvent.Notes = entry.Notes;
				calendarEvent.Location = entry.Location;
				calendarEvent.AllDay = entry.IsAllDayEvent;
				calendarEvent.StartDate = entry.StartDate;
				calendarEvent.EndDate = entry.EndDate;
				
				SystemLogger.Log(SystemLogger.Module.PLATFORM, "Creating Calendar Event: " + calendarEvent.Title);
				SystemLogger.Log(SystemLogger.Module.PLATFORM, "Default Calendar: " + calendar.Title);
				SystemLogger.Log(SystemLogger.Module.PLATFORM, "Event StartDate: " + calendarEvent.StartDate.ToString());
				SystemLogger.Log(SystemLogger.Module.PLATFORM, "Event EndDate: " + calendarEvent.EndDate.ToString());
				// TODO: locate how to translate this features
				// entry.Type (birthday, exchange, etc)
				//calendarEvent.  = entry.IsEditable
				
				// Attendees
				if(entry.Attendees != null && entry.Attendees.Length >0) {
					int attendeesNum = entry.Attendees.Length;
					// TODO : check another way to add participants
					// calendarEvent.Attendees --> READ ONLY !!!
				}
				
				// Alarms
				if(entry.Alarms != null && entry.Alarms.Length >0) {
					foreach(CalendarAlarm alarm in entry.Alarms) {
						EKAlarm eventAlarm = new EKAlarm();
						eventAlarm.AbsoluteDate = alarm.Trigger;
						// TODO: how to manage "action", "sound" and "emailaddress"
						calendarEvent.AddAlarm(eventAlarm);
					}
				}

				if (UIDevice.CurrentDevice.CheckSystemVersion (5, 0)) {
					// Recurrence Rules
					if(entry.IsRecurrentEvent && entry.Recurrence != null) {
						EKRecurrenceFrequency recurrenceFrequency = EKRecurrenceFrequency.Daily;
						if(entry.Recurrence.Type == RecurrenceType.Weekly) {
							recurrenceFrequency = EKRecurrenceFrequency.Weekly;
						} else if(entry.Recurrence.Type == RecurrenceType.Montly) {
							recurrenceFrequency = EKRecurrenceFrequency.Monthly;
						} else if(entry.Recurrence.Type == RecurrenceType.Yearly) {
							recurrenceFrequency = EKRecurrenceFrequency.Yearly;
						} else if(entry.Recurrence.Type == RecurrenceType.FourWeekly) {
							recurrenceFrequency = EKRecurrenceFrequency.Weekly;
							entry.Recurrence.Interval = 4; // force event to be repeated "every 4 weeks"
						} else if(entry.Recurrence.Type == RecurrenceType.Fortnightly) {
							recurrenceFrequency = EKRecurrenceFrequency.Weekly;
							entry.Recurrence.Interval = 2; // force event to be repeated "every 2 weeks"
						}
						EKRecurrenceEnd recurrenceEnd = null;
						SystemLogger.Log(SystemLogger.Module.PLATFORM, "Recurrence Frequency: " + recurrenceFrequency);
						SystemLogger.Log(SystemLogger.Module.PLATFORM, "Recurrence Interval: " + entry.Recurrence.Interval);
						SystemLogger.Log(SystemLogger.Module.PLATFORM, "Recurrence EndDate (requested): " + entry.Recurrence.EndDate.ToString());
						SystemLogger.Log(SystemLogger.Module.PLATFORM, "Recurrence number: " + entry.RecurrenceNumber);
						if(entry.Recurrence.EndDate.CompareTo(entry.EndDate)>0) {
							recurrenceEnd = EKRecurrenceEnd.FromEndDate(entry.Recurrence.EndDate);
							SystemLogger.Log(SystemLogger.Module.PLATFORM, "Recurrence EndDate (applied): " + recurrenceEnd.EndDate.ToString());
						} else if(entry.RecurrenceNumber > 0) {
							recurrenceEnd = EKRecurrenceEnd.FromOccurrenceCount((int)entry.RecurrenceNumber);
							SystemLogger.Log(SystemLogger.Module.PLATFORM, "Recurrence OcurrenceCount: " + recurrenceEnd.OccurrenceCount);
						} else {
							recurrenceEnd = new EKRecurrenceEnd();
						}
						EKRecurrenceRule recurrenceRule = new EKRecurrenceRule(recurrenceFrequency,entry.Recurrence.Interval, recurrenceEnd);
						if(entry.Recurrence.DayOfTheWeek > 0) {
							EKRecurrenceDayOfWeek dayOfWeek = EKRecurrenceDayOfWeek.FromWeekDay(entry.Recurrence.DayOfTheWeek,0);
							EKRecurrenceDayOfWeek[] arrayDayOfWeek = new EKRecurrenceDayOfWeek[1];
							arrayDayOfWeek[0] = dayOfWeek;
							SystemLogger.Log(SystemLogger.Module.PLATFORM, "Setting DayOfTheWeek: " + dayOfWeek.DayOfTheWeek);
							recurrenceRule = new EKRecurrenceRule(recurrenceFrequency,entry.Recurrence.Interval, arrayDayOfWeek, null, null, null, null, null, recurrenceEnd);
						}
						
						calendarEvent.AddRecurrenceRule(recurrenceRule);
					}
				}
			} catch (Exception e) {
				SystemLogger.Log(SystemLogger.Module.PLATFORM, "ERROR Creating Calendar Event [" + calendarEvent.Title + "]. Error message: " + e.Message);
			}
			
			EKEventEditViewController eventViewController = new EKEventEditViewController();
			eventViewController.Event = calendarEvent;
			eventViewController.EventStore = store;
			eventViewController.Completed += delegate(object sender, EKEventEditEventArgs e) {
				UIApplication.SharedApplication.InvokeOnMainThread (delegate {
					SystemLogger.Log(SystemLogger.Module.PLATFORM, "On EKEventEditViewController Completed: " + e.Action);
					if(e.Action == EKEventEditViewAction.Saved) {
						INotification notificationService = (INotification)IPhoneServiceLocator.GetInstance ().GetService ("notify");
						if (notificationService != null) {
							notificationService.StartNotifyAlert ("Calendar Alert", "Calendar Entry Saved", "OK");
						}
					}
					IPhoneServiceLocator.CurrentDelegate.MainUIViewController ().DismissModalViewControllerAnimated(true);
				});
			};
			
			IPhoneServiceLocator.CurrentDelegate.MainUIViewController ().PresentModalViewController (eventViewController, true);
			IPhoneServiceLocator.CurrentDelegate.SetMainUIViewControllerAsTopController(false);
		}
		
		/*
		[Export("ShowCreateCalendarEventView")]
		private void ShowCreateCalendarEventView (object entryObject)
		{
			CalendarEntry entry  = (CalendarEntry)entryObject;
			RequestAccess (EKEntityType.Event, () => { LaunchCreateNewEvent (entry); } );
		}
		*/
		
		public override bool DeleteCalendarEntry (CalendarEntry entry)
		{
			throw new System.NotImplementedException();
		}
		
		public override bool MoveCalendarEntry (CalendarEntry entry, DateTime newStartDate, DateTime newEndDate)
		{
			throw new System.NotImplementedException();
		}
		
		public override CalendarEntry[] ListCalendarEntries (DateTime date)
		{
			// list calendar entries fro the same start and end dates.
			return ListCalendarEntries(date, date);
		}
		
		/// <summary>
		/// This method retrieves all the events for the past week via a query and displays them
		/// on the EventList Screen.
		/// </summary>
		protected void GetEventsViaQuery (DateTime startDate, DateTime endDate)
		{
			List<CalendarEntry> eventsList = new List<CalendarEntry>();
			EKEventStore store = new EKEventStore();
			EKCalendar calendar = store.DefaultCalendarForNewEvents;
			
			// Query the event
			if (calendar != null)
			{
				// Searches for every event in the range of given dates
				NSPredicate predicate = store.PredicateForEvents(startDate,endDate,new EKCalendar[] {calendar});
				store.EnumerateEvents(predicate, delegate(EKEvent currentEvent, ref bool stop)
				                      {
					// Perform your check for an event type
					CalendarEntry entry = new CalendarEntry();
					entry.Uid = currentEvent.EventIdentifier;
					entry.Title = currentEvent.Title;
					entry.Notes = currentEvent.Notes;
					entry.Location = currentEvent.Location;
					entry.IsAllDayEvent = currentEvent.AllDay;
					entry.StartDate = currentEvent.StartDate;
					entry.EndDate = currentEvent.EndDate;
					
					// TODO: locate how to translate this features
					// entry.Type (birthday, exchange, etc)
					//calendarEvent.  = entry.IsEditable
					
					// Attendees
					if(currentEvent.Attendees != null && currentEvent.Attendees.Length>0) {
						int attendeesNum = currentEvent.Attendees.Length;
						entry.Attendees = new CalendarAttendee[attendeesNum];
						int index = 0;
						foreach(EKParticipant participant in currentEvent.Attendees) {
							CalendarAttendee attendee = new CalendarAttendee();
							attendee.Name = participant.Name;
							attendee.Address = participant.Url.AbsoluteString;
							if(participant.ParticipantStatus == EKParticipantStatus.Unknown || participant.ParticipantStatus == EKParticipantStatus.Pending) {
								attendee.Status = AttendeeStatus.NeedsAction;
							}
							entry.Attendees[index] = attendee;
							index++;
						}
					}
					
					
					// Alarms
					if(currentEvent.HasAlarms && currentEvent.Alarms != null && currentEvent.Alarms.Length >0) {
						int alarmsNum = currentEvent.Alarms.Length;
						entry.Alarms = new CalendarAlarm[alarmsNum];
						int index = 0;
						foreach(EKAlarm alarm in currentEvent.Alarms) {
							CalendarAlarm eventAlarm = new CalendarAlarm();
							eventAlarm.Trigger = alarm.AbsoluteDate;
							// TODO: how to manage "action", "sound" and "emailaddress"
							entry.Alarms[index] = eventAlarm;
							index++;
						}
					}
					
					// Recurrence Rules (pick only the first one)
					if(currentEvent.HasRecurrenceRules && currentEvent.RecurrenceRules != null && currentEvent.RecurrenceRules.Length >0) {
						entry.IsRecurrentEvent = true;
						EKRecurrenceRule rule = currentEvent.RecurrenceRules[0];
						if(rule != null) {
							entry.Recurrence = new CalendarRecurrence();
							if(rule.Frequency == EKRecurrenceFrequency.Weekly) {
								entry.Recurrence.Type = RecurrenceType.Weekly;
							} else if(rule.Frequency == EKRecurrenceFrequency.Monthly) {
								entry.Recurrence.Type = RecurrenceType.Montly;
							} else if(rule.Frequency == EKRecurrenceFrequency.Yearly) {
								entry.Recurrence.Type = RecurrenceType.Yearly;
							}
							if(rule.RecurrenceEnd != null) {
								entry.Recurrence.EndDate = rule.RecurrenceEnd.EndDate;
								entry.Recurrence.Interval = rule.Interval;
								entry.RecurrenceNumber = rule.RecurrenceEnd.OccurrenceCount;
							}
						}
					}
					
					eventsList.Add(entry);
				});
				
			}

			// TODO :: this API could no longer be invoked in this way.
			// the list of entries must be queried after checking access --> so, process has to send data via callback
			
			// return eventsList.ToArray();
		}
		
		public override CalendarEntry[] ListCalendarEntries (DateTime startDate, DateTime endDate)
		{
			if (UIDevice.CurrentDevice.CheckSystemVersion (6, 0)) {
				RequestAccessToCalendar (EKEntityType.Event, () => { GetEventsViaQuery (startDate, endDate); } );
			} else {
				GetEventsViaQuery (startDate, endDate);
			}
			// TODO provide a way to send back the query result data
			return null;
		}

#endregion
	}
}
