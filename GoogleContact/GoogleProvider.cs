using System;
using System.Collections.Generic;
using System.Data;
using System.Collections;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Diagnostics.Eventing;
using System.Drawing;

using Google.GData.Contacts;
using Google.GData.Client;
using Google.GData.Extensions;

using Outlook = Microsoft.Office.Interop.Outlook;
using Office = Microsoft.Office.Core;
using System.IO;


namespace GoogleContact
{
    /// <summary>
    /// Google provder, it's uses as singletone class
    /// </summary>
    class GoogleProvider
    {
        #region global variables
        private string _userName = "";
        private string _userPwd = "";
        private Google.Contacts.ContactsRequest _cr = null;
        private bool _isLogon = false;
        private static GoogleProvider _gp = null;
        private Feed<Google.Contacts.Contact> _contactItems = null;
        private bool _isUpdated = true;
        #endregion

        #region Creator and reference to singletone
        private GoogleProvider()
        {
            LoggerProvider.Instance.Logger.Debug("Class GoogleProvider created");
        }
        /// <summary>
        /// Return singletone reference to this class
        /// </summary>
        public static GoogleProvider GetProvider
        {
            get
            {
                if (_gp == null)
                {
                    _gp = new GoogleProvider();
                    LoggerProvider.Instance.Logger.Debug("Request for create new GoogleProvider");
                   
                }
                LoggerProvider.Instance.Logger.Debug("Return current GoogleProvider class");
                return _gp;
            }
        }
        /// <summary>
        /// ContactRequest, is internal class doesn't exist, system try create it and prepare for work. 
        /// When inrenal class is ready system reuse it.
        /// </summary>
        private Google.Contacts.ContactsRequest cr
        {
            get
            {
                //LoggerProvider.Instance.Logger.Debug("try RequestSettings(Constants.ApplicationName, _userName, _userPwd) ({0},{1},{2})", Constants.ApplicationName, _userName, _userPwd);
                if (_cr == null)
                {
                    LoggerProvider.Instance.Logger.Debug("Google.Contacts.ContactRequest doesn't exist");
                    RequestSettings rs = new RequestSettings(Constants.ApplicationName, _userName, _userPwd);
                    rs.AutoPaging = true;
                    try
                    {
                        _cr = new Google.Contacts.ContactsRequest(rs);
                        _isLogon = true;
                    }
                    catch (System.Exception e)
                    {
                        _isLogon = false;
                        _cr = null;
                        LoggerProvider.Instance.Logger.Error("Problem try read all contacts Request.");
                        LoggerProvider.Instance.Logger.Error(e);
                    }
                }
                LoggerProvider.Instance.Logger.Debug(_cr == null ? "Return NULL for Google.Contacts.ContactRequest" : "Return actual Google.Contacts.ContactRequest");
                return _cr;
            }
        }
        /// <summary>
        /// Log off and close CS
        /// </summary>
        private void Logoff()
        {
            if (_isLogon)
            {
                LoggerProvider.Instance.Logger.Debug("Log off from Google account");
                _cr = null;
                _isLogon = false;
            }
        }
        #endregion

        #region Logon methods and Feed<ContactItems>
        /// <summary>
        /// Prepare class for request with specified credentials
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        public bool Logon(string username, string password)
        {
            bool noMatch = true;
            if (_userName != username.ToLower())
            {
                _userName = username.ToLower();
                noMatch = false;
            }
            if (_userPwd != password)
            {
                _userPwd = password;
                noMatch = false;
            }
            if (_isLogon && !noMatch) // new user or password. Need reload
                Logoff();
            LoggerProvider.Instance.Logger.Debug("Try logon to google account");
            Google.Contacts.ContactsRequest s = cr; // prepare class for request ... this isn't realy touch google
            return _isLogon;
        }
        /// <summary>
        /// Use local credentials and prepare class for request without any touch of google
        /// </summary>
        /// <returns></returns>
        public bool Logon()
        {
            return Logon(_userName, _userPwd);
        }
        /// <summary>
        /// Is class for google request prepared? (uses method Logon())
        /// </summary>
        public bool isLogon
        {
            get { return _isLogon; }
        }

        /// <summary>
        /// Return contact feed. Is possible to reuse it
        /// </summary>
        public Feed<Google.Contacts.Contact> ContactItems
        {
            get
            {
                if ((_contactItems == null) || _isUpdated)
                {
                    _contactItems = cr.GetContacts();
                    LoggerProvider.Instance.Logger.Debug("Now first time read od re-read Contact feed");
                }
                else
                {

                }
                _isUpdated = false;
                return _contactItems;
            }
        }
        public void ClearContactItems()
        {
            _isUpdated=true;
        }
        #endregion

