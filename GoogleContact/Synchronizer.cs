#define SIMULATE_SAVE
/// use for simulation of updates
#define DUMP_CONTACTS
/// use for dump data to log

#undef SIMULATE_SAVE
//#undef DUMP_CONTACTS

using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

using Google.GData.Contacts;
using Google.GData.Client;
using Google.GData.Extensions;

using Outlook = Microsoft.Office.Interop.Outlook;
using Office = Microsoft.Office.Core;
using Microsoft.Office.Tools;

namespace GoogleContact 
{
    /// <summary>
    /// Realize synchronize between O &amp; G
    /// </summary>
    class Synchronizer : System.IDisposable
    {
        #region Private variables
        private GoogleProvider gp;
        private OutlookProvider op;
        private SyncInfo syncinfo;
        private const int MaxSteps = Constants.MaxSyncStep;
        private Hashtable ouContacts;
        private DateTime ouCacheTime = DateTime.MinValue;
        private int _ouMaxContacts;
        private Hashtable goContacts;
        private DateTime goCacheTime = DateTime.MinValue;
        private int _goMaxContacts;
        private int _ActualStep;
        private LastStatistic _lastStatistic;
        #endregion

        #region Constructor 
        public Synchronizer(ref LastStatistic lastStatistic)
        {
            gp = GoogleProvider.GetProvider;
            op=OutlookProvider.Instance;
            syncinfo = new SyncInfo(MaxSteps);
            ouContacts = new Hashtable();
            goContacts = new Hashtable();
            _lastStatistic = lastStatistic;
            LoggerProvider.Instance.Logger.Debug("Synchronizer class created");
        }
        /// <summary>
        /// Clear data 
        /// </summary>
        ~Synchronizer()
        {
            LoggerProvider.Instance.Logger.Debug("Synchronizer class remove from RAM");
            if (syncinfo != null)
            {
                try
                {
                    syncinfo.Close();
                    syncinfo.Dispose();
                }
                catch (Exception e)
                {
                    LoggerProvider.Instance.Logger.Error(e);
                }
            }
            if (ouContacts != null)
                ouContacts.Clear();
            if (goContacts != null)
                goContacts.Clear();
            ouContacts = null;
            goContacts = null;
        }

        /// <summary>
        /// Change by FxCop recomendation
        /// http://msdn.microsoft.com/library/ms182269(VS.90).aspx
        /// </summary>
        public void Dispose()
        {
            LoggerProvider.Instance.Logger.Debug("Synchronizer class Disposed");
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (syncinfo != null)
                {
                    syncinfo.Close();
                    syncinfo.Dispose();
                    syncinfo = null;
                    if (ouContacts != null)
                        ouContacts.Clear();
                    if (goContacts != null)
                        goContacts.Clear();
                    ouContacts = null;
                    goContacts = null;
                }
            }
        }

        #endregion

