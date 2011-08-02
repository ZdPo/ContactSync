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
        private static OutlookProvider _op;
        private Outlook.MAPIFolder _ContactFolder;
        private Outlook.NameSpace _NameSpace;
        private Outlook.Categories _Category;

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
                _Category = _NameSpace.Categories;
            }
        }
        #endregion

        #region Contact parameters
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
        /// Vraci seznam outlook items
        /// </summary>
        /// <returns></returns>
        public Outlook.Items OutlookItems()
        {
            return _ContactFolder.Items;
        }
        #endregion

        #region Work with category
        /// <summary>
        /// Update Outlook master category set based on CategoryName
        /// </summary>
        /// <param name="CategoryName"></param>
        /// <returns></returns>
        public string UpdateCategory(string CategoryName)
        {
            for (int i = 0; i < _Category.Count; i++)
            {
                if (string.Equals(_Category[i+1].Name, CategoryName))
                    return CategoryName;
            }
            LoggerProvider.Instance.Logger.Debug("Add new category {0}");
            _Category.Add(CategoryName);
            return CategoryName;
        }
        /// <summary>
        /// Remove Master category from Outlook
        /// </summary>
        /// <param name="CategoryName"></param>
        public void DeleteCategory(string CategoryName)
        {
            for (int i = 0; i < _Category.Count; i++)
            {
                if (string.Equals(_Category[i].Name, CategoryName))
                {
                    _Category.Remove(i);
                    break;
                }
            }
        }
        #endregion
    }
}
