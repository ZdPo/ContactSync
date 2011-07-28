using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Outlook = Microsoft.Office.Interop.Outlook;
using Office = Microsoft.Office.Core;
using Microsoft.Office.Tools;


namespace GoogleContact
{
    
    class OutlookProvider
    {
        #region start data
        private static OutlookProvider _op = null;
        private Outlook.MAPIFolder _ContactFolder = null;
        private Outlook.NameSpace _NameSpace=null;

        private OutlookProvider()
        {
            LoggerProvider.Instance.Logger.Debug("Class OutlookProvider created");
        }

        public static OutlookProvider Instance
        {
            get
            {
                if (_op == null)
                    _op = new OutlookProvider();
                return _op;
            }
        }
        public Outlook.NameSpace NameSpace
        {
            get { return _NameSpace; }
            set 
            { 
                _NameSpace = value;
                _ContactFolder = _NameSpace.GetDefaultFolder(Outlook.OlDefaultFolders.olFolderContacts);
            }
        }
        #endregion

        /// <summary>
        /// Pocet zaznamu v Oulooku
        /// </summary>
        /// <returns></returns>
        public int CountContact()
        {
            Outlook.Items it = _ContactFolder.Items;
            int i = it.Count;
            return i;
        }

        /// <summary>
        /// Vraci novou položku pro Outlook Contact
        /// </summary>
        /// <returns></returns>
        public Outlook.ContactItem CreateNewContactItem()
        {
            return (Outlook.ContactItem) _ContactFolder.Items.Add(Outlook.OlItemType.olContactItem);
        }
        /// <summary>
        /// Seznam vsech items
        /// </summary>
        /// <returns></returns>
        public List<OneContact> GetAllContactItems()
        {
            List<OneContact> ouContact = new List<OneContact>();
            Outlook.ContactItem outC = null;
            
            Outlook.Items it = _ContactFolder.Items;
            for(int i=0;i<it.Count;i++)
            {
                if (i==0)
                    outC = (Outlook.ContactItem)it.GetFirst();
                else
                    outC = (Outlook.ContactItem)it.GetNext();

            }

            return ouContact; ;
        }
        /// <summary>
        /// Vraci seznam outlook items
        /// </summary>
        /// <returns></returns>
        public Outlook.Items OutlookItems()
        {
            return _ContactFolder.Items;
        }
    }
}