        #region Procedures work start
        /// <summary>
        /// Prepare info windows and show it
        /// </summary>
        public void SetupSync()
        {
            syncinfo.WorkOn = 0;
            syncinfo.ActualStep = Constants.SyncSteps()[_ActualStep];
            LoggerProvider.Instance.Logger.Debug("SetupSync invoke Syncstep: {0}",Constants.SyncSteps()[_ActualStep]);
            syncinfo.Show();
        }
        /// <summary>
        /// realize steps to synchronize data
        /// </summary>
        public void Synchronize()
        {
            _lastStatistic.Clear();

            #region read data from source
            syncinfo.ActualNextStep();
            /// need read contacts from oulook
            _ActualStep = 1;
            LoggerProvider.Instance.Logger.Debug("Synchronize step 1 ({0})", Constants.SyncSteps()[_ActualStep]);
            UpdateSyncInfo();
            Step1ReadOutlook();
            syncinfo.ActualNextStep();
            /// need read all cntact from google
            _ActualStep = 2;
            LoggerProvider.Instance.Logger.Debug("Synchronize step 2 ({0})", Constants.SyncSteps()[_ActualStep]);
            UpdateSyncInfo();
            Step2ReadGoogle();
            syncinfo.ActualNextStep();
            #endregion

            #region Insert data to other side
            //if (SettingsProvider.Instance.IsAddToGoogle)
            {
                /// Next step Insert new contact to google
                _ActualStep = 3;
                LoggerProvider.Instance.Logger.Debug("Synchronize step 3 ({0})", Constants.SyncSteps()[_ActualStep]);
                UpdateSyncInfo();
                Step3AddToGoogle();
                syncinfo.ActualNextStep();
            }
            /// Insert new contact to Outlook
            //if (SettingsProvider.Instance.IsAddToOutlook)
            {
                _ActualStep = 4;
                LoggerProvider.Instance.Logger.Debug("Synchronize step 4 ({0})", Constants.SyncSteps()[_ActualStep]);
                UpdateSyncInfo();
                Step4AddToOutlook();
                syncinfo.ActualNextStep();
#if (DUMP_CONTACTS)
                DumpContactToLog(ref ouContacts);
                DumpContactToLog(ref goContacts);
#endif
            }
            #endregion

            #region Delete old contacts
            /// Delete contact from outlook
            //if (SettingsProvider.Instance.IsDeleteFromOutlook)
            {
                _ActualStep = 5;
                LoggerProvider.Instance.Logger.Debug("Synchronize step 5 ({0})", Constants.SyncSteps()[_ActualStep]);
                UpdateSyncInfo();
                Step5DeleteInOutlook();
                syncinfo.ActualNextStep();
            }
            /// Delete contact from Google
            //if (SettingsProvider.Instance.IsDeleteFromGoogle)
            {
                _ActualStep = 6;
                LoggerProvider.Instance.Logger.Debug("Synchronize step 6 ({0})", Constants.SyncSteps()[_ActualStep]);
                UpdateSyncInfo();
                Step6DeleteInGoogle();
                syncinfo.ActualNextStep();
#if (DUMP_CONTACTS)
                DumpContactToLog(ref ouContacts);
                DumpContactToLog(ref goContacts);
#endif
            }
            #endregion

            #region Update statistic on screen
            syncinfo.GoogleContacts = goContacts.Count;
            syncinfo.OutlookContacts = ouContacts.Count;
            syncinfo.Update();
            #endregion

            #region Update both side
            /// Update contact
            _ActualStep = 7;
            LoggerProvider.Instance.Logger.Debug("Synchronize step 7 ({0})", Constants.SyncSteps()[_ActualStep]);
            UpdateSyncInfo();
            Step7Update();
            syncinfo.ActualNextStep();
#if (DUMP_CONTACTS)
            DumpContactToLog(ref ouContacts);
            DumpContactToLog(ref goContacts);
#endif
            #endregion

            #region Reset first update flag
            if (SettingsProvider.Instance.IsFirstTime)
            {
                SettingsProvider.Instance.IsFirstTime = false;
                SettingsProvider.Instance.Save();
                LoggerProvider.Instance.Logger.Debug("First time synchronize finish. Next is standard synchronization");
            }
            #endregion
            
            LoggerProvider.Instance.Logger.Debug("Synchronize ends");

            #region Write to cache
            if (SettingsProvider.Instance.IsUseGoogleCache)
                Utils.WriteGoogleToCache(goContacts);
            if (SettingsProvider.Instance.IsUseOutlookCache)
                Utils.WriteOutlookToCache(ouContacts);
            #endregion

        }
        #endregion

        #region Steps in synchronize