        #region helper function
        /// <summary>
        /// Number of contact in feed
        /// </summary>
        /// <returns></returns>
        public int CountContact()
        {
            if (!isLogon)
            {
                LoggerProvider.Instance.Logger.Error("GoogleProvider can't count contact, because it don't log on");
                return 0;
            }
            LoggerProvider.Instance.Logger.Debug("Actual on google Contact Feed start read");
            int i = ContactItems.TotalResults;  //.Entries.Count();
            LoggerProvider.Instance.Logger.Debug("Actual on google Contact Feed {0} contact(s)", i);
            return i;
        }

        /// <summary>
        /// Return Image for specific contact
        /// </summary>
        /// <param name="contact"></param>
        /// <returns></returns>
        public Image GetImage(Google.Contacts.Contact contact)
        {
            Stream photoStream = null;
            Image img = null;
            try
            {
                photoStream = cr.GetPhoto(contact);
                img = Image.FromStream(photoStream);
            }
            catch (Exception e)
            {
                LoggerProvider.Instance.Logger.Error(e);
            }
            return img;
        }
        #endregion

        #region Insert/Update/Delete
        /// <summary>
        /// Insert new contact to Google and return it's references
        /// </summary>
        /// <param name="contact"></param>
        /// <returns></returns>
        public Google.Contacts.Contact Insert(Google.Contacts.Contact contact)
        {
            Uri feedUri = new Uri(ContactsQuery.CreateContactsUri("default"));
            Google.Contacts.Contact ret = null;
            try
            {
                ret = cr.Insert(feedUri, contact);
                _isUpdated = true;
                LoggerProvider.Instance.Logger.Debug("New contact inserted sucessful");
            }
            catch (GDataRequestException e)
            {
                Stream receiver = e.Response.GetResponseStream();
                if (receiver != null)
                {
                    // Pipe the stream to ahigher level stream reader with the default encoding 
                    // format. which is UTF8 
                    StreamReader readStream = new StreamReader(receiver);

                    // Read 256 charcters at a time. 
                    char[] buffer = new char[256];
                    StringBuilder builder = new StringBuilder(1024);
                    int count = readStream.Read(buffer, 0, 256);
                    while (count > 0)
                    {
                        builder.Append(buffer);
                        count = readStream.Read(buffer, 0, 256);
                    }

                    // Release the resources of stream object. 
                    readStream.Close();
                    receiver.Close();
                    LoggerProvider.Instance.Logger.Error(builder.ToString());
                    LoggerProvider.Instance.Logger.Error(e);
                }
            }
            return ret;
        }

        /// <summary>
        /// Update curent contact
        /// </summary>
        /// <param name="contact"></param>
        /// <returns></returns>
        public Google.Contacts.Contact Update(Google.Contacts.Contact contact)
        {
            Google.Contacts.Contact ret = null;
            try
            {
                ret = cr.Update(contact);
                _isUpdated = true;
                LoggerProvider.Instance.Logger.Debug("New contact Updated sucessful");
            }
            catch (GDataRequestException e)
            {
                Stream receiver = e.Response.GetResponseStream();
                if (receiver != null)
                {
                    // Pipe the stream to ahigher level stream reader with the default encoding 
                    // format. which is UTF8 
                    StreamReader readStream = new StreamReader(receiver);

                    // Read 256 charcters at a time. 
                    char[] buffer = new char[256];
                    StringBuilder builder = new StringBuilder(1024);
                    int count = readStream.Read(buffer, 0, 256);
                    while (count > 0)
                    {
                        builder.Append(buffer);
                        count = readStream.Read(buffer, 0, 256);
                    }

                    // Release the resources of stream object. 
                    readStream.Close();
                    receiver.Close();
                    LoggerProvider.Instance.Logger.Error(builder.ToString());
                    LoggerProvider.Instance.Logger.Error(e);
                }
            }
            return ret;
        }

        /// <summary>
        /// Delete contact
        /// </summary>
        /// <param name="contact"></param>
        public void Delete(Google.Contacts.Contact contact)
        {
            try
            {
                cr.Delete(contact);
                _isUpdated = true;
                LoggerProvider.Instance.Logger.Debug("Contact delete sucesfull");
            }
            catch (GDataRequestException e)
            {
                Stream receiver = e.Response.GetResponseStream();
                if (receiver != null)
                {
                    // Pipe the stream to ahigher level stream reader with the default encoding 
                    // format. which is UTF8 
                    StreamReader readStream = new StreamReader(receiver);

                    // Read 256 charcters at a time. 
                    char[] buffer = new char[256];
                    StringBuilder builder = new StringBuilder(1024);
                    int count = readStream.Read(buffer, 0, 256);
                    while (count > 0)
                    {
                        builder.Append(buffer);
                        count = readStream.Read(buffer, 0, 256);
                    }

                    // Release the resources of stream object. 
                    readStream.Close();
                    receiver.Close();
                    LoggerProvider.Instance.Logger.Error(builder.ToString());
                    LoggerProvider.Instance.Logger.Error(e);
                }
            }
        }
        #endregion
    }
}
