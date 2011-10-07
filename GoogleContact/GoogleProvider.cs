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
        private Google.Contacts.ContactsRequest _cr;
        private bool _isLogon;
        private static GoogleProvider _gp;
//        private Feed<Google.Contacts.Contact> _contactItems;
//        private Feed<Google.Contacts.Group> _contactGroups;
        private Dictionary<string, Google.Contacts.Group> _groupList;
//        private bool _isUpdated = true;
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
                if (_cr == null)
                    LoggerProvider.Instance.Logger.Debug("Return NULL for Google.Contacts.ContactRequest");
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

        #region Log-on methods
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
        /// Is class for google request prepared? (uses method Logon())
        /// </summary>
        public bool isLogon
        {
            get { return _isLogon; }
        }
        #endregion

        #region Feed read ContactItem
        /// <summary>
        /// Return contact feed. Is possible to reuse it
        /// </summary>
        public Feed<Google.Contacts.Contact> ContactItems
        {
            get
            {
                return cr.GetContacts(); // now read on every time all contacts
            }
        }
        /// <summary>
        /// Read all contact from google witch change after LastCacheTime
        /// </summary>
        /// <param name="LastCacheTime"></param>
        /// <returns></returns>
        public Feed<Google.Contacts.Contact> ContactItemsChangedAfter(DateTime LastCacheTime)
        {
            ContactsQuery query = new ContactsQuery(ContactsQuery.CreateContactsUri("default"));
            query.StartDate = LastCacheTime;
            query.ShowDeleted = true;
            return cr.Get<Google.Contacts.Contact>(query);
        }
        /// <summary>
        /// Return one contact based on their conntact ID
        /// </summary>
        /// <param name="ContactID">Same asi MyID from OneContact structure</param>
        /// <returns></returns>
        public Google.Contacts.Contact GetOneContact(string ContactID)
        {
            return cr.Retrieve<Google.Contacts.Contact>(new Uri(ContactID));
        }
        #endregion

        #region Feed Group
        /// <summary>
        /// Find detail information about group by Name
        /// </summary>
        /// <param name="GroupName"></param>
        /// <returns>Return NULL when Group doesn't exist</returns>
        public Google.Contacts.Group GetContactGroupByName(string GroupName)
        {
            ContactGroupsInitialize();

            if (GroupName.StartsWith("System Group: ")) // for all system group need remove this part from name
                GroupName = GroupName.Substring("System Group: ".Length);
            if (_groupList.ContainsKey(GroupName))
                return _groupList[GroupName];
            return null;
        }
        /// <summary>
        /// Find detail information about Group by their ID
        /// </summary>
        /// <param name="GroupID"></param>
        /// <returns>Return NULL when Group doesn't exist</returns>
        public Google.Contacts.Group GetContactGroupByID(string GroupID)
        {
            ContactGroupsInitialize();
            foreach(Google.Contacts.Group g in _groupList.Values)
                if (g.Id==GroupID)
                    return g;
            return null;
        }
        /// <summary>
        /// Create new group by group name
        /// </summary>
        /// <param name="GroupName"></param>
        /// <returns></returns>
        public Google.Contacts.Group AddContactGroup(string GroupName)
        {
            ContactGroupsInitialize();
            Google.Contacts.Group newGroup = new Google.Contacts.Group();
            newGroup.Title = GroupName;
            Google.Contacts.Group createdGroup = cr.Insert(new Uri("https://www.google.com/m8/feeds/groups/default/full"), newGroup);
            _groupList.Add(createdGroup.Title, createdGroup);
            return createdGroup;
        }
        /// <summary>
        /// Gets default Group System Group "My Contacts"
        /// </summary>
        /// <returns>If blanks the system Group is missing</returns>
        public string GetMyContactDefaultGroupID()
        {
            ContactGroupsInitialize();
            foreach (Google.Contacts.Group g in _groupList.Values)
                if (g.SystemGroup == "Contacts")
                    return g.Id;
            return "";
        }
        /// <summary>
        /// Delete non-system group
        /// </summary>
        /// <param name="GroupName"></param>
        public void DeleteContactGroupName(string GroupName)
        {
            // Retrieving the contact group is required in order to get the Etag.
            Google.Contacts.Group group = GetContactGroupByName(GroupName);
            if (!group.ReadOnly)
            {
                try
                {
                    cr.Delete(group);
                }
                catch (GDataVersionConflictException e)
                {
                    LoggerProvider.Instance.Logger.Error("Can't delete Contact Group from Google {0}", GroupName);
                    LoggerProvider.Instance.Logger.Error(e);
                }
                _groupList.Remove(GroupName);
            }
        }
        /// <summary>
        /// Check group list and update dictionary
        /// </summary>
        private void ContactGroupsInitialize()
        {
            //if (_contactGroups == null)
            {
                Feed<Google.Contacts.Group> _contactGroups = cr.GetGroups();
                LoggerProvider.Instance.Logger.Debug("Read Google Groups");
                //if (_contactItems == null)
                //    throw new NullReferenceException("Can't get Google.Contacts.Contact feed.");
                // Fill internal Dictionary
                if (_groupList == null)
                    _groupList = new Dictionary<string, Google.Contacts.Group>();
                else
                    _groupList.Clear();
                foreach (Google.Contacts.Group g in _contactGroups.Entries)
                {
                    if (string.IsNullOrEmpty(g.SystemGroup))
                    {
                        if (!_groupList.ContainsKey(g.Title)) // problem when rename my group to same name as System Group
                            _groupList.Add(g.Title, g);
                    }
                    else
                    {
                        if (!_groupList.ContainsKey(g.SystemGroup))
                            _groupList.Add(g.SystemGroup, g);
                    }
                }
            }
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
        #endregion

        #region Work with Image
        /// <summary>
        /// Return Image for specific contact
        /// </summary>
        /// <param name="contact"></param>
        /// <returns></returns>
        public Image GetImage(Google.Contacts.Contact contact)
        {
            Image img=null;
            try
            {
                contact.PhotoEtag = "";
                using (Stream photoStream = cr.GetPhoto(contact))
                    img = Image.FromStream(photoStream);
            }
            catch (GDataNotModifiedException gd)
            {
                
                LoggerProvider.Instance.Logger.Error("Problem when read data (GDataNotModifiedException)");
                LoggerProvider.Instance.Logger.Error(gd);
            }
            catch (GDataRequestException re)
            {
                LoggerProvider.Instance.Logger.Error("Problem when read data (GDataRequestException)");
                LoggerProvider.Instance.Logger.Error(re);
            }
            return img;
        }

        /// <summary>
        /// Add or Update photo in RAW Google contact
        /// </summary>
        /// <param name="contact"></param>
        /// <param name="photoContact"></param>
        public Google.Contacts.Contact AddOrUpdateContactPhoto(Google.Contacts.Contact contact, string photoPath)
        {
            using (Stream s = new FileStream(photoPath, FileMode.Open))
            {
                LoggerProvider.Instance.Logger.Debug("Start upload picture to Google contact: {0}", photoPath);
                string et = contact.PhotoEtag;
                try
                {
                    cr.SetPhoto(contact, s);
                    contact = cr.Retrieve<Google.Contacts.Contact>(new Uri(contact.Id));
                }
                catch (GDataVersionConflictException e)
                {
                    LoggerProvider.Instance.Logger.Error("Problem in Update photo");
                    LoggerProvider.Instance.Logger.Error(e);
                }
                catch (ArgumentNullException ee)
                {
                    LoggerProvider.Instance.Logger.Error("Problem in Update photo");
                    LoggerProvider.Instance.Logger.Error(ee);
                }
                catch (GDataRequestException ge)
                {
                    using (Stream receiver = ge.Response.GetResponseStream())
                    {
                        if (receiver != null)
                        {
                            StringBuilder builder = new StringBuilder(1024);
                            using (StreamReader readStream = new StreamReader(receiver))
                            {

                                char[] buffer = new char[256];
                                int count = readStream.Read(buffer, 0, 256);
                                while (count > 0)
                                {
                                    builder.Append(buffer);
                                    count = readStream.Read(buffer, 0, 256);
                                }
                                readStream.Close();
                            }
                            receiver.Close();
                            LoggerProvider.Instance.Logger.Error("Error in Add or Update image to Google Contact.\r\n{0}", builder.ToString());
                            LoggerProvider.Instance.Logger.Error(ge);
                        }
                    }
                }
                LoggerProvider.Instance.Logger.Debug("Old/New Etag is: {0}/{1}", et, contact.PhotoEtag);
            }
            return contact;
        }
        /// <summary>
        /// Delete photo from Google contact
        /// </summary>
        /// <param name="contact"></param>
        public void DeleteContactPhoto(Google.Contacts.Contact contact)
        {
            try
            {
                cr.Delete(contact.PhotoUri, contact.PhotoEtag);
            }
            catch (GDataVersionConflictException e)
            {
                LoggerProvider.Instance.Logger.Error("Problem in delete photo");
                LoggerProvider.Instance.Logger.Error(e);
            }
        }
        #endregion

        #region Insert/Update/Delete Contact Item
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
//                _isUpdated = true;
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
//                _isUpdated = true;
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
//                _isUpdated = true;
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
