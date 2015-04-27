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
package com.gft.unity.android;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.Calendar;
import java.util.Date;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.Map.Entry;

import android.app.Activity;
import android.app.ActivityManager;
import android.content.ContentResolver;
import android.content.Context;
import android.content.Intent;
import android.database.Cursor;
import android.net.Uri;
import android.os.Build;
import android.os.Bundle;
import android.provider.ContactsContract;
import android.provider.ContactsContract.CommonDataKinds;
import android.provider.ContactsContract.CommonDataKinds.Email;
import android.provider.ContactsContract.CommonDataKinds.GroupMembership;
import android.provider.ContactsContract.CommonDataKinds.Nickname;
import android.provider.ContactsContract.CommonDataKinds.Note;
import android.provider.ContactsContract.CommonDataKinds.Organization;
import android.provider.ContactsContract.CommonDataKinds.Phone;
import android.provider.ContactsContract.CommonDataKinds.Photo;
import android.provider.ContactsContract.CommonDataKinds.Relation;
import android.provider.ContactsContract.CommonDataKinds.StructuredName;
import android.provider.ContactsContract.CommonDataKinds.StructuredPostal;
import android.provider.ContactsContract.CommonDataKinds.Website;
import android.provider.ContactsContract.Intents.Insert;
import android.provider.ContactsContract.RawContactsEntity;
import android.util.Base64;

import com.gft.unity.android.activity.IActivityManager;
import com.gft.unity.core.pim.AbstractPim;
import com.gft.unity.core.pim.CalendarEntry;
import com.gft.unity.core.pim.Contact;
import com.gft.unity.core.pim.ContactAddress;
import com.gft.unity.core.pim.ContactEmail;
import com.gft.unity.core.pim.ContactLite;
import com.gft.unity.core.pim.ContactPhone;
import com.gft.unity.core.pim.ContactQuery;
import com.gft.unity.core.pim.ContactQueryColumn;
import com.gft.unity.core.pim.ContactQueryCondition;
import com.gft.unity.core.pim.DateTime;
import com.gft.unity.core.pim.DispositionType;
import com.gft.unity.core.pim.NumberType;
import com.gft.unity.core.pim.RelationshipType;
import com.gft.unity.core.system.log.Logger;
import com.gft.unity.core.system.log.Logger.LogCategory;

// TODO all devices should have at least 1 google account setup for add/update to work??
public class AndroidPim extends AbstractPim {
	
	private static final String LOGGER_MODULE_CONTACTS = "IContacts";
	private static final String LOGGER_MODULE_CALENDAR = "ICalendar";
	private static final Logger LOGGER_CONTACTS = Logger.getInstance(
			LogCategory.PLATFORM, LOGGER_MODULE_CONTACTS);
	private static final Logger LOGGER_CALENDAR = Logger.getInstance(
			LogCategory.PLATFORM, LOGGER_MODULE_CALENDAR);

	private static final Map<Integer, NumberType> PHONE_TYPES_MAP = new HashMap<Integer, NumberType>();
	private static final Map<Integer, DispositionType> ADDRESS_TYPES_MAP = new HashMap<Integer, DispositionType>();
	private static final Map<Integer, RelationshipType> RELATION_TYPES_MAP = new HashMap<Integer, RelationshipType>();

	public static final String PRIMARY_ACCOUNT_TYPE = "com.google";

	static {

		// Phone
		PHONE_TYPES_MAP.put(Phone.TYPE_FAX_HOME, NumberType.HomeFax);
		PHONE_TYPES_MAP.put(Phone.TYPE_HOME, NumberType.FixedLine);
		PHONE_TYPES_MAP.put(Phone.TYPE_MOBILE, NumberType.Mobile);
		PHONE_TYPES_MAP.put(Phone.TYPE_PAGER, NumberType.Pager);
		PHONE_TYPES_MAP.put(Phone.TYPE_WORK, NumberType.Work);
		PHONE_TYPES_MAP.put(Phone.TYPE_FAX_WORK, NumberType.WorkFax);
		PHONE_TYPES_MAP.put(Phone.TYPE_COMPANY_MAIN, NumberType.Work);
		PHONE_TYPES_MAP.put(Phone.TYPE_WORK_MOBILE, NumberType.Work);
		PHONE_TYPES_MAP.put(Phone.TYPE_WORK_PAGER, NumberType.Pager);
		PHONE_TYPES_MAP.put(Phone.TYPE_CUSTOM, NumberType.Other);

		// Address
		ADDRESS_TYPES_MAP.put(StructuredPostal.TYPE_HOME,
				DispositionType.Personal);
		ADDRESS_TYPES_MAP.put(StructuredPostal.TYPE_WORK, DispositionType.Work);
		ADDRESS_TYPES_MAP.put(StructuredPostal.TYPE_OTHER,
				DispositionType.Other);

		// relationship
		RELATION_TYPES_MAP.put(Relation.TYPE_BROTHER, RelationshipType.Brother);
		RELATION_TYPES_MAP.put(Relation.TYPE_CHILD, RelationshipType.Child);
		RELATION_TYPES_MAP.put(Relation.TYPE_FRIEND, RelationshipType.Friend);
		RELATION_TYPES_MAP.put(Relation.TYPE_PARENT, RelationshipType.Parent);
		RELATION_TYPES_MAP.put(Relation.TYPE_PARTNER, RelationshipType.Partner);
		RELATION_TYPES_MAP.put(Relation.TYPE_RELATIVE,
				RelationshipType.Relative);
		RELATION_TYPES_MAP.put(Relation.TYPE_SISTER, RelationshipType.Sister);
		RELATION_TYPES_MAP.put(Relation.TYPE_SPOUSE, RelationshipType.Spouse);
	}

	
	private final class CalendarContract {
		public static final String EXTRA_EVENT_BEGIN_TIME = "beginTime";
		public static final String EXTRA_EVENT_END_TIME = "endTime";
		
	}
	