        #region Steps read data
        /// <summary>
        /// Step first - read all outlook contacts
        /// </summary>
        private void Step1ReadOutlook()
        {
            bool IsNeedReadAllData = true;
            bool IsNeedReadNewData = false;
            List<string> toread = new List<string>(); /// list EntryID for reading
            List<string> todelete = new List<string>(); /// list EntryID for delete
            if (SettingsProvider.Instance.IsUseOutlookCache)
            {
                LoggerProvider.Instance.Logger.Debug("Read data from cache for Outlook");
                ouCacheTime = Utils.LoadCacheDate(true);
                ouContacts = Utils.ReadOutlookFromCache(ref ouCacheTime);
                if ((ouCacheTime > DateTime.MinValue) && (ouCacheTime < DateTime.Now) && (ouContacts.Count > 0)) // Data read from cache is good
                {
                    //_lastStatistic.ouReadContacts = ouContacts.Count;
                    Dictionary<string, DateTime> ouall = OutlookProvider.Instance.GetTableFilter(ouCacheTime);
                    DateTime ouDate;

                    foreach (string s in ouContacts.Keys)
                        todelete.Add(s);

                    foreach (string EntID in ouall.Keys)
                    {
                        ouDate = ouall[EntID];
                        if (ouContacts.ContainsKey(EntID)) /// is EntID found in current cached data
                        {
                            todelete.Remove(EntID);
                            if (((OneContact)ouContacts[EntID])._ModifyDateTime < ouDate) //date from curent select is 
                            {
                                ouContacts.Remove(EntID);
                                toread.Add(EntID);
                            }
                        }
                        else
                            toread.Add(EntID);
                    }
                    /// in toread now only new EntryID
                    /// in todelete now only contact deleted in outlook, this need clear from cache, because in last two steps it delete from google to
                    foreach (string s in todelete)
                        ouContacts.Remove(s);
                    IsNeedReadNewData = toread.Count > 0;
                }
                else
                    LoggerProvider.Instance.Logger.Debug("Data from Outlook cache isn't valid");
            }
            if (IsNeedReadNewData) // need read new contact data to cache
            {
                LoggerProvider.Instance.Logger.Debug("Read only new data from Outlook");
                Outlook.Items it = op.OutlookItems();
                //_ouMaxContacts = op.CountContact();
                syncinfo.OutlookContacts = toread.Count;
                syncinfo.WorkOnMax = toread.Count;
                Outlook.ContactItem oci = null;
                OneContact oc = null;
                object works = null;
                int i = 0;
                int read = 0;

                syncinfo.WorkOnNextStep();
                for (; i < toread.Count; i++)
                {
                    syncinfo.WorkOn = i + 1;
                    syncinfo.WorkOnNextStep();
                    
                    works = OutlookProvider.Instance.FindItemfromID(toread[i]);

                    //if (i == 0)
                    //    works = it.GetFirst();
                    //else
                    //    works = it.GetNext();
                    if (works is Outlook.DistListItem)
                        continue;
                    oci = works as Outlook.ContactItem;
                    if (works == null)
                        continue;
                    if (toread.Contains(oci.EntryID))
                    {
                        oc = new OneContact(oci);
                        if (SettingsProvider.Instance.IsFirstTime)
                            oc.ClearReference();
                        ouContacts.Add(oci.EntryID, oc);
                    }
                    read++;
                }
                _lastStatistic.ouReadContacts += read;
                syncinfo.OutlookContacts = ouContacts.Count;
                IsNeedReadAllData = false;
            }
            if (IsNeedReadAllData)
            {
                /// because need read all data. Before this need remove all cached data
                ouContacts.Clear();
                /// start read all data
                ouCacheTime=DateTime.MinValue;
                LoggerProvider.Instance.Logger.Debug("Read all data from Outlook");
                Outlook.Items it = op.OutlookItems();
                _ouMaxContacts = op.CountContact();
                syncinfo.OutlookContacts = _ouMaxContacts;
                syncinfo.WorkOnMax = _ouMaxContacts;
                Outlook.ContactItem oci = null;
                OneContact oc = null;
                object works = null;
                int i = 0;
                int read = 0;

                syncinfo.WorkOnNextStep();
                for (; i < _ouMaxContacts; i++)
                {
                    syncinfo.WorkOn = i + 1;
                    syncinfo.WorkOnNextStep();
                    if (i == 0)
                        works = it.GetFirst();
                    else
                        works = it.GetNext();
                    if (works is Outlook.DistListItem)
                        continue;
                    oci = works as Outlook.ContactItem;
                    if (works == null)
                        continue;
                    oc = new OneContact(oci);
                    if (SettingsProvider.Instance.IsFirstTime)
                        oc.ClearReference();
                    ouContacts.Add(oci.EntryID, oc);
                    read++;
                }
                _lastStatistic.ouReadContacts += read;
                syncinfo.OutlookContacts = ouContacts.Count;
            }
        }

