﻿///#define NoImage
/// Now is problem in image define in project
/// Image save to C:\Users\[UserName]\AppData\Local\Temp\
///#define ANNIVESARY_NOT_WORK
/// Now problem in annivesary - define in project
using System;
using System.Collections.Generic;
using System.Data;
using System.Collections;
using System.IO;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Diagnostics.Eventing;
using System.Drawing;
using System.Text.RegularExpressions;

using Google.GData.Contacts;
using Google.GData.Client;
using Google.GData.Extensions;

using Outlook = Microsoft.Office.Interop.Outlook;
using Office = Microsoft.Office.Core;

namespace GoogleContact
{
#if (NoImage)
#warning NoImage - system doesn't work with Image. Problem when read and save to Google. 
#endif
#if (ANNIVESARY_NOT_WORK)
#warning ANNIVESARY_NOT_WORK - system doesn't work with Annivesary. Problem when save to Google.
#endif
    /// <summary>
    /// Reprezentuje jeden kontakt
    /// </summary>
    class OneContact : OneContactBase
    {

        #region Create instance from source
        /// <summary>
        /// Create from GoogleContac Item
        /// </summary>
        /// <param name="SourceData"></param>
        public OneContact(Google.Contacts.Contact SourceData)
        {
            #region pocatecni kontrola pro naplneni
            _isFromOutlook = false;
            if (string.IsNullOrEmpty(SourceData.Id)) /// neni EntryID a to potrebuji chyba
            {
                LoggerProvider.Instance.Logger.Error("Google contact does'n has ID");
                throw new Exception("Google contact does'n has ID");
            }
            _MyID = SourceData.Id;
            _rawSource = SourceData;
            _ModifyDateTime = SourceData.Updated; // datum posledni modifikace

            if (SourceData.ExtendedProperties.Count > 0) // existuje property
            {
                foreach (ExtendedProperty en in SourceData.ExtendedProperties) // vybereme tu správnou a vyplnime hodnoty
                {
                    if (en.Name == Constants.NameGoogleExtendProperies)
                    {
                        _referenceID = GetSavedReplicaID(en.Value);
                    }
                }
            }
            #endregion

            #region Zpracovani personalnich udaju
            Title = string.IsNullOrEmpty(SourceData.Name.NamePrefix) ? "" : SourceData.Name.NamePrefix;
            FirstName = string.IsNullOrEmpty(SourceData.Name.GivenName) ? "" : SourceData.Name.GivenName;
            MiddleName = string.IsNullOrEmpty(SourceData.Name.AdditionalName) ? "" : SourceData.Name.AdditionalName;
            LastName = string.IsNullOrEmpty(SourceData.Name.FamilyName) ? "" : SourceData.Name.FamilyName;
            Suffix = string.IsNullOrEmpty(SourceData.Name.NameSuffix) ? "" : SourceData.Name.NameSuffix;
            if (!string.IsNullOrEmpty(SourceData.ContactEntry.Birthday))
            {
                Birthday = DateTime.Parse(SourceData.ContactEntry.Birthday);
            }
#if (!ANNIVESARY_NOT_WORK)
            if (SourceData.ContactEntry.Events.Count > 0) // je zde mozna svatek
            {
                foreach (Event en in SourceData.ContactEntry.Events)
                {
                    if (en.Relation == "anniversary")
                        Anniversary = en.When.StartTime;
                }
            }
#endif
            Notes = SourceData.Content;

#if (!NoImage)
            if (!string.IsNullOrEmpty(SourceData.PhotoEtag))
                ImagePath = Utils.GetContactPicturePath(SourceData);
#endif
            if (SourceData.IMs.Count > 0) // Existuje IM
            {
                string imHelp = "";
                bool notMSN = true;
                foreach (IMAddress ia in SourceData.IMs)
                {
                    if (string.IsNullOrEmpty(imHelp))
                        imHelp = ia.Value;
                    if ((ia.Protocol == "MSN") && notMSN)// pouzij MSN jinak prvni
                    {
                        notMSN = false;
                        imHelp = ia.Address;
                    }
                }
                IM = imHelp;
            }
            #endregion

            #region Zpracovani telefonu
            if (SourceData.Phonenumbers.Count > 0) // jsou telefony
            {
                bool notMobil = true;
                bool notBus1 = true;
                bool notBus2 = true;
                bool notOther = true;
                bool notHome = true;
                bool notFaxHome = true;
                bool notFaxbus = true;
                foreach (PhoneNumber pn in SourceData.Phonenumbers)
                {
                    switch (pn.Rel)
                    {
                        case ContactsRelationships.IsOther:
                            if (notOther)
                                Telephone.Add(Enum.GetName(typeof(Constants.PhoneType), Constants.PhoneType.Other),
                                    new PhoneDetail(pn.Value, Constants.PhoneType.Other));
                            notOther = false;
                            break;
                        case ContactsRelationships.IsWork:
                            if (notBus1)
                            {
                                Telephone.Add(Enum.GetName(typeof(Constants.PhoneType), Constants.PhoneType.Business1),
                                    new PhoneDetail(pn.Value, Constants.PhoneType.Business1));
                                notBus1 = false;
                            }
                            else if (notBus2)
                            {
                                Telephone.Add(Enum.GetName(typeof(Constants.PhoneType), Constants.PhoneType.Business2),
                                    new PhoneDetail(pn.Value, Constants.PhoneType.Business2));
                                notBus2 = false;
                            }
                            break;
                        case ContactsRelationships.IsMobile:
                            if (notMobil)
                                Telephone.Add(Enum.GetName(typeof(Constants.PhoneType), Constants.PhoneType.Mobile),
                                    new PhoneDetail(pn.Value, Constants.PhoneType.Mobile));
                            notMobil = false;
                            break;
                        case ContactsRelationships.IsHome:
                            if (notHome)
                                Telephone.Add(Enum.GetName(typeof(Constants.PhoneType), Constants.PhoneType.Home),
                                    new PhoneDetail(pn.Value, Constants.PhoneType.Home));
                            notHome = true;
                            break;
                        case "work_fax":
                            if (notFaxbus)
                                Telephone.Add(Enum.GetName(typeof(Constants.PhoneType), Constants.PhoneType.Fax_work),
                                    new PhoneDetail(pn.Value, Constants.PhoneType.Fax_work));
                            notFaxbus = true;
                            break;
                        case ContactsRelationships.IsHomeFax:
                            if (notFaxHome)
                                Telephone.Add(Enum.GetName(typeof(Constants.PhoneType), Constants.PhoneType.Fax_home),
                                    new PhoneDetail(pn.Value, Constants.PhoneType.Fax_home));
                            notFaxHome = true;
                            break;
                        case ContactsRelationships.IsWorkFax:
                            if (notFaxbus)
                                Telephone.Add(Enum.GetName(typeof(Constants.PhoneType), Constants.PhoneType.Fax_work),
                                  new PhoneDetail(pn.Value, Constants.PhoneType.Fax_work));
                            notFaxbus = true;
                            break;
                    }
                }
            }
            #endregion

            #region adresy
            if (SourceData.PostalAddresses.Count > 0)
            {
                bool notHome = true;
                bool notWork = true;
                bool notOther = true;
                foreach (StructuredPostalAddress em in SourceData.PostalAddresses)
                {
                    switch (em.Rel)
                    {
                        case ContactsRelationships.IsWork:
                            if (notWork)
                                Address.Add(Enum.GetName(typeof(Constants.AddressType), Constants.AddressType.Business),
                                    new AddressDetail(Constants.AddressType.Business, em.Street, em.Pobox,
                                    em.City, em.Postcode, em.Country, em.Region));
                            notWork = false;
                            break;
                        case ContactsRelationships.IsHome:
                            if (notHome)
                                Address.Add(Enum.GetName(typeof(Constants.AddressType), Constants.AddressType.Home),
                                    new AddressDetail(Constants.AddressType.Home, em.Street, em.Pobox,
                                    em.City, em.Postcode, em.Country, em.Region));
                            notHome = false;
                            break;
                        case ContactsRelationships.IsOther:
                            if (notOther)
                                Address.Add(Enum.GetName(typeof(Constants.AddressType), Constants.AddressType.Other),
                                    new AddressDetail(Constants.AddressType.Other, em.Street, em.Pobox,
                                    em.City, em.Postcode, em.Country, em.Region));
                            notOther = false;
                            break;
                        default:
                            break;
                    }
                }
            }
            #endregion

            #region Email address
            if (SourceData.Emails.Count > 0)
            {
                bool notHome = true;
                bool notWork = true;
                bool notOther = true;
                foreach (EMail em in SourceData.Emails)
                {
                    switch (em.Rel)
                    {
                        case ContactsRelationships.IsWork:
                            if (notWork)
                                email1 = em.Address;
                            notWork = false;
                            break;
                        case ContactsRelationships.IsHome:
                            if (notHome)
                                email2 = em.Address;
                            notHome = false;
                            break;
                        case ContactsRelationships.IsOther:
                            if (notOther)
                                email3 = em.Address;
                            notOther = false;
                            break;
                        default:
                            break;
                    }
                }
            }
            #endregion

            #region Company
            if (SourceData.Organizations.Count > 0)
            {
                foreach (Organization en in SourceData.Organizations)
                {
                    if (!string.IsNullOrEmpty(en.Name))
                        Company = en.Name;
                    if (!string.IsNullOrEmpty(en.Title))
                        JobTitle = en.Title;
                    if (!string.IsNullOrEmpty(en.Department))
                        Department = en.Department;
                    break;
                }
            }
            #endregion

            #region other
            if (SourceData.ContactEntry.Websites.Count > 0)
            {
                foreach (Website en in SourceData.ContactEntry.Websites)
                {
                    if (string.Equals("work", en.Rel))
                    {
                        WebServer = en.Href;
                        break;
                    }
                }
            }
            #endregion

            MD5ReCountSelf();
            LoggerProvider.Instance.Logger.Debug("Contact from Google: {0} {1} - {2}-{3}", LastName, FirstName, _MyID, _referenceID);
#if (DUMP_AMEX)
            if ((LastName == "AMEX") || (FirstName == "AMEX"))
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(string.Format("Contact from Google: {0}", _MyID));
                sb.AppendLine(string.Format("User name: {0} {1}", FirstName, LastName));
                sb.AppendLine(string.Format("MD5 Google: {0} ", MD5selfCount));
                sb.AppendLine(string.Format("MD5 source:\r\n{0}", SummAllData()));
                sb.AppendLine(string.Format("Last Update Google:  {0}", UpdateTime));
                LoggerProvider.Instance.Logger.Debug(sb.ToString());
            }
#endif

        }

