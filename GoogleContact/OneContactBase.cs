using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

using Google.GData.Contacts;
using Google.GData.Client;
using Google.GData.Extensions;

using Outlook = Microsoft.Office.Interop.Outlook;
using Office = Microsoft.Office.Core;

namespace GoogleContact
{
    /// <summary>
    /// This is a bse for one contact store in RAM
    /// </summary>
    class OneContactBase
    {
        #region Internal sync identificators
        /// <summary>
        /// Is this class based onOutlook
        /// </summary>
        internal bool _isFromOutlook = false;
        /// <summary>
        /// Id from source
        /// </summary>
        internal string _MyID = "";
        /// <summary>
        /// Pokud je uvedeno OutlookID, pak je toto odkaz na Oulook _OtlookID/EntityID
        /// </summary>
        internal string _referenceID = "";
        /// <summary>
        /// Modifikace v oulooku
        /// </summary>
        internal DateTime _ModifyDateTime = DateTime.MinValue;
        /// <summary>
        /// 
        /// </summary>
        internal string MD5selfCount = "";
        /// <summary>
        /// MD5 read from contact replica ID
        /// </summary>
        internal string MD5fromReplica = "";
        /// <summary>
        /// Reference to exist source for update
        /// </summary>
        internal object _rawSource = null;
        #endregion

        #region Personal data
        internal string Title = "";
        internal string FirstName = "";
        internal string MiddleName = "";
        internal string LastName = "";
        internal string Suffix = "";
#if (!ANNIVESARY_NOT_WORK)
        DateTime Anniversary = DateTime.MinValue;
#endif
        internal DateTime Birthday = DateTime.MinValue;
#if (!NoImage)
        internal string ImagePath = null;
#endif
        internal string Notes = "";
        internal string IM = "";
        #endregion

        #region Telephone
        /// <summary>
        /// List telefonnich cisel
        /// </summary>
        internal Hashtable Telephone = new Hashtable();
        #endregion

        #region Address
        /// <summary>
        /// List adres
        /// </summary>
        internal Hashtable Address = new Hashtable();
        #endregion

        #region Email address
        internal string email1 = "";
        internal string email2 = "";
        internal string email3 = "";
        #endregion

        #region Company
        internal string Company = "";
        internal string Department = "";
        internal string JobTitle = "";
        #endregion

        #region Ostatni
        internal string WebServer = "";
        #endregion

        #region Child class
        /// <summary>
        /// Class for one phone number
        /// </summary>
        public class PhoneDetail
        {
            #region Phone detail
            private string _PhoneNumber = "";
            private Constants.PhoneType _Type = Constants.PhoneType.Mobile;

            /// <summary>
            /// Create PhoneDetail from number with define type
            /// </summary>
            /// <param name="Number"></param>
            /// <param name="typ"></param>
            public PhoneDetail(string Number, Constants.PhoneType typ)
            {
                _PhoneNumber = Number;
                _Type = typ;
            }
            /// <summary>
            /// Return phone number
            /// </summary>
            public string PhoneNumber
            {
                get { return _PhoneNumber; }
            }
            /// <summary>
            /// return typ of number
            /// </summary>
            public Constants.PhoneType Type
            {
                get { return _Type; }
            }
            /// <summary>
            /// Return type in google string
            /// </summary>
            public string GoogleType
            {
                get
                {
                    string ToRet = "";
                    switch (_Type)
                    {
                        case Constants.PhoneType.Mobile:
                            ToRet = "mobile";
                            break;
                        case Constants.PhoneType.Home:
                            ToRet = "home";
                            break;
                        case Constants.PhoneType.Business1:
                            ToRet = "work";
                            break;
                        case Constants.PhoneType.Business2:
                            ToRet = "work";
                            break;
                        case Constants.PhoneType.Other:
                            ToRet = "other";
                            break;
                    }
                    return ToRet;
                }
            }
            #endregion
        }
        /// <summary>
        /// Trida jedne adresy
        /// </summary>
        public class AddressDetail
        {
            #region Address detail
            public Constants.AddressType Type = Constants.AddressType.Business;
            public string Street = "";
            public string POBox = "";
            public string City = "";
            public string PostalCode = "";
            public string CountryRegion = "";
            public string State = "";

            public AddressDetail()
            {
            }