        /// <summary>
        /// Second step is read all google contacts
        /// </summary>
        private void Step2ReadGoogle()
        {
            bool IsReadAllContact = true; // use for reread all contact, When time diferences biggers that 20 days
            List<string> toread = new List<string>(); /// list EntryID for reading
            List<string> todelete = new List<string>(); /// list EntryID for delete
            if (SettingsProvider.Instance.IsUseGoogleCache) // use cache system for Google contact
            {


                LoggerProvider.Instance.Logger.Debug("Read data from cache for Google");
                goCacheTime = Utils.LoadCacheDate(true);
                goContacts = Utils.ReadGoogleFromCache(ref goCacheTime);
                if ((goCacheTime > DateTime.MinValue) && (goCacheTime < DateTime.Now) && DateTime.Now.AddDays(-20) < goCacheTime) // Data read from cache is good
                {
                    _lastStatistic.goReadContacts = goContacts.Count;
                    Feed<Google.Contacts.Contact> gochanged = gp.ContactItemsChangedAfter(goCacheTime);
                    IsReadAllContact = false; // data read
                    if ((gochanged != null) && (gochanged.TotalResults > 0))// return data it's good
                    {
                        OneContact oc = null;
                        foreach (Google.Contacts.Contact gc in gochanged.Entries)
                        {
                            if (goContacts.ContainsKey(gc.Id))
                            {
                                if (gc.Deleted) /// it's deleted?
                                {
                                    goContacts.Remove(gc.Id);
                                    _lastStatistic.goReadContacts--;
                                    continue;
                                }
                                if (gc.Updated > ((OneContact)goContacts[gc.Id])._ModifyDateTime)
                                {
                                    goContacts.Remove(gc.Id);
                                    _lastStatistic.goReadContacts--;
                                }
                                else
                                    continue;
                            }
                            oc = new OneContact(gc);
                            if (SettingsProvider.Instance.IsFirstTime)
                                oc.ClearReference();
                            goContacts.Add(gc.Id, oc);
                            _lastStatistic.goReadContacts++;
                        }
                    }
                }
                else ///need clear cache read data
                {
                    goContacts.Clear();
                }
            }

            if (IsReadAllContact)
            {
                LoggerProvider.Instance.Logger.Debug("Read all data from Gmail");
                OneContact oc = null;
//                gp.ClearContactItems(); // need refresh before start next read, because ContactItems are cached in program
                _goMaxContacts = gp.CountContact();
                syncinfo.GoogleContacts = _goMaxContacts;
                syncinfo.WorkOnMax = _goMaxContacts;
                int i = 0;
                syncinfo.WorkOnNextStep();
                foreach (Google.Contacts.Contact gc in gp.ContactItems.Entries)
                {
                    syncinfo.WorkOn = ++i;
                    syncinfo.WorkOnNextStep();
                    oc = new OneContact(gc);
                    if (SettingsProvider.Instance.IsFirstTime)
                        oc.ClearReference();
                    goContacts.Add(gc.Id, oc);
                }
                _lastStatistic.goReadContacts += i;
                syncinfo.GoogleContacts = goContacts.Count;
            }
        }
        #endregion

        #region Insert data
        /// <summary>
        /// Step 3 - add contact from Outlook to Google
        /// </summary>
        private void Step3AddToGoogle()
        {
            LoggerProvider.Instance.Logger.Debug("Start step 3 add new contact to Google");
            syncinfo.WorkOnMax = ouContacts.Count;
            int i = 0;
            int inserted = 0;
            int deleted = 0;
            syncinfo.WorkOnNextStep();

            ArrayList keys=new ArrayList(ouContacts.Keys);
            foreach (string s in keys)
            {
                syncinfo.WorkOn = ++i;
                syncinfo.WorkOnNextStep();
                OneContact c=ouContacts[s] as OneContact;
                if (!c.IsReferToOtherSide) 
                {
                    if (SettingsProvider.Instance.IsAddToGoogle) // if not alow update this side need clear on second side
                        inserted += AddNewGoogleContact(c) ? 1 : 0;
                    else
                        deleted += DeleteFromOutlook(s) ? 1 : 0;
                }
            }
            _lastStatistic.goInsertContacts += inserted;
            _lastStatistic.ouDeleteContacts += deleted;
        }