	private final static class Events {
		public static final Uri CONTENT_URI = Uri.parse("content://com.android.calendar/events"); // just available for API level >=14
		public static final String INTENT_TYPE = "vnd.android.cursor.item/event"; // for API level < 14
		public static final String TITLE = "title";
		public static final String DESCRIPTION = "description";
		public static final String ALL_DAY = "allDay";
		public static final String EVENT_LOCATION = "eventLocation";
		
		public static final String AVAILABILITY = "availability";
		public static final int AVAILABILITY_BUSY = 0;
		public static final int AVAILABILITY_FREE = 1;
		
	}
	
	@Override
	public CalendarEntry CreateCalendarEntry(CalendarEntry entry) {
		
		if (entry != null) {
			
			try {
			
				// getting begintime in milliseconds
				DateTime beginDate = entry.getStartDate();
				Calendar calBeginDate = Calendar.getInstance();
				if (beginDate != null) {
					// DateTime.getMonth() are values between 1 and 12 --> on
					// Calendar.set() month starts at 0...
					if(beginDate.getHour() > 0) {
						calBeginDate.set(beginDate.getYear(), beginDate.getMonth() - 1,
							beginDate.getDay(), beginDate.getHour(),
							beginDate.getMinute(), beginDate.getSecond());
					} else {
						calBeginDate.set(beginDate.getYear(), beginDate.getMonth() - 1, beginDate.getDay());
					}
				}
				
				LOGGER_CALENDAR.logInfo("CreateCalendarEntry", "Begin Date: " + beginDate.toString());
				
				// getting endtime in milliseconds
				DateTime endDate = entry.getEndDate();
				Calendar calEndDate = (Calendar) calBeginDate.clone();
				if (endDate == null) {
					// If the endtime was not provided, the endtime will be by default 1
					// hour later
					calEndDate.add(Calendar.HOUR, 1);
				} else {
					// DateTime.getMonth() are values between 1 and 12 --> on
					// Calendar.set() month starts at 0...
					if(endDate.getHour() > 0) {
						calEndDate.set(endDate.getYear(), endDate.getMonth() - 1,
							endDate.getDay(), endDate.getHour(), endDate.getMinute(),
							endDate.getSecond());
					} else {
						calEndDate.set(endDate.getYear(), endDate.getMonth() - 1, endDate.getDay());
					}
				}
				
				LOGGER_CALENDAR.logInfo("CreateCalendarEntry", "End Date: " + endDate.toString());
				
				Intent intent = null;
				
				if(Build.VERSION.SDK_INT < 14) {
					
					intent = new Intent(Intent.ACTION_EDIT);
					intent.setType(Events.INTENT_TYPE);
					
				} else {
					intent = new Intent(Intent.ACTION_INSERT)
							.setData(Events.CONTENT_URI);
				}
				if(intent!= null) {
					intent
					.putExtra(CalendarContract.EXTRA_EVENT_BEGIN_TIME, calBeginDate.getTimeInMillis())
					.putExtra(CalendarContract.EXTRA_EVENT_END_TIME, calEndDate.getTimeInMillis())
					.putExtra(Events.ALL_DAY, entry.getIsAllDayEvent())
					.putExtra(Events.TITLE, (entry.getTitle()!=null? entry.getTitle() : ""))
					.putExtra(Events.DESCRIPTION, (entry.getNotes()!=null? entry.getNotes() : ""))
					.putExtra(Events.EVENT_LOCATION, (entry.getLocation()!=null? entry.getLocation() : ""))
					.putExtra(Events.AVAILABILITY, Events.AVAILABILITY_BUSY);
					
					AndroidServiceLocator.getContext().startActivity(intent);
				}
			} catch(Exception ex) {
				LOGGER_CALENDAR.logError("CreateCalendarEntry", 
						"There was an error trying to insert a calendar entry", ex);
			}
		}
				
		return entry;
		
	}
	
	// TODO old implementation
	protected CalendarEntry CreateCalendarEntryOld(CalendarEntry entry) {

		Intent intent = new Intent(Intent.ACTION_EDIT);
		intent.setType("vnd.android.cursor.item/event");

		// getting begintime in milliseconds
		DateTime beginDate = entry.getStartDate();
		Calendar calBeginDate = Calendar.getInstance();
		if (beginDate != null) {
			// DateTime.getMonth() are values between 1 and 12 --> on
			// Calendar.set() month starts at 0...
			calBeginDate.set(beginDate.getYear(), beginDate.getMonth() - 1,
					beginDate.getDay(), beginDate.getHour(),
					beginDate.getMinute(), beginDate.getSecond());
		}
		intent.putExtra("beginTime", calBeginDate.getTimeInMillis());

		// getting endtime in milliseconds
		DateTime endDate = entry.getEndDate();
		Calendar calEndDate = (Calendar) calBeginDate.clone();
		if (endDate == null) {
			// If the endtime was not provided, the endtime will be by default 1
			// hour later
			calEndDate.add(Calendar.HOUR, 1);
		} else {
			// DateTime.getMonth() are values between 1 and 12 --> on
			// Calendar.set() month starts at 0...
			calEndDate.set(endDate.getYear(), endDate.getMonth() - 1,
					endDate.getDay(), endDate.getHour(), endDate.getMinute(),
					endDate.getSecond());
		}
		intent.putExtra("endTime", calEndDate.getTimeInMillis());
		intent.putExtra("allDay", entry.getIsAllDayEvent());
		intent.putExtra("title", entry.getTitle());
		intent.putExtra("eventLocation", entry.getLocation());
		intent.putExtra("description", entry.getNotes());
		AndroidServiceLocator.getContext().startActivity(intent);

		return entry;
	}