            public AddressDetail(Constants.AddressType addrtype, string street, string pobox, string city, string postalcode, string countryregion, string state)
            {
                Street = string.IsNullOrEmpty(street) ? "" : street;
                POBox = string.IsNullOrEmpty(pobox) ? "" : pobox;
                City = string.IsNullOrEmpty(city) ? "" : city;
                PostalCode = string.IsNullOrEmpty(postalcode) ? "" : postalcode;
                CountryRegion = string.IsNullOrEmpty(countryregion) ? "" : countryregion;
                State = string.IsNullOrEmpty(state) ? "" : state;
                Type = addrtype;
            }
            /// <summary>
            /// Create Google StructuredPostalAddress from self
            /// </summary>
            public StructuredPostalAddress GetPostalAddress
            {
                get
                {
                    StructuredPostalAddress pa = new StructuredPostalAddress();
                    switch (Type)
                    {
                        case Constants.AddressType.Business:
                            pa.Rel = ContactsRelationships.IsWork;
                            break;
                        case Constants.AddressType.Home:
                            pa.Rel = ContactsRelationships.IsHome;
                            break;
                        default:
                            pa.Rel = ContactsRelationships.IsOther;
                            break;
                    }
                    if (!string.IsNullOrEmpty(Street))
                        pa.Street = Street;
                    if (!string.IsNullOrEmpty(POBox))
                        pa.Pobox = POBox;
                    if (!string.IsNullOrEmpty(PostalCode))
                        pa.Postcode = PostalCode;
                    if (!string.IsNullOrEmpty(State))
                        pa.Region = State;
                    if (!string.IsNullOrEmpty(City))
                        pa.City = City;
                    if (!string.IsNullOrEmpty(CountryRegion))
                        pa.Country = CountryRegion;

                    return pa;
                }
            }
            #endregion
        }
        #endregion

        #region Destructor & some props
        /// <summary>
        /// Need delete image !!
        /// </summary>
        ~OneContactBase()
        {
#if (!NoImage)
            LoggerProvider.Instance.Logger.Debug("Clear the image [{0}]", ImagePath);
            Utils.CleanupContactPictures(ImagePath);
#endif
        }
        #endregion

        #region Count MD5 and other Internal function
        /// <summary>
        /// Recount self MD5
        /// </summary>
        internal void MD5ReCountSelf()
        {
            MD5selfCount = MD5Actual();
        }
        /// <summary>
        /// Count actual MD5
        /// </summary>
        /// <returns></returns>
        internal string MD5Actual()
        {

            return Utils.CountMD5(SummAllData());
        }
        /// <summary>
        /// Return data in oneLongString
        /// </summary>
        /// <returns></returns>
        internal string SummAllData()
        {
            StringBuilder sb = new StringBuilder("");
            ///TODO: Need add more variables to MD5 source string in MD5Actual()
            sb.Append(Title);
            sb.Append(FirstName);
            sb.Append(MiddleName);
            sb.Append(LastName);
            sb.Append(Suffix);
#if (!ANNIVESARY_NOT_WORK)
            sb.Append(Anniversary.ToString("yyyyMMdd"));
#endif
            sb.Append(Birthday.ToString("yyyyMMdd"));
            sb.Append(Notes);
            sb.Append(IM);
#if (!NoImage)
            ///TODO: Bude se řešit MD5 i pro obrazek? asi ne
#endif

            foreach (Constants.PhoneType t in Enum.GetValues(typeof(Constants.PhoneType)))
            {
                sb.Append(GetRightPhoneNumber(t));
            }
            foreach (Constants.AddressType t in Enum.GetValues(typeof(Constants.AddressType)))
            {
                AddressDetail ad = GetRightAddress(t);
                if (ad != null)
                {
                    sb.Append(ad.Street);
                    sb.Append(ad.State);
                    sb.Append(ad.PostalCode);
                    sb.Append(ad.POBox);
                    sb.Append(ad.CountryRegion);
                    sb.Append(ad.City);
                    sb.Append(Enum.GetName(typeof(Constants.AddressType), ad.Type));
                }

            }
            sb.Append(email1);
            sb.Append(email2);
            sb.Append(email3);
            sb.Append(Company);
            sb.Append(Department);
            sb.Append(JobTitle);
            sb.Append(WebServer);

            return sb.ToString().Replace("\r", "").Replace("\n", "");
        }