        /// <summary>
        /// Step 4 - add contact from Google to outlook
        /// </summary>
        private void Step4AddToOutlook()
        {
            LoggerProvider.Instance.Logger.Debug("Start step 4 add new contact to Outlook");
            syncinfo.WorkOnMax = goContacts.Count;
            int i = 0;
            int inserted = 0;
            int delete = 0;
            syncinfo.WorkOnNextStep();
            OneContact c;
            ArrayList keys = new ArrayList(goContacts.Keys);
            foreach (string s in keys)
            {
                syncinfo.WorkOn = ++i;
                syncinfo.WorkOnNextStep();
                c = goContacts[s] as OneContact;
                if (!c.IsReferToOtherSide) // there isn't reference to second side. Need add it
                {
                    if (SettingsProvider.Instance.IsAddToOutlook) // if not alow update this side need clear on second side
                        inserted += AddNewOutlookContact(c) ? 1 : 0;
                    else
                        delete += DeleteFromGoogle(s) ? 1 : 0;
                }
            }
            _lastStatistic.ouInsertContacts += inserted;
            _lastStatistic.goDeleteContacts += delete;
        }
        #endregion

        #region Delete data
        /// <summary>
        /// Step 5 - delete contact from Outlook
        /// </summary>
        private void Step5DeleteInOutlook()
        {
            LoggerProvider.Instance.Logger.Debug("Start step 5 delete contact from Outlook");
            syncinfo.WorkOnMax = ouContacts.Count; // Setup count of cotacts
            syncinfo.WorkOnNextStep();
            int delete = 0;
            int insert = 0;
            int i=0;
            ArrayList keys = new ArrayList(ouContacts.Keys);
            foreach(string s in keys)
            {
                syncinfo.WorkOn = ++i;
                syncinfo.WorkOnNextStep();
                if (!goContacts.ContainsKey(((OneContact)ouContacts[s]).ReferenceID))
                {
                    if (SettingsProvider.Instance.IsDeleteFromOutlook) /// Is alow delete on Outlook? Otherwise need add this contact to Google
                        delete += DeleteFromOutlook(s) ? 1 : 0;
                    else
                        insert += AddNewGoogleContact((OneContact)ouContacts[s]) ? 1 : 0;
                }
            }
            _lastStatistic.ouDeleteContacts += delete;
            _lastStatistic.goInsertContacts += insert;
        }

        /// <summary>
        /// Step 6 - delete contact from Google
        /// </summary>
        private void Step6DeleteInGoogle()
        {
            LoggerProvider.Instance.Logger.Debug("Start step 6 delete contact from Google");
            syncinfo.WorkOnMax = goContacts.Count; // nastaveni prochazenych odkazu
            syncinfo.WorkOnNextStep();
            int delete = 0;
            int insert = 0;
            int i = 0;
            ArrayList keys = new ArrayList(goContacts.Keys);
            foreach (string s in keys)
            {
                syncinfo.WorkOn = ++i;
                syncinfo.WorkOnNextStep();
                if (!ouContacts.ContainsKey(((OneContact)goContacts[s]).ReferenceID))
                {
                    if (SettingsProvider.Instance.IsDeleteFromGoogle)
                        delete += DeleteFromGoogle(s) ? 1 : 0;
                    else
                        insert += AddNewOutlookContact((OneContact)goContacts[s]) ? 1 : 0;
                }
            }
            _lastStatistic.goDeleteContacts += delete;
            _lastStatistic.ouInsertContacts += insert;
        }
        #endregion