	@Override
	public boolean DeleteCalendarEntry(CalendarEntry entry) {
		// TODO implement ICalendar.DeleteCalendarEntry
		throw new UnsupportedOperationException("Not supported yet.");
	}

	@Override
	public void ListCalendarEntries(DateTime date) {
		// TODO implement ICalendar.ListCalendarEntries
		List<CalendarEntry> list = new ArrayList<CalendarEntry>();
		//TODO Implement method
		IActivityManager am = (IActivityManager) AndroidServiceLocator
				.GetInstance().GetService(
						AndroidServiceLocator.SERVICE_ANDROID_ACTIVITY_MANAGER);	
        am.executeJS("Appverse.Pim.onListCalendarEntriesEnd", new Object[]{ list.toArray()});
	}

	@Override
	public void ListCalendarEntries(DateTime startDate,
			DateTime endDate) {
		
		List<CalendarEntry> list = new ArrayList<CalendarEntry>();
		//TODO Implement method
		IActivityManager am = (IActivityManager) AndroidServiceLocator
				.GetInstance().GetService(
						AndroidServiceLocator.SERVICE_ANDROID_ACTIVITY_MANAGER);	
        am.executeJS("Appverse.Pim.onListCalendarEntriesEnd", new Object[]{ list.toArray()});
	}

	@Override
	public boolean MoveCalendarEntry(CalendarEntry entry,
			DateTime newStartDate, DateTime newEndDate) {
		// TODO implement ICalendar.MoveCalendarEntry
		throw new UnsupportedOperationException("Not supported yet.");
	}

	@Override
	// TODO handle all contact data fields
	public Contact CreateContact(Contact contactData) {

		LOGGER_CONTACTS.logInfo("CreateContact", "creating contact data for " + contactData.getName());
		ContactAddress[] addresses = contactData.getAddresses();
		ContactPhone[] phones = contactData.getPhones();
		ContactEmail[] emails = contactData.getEmails();
		Context context = AndroidServiceLocator.getContext();

		Bundle extras = new Bundle();

		Uri uri = null;
		if (phones != null && phones.length > 0) {
			LOGGER_CONTACTS.logInfo("CreateContact", "adding " + phones.length + " phones to the contact");
			for (ContactPhone phone : phones) {
				if (uri == null) {
					uri = Uri.parse("tel:" + phone.getNumber());
				} else {
					if (extras.getString(Insert.PHONE) == null) {
						extras.putString(Insert.PHONE, phone.getNumber());
						extras.putBoolean(Insert.PHONE_ISPRIMARY,
								phone.getIsPrimary());
						extras.putInt(Insert.PHONE_TYPE,
								getPhoneType(phone.getType()));
					} else if (extras.getString(Insert.SECONDARY_PHONE) == null) {
						extras.putString(Insert.SECONDARY_PHONE,
								phone.getNumber());
						extras.putInt(Insert.SECONDARY_PHONE_TYPE,
								getPhoneType(phone.getType()));
					} else if (extras.getString(Insert.TERTIARY_PHONE) == null) {
						extras.putString(Insert.TERTIARY_PHONE,
								phone.getNumber());
						extras.putInt(Insert.TERTIARY_PHONE_TYPE,
								getPhoneType(phone.getType()));
					}
				}
			}
		}

		if (emails != null && emails.length > 0) {
			LOGGER_CONTACTS.logInfo("CreateContact", "adding " + emails.length + " emails to the contact");
			for (ContactEmail email : emails) {
				if (uri == null) {
					uri = Uri.parse("mailto:" + email.getAddress());
				} else {
					if (extras.getString(Insert.EMAIL) == null) {
						extras.putString(Insert.EMAIL, email.getAddress());
						extras.putBoolean(Insert.EMAIL_ISPRIMARY,
								email.getIsPrimary());
						extras.putInt(Insert.EMAIL_TYPE,
								getAddressType(email.getType()));
					} else if (extras.getString(Insert.SECONDARY_EMAIL) == null) {
						extras.putString(Insert.SECONDARY_EMAIL,
								email.getAddress());
						extras.putInt(Insert.SECONDARY_EMAIL_TYPE,
								getAddressType(email.getType()));
					} else if (extras.getString(Insert.TERTIARY_EMAIL) == null) {
						extras.putString(Insert.TERTIARY_EMAIL,
								email.getAddress());
						extras.putInt(Insert.TERTIARY_EMAIL_TYPE,
								getAddressType(email.getType()));
					}
				}
			}
		}

		// Only 1 address can be added
		if (addresses != null && addresses.length > 0) {
			for (ContactAddress address : addresses) {
				if (extras.getString(Insert.POSTAL) == null) {
					extras.putString(Insert.POSTAL, address.getAddress());
					// No primary in the ContactAddress class
					extras.putBoolean(Insert.POSTAL_ISPRIMARY, true);
					extras.putInt(Insert.POSTAL_TYPE,
							getAddressType(address.getType()));
				}
			}
		}

		if (contactData.getNotes() != null) {
			extras.putString(Insert.NOTES, contactData.getNotes());
		}
		if (contactData.getCompany() != null) {
			extras.putString(Insert.COMPANY, contactData.getCompany());
		}
		if (contactData.getName() != null) {
			extras.putString(Insert.NAME, contactData.getName());
		}
		if (contactData.getJobTitle() != null) {
			extras.putString(Insert.JOB_TITLE, contactData.getJobTitle());
		}

		Intent intent;
		if (uri != null) {
			intent = new Intent(
					ContactsContract.Intents.SHOW_OR_CREATE_CONTACT, uri);
		} else {
			intent = new Intent(
					ContactsContract.Intents.SHOW_OR_CREATE_CONTACT,
					Uri.parse("mailto:"));
		}
		intent.putExtra(ContactsContract.Intents.EXTRA_FORCE_CREATE, true);
		intent.putExtras(extras);
		((Activity) context).startActivity(intent);

		LOGGER_CONTACTS.logDebug("Created contact ID: ",contactData.getID());
		return contactData;
	}

