using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;
using System.Xml.Serialization;

using Google.GData.Contacts;
using Google.GData.Client;
using Google.GData.Extensions;

using Outlook = Microsoft.Office.Interop.Outlook;
using Office = Microsoft.Office.Core;
using System.Xml;

namespace GoogleContact
{
    /// <summary>
    /// This is a base for one contact store in RAM. Data are independ of source, it's normalize do same structure and prepare for serialization.
    /// </summary>
    [XmlRoot(ElementName = "OneContactBase", DataType = "GoogleContact.OneContactBase", IsNullable = false)]
    public class OneContactBase // : IXmlSerializable
    {
        #region Constructor
        /// <summary>
        /// It's only for serialization
        /// </summary>
        public OneContactBase()
        {
        }

        /// <summary>
        /// This is set only when data read from cache
        /// </summary>
        [XmlElement(ElementName="IsFromCache", Type=typeof(bool))]
        public bool IsFromCache = false;
        #endregion

        #region Internal sync identificators
        /// <summary>
        /// Is this class based onOutlook
        /// </summary>
        [XmlElement("IsFromOutlook", typeof(bool))]
        public bool _isFromOutlook;
        /// <summary>
        /// Id from source
        /// </summary>
        [XmlElement("MyID",typeof(string))]
        public string _MyID = "";
        /// <summary>
        /// Based _isFromOutlook contains reference do other side _OtlookID/EntityID
        /// </summary>
        [XmlElement("RefernceID", typeof(string))]
        public string _referenceID = "";
        /// <summary>
        /// Modifikace in oulooku
        /// </summary>
        [XmlElement("ModifyDateTime", typeof(DateTime))]
        public DateTime _ModifyDateTime = DateTime.MinValue;
        /// <summary>
        /// My MD5 hash
        /// </summary>
        [XmlIgnore()]
        public string MD5selfCount = "";
        /// <summary>
        /// MD5 read from contact replica ID
        /// </summary>
        [XmlElement("MD5fromReplica", typeof(string))]
        public string MD5fromReplica = "";
        /// <summary>
        /// Reference to exist source for update
        /// </summary>
        [XmlIgnore()]
        public object _rawSource;
        #endregion

        #region Personal data
        [XmlElement("Title", typeof(string))]
        public string Title = "";
        [XmlElement("FirstName", typeof(string))]
        public string FirstName = "";
        [XmlElement("MiddleName", typeof(string))]
        public string MiddleName = "";
        [XmlElement("LastName", typeof(string))]
        public string LastName = "";
        [XmlElement("Suffix", typeof(string))]
        public string Suffix = "";
#if (!ANNIVESARY_NOT_WORK)
        DateTime Anniversary = DateTime.MinValue;
#endif
        [XmlElement("Birthday", typeof(DateTime))]
        public DateTime Birthday = DateTime.MinValue;
        [XmlElement("ImagePath", typeof(string))]
        public string ImagePath = null;
        [XmlElement("ImageHash", typeof(string))]
        public string ImageHash = "";
        [XmlElement("ImageETag", typeof(string))]
        public string ImageETag = "";
        [XmlElement("Notes", typeof(string))]
        public string Notes = "";
        [XmlElement("IM", typeof(string))]
        public string IM = "";
        #endregion

        #region Telephone
        /// <summary>
        /// List telefonnich cisel
        /// </summary>
        [XmlElement("Telephone", typeof(List<OneContactBase.PhoneDetail>))]
        public List<OneContactBase.PhoneDetail> Telephone = new List<OneContactBase.PhoneDetail>();
        #endregion

        #region Address
        /// <summary>
        /// List adres
        /// </summary>
        [XmlElement("Address", typeof(List<OneContactBase.AddressDetail>))]
        public List<OneContactBase.AddressDetail> Address = new List<OneContactBase.AddressDetail>();
        #endregion