        #region Update data
        private void Step7Update()
        {
            LoggerProvider.Instance.Logger.Debug("Start step 7 update contacts");
            syncinfo.WorkOnMax = goContacts.Count; // nastaveni prochazenych odkazu
            syncinfo.WorkOnNextStep();
            int updateGo = 0;
            int updateOut = 0;
            int i = 0;
            if (ouContacts.Count != goContacts.Count)
            {
                LoggerProvider.Instance.Logger.Error("Problem in check Update - number of contacts are different Outlook/Google: {0}/{1}", 
                    ouContacts.Count, goContacts.Count);
                return;
            }
            OneContact outItem=null;
            OneContact goItem=null;
            StringBuilder sb = new StringBuilder();
            ArrayList keys = new ArrayList(goContacts.Keys);
            foreach (string s in keys)
            {
                syncinfo.WorkOn = ++i;
                syncinfo.WorkOnNextStep();
                goItem=(OneContact)goContacts[s];
                if (!ouContacts.ContainsKey(goItem.ReferenceID))
                {
                    LoggerProvider.Instance.Logger.Error("Contacts don't found in table");
                    continue;
                }
                outItem=(OneContact)ouContacts[goItem.ReferenceID];

                if (outItem.MD5selfCount != goItem.MD5selfCount)
                {
                    sb.Remove(0, sb.ToString().Length);
                    sb.AppendLine(string.Format("Update contact Outlook - Google: {0} - {1}", outItem._MyID, goItem._MyID));
                    sb.AppendLine(string.Format("User name: {0} {1}", outItem.FirstName, outItem.LastName));
                    sb.AppendLine(string.Format("MD5 Oulook/Google: {0} / {1}", outItem.MD5selfCount, goItem.MD5selfCount));
                    sb.AppendLine(string.Format("MD5 source:\r\n{0}\r\n{1}", outItem.SummAllData(), goItem.SummAllData()));
                    sb.AppendLine(string.Format("Last Update Outlook/Google:  {0} / {1}", outItem.UpdateTime, goItem.UpdateTime));
                    if (outItem.UpdateTime < goItem.UpdateTime) // What is olders
                    {
                        if (SettingsProvider.Instance.IsUpdateToOutlook)
                        {
                            sb.AppendLine("Update on Outlook");
                            updateOut += UpdateOutlookFromGoogle(goItem, outItem) ? 1 : 0;
                        }
                        else
                        {
                            sb.AppendLine("Update on Google thru configurate way to update");
                            updateGo += UpdateGoogleFromOutlook(outItem, goItem) ? 1 : 0;
                        }
                    }
                    else
                    {
                        if (SettingsProvider.Instance.IsUpdateToGoogle)
                        {
                            sb.AppendLine("Update on Google");
                            updateGo += UpdateGoogleFromOutlook(outItem, goItem) ? 1 : 0;
                        }
                        else
                        {
                            sb.AppendLine("Update on Outlook thru configurate way to update");
                            updateOut += UpdateOutlookFromGoogle(goItem, outItem) ? 1 : 0;
                        }
                    }
                    LoggerProvider.Instance.Logger.Debug(sb.ToString());
                }
            }
            _lastStatistic.goUpdateContacts += updateGo;
            _lastStatistic.ouUpdateContacts += updateOut;

        }
        #endregion
        #endregion

        #region Private methods
        /// <summary>
        /// Update infromationabout actual step
        /// </summary>
        private void UpdateSyncInfo()
        {
            syncinfo.ActualStep = Constants.SyncSteps()[_ActualStep];
            //syncinfo.ActualStepIndex = _ActualStep;
        }
#if (DUMP_CONTACTS)
        /// <summary>
        /// Dump contact to Log. This is uses for debug only
        /// </summary>
        /// <param name="contacts">Referces to one of local list</param>
        private static void DumpContactToLog(ref Hashtable contacts)
        {
#if (DEBUG1)
            foreach (OneContact c in contacts.Values)
            {
                c.DumpActualDataToLog();
            }
#endif
        }
#endif
        #endregion

        #region AddMetods/Detele/Update one contact method
        /// <summary>
        /// Add one contact to Google
        /// </summary>
        /// <param name="c">Outlook contact to add</param>
        /// <returns>true if add success</returns>
        internal bool AddNewGoogleContact(OneContact c)
        {
            Google.Contacts.Contact goContact = null;
#warning There is a problem with add google contact with Image
#if (!SIMULATE_SAVE)
            Google.Contacts.Contact goContactNew = null;
            OneContact hlp = null;
#endif

            try
            {
                goContact = c.GetGoogle();
            }
            catch (Exception e)
            {
                /// Can't create Google Entity from OneContacs Class
                LoggerProvider.Instance.Logger.Error("Problem to read Google.Contacts.Contact from GoogleSynchronizer.OneContact");
                LoggerProvider.Instance.Logger.Error(e);
                return false; ;
            }
            if (goContact == null)
            {
                LoggerProvider.Instance.Logger.Error("Can't insert NULL object");
                return false;
            }
#if (!SIMULATE_SAVE)
            goContactNew = GoogleProvider.GetProvider.Insert(goContact);
            // If don't insert new contact to Google need continue
            if (goContactNew == null)
                return false;
#endif

#if (!SIMULATE_SAVE)
            hlp = new OneContact(goContactNew);
            c.ReferenceID = hlp.ContactID;
            goContacts.Add(hlp.ContactID, hlp);
            c.UpdateRefInOutlook(hlp.CreateReferenceID());
#endif
            LoggerProvider.Instance.Logger.Debug("Save update to Outlook");

            #region DEBUG infos
#if (DEBUG1)
            LoggerProvider.Instance.Logger.Debug("Source contact MD5/Count MD5: {0}/{1}", c.MD5selfCount, Utils.CountMD5(c.SummAllData()));
            LoggerProvider.Instance.Logger.Debug(c.SummAllData());
            LoggerProvider.Instance.Logger.Debug("Destination contact MD5/Count MD5: {0}/{1}", hlp.MD5selfCount, Utils.CountMD5(hlp.SummAllData()));
            LoggerProvider.Instance.Logger.Debug(hlp.SummAllData());
            hlp = null;
#endif

            #endregion
            return true;
        }