	@Override
	public boolean DeleteContact(Contact contact) {
		throw new UnsupportedOperationException("Not supported yet.");
	}
	

	@Override
	public void GetContact(String id) {
		Contact contact = new Contact();
		contact.setID(id.toString());
		
		LOGGER_CONTACTS.logInfo("GetContactDetailsById", "Getting contact data for id: " + id);
		
		String company = null, department = null, firstname = null, group = null, jobTitle = null, lastname = null, name = null, notes = null, photoBase64Encoded = null, displayName = null;
		List<ContactAddress> addressList = new ArrayList<ContactAddress>();
		List<ContactEmail> emailList = new ArrayList<ContactEmail>();
		List<ContactPhone> phoneList = new ArrayList<ContactPhone>();
		List<String> webSiteList = new ArrayList<String>();
		byte[] photo = new byte[0]; // TODO gets the default contacts
		// photo				
		RelationshipType relationship = RelationshipType.None;
		
		Context context = AndroidServiceLocator.getContext();
		ContentResolver cr = context.getContentResolver();
		String selectionSingle = RawContactsEntity.CONTACT_ID + " = " + id;
		Cursor raws = cr.query(RawContactsEntity.CONTENT_URI, null,
				selectionSingle, null, null);
		if(raws.getCount() == 0){
			IActivityManager am = (IActivityManager) AndroidServiceLocator
					.GetInstance().GetService(
							AndroidServiceLocator.SERVICE_ANDROID_ACTIVITY_MANAGER);	
	        am.executeJS("Appverse.Pim.onContactFound", null);
	        return;
		}
		try {
			raws.moveToFirst();
			while (!raws.isAfterLast()) {
				String type = raws
						.getString(raws
								.getColumnIndex(ContactsContract.Data.MIMETYPE));
				if (StructuredName.CONTENT_ITEM_TYPE.equals(type)) {
					// structured name
					
					displayName = raws.getString(raws.getColumnIndex(StructuredName.DISPLAY_NAME));
					
					LOGGER_CONTACTS.logInfo("GetContactDetailsById", "display name: " + displayName);
					
				} else if (Phone.CONTENT_ITEM_TYPE.equals(type)) {
					// it's a phone, gets all properties and add in
					// the phone list
					ContactPhone phone = new ContactPhone();
					String number = raws.getString(raws
							.getColumnIndex(Phone.NUMBER));
					phone.setNumber(number);
					int primary = raws.getInt(raws
							.getColumnIndex(Phone.IS_PRIMARY));
					phone.setIsPrimary(primary != 0);
					int phonetype = raws.getInt(raws
							.getColumnIndex(Phone.TYPE));
					phone.setType(getNumberType(phonetype));
					phoneList.add(phone);
				} else if (StructuredPostal.CONTENT_ITEM_TYPE
						.equals(type)) {
					// it's an Address, gets all properties and add
					// in
					// the address list
					ContactAddress address = new ContactAddress();
					String city = raws.getString(raws
							.getColumnIndex(StructuredPostal.CITY));
					String country = raws.getString(raws
							.getColumnIndex(StructuredPostal.COUNTRY));
					String postcode = raws.getString(raws
							.getColumnIndex(StructuredPostal.POSTCODE));
					String street = raws.getString(raws
							.getColumnIndex(StructuredPostal.STREET));
					// TODO Check about Number, as Android doesn't store
					// number in a different column. (Is in the street)
					// (linked with the way contacts are stored by this
					// class)
					int typeInt = raws.getInt(raws
							.getColumnIndex(StructuredPostal.TYPE));
					DispositionType dType = getAddressType(typeInt);
					address.setType(dType);
					address.setAddress(street);
					address.setCity(city);
					address.setCountry(country);
					address.setPostCode(postcode);
					//
					 //TODO: how to get the number?
					 // address.setAddressNumber(number);
					 //
					addressList.add(address);
				} else if (Email.CONTENT_ITEM_TYPE.equals(type)) {
					// it's an Email address, gets all properties
					// and add in the email list
					ContactEmail email = new ContactEmail();
					String emailaddress = raws
							.getString(raws
									.getColumnIndex(CommonDataKinds.Email.DATA1));
					email.setAddress(emailaddress);
					email.setCommonName(raws.getString(raws
							.getColumnIndex(Email.DISPLAY_NAME)));
					// TODO set first name and last name
					emailList.add(email);
				} else if (Organization.CONTENT_ITEM_TYPE.equals(type)) {
					if (raws.getInt(raws
							.getColumnIndex(RawContactsEntity.IS_PRIMARY)) == 1
							|| company == null || company.equals("")) {
						company = raws.getString(raws
								.getColumnIndex(Organization.COMPANY));
						department = raws
								.getString(raws
										.getColumnIndex(Organization.DEPARTMENT));
						jobTitle = raws.getString(raws
								.getColumnIndex(Organization.TITLE));
					}
				} else if (GroupMembership.CONTENT_ITEM_TYPE
						.equals(type)) {
					group = raws
							.getString(raws
									.getColumnIndex(CommonDataKinds.GroupMembership.DATA1));
				} else if (Relation.CONTENT_ITEM_TYPE.equals(type)) {
					if (raws.getInt(raws
							.getColumnIndex(RawContactsEntity.IS_PRIMARY)) == 1
							|| group == null || group.equals("")) {
						relationship = getRelationShipType(raws
								.getInt(raws
										.getColumnIndex(Relation.TYPE)));
					}
				} else if (Photo.CONTENT_ITEM_TYPE.equals(type)) {
					photo = raws.getBlob(raws
							.getColumnIndex(Photo.PHOTO));
					photoBase64Encoded = photo != null ? new String(
							Base64.encode(photo, Base64.DEFAULT))
							: null;
				} else if (CommonDataKinds.Website.CONTENT_ITEM_TYPE
						.equals(type)) {
					webSiteList.add(raws.getString(raws
							.getColumnIndex(Website.URL)));
				} else if (Nickname.CONTENT_ITEM_TYPE.equals(type)) {
					// No need for nickname 
					
					String nickName = raws.getString(raws.getColumnIndex(Nickname.NAME));
					
					LOGGER_CONTACTS.logInfo("GetContactDetailsById", "nickName: " + nickName);
					
				} else if (Note.CONTENT_ITEM_TYPE.equals(type)) {
					if (raws.getInt(raws
							.getColumnIndex(RawContactsEntity.IS_PRIMARY)) == 1
							|| notes == null || notes.equals("")) {
						notes = raws.getString(raws
								.getColumnIndex(Note.NOTE));
					}
				}
				raws.moveToNext();
			}

		} finally {
			if (raws != null) {
				raws.close();
			}
		}
		
		// filling the bean
		
		contact.setAddresses(addressList
					.toArray(new ContactAddress[addressList.size()]));					
		contact.setWebSites(webSiteList.toArray(new String[webSiteList
				.size()]));
		contact.setCompany(company);
		contact.setDepartment(department);
		contact.setPhoto(photo);
		contact.setPhotoBase64Encoded(photoBase64Encoded);
		contact.setRelationship(relationship);
		contact.setGroup(group);
		contact.setJobTitle(jobTitle);
		contact.setNotes(notes);
		
		contact.setEmails(emailList.toArray(new ContactEmail[emailList.size()]));
		contact.setPhones(phoneList.toArray(new ContactPhone[phoneList.size()]));
		contact.setFirstname(firstname);
		contact.setLastname(lastname);		
		contact.setName(name);
		
		contact.setDisplayName(displayName);
		
		
		IActivityManager am = (IActivityManager) AndroidServiceLocator
				.GetInstance().GetService(
						AndroidServiceLocator.SERVICE_ANDROID_ACTIVITY_MANAGER);	
        am.executeJS("Appverse.Pim.onContactFound", contact);
	}
	

