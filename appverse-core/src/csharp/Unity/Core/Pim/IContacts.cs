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
#if WP8
using System.Threading.Tasks;
#endif
namespace Unity.Core.Pim
{
    public interface IContacts
    {
#if !WP8
		
		/// <summary>
		/// Get all the available data for a Contact
		/// </summary>
		void GetContact (string id);

		/// <summary>
		/// List of stored phone contacts.
		/// </summary>
		void ListContacts ();

		/// <summary>
		/// List of stored phone contacts that match given query.
		/// </summary>
		/// <param name="query">Search query.</param>
		/// <returns>List of contacts.</returns>
		void ListContacts (ContactQuery query);

		/// <summary>
		/// Creates a Contact based on given contact data.
		/// </summary>
		/// <param name="contactData">Contact data.</param>
		/// <returns>Created contact.</returns>
		Contact CreateContact (Contact contactData);

		/// <summary>
		/// Updates contact data (given its ID) with the given contact data.
		/// </summary>
		/// <param name="ID">Contact identifier.</param>
		/// <param name="newContactData">Data to add to contact.</param>
		/// <returns>True on successful update.</returns>
		bool UpdateContact (string ID, Contact newContactData);

		/// <summary>
		/// Deletes the given contact.
		/// </summary>
		/// <param name="contact">Contact to be deleted.</param>
		/// <returns>True on successful deletion.</returns>
		bool DeleteContact (Contact contact);

		/// TODO: share contact.
#else
        /// <summary>
        /// Get all the available data for a Contact
        /// </summary>
        Task GetContact(string id);

        /// <summary>
        /// List of stored phone contacts.
        /// </summary>
        Task ListContacts();

        /// <summary>
        /// List of stored phone contacts that match given query.
        /// </summary>
        /// <param name="query">Search query.</param>
        /// <returns>List of contacts.</returns>
        Task ListContacts(ContactQuery query);

        /// <summary>
        /// Creates a Contact based on given contact data.
        /// </summary>
        /// <param name="contactData">Contact data.</param>
        /// <returns>Created contact.</returns>
        Task<Contact> CreateContact(Contact contactData);

        /// <summary>
        /// Updates contact data (given its ID) with the given contact data.
        /// </summary>
        /// <param name="ID">Contact identifier.</param>
        /// <param name="newContactData">Data to add to contact.</param>
        /// <returns>True on successful update.</returns>
        Task<bool> UpdateContact(string ID, Contact newContactData);

        /// <summary>
        /// Deletes the given contact.
        /// </summary>
        /// <param name="contact">Contact to be deleted.</param>
        /// <returns>True on successful deletion.</returns>
        Task<bool> DeleteContact(Contact contact);
#endif

    }//end IContacts

}//end namespace Pim