        /// <summary>
        /// Add one contact to Outlook
        /// </summary>
        /// <param name="c">Google cotact to add</param>
        /// <returns>true if add success</returns>
        internal bool AddNewOutlookContact(OneContact c)
        {
            Outlook.ContactItem newContact;

            try
            {
                newContact = c.GetOutlook();
            }
            catch
            {
                /// Can't create Google Entity from OneContacs Class
                LoggerProvider.Instance.Logger.Error("Problem to read Outlook.ContactItem from GoogleSynchronizer.OneContact");
                return false;
            }
            if (newContact == null)
            {
                LoggerProvider.Instance.Logger.Error("Can't insert NULL object");
                return false;
            }
#if (!SIMULATE_SAVE)
            newContact.Save();
#endif
#if (!SIMULATE_SAVE)
            OneContact hlp = new OneContact(newContact);
            c.ReferenceID = newContact.EntryID;
            ouContacts.Add(newContact.EntryID, hlp);
            c.UpdateRefInGoogle(hlp.CreateReferenceID());
#endif
            LoggerProvider.Instance.Logger.Debug("Save update to Google");
            return true;
        }
        
        /// <summary>
        /// Delete specific contact from Outlook
        /// </summary>
        /// <param name="Name">Name to ouContact to delete</param>
        /// <returns>True if delete success</returns>
        internal bool DeleteFromOutlook(string Name)
        {
            ((OneContact)ouContacts[Name]).Delete();
            ouContacts.Remove(Name);
            return true;
        }

        /// <summary>
        /// Delete specific contact from Google
        /// </summary>
        /// <param name="Name">Name to goContact to delete</param>
        /// <returns>True if delete success</returns>
        internal bool DeleteFromGoogle(string Name)
        {
            ((OneContact)goContacts[Name]).Delete();
            goContacts.Remove(Name);
            return true;
        }

        /// <summary>
        /// Update Outlook contact from Google
        /// </summary>
        /// <param name="sourceGoogle">Source Google contact</param>
        /// <param name="destinationOutlook">destination Outlook contact</param>
        /// <returns>True if delete success</returns>
        internal static bool UpdateOutlookFromGoogle(OneContact sourceGoogle, OneContact destinationOutlook)
        {
            if (sourceGoogle.IsSourceOutlook || (!destinationOutlook.IsSourceOutlook))
                return false;

#if (DUMP_CONTACTS)
            sourceGoogle.DumpActualDataToLog();
            destinationOutlook.DumpActualDataToLog();
#endif

            destinationOutlook.UpdateFromOther(sourceGoogle);
            return true;
        }

        /// <summary>
        /// Update Google contact from outlook
        /// </summary>
        /// <param name="sourceGoogle">Source outlook contact</param>
        /// <param name="destinationOutlook">destination google contact</param>
        /// <returns>True if delete success</returns>
        internal static bool UpdateGoogleFromOutlook(OneContact sourceOutlook, OneContact destinationGoogle)
        {
            if ((!sourceOutlook.IsSourceOutlook) || destinationGoogle.IsSourceOutlook)
                return false;
            destinationGoogle.UpdateFromOther(sourceOutlook);
            return true;
        }
        #endregion
    }
}
