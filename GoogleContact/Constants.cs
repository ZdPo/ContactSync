using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GoogleContact
{
    public class Constants
    {
        public const string ApplicationName = "ZdPo.GoogleContactSynchronizer";
        /// <summary>
        /// Jmeno promene pro identifikaci odkazu na google account
        /// </summary>
        public const string NameOutlookUserField3 = "PokZdeGC:";
        /// <summary>
        /// provadi formatovani do Outlooku User Field 3, kam uklada unikátní ID Google kontantu
        /// </summary>
        public const string FormatOutlookUserField3 = NameOutlookUserField3 + "[{0}]";
        /// <summary>
        /// Format pro Google Extend properties obsahujici unikatní ID z Outlooku
        /// </summary>
        public const string FormatGoogleExtendProperies = "[{0}]";
        /// <summary>
        /// Jmeno pro Google Extended Properties v Outlooku
        /// </summary>
        public const string NameGoogleExtendProperies = "PokZdeGC_Outlook";
        /// <summary>
        /// Typy telefonnych cisel
        /// </summary>
        public enum PhoneType { Mobile = 0, Home, Business1, Business2, Other, Fax_home, Fax_work };
        /// <summary>
        /// typy adres
        /// </summary>
        public enum AddressType { Business, Home, Other };
        /// <summary>
        /// Format string for Synchronize All need two parameters
        /// "{0:# ##0} / {1:# ##0}"
        /// </summary>
        public const string FormatSyncAll = "{0:# ##0} / {1:# ##0}";
        /// <summary>
        /// Format string for Synchronize actual need one parameter
        /// "{0:# ##0}"
        /// </summary>
        public const string FormatSyncActual = "{0:# ##0}";
        /// <summary>
        /// Steps for both side replication
        /// </summary>
        public static readonly string[] SyncSteps = {"Start work",
                                                        "Read Outlook contacts", "Read Google contacts", 
                                                        "Add new Google contacts", "Add new outlook contact", 
                                                        "Delete Outlook contacts", "Delete Google Contacts",  
                                                        "Compare contacts"};
        /// <summary>
        /// Number of steps in Both side synchronization
        /// </summary>
        public const int MaxSyncStep = 8; //SyncSteps.Length;
        /// <summary>
        /// For first synchronize, how method use
        /// </summary>
        public enum FirstSetupSynchronize { Outlook2Google = 0, Google2Outlook, SynchronizeBoth };
        /// <summary>
        /// Fos synchronize witch method use
        /// </summary>
        public enum SetupSynchronize { Outlook2Google = 0, Google2Outlook, SynchronizeBothSide };
        /// <summary>
        /// Level for logging errors and work
        /// </summary>
        public enum LogLevels { None = 0, Fatal, Error, Warning, Debug };
    }
}