        #region Email address
        [XmlElement("email1", typeof(string))]
        public string email1 = "";
        [XmlElement("email2", typeof(string))]
        public string email2 = "";
        [XmlElement("email3", typeof(string))]
        public string email3 = "";
        #endregion

        #region Company
        /// <summary>
        /// Company Name 
        /// Outlook = CompanyName
        /// Google = Organizations[].Name
        /// </summary>
        [XmlElement("Company", typeof(string))]
        public string Company = "";
        [XmlElement("Department", typeof(string))]
        public string Department = "";
        [XmlElement("JobTitle", typeof(string))]
        public string JobTitle = "";
        #endregion

        #region Ostatni
        [XmlElement("WebServer", typeof(string))]
        public string WebServer = "";
        [XmlElement("Category", typeof(string))]
        public List<string> Category = new List<string>();
        #endregion

        #region Child class
        /// <summary>
        /// Class for one phone number
        /// </summary>
        //[XmlRoot()]
        public class PhoneDetail
        {
            #region Phone detail
            [XmlElement(ElementName = "_PhoneNumber", Type = typeof(string))]
            public string _PhoneNumber = "";
            [XmlElement(ElementName = "_Type", Type = typeof(Constants.PhoneType))]
            public Constants.PhoneType _Type = Constants.PhoneType.Mobile;

            /// <summary>
            /// Its for serialization
            /// </summary>
            public PhoneDetail()
            {}
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
            [XmlIgnore]
            public string PhoneNumber
            {
                get { return _PhoneNumber; }
            }
            /// <summary>
            /// return typ of number
            /// </summary>
            [XmlIgnore]
            public Constants.PhoneType Type
            {
                get { return _Type; }
            }
            /// <summary>
            /// Return type in google string
            /// </summary>
            [XmlIgnore]
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
        ///[XmlRoot(ElementName = "OneContactBase.AddressDetail", DataType = "GoogleContact.OneContactBase.AddressDetail", IsNullable = false)]
        public class AddressDetail
        {
            #region Address detail
            [XmlElement(ElementName = "Type", Type = typeof(Constants.AddressType))]
            public Constants.AddressType Type = Constants.AddressType.Business;
            [XmlElement(ElementName="Street",Type=typeof(string), IsNullable=true)]
            public string Street = "";
            [XmlElement(ElementName = "POBox", Type = typeof(string), IsNullable = true)]
            public string POBox = "";
            [XmlElement(ElementName = "City", Type = typeof(string), IsNullable = true)]
            public string City = "";
            [XmlElement(ElementName = "PostalCode", Type = typeof(string), IsNullable = true)]
            public string PostalCode = "";
            [XmlElement(ElementName = "CountryRegion", Type = typeof(string), IsNullable = true)]
            public string CountryRegion = "";
            [XmlElement(ElementName = "State", Type = typeof(string), IsNullable = true)]
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
            [XmlIgnore]
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

        #region Count MD5 and other Internal function
        /// <summary>
        /// Recount self MD5
        /// </summary>
        public void MD5ReCountSelf()
        {
            MD5selfCount = MD5Actual();
        }
        /// <summary>
        /// Count actual MD5
        /// </summary>
        /// <returns></returns>
        public string MD5Actual()
        {

            return Utils.CountMD5(SummAllData());
        }
        /// <summary>
        /// Return data in one long String
        /// </summary>
        /// <returns></returns>
        public string SummAllData()
        {
            StringBuilder sb = new StringBuilder("");
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
            ///TODO: How control image change in MD5 hash
            sb.Append(string.IsNullOrEmpty(ImagePath));
            //sb.Append(ImageHash);

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
            foreach (string cat in Category)
            {
                sb.Append(cat);
            }

            return sb.ToString().Replace("\r", "").Replace("\n", "");
        }