	private boolean queryValueRequiredFailed(ContactQueryColumn column, String value) {

		boolean valueEmpty = (value == null || value.trim().equals(""));
		switch (column) {
			case ID:
			case Name:
				return valueEmpty;
			default:
				return false;
		}
	}
	
	@Override
	public void ListContacts(ContactQuery query) {
		
		LOGGER_CONTACTS.logInfo("ListContacts", "Start listing contacts...");
		Date startTotal = new Date();
		Uri uri = null;
		String selectionSingle;
		Context context = AndroidServiceLocator.getContext();
		ContentResolver cr = context.getContentResolver();
		Cursor contactsCursor = null;
		String sortOrder = ContactsContract.Contacts.DISPLAY_NAME + " COLLATE LOCALIZED ASC";
		LOGGER_CONTACTS.logDebug("Contacts selection","##################### ListContacts query:"+query.toString());
		if(query != null && !queryValueRequiredFailed(query.getColumn(), query.getValue())){
			LOGGER_CONTACTS.logDebug("Contacts selection","##################### ListContacts query");
			LOGGER_CONTACTS.logInfo("ListContacts", "Start listing contacts... queryText " + query.toString());
			String selection = null;
			String[] selectionValues = null;
		
			String value = query.getValue()!=null?query.getValue():"";
			LOGGER_CONTACTS.logInfo("Value", value);
			
			switch(query.getColumn()){
				case ID:
					selection = ContactsContract.Contacts._ID + "= ?";
					selectionValues = new String[]{ value };
					break;
				case Phone: // Phone column should be filtered later against another table (RawContactsEntity)
					selection = null;
					selectionValues = null;
					break;
				case Name:
					selection = ContactsContract.Contacts.DISPLAY_NAME + " LIKE ?";			
					switch(query.getCondition()){
						case Equals:
							selectionValues = new String[]{ value };
							break;
						case StartsWith:
							selectionValues = new String[]{ value + "%"};
							break;		
						case EndsWith:
							selectionValues = new String[]{ "%" + value};
							break;
						case Contains:
							selectionValues = new String[]{ "%" + value + "%"};
							break;
					}
					break;				
			}
			
			uri = ContactsContract.Contacts.CONTENT_URI;			
			LOGGER_CONTACTS.logDebug("Contacts selection", selection + ", selection values: " + (selectionValues!=null? selectionValues.toString() : "null"));
			
			Date start = new Date();			
			contactsCursor = cr.query(uri, new String[] {
					ContactsContract.Contacts._ID,
					ContactsContract.Contacts.DISPLAY_NAME }, selection, selectionValues, sortOrder);
			LOGGER_CONTACTS.logDebug("Time lapsed (main query A)",(new Date().getTime()-start.getTime())+" ms");
			
		}else{
			LOGGER_CONTACTS.logDebug("Contacts selection","##################### ListContacts !query");
			LOGGER_CONTACTS.logInfo("ListContacts", "Start listing contacts... without query");
			Date start = new Date();			
			uri = ContactsContract.Contacts.CONTENT_URI;
			contactsCursor = cr.query(uri, new String[] {
					ContactsContract.Contacts._ID,
					ContactsContract.Contacts.DISPLAY_NAME }, null, null, sortOrder);
			LOGGER_CONTACTS.logDebug("Time lapsed (main query B)",(new Date().getTime()-start.getTime())+" ms");
			
		}
		
		List<ContactLite> contactList = new ArrayList<ContactLite>();
		
		try {
			// iterate cursor to 
			contactsCursor.moveToFirst();
			int excludedContacts = 0;
			Date startLoop = new Date();
			Map<String, ContactLite> contactsIDs = new HashMap<String, ContactLite>();
			String contactsIDsString = "";
			while (!contactsCursor.isAfterLast()) {
				
				Long id = contactsCursor.getLong(0);
				String displayName = contactsCursor.getString(1);
				
				ContactLite contact = new ContactLite();
				contact.setID(id.toString());
				contact.setDisplayName(displayName);
				
				contactsIDs.put(""+id, contact);
				contactsIDsString += (contactsIDsString.length()>0?",":"") + "'" + id + "'";
				contactsCursor.moveToNext();
			}
			LOGGER_CONTACTS.logDebug("Time lapsed (looping contacts)",(new Date().getTime()-startLoop.getTime())+" ms");
			
			// querying the content resolver for all contacts
			
			String selectionMultiple = RawContactsEntity.CONTACT_ID + " IN (" + contactsIDsString +") ";
			Date startRawQuery = new Date();
			Cursor raws = cr.query(RawContactsEntity.CONTENT_URI, null,
					selectionMultiple, null, null);
			LOGGER_CONTACTS.logDebug("Time lapsed (query raw data for all contacts matched)",(new Date().getTime()-startRawQuery.getTime())+" ms");
			
			Date startLoopingRawData = new Date();
			try {
				raws.moveToFirst();
				
				// it seems that there is a time saving by querying the indexes before the cursor loop
				int columnIndex_CONTACTID = raws.getColumnIndex(ContactsContract.RawContactsEntity.CONTACT_ID);
				int columnIndex_MIMETYPE = raws.getColumnIndex(ContactsContract.Data.MIMETYPE);
				int columnIndex_PHONE_NUMBER =  raws.getColumnIndex(Phone.NUMBER);
				int columnIndex_IS_PRIMARY = raws.getColumnIndex(Phone.IS_PRIMARY);
				int columnIndex_PHONE_TYPE = raws.getColumnIndex(Phone.TYPE);
				int columnIndex_EMAIL_ADDRESS = raws.getColumnIndex(CommonDataKinds.Email.DATA1);
				int columnIndex_DISPLAY_NAME = raws.getColumnIndex(Email.DISPLAY_NAME);
				int columnIndex_GROUP_MEMBERSHIP = raws.getColumnIndex(CommonDataKinds.GroupMembership.DATA1);
				
				while (!raws.isAfterLast()) {
					
					String contactId = raws.getString(columnIndex_CONTACTID);
					
					String firstname = null, group = null, lastname = null, name = null;
					List<ContactEmail> emailList = new ArrayList<ContactEmail>();
					List<ContactPhone> phoneList = new ArrayList<ContactPhone>();
					ContactLite contact = null;
					
					if(contactsIDs!=null && contactsIDs.containsKey(contactId)) {
						contact = contactsIDs.get(contactId);
						// asList returns unmodifiable lists, so we need to create new lists from them
						if(contact.getEmails()!=null) emailList.addAll(Arrays.asList(contact.getEmails()));
						if(contact.getPhones()!=null) phoneList.addAll(Arrays.asList(contact.getPhones()));
					}
					
					String type = raws.getString(columnIndex_MIMETYPE);
					if (StructuredName.CONTENT_ITEM_TYPE.equals(type)) {
						// structured name
					} else if (Phone.CONTENT_ITEM_TYPE.equals(type)) {
						// it's a phone, gets all properties and add in
						// the phone list
						ContactPhone phone = new ContactPhone();
						String number = raws.getString(columnIndex_PHONE_NUMBER);
						phone.setNumber(number);
						int primary = raws.getInt(columnIndex_IS_PRIMARY);
						phone.setIsPrimary(primary != 0);
						int phonetype = raws.getInt(columnIndex_PHONE_TYPE);
						phone.setType(getNumberType(phonetype));
						phoneList.add(phone);
					} else if (Email.CONTENT_ITEM_TYPE.equals(type)) {
						// it's an Email address, gets all properties
						// and add in the email list
						ContactEmail email = new ContactEmail();
						String emailaddress = raws.getString(columnIndex_EMAIL_ADDRESS);
						email.setAddress(emailaddress);
						email.setCommonName(raws.getString(columnIndex_DISPLAY_NAME));
						// TODO set first name and last name
						emailList.add(email);
					} else if (GroupMembership.CONTENT_ITEM_TYPE
							.equals(type)) {
						group = raws.getString(columnIndex_GROUP_MEMBERSHIP);
					} 
					raws.moveToNext();
					
					// filling the bean
					if(contact!=null) {
						if(emailList.size()>0) contact.setEmails(emailList.toArray(new ContactEmail[emailList.size()]));
						if(phoneList.size()>0) contact.setPhones(phoneList.toArray(new ContactPhone[phoneList.size()]));
						if(firstname!=null) contact.setFirstname(firstname);
						if(group!=null) contact.setGroup(group);
						if(lastname!=null) contact.setLastname(lastname);
						if(name!=null) contact.setName(name);
						
						// replace contact in the hashmap
						contactsIDs.put(contactId, contact);
					}
				}

			} finally {
				if (raws != null) {
					raws.close();
				}
			}
				
			LOGGER_CONTACTS.logDebug("Time lapsed (end loop raw data)",(new Date().getTime()-startLoopingRawData.getTime())+" ms");
			
			if(query!=null 
						&& query.getColumn().equals(ContactQueryColumn.Phone) 
						&& query.getCondition().equals(ContactQueryCondition.Available)) {
				LOGGER_CONTACTS.logDebug("ListContacts", "Checking contacts to exclude due to condition (phone available)...");
				for(ContactLite contact : contactsIDs.values()) {
					boolean addContactToFilteredList = true;
					if(contact.getPhones() == null || contact.getPhones().length<=0) {
						addContactToFilteredList = false; // do not include contacts without phones available
						excludedContacts++;
					}
					
					if(addContactToFilteredList) contactList.add(contact);
				}
			
				if(excludedContacts>0)
					LOGGER_CONTACTS.logDebug("ListContacts", "Excluded contacts due to condition (phone available): " + excludedContacts);
			} else {
				// convert hashmap to list directly
				contactList = new ArrayList<ContactLite>(contactsIDs.values());
			}
			
		} finally {
			if (contactsCursor != null) {
				contactsCursor.close();
			}
		}
		
		LOGGER_CONTACTS.logInfo("ListContacts", "Contacts list size: " + contactList.size());
		
		IActivityManager am = (IActivityManager) AndroidServiceLocator
				.GetInstance().GetService(
						AndroidServiceLocator.SERVICE_ANDROID_ACTIVITY_MANAGER);	
        am.executeJS("Appverse.Pim.onListContactsEnd", new Object[]{ contactList.toArray()});
        
        LOGGER_CONTACTS.logDebug("Time lapsed (TOTAL)",(new Date().getTime()-startTotal.getTime())+" ms");
		
	}
		
