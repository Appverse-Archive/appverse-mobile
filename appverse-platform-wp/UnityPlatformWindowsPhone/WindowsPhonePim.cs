/*
 Copyright (c) 2015 GFT Appverse, S.L., Sociedad Unipersonal.

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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Contacts;
using Newtonsoft.Json;
using Unity.Core.Pim;
using UnityPlatformWindowsPhone.Internals;
using Contact = Unity.Core.Pim.Contact;
using ContactEmail = Unity.Core.Pim.ContactEmail;
using ContactPhone = Unity.Core.Pim.ContactPhone;

namespace UnityPlatformWindowsPhone
{
    public sealed class WindowsPhonePim : AbstractPim, IAppverseService
    {
        private readonly List<string> _alphabet = new List<string> { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "_", "-" };
        private const string ListContactsCallback = "Appverse.Pim.onListContactsEnd";
        private const string AccessDeniedContactsCallback = "Appverse.Pim.onAccessToContactsDenied";
        private const string WpContactsFailedCallback = "Appverse.Pim.onWPContactException";
        private const string GetContactCallback = "Appverse.Pim.onContactFound";

        private ConcurrentBag<string> _faultyLetters;
        private ConcurrentBag<string> _startingLetters;
        private readonly ConcurrentDictionary<string, ContactLite> _finalUsers = new ConcurrentDictionary<string, ContactLite>();

        public WindowsPhonePim()
        {
            MethodList = new List<MethodInvoker>(WindowsPhoneUtils.GetMethodInvokersList(this));
        }

        public override async Task ListCalendarEntries(DateTime date)
        {

            throw new NotImplementedException();

        }

        public override async Task ListCalendarEntries(DateTime startDate, DateTime endDate)
        {
            throw new NotImplementedException();

        }

        public override async Task<CalendarEntry> CreateCalendarEntry(CalendarEntry entry)
        {
            throw new NotImplementedException();
        }

        public override async Task<bool> DeleteCalendarEntry(CalendarEntry entry)
        {
            throw new NotImplementedException();
        }

        public override async Task<bool> MoveCalendarEntry(CalendarEntry entry, DateTime newStartDate, DateTime newEndDate)
        {
            throw new NotImplementedException();
        }

        public override async Task GetContact(string id)
        {
            //TODO: Buscar forma alternativa de ID
            if (String.IsNullOrWhiteSpace(id)) return;
            var contactIDSegments = id.Split(new[] { "--" }, StringSplitOptions.RemoveEmptyEntries);
            var contactID = String.Concat("{", contactIDSegments[0], ".", contactIDSegments[1], ".", "}");

            try
            {
                var agenda = await ContactManager.RequestStoreAsync();
                var contact = await agenda.GetContactAsync(contactID);
                if (contact == null) return;
                var returnContact = new ContactLite
                {
                    ID = id,
                    DisplayName = contact.DisplayName,
                    Name = contact.FirstName,
                    Firstname = contact.MiddleName,
                    Lastname = contact.LastName,
                    Phones = GetContactPhonesArray(contact),
                    Emails = GetContactEmailArray(contact)
                };
                WindowsPhoneUtils.InvokeCallback(GetContactCallback, WindowsPhoneUtils.CALLBACKID, JsonConvert.SerializeObject(returnContact));

            }
            catch (UnauthorizedAccessException)
            {
                WindowsPhoneUtils.Log("Not enough privileges to access Contacts");
                WindowsPhoneUtils.InvokeCallback(AccessDeniedContactsCallback, WindowsPhoneUtils.CALLBACKID, null);
            }
            catch (Exception ex)
            {
                WindowsPhoneUtils.Log("Unhandled error recovering contacts:" + ex.Message);
            }

        }

        public override async Task ListContacts()
        {
            ListContacts(null);
        }

        public override async Task ListContacts(ContactQuery query)
        {

            ContactLite[] returnUsers = null;
            Task.Run(async () =>
            {
                var bExceptionRaised = false;
                try
                {
                    var finallist = new ConcurrentBag<ContactLite>();
                    var agenda = await ContactManager.RequestStoreAsync();
                    var foundContactList = await agenda.FindContactsAsync();

                    if (query != null)
                    {
                        WindowsPhoneUtils.Log("Listing contacts by query: " + query);
                        FilterReturnedContacts(foundContactList, finallist, query);
                    }
                    else
                    {
                        WindowsPhoneUtils.Log("Listing ALL contacts...");
                        foundContactList.AsParallel()
                        .ForAll(contact => finallist.Add(new ContactLite
                        {
                            ID = contact.Id.Replace("{", "").Replace(".", "").Replace("}", ""),
                            DisplayName = contact.DisplayName,
                            Name = contact.FirstName,
                            Firstname = contact.MiddleName,
                            Lastname = contact.LastName,
                            Phones = GetContactPhonesArray(contact),
                            Emails = GetContactEmailArray(contact)
                        }));
                    }
                    returnUsers = finallist.ToArray();
                }
                catch (UnauthorizedAccessException ex)
                {
                    WindowsPhoneUtils.Log("Not enough privileges to access Contacts");
                    WindowsPhoneUtils.InvokeCallback(AccessDeniedContactsCallback, WindowsPhoneUtils.CALLBACKID, null);
                    return;
                }
                catch (Exception ex)
                {
                    //error
                    bExceptionRaised = true;
                }

                if (bExceptionRaised)
                {
                    try
                    {
                        _faultyLetters = new ConcurrentBag<string>();
                        _startingLetters = new ConcurrentBag<string>();
                        _finalUsers.Clear();
                        await StartContactSeach(String.Empty, query);
                        returnUsers = _finalUsers.Values.ToArray();


                        _startingLetters.AsParallel().ForAll(startingLetter =>
                        {
                            if (!_faultyLetters.Any(
                                    faultySilabe =>
                                        faultySilabe.StartsWith(startingLetter,
                                            StringComparison.CurrentCultureIgnoreCase)))
                                _faultyLetters.Add(startingLetter);
                        });

                        WindowsPhoneUtils.InvokeCallback(WpContactsFailedCallback, WindowsPhoneUtils.CALLBACKID, JsonConvert.SerializeObject(_faultyLetters.OrderBy(x => x).ToArray()));
                    }
                    catch (Exception ex)
                    {
                        //UNHANDLED ERROR
                        WindowsPhoneUtils.Log("Unhandled error recovering contacts:" + ex.Message);
                    }
                }
                WindowsPhoneUtils.InvokeCallback(ListContactsCallback, WindowsPhoneUtils.CALLBACKID, JsonConvert.SerializeObject(returnUsers));
            });
        }

        public override async Task<Contact> CreateContact(Contact contactData)
        {
            throw new NotImplementedException();
        }

        public override async Task<bool> UpdateContact(string ID, Contact newContactData)
        {
            throw new NotImplementedException();
        }

        public override async Task<bool> DeleteContact(Contact contact)
        {
            throw new NotImplementedException();
        }

        public IReadOnlyList<MethodInvoker> MethodList { get; private set; }

        private ContactPhone[] GetContactPhonesArray(Windows.ApplicationModel.Contacts.Contact contact)
        {
            var phoneList = new List<ContactPhone>();

            foreach (var phone in contact.Phones.Where(phone => !String.IsNullOrWhiteSpace(phone.Number)))
            {
                var conPhone = new ContactPhone { Number = phone.Number };
                switch (phone.Kind)
                {
                    case ContactPhoneKind.Home:
                        conPhone.Type = NumberType.FixedLine;
                        break;

                    case ContactPhoneKind.Mobile:
                        conPhone.Type = NumberType.Mobile;
                        break;

                    case ContactPhoneKind.Work:
                        conPhone.Type = NumberType.Work;
                        break;

                    default:
                        conPhone.Type = NumberType.Other;
                        break;
                }
                phoneList.Add(conPhone);
            }
            return phoneList.ToArray();
        }

        private ContactEmail[] GetContactEmailArray(Windows.ApplicationModel.Contacts.Contact contact)
        {
            var emailList = new List<ContactEmail>();
            foreach (var email in contact.Emails.Where(email => !String.IsNullOrWhiteSpace(email.Address)))
            {
                var conEmail = new ContactEmail { Address = email.Address };
                switch (email.Kind)
                {
                    case ContactEmailKind.Personal:
                        conEmail.Type = DispositionType.Personal;
                        break;

                    case ContactEmailKind.Work:
                        conEmail.Type = DispositionType.Work;
                        break;

                    default:
                        conEmail.Type = DispositionType.Other;
                        break;
                }
                emailList.Add(conEmail);
            }
            return emailList.ToArray();
        }

        private async Task StartContactSeach(string sLetters, ContactQuery query)
        {
            var subTask = new ConcurrentBag<Task>();
            Parallel.ForEach(_alphabet, letter =>
            {
                try
                {
                    var sStartLetters = (String.IsNullOrWhiteSpace(sLetters)) ? letter : String.Concat(sLetters, letter);
                    subTask.Add(SearchContactsWithLetter(sStartLetters, query));
                }
                catch (Exception ex)
                {
                    WindowsPhoneUtils.Log(ex.Message);
                }
            });
            await Task.WhenAll(subTask);
        }

        private async Task SearchContactsWithLetter(string sLetters, ContactQuery query)
        {
            var bExceptionRaised = false;
            try
            {
                var agenda = await ContactManager.RequestStoreAsync();
                var contacts = await agenda.FindContactsAsync(sLetters);
                var finallist = new ConcurrentBag<ContactLite>();
                FilterReturnedContacts(contacts, finallist, query);
                finallist.AsParallel().ForAll(contact => _finalUsers.TryAdd(contact.ID, contact));
            }
            catch (Exception ex)
            {
                //Security Error --> '@' character in websites collection URL
                if (ex.HResult == -2146697202)
                {
                    bExceptionRaised = true;
                }
            }
            if (bExceptionRaised)
            {
                if (sLetters.Length < 2)
                {
                    _startingLetters.Add((sLetters));
                    await StartContactSeach(sLetters, query);
                }
                else
                    _faultyLetters.Add(sLetters);
            }
        }

        private void FilterReturnedContacts(IReadOnlyList<Windows.ApplicationModel.Contacts.Contact> foundContactList, ConcurrentBag<ContactLite> finallist, ContactQuery query)
        {
            var value = query.Value;
            switch (query.Column)
            {
                case ContactQueryColumn.ID:
                    //WindowsPhoneUtils.Log("by ID");
                    foundContactList.AsParallel()
                        .ForAll(contact => finallist.Add(new ContactLite
                        {
                            ID = contact.Id.Replace("{", "").Replace(".", "").Replace("}", ""),
                            DisplayName = contact.DisplayName,
                            Name = contact.FirstName,
                            Firstname = contact.MiddleName,
                            Lastname = contact.LastName,
                            Phones = GetContactPhonesArray(contact),
                            Emails = GetContactEmailArray(contact)
                        }));
                    break;
                case ContactQueryColumn.Phone:
                    //WindowsPhoneUtils.Log("by Phone available");
                    foundContactList.AsParallel()
                        .Where(contact => contact.Phones != null && contact.Phones.Count > 0)
                        .ForAll(contact => finallist.Add(new ContactLite
                        {
                            ID = contact.Id.Replace("{", "").Replace(".", "").Replace("}", ""),
                            DisplayName = contact.DisplayName,
                            Name = contact.FirstName,
                            Firstname = contact.MiddleName,
                            Lastname = contact.LastName,
                            Phones = GetContactPhonesArray(contact),
                            Emails = GetContactEmailArray(contact)
                        }));
                    break;
                case ContactQueryColumn.Name:
                    //WindowsPhoneUtils.Log("by NAME in a list of " + foundContactList.Count + " people");
                    switch (query.Condition)
                    {
                        case ContactQueryCondition.Equals:
                            //WindowsPhoneUtils.Log("EQUALS ");
                            foundContactList.AsParallel()
                                .Where(contact =>
                                    (!String.IsNullOrWhiteSpace(contact.DisplayName) && contact.DisplayName.Equals(value))
                                    || (!String.IsNullOrWhiteSpace(contact.FirstName) && contact.FirstName.Equals(value))
                                    || (!String.IsNullOrWhiteSpace(contact.LastName) && contact.LastName.Equals(value))
                                    || (!String.IsNullOrWhiteSpace(contact.MiddleName) && contact.MiddleName.Equals(value))
                                    || (!String.IsNullOrWhiteSpace(contact.Name) && contact.Name.Equals(value)))
                                .ForAll(contact => finallist.Add(new ContactLite
                                {
                                    ID = contact.Id.Replace("{", "").Replace(".", "").Replace("}", ""),
                                    DisplayName = contact.DisplayName,
                                    Name = contact.FirstName,
                                    Firstname = contact.MiddleName,
                                    Lastname = contact.LastName,
                                    Phones = GetContactPhonesArray(contact),
                                    Emails = GetContactEmailArray(contact)
                                }));
                            break;
                        case ContactQueryCondition.StartsWith:
                            //WindowsPhoneUtils.Log("STARTSWITH ");
                            foundContactList.AsParallel()
                                .Where(contact =>
                                    (!String.IsNullOrWhiteSpace(contact.DisplayName) && contact.DisplayName.StartsWith(value))
                                    || (!String.IsNullOrWhiteSpace(contact.FirstName) && contact.FirstName.StartsWith(value))
                                    || (!String.IsNullOrWhiteSpace(contact.LastName) && contact.LastName.StartsWith(value))
                                    || (!String.IsNullOrWhiteSpace(contact.MiddleName) && contact.MiddleName.StartsWith(value))
                                    || (!String.IsNullOrWhiteSpace(contact.Name) && contact.Name.StartsWith(value)))
                                .ForAll(contact => finallist.Add(new ContactLite
                                {
                                    ID = contact.Id.Replace("{", "").Replace(".", "").Replace("}", ""),
                                    DisplayName = contact.DisplayName,
                                    Name = contact.FirstName,
                                    Firstname = contact.MiddleName,
                                    Lastname = contact.LastName,
                                    Phones = GetContactPhonesArray(contact),
                                    Emails = GetContactEmailArray(contact)
                                }));
                            break;
                        case ContactQueryCondition.EndsWith:
                            //WindowsPhoneUtils.Log("ENDSWITH");
                            foundContactList.AsParallel()
                                .Where(contact =>
                                    (!String.IsNullOrWhiteSpace(contact.DisplayName) && contact.DisplayName.EndsWith(value))
                                    || (!String.IsNullOrWhiteSpace(contact.FirstName) && contact.FirstName.EndsWith(value))
                                    || (!String.IsNullOrWhiteSpace(contact.LastName) && contact.LastName.EndsWith(value))
                                    || (!String.IsNullOrWhiteSpace(contact.MiddleName) && contact.MiddleName.EndsWith(value))
                                    || (!String.IsNullOrWhiteSpace(contact.Name) && contact.Name.EndsWith(value)))
                                .ForAll(contact => finallist.Add(new ContactLite
                                {
                                    ID = contact.Id.Replace("{", "").Replace(".", "").Replace("}", ""),
                                    DisplayName = contact.DisplayName,
                                    Name = contact.FirstName,
                                    Firstname = contact.MiddleName,
                                    Lastname = contact.LastName,
                                    Phones = GetContactPhonesArray(contact),
                                    Emails = GetContactEmailArray(contact)
                                }));
                            break;
                        case ContactQueryCondition.Contains:
                            //WindowsPhoneUtils.Log("CONTAINS");
                            foundContactList.AsParallel()
                                .Where(contact =>
                                    (!String.IsNullOrWhiteSpace(contact.DisplayName) && contact.DisplayName.Contains(value))
                                    || (!String.IsNullOrWhiteSpace(contact.FirstName) && contact.FirstName.Contains(value))
                                    || (!String.IsNullOrWhiteSpace(contact.LastName) && contact.LastName.Contains(value))
                                    || (!String.IsNullOrWhiteSpace(contact.MiddleName) && contact.MiddleName.Contains(value))
                                    || (!String.IsNullOrWhiteSpace(contact.Name) && contact.Name.Contains(value)))
                                .ForAll(contact => finallist.Add(new ContactLite
                                {
                                    ID = contact.Id.Replace("{", "").Replace(".", "").Replace("}", ""),
                                    DisplayName = contact.DisplayName,
                                    Name = contact.FirstName,
                                    Firstname = contact.MiddleName,
                                    Lastname = contact.LastName,
                                    Phones = GetContactPhonesArray(contact),
                                    Emails = GetContactEmailArray(contact)
                                }));
                            break;
                    }
                    break;
            }
        }
    }
}