        /// <summary>
        /// Retrieve saved ID from replica string  (if is) and return right ID or Empty string.
        /// Same time save find MD5 to MD5fromReplica
        /// </summary>
        /// <param name="source">In Outlook is UserFiled3, in GoogleContact ExtendProperties</param>
        /// <returns></returns>
        public string GetSavedReplicaID(string source)
        {
            string ret = "";
            string md5 = "";
            if (source.StartsWith("[") && source.Contains("]-[") && source.EndsWith("]", StringComparison.InvariantCulture))
            {
                ret = source.Substring(1, source.IndexOf("]-[", StringComparison.InvariantCulture) - 1);
                md5 = source.Substring(source.IndexOf("]-[", StringComparison.InvariantCulture) + 3, source.Length -
                    source.IndexOf("]-[", StringComparison.InvariantCulture) - 4);
            }
            MD5fromReplica = md5;
            return ret;
        }
        /// <summary>
        /// Return data or string.empty
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string DataOrEmpty(string source)
        {
            if (string.IsNullOrEmpty(source))
                return string.Empty;
            return source;
        }
        /// <summary>
        /// Clear reference for this record when set first time replica
        /// </summary>
        public void ClearReference()
        {
            if (SettingsProvider.Instance.IsFirstTime)
            {
                Constants.FirstSetupSynchronize fs = SettingsProvider.FirstSynchronizeGet();
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
        /// Return right phone number or ""
        /// </summary>
        /// <param name="typ"></param>
        /// <returns></returns>
        internal string GetRightPhoneNumber(Constants.PhoneType typ)
        {
            foreach(PhoneDetail d in Telephone)
            {
                if (d.Type==typ)
                return d.PhoneNumber;
            }
            return string.Empty;
        }
        /// <summary>
        /// Return requested address
        /// </summary>
        /// <param name="typ"></param>
        /// <returns></returns>
        internal AddressDetail GetRightAddress(Constants.AddressType typ)
        {
            foreach (AddressDetail a in Address)
            {
                if (a.Type == typ)
                    return a;
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
            ImagePath = updater.ImagePath;

            Notes = updater.Notes;
            IM = updater.IM;
            #endregion

            #region Telephone
            Telephone.Clear();
            if (updater.Telephone.Count > 0)
                foreach (PhoneDetail s in updater.Telephone)
                    Telephone.Add(s);
            #endregion

            #region Address
            Address.Clear();
            if (updater.Address.Count > 0)
                foreach (AddressDetail s in updater.Address)
                    Address.Add(s);
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
            WebServer = updater.WebServer;
            IsFromCache = false;
            Category.Clear();
            foreach (string cat in updater.Category)
                Category.Add(cat);
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
            sb.AppendFormat("ImagePath: {0}\r\n", ImagePath);

            sb.AppendFormat("Email1 / Email2 / Email3: {0} / {1} / {2}\r\n", email1, email2, email3);
            sb.AppendFormat("Telephone.Count: {0}\r\n", Telephone.Count);
            foreach (PhoneDetail p in Telephone)
                sb.AppendFormat("\tPhoneNumber / GoogleType / Type: {0} / {1} / {2}\r\n", p.PhoneNumber, p.GoogleType, Enum.GetName(typeof(Constants.PhoneType), p.Type));

            sb.AppendFormat("Address.Count: {0}\r\n", Address.Count);
            foreach (AddressDetail a in Address)
                sb.AppendFormat("\tStreet / POBox / PostalCode / City / CountryRegion /State / Type: {0} / {1} / {2} / {3} / {4} / {5} / {6}\r\n",
                    a.Street, a.POBox, a.PostalCode, a.City, a.CountryRegion, a.State, Enum.GetName(typeof(Constants.AddressType), a.Type));

            sb.AppendFormat("Notes:\r\n{0}\r\nNotes End\r\n****** End OneContactBase Dump ******", Notes);
            LoggerProvider.Instance.Logger.Debug(sb.ToString());
        }
        #endregion

    }
}