        /// <summary>
        /// Retrieve saved ID from replica string  (if is) and return right ID or Empty string.
        /// Same time save find MD5 to MD5fromReplica
        /// </summary>
        /// <param name="source">In Outlook is UserFiled3, in GoogleContact ExtendProperties</param>
        /// <returns></returns>
        internal string GetSavedReplicaID(string source)
        {
            string ret = "";
            string md5 = "";
            if (source.StartsWith("[") && source.Contains("]-[") && source.EndsWith("]"))
            {
                ret = source.Substring(1, source.IndexOf("]-[") - 1);
                md5 = source.Substring(source.IndexOf("]-[") + 3, source.Length - source.IndexOf("]-[") - 4);
            }
            MD5fromReplica = md5;
            return ret;
        }
        /// <summary>
        /// Return data or string.empty
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        internal string DataOrEmpty(string source)
        {
            if (string.IsNullOrEmpty(source))
                return string.Empty;
            return source;
        }
        /// <summary>
        /// Clear reference for this record when set first time replica
        /// </summary>
        internal void ClearReference()
        {
            if (SettingsProvider.Instance.IsFirstTime)
            {
                Constants.FirstSetupSynchronize fs = SettingsProvider.Instance.FirstSynchronizeGet();
                switch (fs)
                {
                    case Constants.FirstSetupSynchronize.Outlook2Google:
                        if (_isFromOutlook)
                        {
                            _referenceID = "";
                            MD5fromReplica = "";
                        }
                        else
                        {
                            _referenceID = Utils.CountMD5(_MyID);
                            MD5fromReplica = Utils.CountMD5(_referenceID);
                        }
                        break;
                    case Constants.FirstSetupSynchronize.Google2Outlook:
                        if (_isFromOutlook)
                        {
                            _referenceID = Utils.CountMD5(_MyID);
                            MD5fromReplica = Utils.CountMD5(_referenceID);
                        }
                        else
                        {
                            _referenceID = "";
                            MD5fromReplica = "";
                        }
                        break;
                    case Constants.FirstSetupSynchronize.SynchronizeBoth:
                        _referenceID = "";
                        MD5fromReplica = "";
                        break;
                }
                LoggerProvider.Instance.Logger.Debug("Replica ID reset for first replica");
            }
        }
        #endregion

        #region Private function
        /// <summary>
        /// Vraci odpovidajici telephone nebo ""
        /// </summary>
        /// <param name="typ"></param>
        /// <returns></returns>
        private string GetRightPhoneNumber(Constants.PhoneType typ)
        {
            if (Telephone.ContainsKey(Enum.GetName(typeof(Constants.PhoneType), typ)))
            {
                return ((PhoneDetail)Telephone[Enum.GetName(typeof(Constants.PhoneType), typ)]).PhoneNumber;
            }
            return "";
        }
        /// <summary>
        /// Vraci odpovidajici adresu
        /// </summary>
        /// <param name="typ"></param>
        /// <returns></returns>
        private AddressDetail GetRightAddress(Constants.AddressType typ)
        {
            if (Address.ContainsKey(Enum.GetName(typeof(Constants.AddressType), typ)))
            {
                return (AddressDetail)Address[Enum.GetName(typeof(Constants.AddressType), typ)];
            }
            return null;
        }
        #endregion