	public void ListContacts() {
		
		LOGGER_CONTACTS.logInfo("ListContacts", "Start listing contacts...");
		Date startTotal = new Date();
		Uri uri = null;
		String selectionSingle;
		Context context = AndroidServiceLocator.getContext();
		ContentResolver cr = context.getContentResolver();
		Cursor contactsCursor = null;
		String sortOrder = ContactsContract.Contacts.DISPLAY_NAME + " COLLATE LOCALIZED ASC";
		
			LOGGER_CONTACTS.logDebug("Contacts selection","##################### ListContacts !query");
			LOGGER_CONTACTS.logInfo("ListContacts", "Start listing contacts... without query");
			Date start = new Date();			
			uri = ContactsContract.Contacts.CONTENT_URI;
			contactsCursor = cr.query(uri, new String[] {
					ContactsContract.Contacts._ID,
					ContactsContract.Contacts.DISPLAY_NAME }, null, null, sortOrder);
			LOGGER_CONTACTS.logDebug("Time lapsed (main query B)",(new Date().getTime()-start.getTime())+" ms");
			
		
		
		List<ContactLite> contactList = new ArrayList<ContactLite>();
		
		try {
			// iterate cursor to 
			contactsCursor.moveToFirst();
			int excludedContacts = 0;
			Date startLoop = new Date();
			Map<String, ContactLite> contactsIDs = new HashMap<String, ContactLite>();
			String contactsIDsString = "";
			while (!contactsCursor.isAfterLast()) {
				
				Long id = contactsCursor.getLong(0);
				String displayName = contactsCursor.getString(1);
				
				ContactLite contact = new ContactLite();
				contact.setID(id.toString());
				contact.setDisplayName(displayName);
				
				contactsIDs.put(""+id, contact);
				contactsIDsString += (contactsIDsString.length()>0?",":"") + "'" + id + "'";
				contactsCursor.moveToNext();
			}
			LOGGER_CONTACTS.logDebug("Time lapsed (looping contacts)",(new Date().getTime()-startLoop.getTime())+" ms");
			
			// querying the content resolver for all contacts
			
			String selectionMultiple = RawContactsEntity.CONTACT_ID + " IN (" + contactsIDsString +") ";
			Date startRawQuery = new Date();
			Cursor raws = cr.query(RawContactsEntity.CONTENT_URI, null,
					selectionMultiple, null, null);
			LOGGER_CONTACTS.logDebug("Time lapsed (query raw data for all contacts matched)",(new Date().getTime()-startRawQuery.getTime())+" ms");
			
			Date startLoopingRawData = new Date();
			try {
				raws.moveToFirst();
				
				// it seems that there is a time saving by querying the indexes before the cursor loop
				int columnIndex_CONTACTID = raws.getColumnIndex(ContactsContract.RawContactsEntity.CONTACT_ID);
				int columnIndex_MIMETYPE = raws.getColumnIndex(ContactsContract.Data.MIMETYPE);
				int columnIndex_PHONE_NUMBER =  raws.getColumnIndex(Phone.NUMBER);
				int columnIndex_IS_PRIMARY = raws.getColumnIndex(Phone.IS_PRIMARY);
				int columnIndex_PHONE_TYPE = raws.getColumnIndex(Phone.TYPE);
				int columnIndex_EMAIL_ADDRESS = raws.getColumnIndex(CommonDataKinds.Email.DATA1);
				int columnIndex_DISPLAY_NAME = raws.getColumnIndex(Email.DISPLAY_NAME);
				int columnIndex_GROUP_MEMBERSHIP = raws.getColumnIndex(CommonDataKinds.GroupMembership.DATA1);
				
				while (!raws.isAfterLast()) {
					
					String contactId = raws.getString(columnIndex_CONTACTID);
					
					String firstname = null, group = null, lastname = null, name = null;
					List<ContactEmail> emailList = new ArrayList<ContactEmail>();
					List<ContactPhone> phoneList = new ArrayList<ContactPhone>();
					ContactLite contact = null;
					
					if(contactsIDs!=null && contactsIDs.containsKey(contactId)) {
						contact = contactsIDs.get(contactId);
						// asList returns unmodifiable lists, so we need to create new lists from them
						if(contact.getEmails()!=null) emailList.addAll(Arrays.asList(contact.getEmails()));
						if(contact.getPhones()!=null) phoneList.addAll(Arrays.asList(contact.getPhones()));
					}
					
					String type = raws.getString(columnIndex_MIMETYPE);
					if (StructuredName.CONTENT_ITEM_TYPE.equals(type)) {
						// structured name
					} else if (Phone.CONTENT_ITEM_TYPE.equals(type)) {
						// it's a phone, gets all properties and add in
						// the phone list
						ContactPhone phone = new ContactPhone();
						String number = raws.getString(columnIndex_PHONE_NUMBER);
						phone.setNumber(number);
						int primary = raws.getInt(columnIndex_IS_PRIMARY);
						phone.setIsPrimary(primary != 0);
						int phonetype = raws.getInt(columnIndex_PHONE_TYPE);
						phone.setType(getNumberType(phonetype));
						phoneList.add(phone);
					} else if (Email.CONTENT_ITEM_TYPE.equals(type)) {
						// it's an Email address, gets all properties
						// and add in the email list
						ContactEmail email = new ContactEmail();
						String emailaddress = raws.getString(columnIndex_EMAIL_ADDRESS);
						email.setAddress(emailaddress);
						email.setCommonName(raws.getString(columnIndex_DISPLAY_NAME));
						// TODO set first name and last name
						emailList.add(email);
					} else if (GroupMembership.CONTENT_ITEM_TYPE
							.equals(type)) {
						group = raws.getString(columnIndex_GROUP_MEMBERSHIP);
					} 
					raws.moveToNext();
					
					// filling the bean
					if(contact!=null) {
						if(emailList.size()>0) contact.setEmails(emailList.toArray(new ContactEmail[emailList.size()]));
						if(phoneList.size()>0) contact.setPhones(phoneList.toArray(new ContactPhone[phoneList.size()]));
						if(firstname!=null) contact.setFirstname(firstname);
						if(group!=null) contact.setGroup(group);
						if(lastname!=null) contact.setLastname(lastname);
						if(name!=null) contact.setName(name);
						
						// replace contact in the hashmap
						contactsIDs.put(contactId, contact);
					}
				}

			} finally {
				if (raws != null) {
					raws.close();
				}
			}
				
			LOGGER_CONTACTS.logDebug("Time lapsed (end loop raw data)",(new Date().getTime()-startLoopingRawData.getTime())+" ms");
			
			
				// convert hashmap to list directly
				contactList = new ArrayList<ContactLite>(contactsIDs.values());
			
			
		} finally {
			if (contactsCursor != null) {
				contactsCursor.close();
			}
		}
		
		LOGGER_CONTACTS.logInfo("ListContacts", "Contacts list size: " + contactList.size());
		
		IActivityManager am = (IActivityManager) AndroidServiceLocator
				.GetInstance().GetService(
						AndroidServiceLocator.SERVICE_ANDROID_ACTIVITY_MANAGER);	
        am.executeJS("Appverse.Pim.onListContactsEnd", new Object[]{ contactList.toArray()});
        
        LOGGER_CONTACTS.logDebug("Time lapsed (TOTAL)",(new Date().getTime()-startTotal.getTime())+" ms");
		
	}
	