        /// <summary>
        /// Create from Outlook Contact
        /// </summary>
        /// <param name="SourceData"></param>
        public OneContact(Outlook.ContactItem SourceData)
        {
            #region pocatecni kontrola pro naplneni
            _isFromOutlook = true;
            if (string.IsNullOrEmpty(SourceData.EntryID)) /// neni EntryID a to potrebuji chyba
            {
                LoggerProvider.Instance.Logger.Error("Outlook contact does'n has EntryID");
                throw new Exception("Outlook contact does'n has EntryID");
            }
            _MyID = SourceData.EntryID; // zadame ID
            _rawSource = SourceData;
            _ModifyDateTime = SourceData.LastModificationTime; // datum posledni modifikace
            if (!string.IsNullOrEmpty(SourceData.User3)) // nalezeni GoogleID pokud je soucasti
            {
                _referenceID = GetSavedReplicaID(SourceData.User3);
            }
            #endregion

            #region Zpracovani personalnich udaju
            Title = string.IsNullOrEmpty(SourceData.Title) ? "" : SourceData.Title;
            FirstName = string.IsNullOrEmpty(SourceData.FirstName) ? "" : SourceData.FirstName;
            MiddleName = string.IsNullOrEmpty(SourceData.MiddleName) ? "" : SourceData.MiddleName;
            LastName = string.IsNullOrEmpty(SourceData.LastName) ? "" : SourceData.LastName;
            Suffix = string.IsNullOrEmpty(SourceData.Suffix) ? "" : SourceData.Suffix;
#if (!ANNIVESARY_NOT_WORK)
            //TODO: po nalezeni Anniversary v GoogleContact je mozno pouzit
            if (SourceData.Anniversary < DateTime.Parse("1/1/4000"))
                Anniversary = SourceData.Anniversary;
#endif
            if (SourceData.Birthday < DateTime.Parse("1/1/4000"))
                Birthday = SourceData.Birthday;
            Notes = SourceData.Body;
#if (!NoImage)
            if (SourceData.HasPicture)
            {
                ImagePath = Utils.GetContactPicturePath(SourceData);
            }
#endif
            IM = string.IsNullOrEmpty(SourceData.IMAddress) ? "" : SourceData.IMAddress;
            #endregion

            #region Zpracovani telefonu
            if (!string.IsNullOrEmpty(SourceData.MobileTelephoneNumber))
                Telephone.Add(Enum.GetName(typeof(Constants.PhoneType), Constants.PhoneType.Mobile),
                    new PhoneDetail(SourceData.MobileTelephoneNumber, Constants.PhoneType.Mobile));
            if (!string.IsNullOrEmpty(SourceData.BusinessTelephoneNumber))
                Telephone.Add(Enum.GetName(typeof(Constants.PhoneType), Constants.PhoneType.Business1),
                    new PhoneDetail(SourceData.BusinessTelephoneNumber, Constants.PhoneType.Business1));
            if (!string.IsNullOrEmpty(SourceData.Business2TelephoneNumber))
                Telephone.Add(Enum.GetName(typeof(Constants.PhoneType), Constants.PhoneType.Business2),
                    new PhoneDetail(SourceData.Business2TelephoneNumber, Constants.PhoneType.Business2));
            if (!string.IsNullOrEmpty(SourceData.HomeTelephoneNumber))
                Telephone.Add(Enum.GetName(typeof(Constants.PhoneType), Constants.PhoneType.Home),
                    new PhoneDetail(SourceData.HomeTelephoneNumber, Constants.PhoneType.Home));
            if (!string.IsNullOrEmpty(SourceData.OtherTelephoneNumber))
                Telephone.Add(Enum.GetName(typeof(Constants.PhoneType), Constants.PhoneType.Other),
                    new PhoneDetail(SourceData.OtherTelephoneNumber, Constants.PhoneType.Other));
            if (!string.IsNullOrEmpty(SourceData.BusinessFaxNumber))
                Telephone.Add(Enum.GetName(typeof(Constants.PhoneType), Constants.PhoneType.Fax_work),
                    new PhoneDetail(SourceData.BusinessFaxNumber, Constants.PhoneType.Fax_work));
            if (!string.IsNullOrEmpty(SourceData.HomeFaxNumber))
                Telephone.Add(Enum.GetName(typeof(Constants.PhoneType), Constants.PhoneType.Fax_home),
                    new PhoneDetail(SourceData.HomeFaxNumber, Constants.PhoneType.Fax_home));
            #endregion

            #region adresy
            if (!string.IsNullOrEmpty(SourceData.BusinessAddress)) // exituje adresa
                Address.Add(Enum.GetName(typeof(Constants.AddressType), Constants.AddressType.Business),
                    new AddressDetail(Constants.AddressType.Business, SourceData.BusinessAddressStreet, SourceData.BusinessAddressPostOfficeBox,
                    SourceData.BusinessAddressCity, SourceData.BusinessAddressPostalCode, SourceData.BusinessAddressCountry, SourceData.BusinessAddressState));
            if (!string.IsNullOrEmpty(SourceData.HomeAddress))
                Address.Add(Enum.GetName(typeof(Constants.AddressType), Constants.AddressType.Home),
                    new AddressDetail(Constants.AddressType.Home, SourceData.HomeAddressStreet, SourceData.HomeAddressPostOfficeBox,
                    SourceData.HomeAddressCity, SourceData.HomeAddressPostalCode, SourceData.HomeAddressCountry, SourceData.HomeAddressState));
            if (!string.IsNullOrEmpty(SourceData.OtherAddress))
                Address.Add(Enum.GetName(typeof(Constants.AddressType), Constants.AddressType.Other),
                    new AddressDetail(Constants.AddressType.Other, SourceData.OtherAddressStreet, SourceData.OtherAddressPostOfficeBox,
                    SourceData.OtherAddressCity, SourceData.OtherAddressPostalCode, SourceData.OtherAddressCountry, SourceData.OtherAddressState));
            #endregion

            #region Email address
            if (!string.IsNullOrEmpty(SourceData.Email1Address))
                email1 = SourceData.Email1Address;
            if (!string.IsNullOrEmpty(SourceData.Email2Address))
                email2 = SourceData.Email2Address;
            if (!string.IsNullOrEmpty(SourceData.Email3Address))
                email3 = SourceData.Email3Address;
            #endregion

            #region Company
            if (!string.IsNullOrEmpty(SourceData.Companies))
                Company = SourceData.Companies;
            if (!string.IsNullOrEmpty(SourceData.Department))
                Department = SourceData.Department;
            if (!string.IsNullOrEmpty(SourceData.JobTitle))
                JobTitle = SourceData.JobTitle;
            #endregion

            #region other
            if (!string.IsNullOrEmpty(SourceData.WebPage))
                WebServer = SourceData.WebPage;
            #endregion

            MD5ReCountSelf();

            LoggerProvider.Instance.Logger.Debug("Contact from outlook: {0} {1} - {2}-{3}", LastName, FirstName, _MyID, _referenceID);

#if (DUMP_AMEX)
            if ((LastName == "AMEX") || (FirstName == "AMEX"))
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(string.Format("Contact from Outlook: {0}", _MyID));
                sb.AppendLine(string.Format("User name: {0} {1}", FirstName, LastName));
                sb.AppendLine(string.Format("MD5 Outlook: {0} ", MD5selfCount));
                sb.AppendLine(string.Format("MD5 source:\r\n{0}", SummAllData()));
                sb.AppendLine(string.Format("Last Update Outlook:  {0}", UpdateTime));
                LoggerProvider.Instance.Logger.Debug(sb.ToString());
            }
#endif
        }
        #endregion

        #region Destructor & some props
        /// <summary>
        /// Is in Contact refernce to other side?
        /// </summary>
        public bool IsReferToOtherSide
        {
            get { return (!string.IsNullOrEmpty(_referenceID)); }
        }
        /// <summary>
        /// Reference in outlook to google pair
        /// </summary>
        public string ReferenceID
        {
            get { return _referenceID; }
            ///TODO: Need update data in original contact and save it
            set { _referenceID = value; }
        }
        /// <summary>
        /// Reference to contact ID
        /// </summary>
        public string ContactID
        {
            get { return _MyID; }
            set { _MyID = value; }
        }
        /// <summary>
        /// Return true if source is Outlook ContactItem
        /// </summary>
        public bool IsSourceOutlook
        {
            get { return _isFromOutlook; }
        }
        #endregion

        #region Update reference data
        /// <summary>
        /// Update refernces to google in Oulook contact
        /// </summary>
        /// <param name="GoogleRef">from OneContact.CretaeGoogleID()</param>
        public void UpdateRefInOutlook(string GoogleRef)
        {
            if (!_isFromOutlook)
            {
                LoggerProvider.Instance.Logger.Error("Contact don't create from Outlook");
            }
            if (_rawSource == null)
            {
                LoggerProvider.Instance.Logger.Error("This function is run only on exist Outlook contact");
                return;
            }
            LoggerProvider.Instance.Logger.Debug("Update outlook contact: {0} {1} - {2}-{3}", LastName, FirstName, _MyID, _referenceID);
            ((Outlook.ContactItem)_rawSource).User3 = GoogleRef;
            ((Outlook.ContactItem)_rawSource).Save();
        }

        /// <summary>
        /// Update refernces to outlook in google contact
        /// </summary>
        /// <param name="GoogleRef">from OneContact.CretaeOutlookID()</param>
        public void UpdateRefInGoogle(string OutlookRef)
        {
            if (_isFromOutlook)
            {
                LoggerProvider.Instance.Logger.Error("Contact don't create from Google");
            }
            if (_rawSource == null)
            {
                LoggerProvider.Instance.Logger.Error("This function is run only on exist Google contact");
                return;
            }
            LoggerProvider.Instance.Logger.Debug("Update relica ID in Google {0}", OutlookRef);
            ExtendedProperty ep = null; //new ExtendedProperty(CreateOutlookID(), Constants.NameGoogleExtendProperies);
            int i = 0;
            if (((Google.Contacts.Contact)_rawSource).ExtendedProperties.Count > 0)
            {
                foreach (ExtendedProperty en in ((Google.Contacts.Contact)_rawSource).ExtendedProperties) // vybereme tu správnou a vyplnime hodnoty
                {
                    if (en.Name == Constants.NameGoogleExtendProperies)
                    {
                        ep = en;
                        break;
                    }
                    i++;
                }

            }
            if (ep == null) 
            {
                ep = new ExtendedProperty(OutlookRef, Constants.NameGoogleExtendProperies);
                ((Google.Contacts.Contact)_rawSource).ExtendedProperties.Add(ep);
            }
            else 
                ((Google.Contacts.Contact)_rawSource).ExtendedProperties[i].Value = OutlookRef;
            Google.Contacts.Contact ret = GoogleProvider.GetProvider.Update(((Google.Contacts.Contact)_rawSource));
            LoggerProvider.Instance.Logger.Debug("Update Google contact: {0} {1} - {2}-{3}", LastName, FirstName, _MyID, _referenceID);
            if (ret != null)
                _rawSource = ret;
        }
        #endregion

        #region create new Outllok/Google data from this class
        /// <summary>
        /// Vraci Outlook kontakt z teto tridy
        /// </summary>
        /// <param name="outContact">novy outlook kontakt</param>
        /// <returns></returns>
        public Outlook.ContactItem GetOutlook()
        {
            Outlook.ContactItem outContact = OutlookProvider.Instance.CreateNewContactItem(); /// vytvorime novou 
            LoggerProvider.Instance.Logger.Debug("Create new Outlook.ContactItem instance");                                                                                              
            outContact=SaveToOutlook(outContact);
            return outContact;
        }

        /// <summary>
        /// Vraci Google contact z teto tridy
        /// </summary>
        /// <returns></returns>
        public Google.Contacts.Contact GetGoogle()
        {
            Google.Contacts.Contact goContact = new Google.Contacts.Contact();
            LoggerProvider.Instance.Logger.Debug("Create new Google.Contacts.Contact instance!");
            goContact = SaveToGoogle(goContact);
            return goContact;
        }
        #endregion

        #region Delete Contact
        public void Delete()
        {
            LoggerProvider.Instance.Logger.Debug("Detele form {4}: {0} {1} - {2}-{3}", LastName, FirstName, _MyID, _referenceID, _isFromOutlook ? "Outlook" : "Google");
            if (_isFromOutlook) // If it's from outlook
            {
                try
                {
                    ((Outlook.ContactItem)_rawSource).Delete();
                }
                catch (Exception e)
                {
                    LoggerProvider.Instance.Logger.Error("Problem when delete contact from Outlook");
                    LoggerProvider.Instance.Logger.Error(e);
                }
                return;
            }
            // It's from google
            GoogleProvider.GetProvider.Delete((Google.Contacts.Contact)_rawSource);
        }

        #endregion

        #region Update / Save this OneContact
        /// <summary>
        /// Update data in Raw contact
        /// </summary>
        /// <param name="updater"></param>
        new public void UpdateFromOther(OneContact updater)
        {
            LoggerProvider.Instance.Logger.Debug("Source is {0} and MyID - RefID {1} - {2}", updater._isFromOutlook ? "Outlook" : "Google", updater._MyID, updater._referenceID);
            LoggerProvider.Instance.Logger.Debug("Destination is {0} and MyID - RefID {1} - {2}", _isFromOutlook ? "Outlook" : "Google", _MyID, _referenceID);
            base.UpdateFromOther(updater);
            if (_isFromOutlook)
            {
                _rawSource = SaveToOutlook((Outlook.ContactItem)_rawSource);
                ((Outlook.ContactItem)_rawSource).User3 = CreateReferenceIDToOther();
            }
            else
            {
                _rawSource = SaveToGoogle((Google.Contacts.Contact)_rawSource);

                string OutlookRef = CreateReferenceIDToOther();
                ExtendedProperty ep = null;
                int i = 0;
                if (((Google.Contacts.Contact)_rawSource).ExtendedProperties.Count > 0)
                {
                    foreach (ExtendedProperty en in ((Google.Contacts.Contact)_rawSource).ExtendedProperties) // Select right value and fill it
                    {
                        if (en.Name == Constants.NameGoogleExtendProperies)
                        {
                            ep = en;
                            break;
                        }
                        i++;
                    }

                }
                if (ep == null)
                {
                    ep = new ExtendedProperty(OutlookRef, Constants.NameGoogleExtendProperies);
                    ((Google.Contacts.Contact)_rawSource).ExtendedProperties.Add(ep);
                }
                else
                    ((Google.Contacts.Contact)_rawSource).ExtendedProperties[i].Value = OutlookRef;
            }
            SaveUpdatedData();
        }
        /// <summary>
        /// Save actual contact to source
        /// </summary>
        /// <returns></returns>
        private object SaveUpdatedData()
        {
            LoggerProvider.Instance.Logger.Debug("Save to {0} this RefID {1}", _isFromOutlook ? "Outlook" : "Google", _referenceID);
            if (_isFromOutlook)
            {
                LoggerProvider.Instance.Logger.Debug("In Raw data is refID {0}", ((Outlook.ContactItem)_rawSource).User3);
                ((Outlook.ContactItem)_rawSource).Save();
                return _rawSource;
            }
            Google.Contacts.Contact newContact = null;
            if (((Google.Contacts.Contact)_rawSource).ExtendedProperties.Count > 0)
                LoggerProvider.Instance.Logger.Debug("In Raw data is refID {0}", ((Google.Contacts.Contact)_rawSource).ExtendedProperties[0].Value);
            else
                LoggerProvider.Instance.Logger.Error("Problem in Google _rawSource isn't Reference ID saved in ExtendedProperties");
            newContact = GoogleProvider.GetProvider.Update((Google.Contacts.Contact)_rawSource);
            _rawSource = newContact;
            return _rawSource;
        }
        #endregion

        #region Private function for transfer data to Outllok/Google struct
        /// <summary>
        /// Save data to define Outlook.ContactItem and empty relevant fields
        /// </summary>
        /// <param name="outContact"></param>
        /// <returns></returns>
        private Outlook.ContactItem SaveToOutlook(Outlook.ContactItem outContact)
        {
            string sName = "";
            AddressDetail ad = null;

            if (string.IsNullOrEmpty(MD5selfCount))
                MD5ReCountSelf();
            outContact.User3 = CreateReferenceID();

            #region Personal data
            outContact.Title = DataOrEmpty(Title);
            outContact.FirstName = DataOrEmpty(FirstName);
            outContact.LastName = DataOrEmpty(LastName);
            outContact.MiddleName = DataOrEmpty(MiddleName);
            outContact.Suffix = DataOrEmpty(Suffix);
#if (!ANNIVESARY_NOT_WORK)
            if (Anniversary > DateTime.MinValue)
                outContact.Anniversary = Anniversary;
            ///TODO: need how tell outlook that date isn't set
#endif
            if (Birthday > DateTime.MinValue)
                outContact.Birthday = Birthday;
#if (!NoImage)
            //TODO: Need work on image
            //if (image != null)
            //    outContact.AddPicture();
#endif
            outContact.Body = DataOrEmpty(Notes);
            #endregion

            #region Telephone
            if (Telephone.ContainsKey(Enum.GetName(typeof(Constants.PhoneType), Constants.PhoneType.Mobile)))
                outContact.MobileTelephoneNumber = ((PhoneDetail)Telephone[Enum.GetName(typeof(Constants.PhoneType), Constants.PhoneType.Mobile)]).PhoneNumber;
            else
                outContact.MobileTelephoneNumber = string.Empty;
            if (Telephone.ContainsKey(Enum.GetName(typeof(Constants.PhoneType), Constants.PhoneType.Business1)))
                outContact.BusinessTelephoneNumber = ((PhoneDetail)Telephone[Enum.GetName(typeof(Constants.PhoneType), Constants.PhoneType.Business1)]).PhoneNumber;
            else
                outContact.BusinessTelephoneNumber = string.Empty;
            if (Telephone.ContainsKey(Enum.GetName(typeof(Constants.PhoneType), Constants.PhoneType.Business2)))
                outContact.Business2TelephoneNumber = ((PhoneDetail)Telephone[Enum.GetName(typeof(Constants.PhoneType), Constants.PhoneType.Business2)]).PhoneNumber;
            else
                outContact.Business2TelephoneNumber = string.Empty;
            if (Telephone.ContainsKey(Enum.GetName(typeof(Constants.PhoneType), Constants.PhoneType.Home)))
                outContact.HomeTelephoneNumber = ((PhoneDetail)Telephone[Enum.GetName(typeof(Constants.PhoneType), Constants.PhoneType.Home)]).PhoneNumber;
            else
                outContact.HomeTelephoneNumber = string.Empty;
            if (Telephone.ContainsKey(Enum.GetName(typeof(Constants.PhoneType), Constants.PhoneType.Other)))
                outContact.OtherTelephoneNumber = ((PhoneDetail)Telephone[Enum.GetName(typeof(Constants.PhoneType), Constants.PhoneType.Other)]).PhoneNumber;
            else
                outContact.OtherTelephoneNumber = string.Empty;
            if (Telephone.ContainsKey(Enum.GetName(typeof(Constants.PhoneType), Constants.PhoneType.Fax_work)))
                outContact.BusinessFaxNumber = ((PhoneDetail)Telephone[Enum.GetName(typeof(Constants.PhoneType), Constants.PhoneType.Fax_work)]).PhoneNumber;
            else
                outContact.BusinessFaxNumber = string.Empty;
            if (Telephone.ContainsKey(Enum.GetName(typeof(Constants.PhoneType), Constants.PhoneType.Fax_home)))
                outContact.HomeFaxNumber = ((PhoneDetail)Telephone[Enum.GetName(typeof(Constants.PhoneType), Constants.PhoneType.Fax_home)]).PhoneNumber;
            else
                outContact.HomeFaxNumber = string.Empty;
            #endregion

            #region Address
            sName = Enum.GetName(typeof(Constants.AddressType), Constants.AddressType.Business);
            if (Address.ContainsKey(sName))
            {
                ad = (AddressDetail)Address[sName];
                outContact.BusinessAddressStreet = DataOrEmpty(ad.Street);
                outContact.BusinessAddressPostOfficeBox = DataOrEmpty(ad.POBox);
                outContact.BusinessAddressPostalCode = DataOrEmpty(ad.PostalCode);
                outContact.BusinessAddressState = DataOrEmpty(ad.State);
                outContact.BusinessAddressCity = DataOrEmpty(ad.City);
                outContact.BusinessAddressCountry = DataOrEmpty(ad.CountryRegion);
            }
            else
            {
                outContact.BusinessAddressStreet = string.Empty;
                outContact.BusinessAddressPostOfficeBox = string.Empty;
                outContact.BusinessAddressPostalCode = string.Empty;
                outContact.BusinessAddressState = string.Empty;
                outContact.BusinessAddressCity = string.Empty;
                outContact.BusinessAddressCountry = string.Empty;

            }
            sName = Enum.GetName(typeof(Constants.AddressType), Constants.AddressType.Home);
            if (Address.ContainsKey(sName))
            {
                ad = (AddressDetail)Address[sName];
                outContact.HomeAddressStreet = DataOrEmpty(ad.Street);
                outContact.HomeAddressPostOfficeBox = DataOrEmpty(ad.POBox);
                outContact.HomeAddressPostalCode = DataOrEmpty(ad.PostalCode);
                outContact.HomeAddressState = DataOrEmpty(ad.State);
                outContact.HomeAddressCity = DataOrEmpty(ad.City);
                outContact.HomeAddressCountry = DataOrEmpty(ad.CountryRegion);
            }
            else
            {
                outContact.HomeAddressStreet = string.Empty;
                outContact.HomeAddressPostOfficeBox = string.Empty;
                outContact.HomeAddressPostalCode = string.Empty;
                outContact.HomeAddressState = string.Empty;
                outContact.HomeAddressCity = string.Empty;
                outContact.HomeAddressCountry = string.Empty;
            }

            sName = Enum.GetName(typeof(Constants.AddressType), Constants.AddressType.Other);
            if (Address.ContainsKey(sName))
            {
                ad = (AddressDetail)Address[sName];
                outContact.OtherAddressStreet = DataOrEmpty(ad.Street);
                outContact.OtherAddressPostOfficeBox = DataOrEmpty(ad.POBox);
                outContact.OtherAddressPostalCode = DataOrEmpty(ad.PostalCode);
                outContact.OtherAddressState = DataOrEmpty(ad.State);
                outContact.OtherAddressCity = DataOrEmpty(ad.City);
                outContact.OtherAddressCountry = DataOrEmpty(ad.CountryRegion);
            }
            else
            {
                {
                    outContact.OtherAddressStreet = string.Empty;
                    outContact.OtherAddressPostOfficeBox = string.Empty;
                    outContact.OtherAddressPostalCode = string.Empty;
                    outContact.OtherAddressState = string.Empty;
                    outContact.OtherAddressCity = string.Empty;
                    outContact.OtherAddressCountry = string.Empty;
                }

            }
            #endregion

            #region Email address
            if (!string.IsNullOrEmpty(email1))
            {
                outContact.Email1Address = email1;
                outContact.Email1AddressType = "SMTP";
            }
            else
            {
                outContact.Email1Address = string.Empty;
                outContact.Email1AddressType = string.Empty;
                outContact.Email1DisplayName = string.Empty;
            }
            if (!string.IsNullOrEmpty(email2))
            {
                outContact.Email2Address = email2;
                outContact.Email2AddressType = "SMTP";
            }
            else
            {
                outContact.Email2Address = string.Empty;
                outContact.Email2AddressType = string.Empty;
                outContact.Email2DisplayName = string.Empty;
            }
            if (!string.IsNullOrEmpty(email3))
            {
                outContact.Email3Address = email3;
                outContact.Email3AddressType = "SMTP";
            }
            else
            {
                outContact.Email3Address = string.Empty;
                outContact.Email3AddressType = string.Empty;
                outContact.Email3DisplayName = string.Empty;
            }
            #endregion

            #region Company
            outContact.Companies = DataOrEmpty(Company);
            outContact.Department = DataOrEmpty(Department);
            outContact.JobTitle = DataOrEmpty(JobTitle);
            #endregion

            #region Ostatni
            outContact.WebPage = DataOrEmpty(WebServer);
            #endregion

            return outContact;
        }

        /// <summary>
        /// Save data to define GoogleContacts.Contac and empty relevant fields
        /// </summary>
        /// <param name="goContact"></param>
        /// <returns></returns>
        private Google.Contacts.Contact SaveToGoogle(Google.Contacts.Contact goContact)
        {
            if (string.IsNullOrEmpty(MD5selfCount))
                MD5ReCountSelf();
            ExtendedProperty ep = new ExtendedProperty(CreateReferenceID(), Constants.NameGoogleExtendProperies);
            LoggerProvider.Instance.Logger.Debug("Save to Google RefID {0}", CreateReferenceID());

            #region Personal data
            Name nm = new Name();
            nm.NamePrefix = DataOrEmpty(Title);
            nm.GivenName = DataOrEmpty(FirstName);
            nm.FamilyName = DataOrEmpty(LastName);
            nm.AdditionalName = DataOrEmpty(MiddleName);
            nm.NameSuffix = DataOrEmpty(Suffix);
            goContact.Name = nm;
            goContact.Content = DataOrEmpty(Notes);

#if (!ANNIVESARY_NOT_WORK)
            if (Anniversary > DateTime.MinValue)
            {
                Event ev = new Event();
                ev.Relation = "anniversary";
                ev.When = new When(Anniversary, Anniversary);
                //ev.When.StartTime = Anniversary;
                ev.When.ValueString = Anniversary.ToString("yyyy-MM-dd");
                //ev.When.StartTime = Anniversary;
                ///TODO: problem with update event.When startDate in bad format
                //goContact.ContactEntry.Events.Add(ev);
            }
#endif
            if (Birthday > DateTime.MinValue)
            {
                goContact.ContactEntry.Birthday = Birthday.ToString("yyyy-MM-dd");
            }
            else
                goContact.ContactEntry.Birthday = null;
#if (!NoImage)

            ///TODO: Add image to contact
#endif
            goContact.IMs.Clear();
            if (!string.IsNullOrEmpty(IM))
            {
                IMAddress im = new IMAddress();
                im.Address = IM;
                im.Primary = true;
                im.Protocol = "MSN";
                im.Rel = ContactsRelationships.IsHome;
                goContact.IMs.Add(im);
            }
            #endregion

            #region Telephone
            Google.GData.Extensions.PhoneNumber pn = null;
            bool isPrimary = false;

            goContact.Phonenumbers.Clear();
            string sName = Enum.GetName(typeof(Constants.PhoneType), Constants.PhoneType.Mobile);
            if (Telephone.ContainsKey(sName))
            {
                pn = new PhoneNumber(((PhoneDetail)Telephone[sName]).PhoneNumber);
                pn.Primary = true;
                isPrimary = true;
                pn.Rel = ContactsRelationships.IsMobile;
                goContact.Phonenumbers.Add(pn);
            }
            sName = Enum.GetName(typeof(Constants.PhoneType), Constants.PhoneType.Business1);
            if (Telephone.ContainsKey(sName))
            {
                pn = new PhoneNumber(((PhoneDetail)Telephone[sName]).PhoneNumber);
                pn.Primary = isPrimary ? false : true;
                isPrimary = true;
                pn.Rel = ContactsRelationships.IsWork;
                goContact.Phonenumbers.Add(pn);
            }
            sName = Enum.GetName(typeof(Constants.PhoneType), Constants.PhoneType.Business2);
            if (Telephone.ContainsKey(sName))
            {
                pn = new PhoneNumber(((PhoneDetail)Telephone[sName]).PhoneNumber);
                pn.Primary = isPrimary ? false : true;
                isPrimary = true;
                pn.Rel = ContactsRelationships.IsWork;
                goContact.Phonenumbers.Add(pn);
            }
            sName = Enum.GetName(typeof(Constants.PhoneType), Constants.PhoneType.Home);
            if (Telephone.ContainsKey(sName))
            {
                pn = new PhoneNumber(((PhoneDetail)Telephone[sName]).PhoneNumber);
                pn.Primary = isPrimary ? false : true;
                isPrimary = true;
                pn.Rel = ContactsRelationships.IsHome;
                goContact.Phonenumbers.Add(pn);
            }
            sName = Enum.GetName(typeof(Constants.PhoneType), Constants.PhoneType.Other);
            if (Telephone.ContainsKey(sName))
            {
                pn = new PhoneNumber(((PhoneDetail)Telephone[sName]).PhoneNumber);
                pn.Primary = isPrimary ? false : true;
                isPrimary = true;
                pn.Rel = ContactsRelationships.IsOther;
                goContact.Phonenumbers.Add(pn);
            }
            sName = Enum.GetName(typeof(Constants.PhoneType), Constants.PhoneType.Fax_work);
            if (Telephone.ContainsKey(sName))
            {
                pn = new PhoneNumber(((PhoneDetail)Telephone[sName]).PhoneNumber);
                pn.Primary = isPrimary ? false : true;
                isPrimary = true;
                pn.Rel = ContactsRelationships.IsWorkFax;
                goContact.Phonenumbers.Add(pn);
            }
            sName = Enum.GetName(typeof(Constants.PhoneType), Constants.PhoneType.Fax_home);
            if (Telephone.ContainsKey(sName))
            {
                pn = new PhoneNumber(((PhoneDetail)Telephone[sName]).PhoneNumber);
                pn.Primary = isPrimary ? false : true;
                isPrimary = true;
                pn.Rel = ContactsRelationships.IsHomeFax;
                goContact.Phonenumbers.Add(pn);
            }
            #endregion

            #region Address
            sName = Enum.GetName(typeof(Constants.AddressType), Constants.AddressType.Business);
            StructuredPostalAddress pa = null;
            AddressDetail ad = null;
            isPrimary = false;
            
            goContact.PostalAddresses.Clear();
            if (Address.ContainsKey(sName))
            {
                ad = (AddressDetail)Address[sName];
                pa = ad.GetPostalAddress;
                pa.Primary = isPrimary ? false : true;
                isPrimary = true;
                goContact.PostalAddresses.Add(pa);
            }
            sName = Enum.GetName(typeof(Constants.AddressType), Constants.AddressType.Home);
            if (Address.ContainsKey(sName))
            {
                ad = (AddressDetail)Address[sName];
                pa = ad.GetPostalAddress;
                pa.Primary = isPrimary ? false : true;
                isPrimary = true;
                goContact.PostalAddresses.Add(pa);
            }
            sName = Enum.GetName(typeof(Constants.AddressType), Constants.AddressType.Other);
            if (Address.ContainsKey(sName))
            {
                ad = (AddressDetail)Address[sName];
                pa = ad.GetPostalAddress;
                pa.Primary = isPrimary ? false : true;
                isPrimary = true;
                goContact.PostalAddresses.Add(pa);
            }

            #endregion

            #region Email address
            bool isFill = true;

            goContact.Emails.Clear();
            EMail em = null;
            if (!string.IsNullOrEmpty(email1))
            {
                em = new EMail(email1);
                em.Primary = isFill;
                em.Rel = ContactsRelationships.IsWork;
                goContact.Emails.Add(em);
                isFill = false;
            }
            if (!string.IsNullOrEmpty(email2))
            {
                em = new EMail(email2);
                em.Primary = isFill;
                em.Rel = ContactsRelationships.IsHome;
                goContact.Emails.Add(em);
                isFill = false;
            }
            if (!string.IsNullOrEmpty(email3))
            {
                em= new EMail(email3);
                em.Primary = isFill;
                em.Rel = ContactsRelationships.IsOther;
                goContact.Emails.Add(em);
                isFill = false;
            }
            #endregion

            #region Company
            Organization org = new Organization();
            org.Rel = ContactsRelationships.IsOther;
            isFill = false;

            goContact.Organizations.Clear();
            if (!string.IsNullOrEmpty(Company))
            {
                org.Name = Company;
                isFill = true;
            }
            if (!string.IsNullOrEmpty(Department))
            {
                org.Department = Department;
                isFill = true;
            }
            if (!string.IsNullOrEmpty(JobTitle))
            {
                org.Title = JobTitle;
                isFill = true;
            }
            if (isFill)
                goContact.Organizations.Add(org);
            #endregion

            #region Ostatni
            goContact.ContactEntry.Websites.Clear();
            if (!string.IsNullOrEmpty(WebServer))
            {
                Website ws = new Website();
                ws.Href = WebServer;
                ws.Rel = "work";
                goContact.ContactEntry.Websites.Add(ws);
            }
            #endregion

            #region Fill Outlook ID and so On
            for (int j = 0; j < goContact.ExtendedProperties.Count; j++)
                if (goContact.ExtendedProperties[j].Name == Constants.NameGoogleExtendProperies)
                {
                    goContact.ExtendedProperties.RemoveAt(j);
                    break;
                }
            try
            {
                goContact.ExtendedProperties.Add(ep);
            }
            catch (Exception e)
            {
                LoggerProvider.Instance.Logger.Debug(e);
            }
            ///TODO: check other way to create default group
            isFill = false;
            for (int j = 0; j < goContact.GroupMembership.Count; j++)
            {
                if (goContact.GroupMembership[j].HRef == "http://www.google.com/m8/feeds/groups/test.zdepok%40gmail.com/base/6")
                {
                    isFill = true;
                }
            }
            if (!isFill)
            {
                GroupMembership gpdefault = new GroupMembership();
                gpdefault.HRef = "http://www.google.com/m8/feeds/groups/test.zdepok%40gmail.com/base/6";
                goContact.GroupMembership.Add(gpdefault);
            }
            #endregion

            return goContact;
        }
        #endregion

    }
}