        #region Public function
        /// <summary>
        /// Create ReferenceID for created contact
        /// </summary>
        /// <returns></returns>
        public string CreateReferenceID()
        {
            return string.Format("[{0}]-[{1}]", _MyID, MD5selfCount);
        }
        /// <summary>
        /// Create Real Reference id
        /// </summary>
        /// <returns></returns>
        public string CreateReferenceIDToOther()
        {
            return string.Format("[{0}]-[{1}]", _referenceID, MD5selfCount);
        }
        /// <summary>
        /// When data is Updated
        /// </summary>
        public DateTime UpdateTime
        {
            get { return _ModifyDateTime; }
        }
        /// <summary>
        /// Update data in curent ContactItem
        /// </summary>
        /// <param name="newer"></param>
        public void UpdateFromOther(OneContact updater)
        {
            #region Personal data
            Title = updater.Title;
            FirstName = updater.FirstName;
            MiddleName = updater.MiddleName;
            LastName = updater.LastName;
            Suffix = updater.Suffix;
#if (!ANNIVESARY_NOT_WORK)
            Anniversary = updater.Anniversary;
#endif
            Birthday = updater.Birthday;
#if (!NoImage)
            ImagePath = updater.ImagePath;
#endif
            Notes = updater.Notes;
            IM = updater.IM;
            #endregion

            #region Telephone
            Telephone.Clear();
            if (updater.Telephone.Count > 0)
                foreach (string s in updater.Telephone.Keys)
                    Telephone.Add(s, (PhoneDetail)updater.Telephone[s]);
            #endregion

            #region Address
            Address.Clear();
            if (updater.Address.Count > 0)
                foreach (string s in updater.Address.Keys)
                    Address.Add(s, (AddressDetail)updater.Address[s]);
            #endregion

            #region Email address
            email1 = updater.email1;
            email2 = updater.email2;
            email3 = updater.email3;
            #endregion

            #region Company
            Company = updater.Company;
            Department = updater.Department;
            JobTitle = updater.JobTitle;
            #endregion

            #region Ostatni
            WebServer = updater.WebServer; ;
            #endregion

            LoggerProvider.Instance.Logger.Debug("Update current RefID from/to: {0} - {1}", _referenceID, updater._MyID);
            _referenceID = updater._MyID;

            MD5ReCountSelf();
        }
        #endregion

        #region Dump Data to Log
        /// <summary>
        /// Dump actual data to log in structure
        /// </summary>
        public void DumpActualDataToLog()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("\r\n****** Start OneContactBase Dump ******\r\n");
            sb.AppendFormat("Source / rawSource: {0}\r\n", _isFromOutlook ? "Outlook" : "Google", _rawSource == null ? "Not defined" : "Defined");
            sb.AppendFormat("_MyID - MD5selfCount: {0} - {1}\r\n", _MyID, MD5selfCount);
            sb.AppendFormat("_refernceID - MD5Replica: {0} - {1}\r\n", _referenceID, MD5fromReplica);
            sb.AppendFormat("_ModificationDate: {0}\r\n", _ModifyDateTime);
            sb.AppendFormat("User (title / First / Middle / Last / Suffif: {0} / {1} / {2} / {3} / {4}\r\n", Title, FirstName, MiddleName, LastName, Suffix);
#if (!ANNIVESARY_NOT_WORK)
            sb.AppendFormat("Birthday / Anniversary: {0} / {1}\r\n", Birthday, Anniversary);
#else
            sb.AppendFormat("Birthday / Anniversary: {0} / {1}\r\n", Birthday, "Not work with");
#endif
            sb.AppendFormat("Organization (Company / Job Title / Department): {0} / {1} / {2}\r\n", Company, JobTitle, Department);
            sb.AppendFormat("IM / WebServer: {0} / {1}\r\n", IM, WebServer);
#if (!NoImage)
            sb.AppendFormat("ImagePath: {0}\r\n", ImagePath);
#endif
            sb.AppendFormat("Email1 / Email2 / Email3: {0} / {1} / {2}\r\n", email1, email2, email3);
            sb.AppendFormat("Telephone.Count: {0}\r\n", Telephone.Count);
            foreach (PhoneDetail p in Telephone.Values)
                sb.AppendFormat("\tPhoneNumber / GoogleType / Type: {0} / {1} / {2}\r\n", p.PhoneNumber, p.GoogleType, Enum.GetName(typeof(Constants.PhoneType), p.Type));

            sb.AppendFormat("Address.Count: {0}\r\n", Address.Count);
            foreach (AddressDetail a in Address.Values)
                sb.AppendFormat("\tStreet / POBox / PostalCode / City / CountryRegion /State / Type: {0} / {1} / {2} / {3} / {4} / {5} / {6}\r\n",
                    a.Street, a.POBox, a.PostalCode, a.City, a.CountryRegion, a.State, Enum.GetName(typeof(Constants.AddressType), a.Type));

            sb.AppendFormat("Notes:\r\n{0}\r\nNotes End\r\n****** End OneContactBase Dump ******", Notes);
            LoggerProvider.Instance.Logger.Debug(sb.ToString());
        }
        #endregion
    }
}