	@Override
	public boolean UpdateContact(String ID, Contact newContactData) {
		// TODO implement IContact.UpdateContact
		throw new UnsupportedOperationException("Not supported yet.");
	}

	private static NumberType getNumberType(int phoneType) {
		return PHONE_TYPES_MAP.get(phoneType) == null ? NumberType.Other
				: PHONE_TYPES_MAP.get(phoneType);
	}

	private static int getPhoneType(NumberType numberType) {
		
		for (Entry<Integer, NumberType> e : PHONE_TYPES_MAP.entrySet()) {
			if (e.getValue().equals(numberType)) {
				return e.getKey();
			}
		}
		
		return Phone.TYPE_OTHER;
	}

	private static DispositionType getAddressType(int addressType) {
		return ADDRESS_TYPES_MAP.get(addressType) == null ? DispositionType.Other
				: ADDRESS_TYPES_MAP.get(addressType);
	}

	private static int getAddressType(DispositionType addressType) {
		
		for (Entry<Integer, DispositionType> e : ADDRESS_TYPES_MAP.entrySet()) {
			if (e.getValue().equals(addressType)) {
				return e.getKey();
			}
		}
		
		return StructuredPostal.TYPE_OTHER;
	}

	private static RelationshipType getRelationShipType(int relationType) {
		return RELATION_TYPES_MAP.get(relationType) == null ? RelationshipType.None
				: RELATION_TYPES_MAP.get(relationType);
	}

}
