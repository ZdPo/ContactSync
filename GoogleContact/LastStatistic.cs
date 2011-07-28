using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GoogleContact
{
    /// <summary>
    /// Summary for statistic about last action
    /// </summary>
    class LastStatistic
    {
        #region Local variable
        private int _goReadContacts = 0;
        private int _goDeleteContacts = 0;
        private int _goUpdateContacts = 0;
        private int _goInsertContacts = 0;
        private int _goNotChangeConatcts = 0;
        private int _ouReadContacts = 0;
        private int _ouDeleteContacts = 0;
        private int _ouUpdateContacts = 0;
        private int _ouInsertContacts = 0;
        private int _ouNotChangeConatcts = 0;
        private DateTime _LastUpdate = DateTime.Now;
        private bool _isSynchronize=false;
        #endregion

        #region method of class
        /// <summary>
        /// Clear all data in class
        /// </summary>
        public void Clear()
        {
            _goReadContacts = 0;
            _goDeleteContacts = 0;
            _goUpdateContacts = 0;
            _goInsertContacts = 0;
            _goNotChangeConatcts = 0;
            _ouReadContacts = 0;
            _ouDeleteContacts = 0;
            _ouUpdateContacts = 0;
            _ouInsertContacts = 0;
            _ouNotChangeConatcts = 0;
            _LastUpdate = DateTime.Now;
            _isSynchronize = false;
        }
        /// <summary>
        /// String for MMessageBox
        /// </summary>
        /// <returns></returns>
        public string StatisticString()
        {
            string form="Statistic:\t\tRead / Insert / Delete / Update\r\nOutlook:\t\t{0} / {1} / {2} / {3}\r\nGoogle:\t\t{4} / {5} / {6} / {7}\r\nLast Update:\t{8}";

            return string.Format(form, _ouReadContacts, _ouInsertContacts, _ouDeleteContacts, _ouUpdateContacts,
                _goReadContacts, _goInsertContacts, _goDeleteContacts, _goUpdateContacts, _LastUpdate);
        }
        #endregion



        #region Property of class
        public int goReadContacts {
            set
            {
                _goReadContacts = value;
                UpdateData();
            }
            get { return _goReadContacts; }
        }
        public int goDeleteContacts
        {
            set
            {
                _goDeleteContacts = value;
                UpdateData();
            }
            get { return _goDeleteContacts; }
        }
        public int goUpdateContacts
        {
            set
            {
                _goUpdateContacts = value;
                UpdateData();
            }
            get { return _goUpdateContacts; }
        }
        public int goInsertContacts
        {
            set
            {
                _goInsertContacts = value;
                UpdateData();
            }
            get { return _goInsertContacts; }
        }
        public int goNotChangeConatcts
        {
            set
            {
                _goNotChangeConatcts = value;
                UpdateData();
            }
            get { return _goNotChangeConatcts; }
        }
        public int ouReadContacts
        {
            set
            {
                _ouReadContacts = value;
                UpdateData();
            }
            get { return _ouReadContacts; }
        }

        public int ouDeleteContacts
        {
            set
            {
                _ouDeleteContacts = value;
                UpdateData();
            }
            get { return _ouDeleteContacts; }
        }

        public int ouUpdateContacts
        {
            set
            {
                _ouUpdateContacts = value;
                UpdateData();
            }
            get { return _ouUpdateContacts; }
        }

        public int ouInsertContacts
        {
            set
            {
                _ouInsertContacts= value;
                UpdateData();
            }
            get { return _ouInsertContacts; }
        }

        public int ouNotChangeConatcts
        {
            set
            {
                _ouNotChangeConatcts = value;
                UpdateData();
            }
            get { return _ouNotChangeConatcts; }
        }

        public DateTime LastUpdate { get { return _LastUpdate; } }
        public bool isSynchronize { get { return _isSynchronize; } }
        #endregion

        #region Private method
        private void UpdateData()
        {
            _isSynchronize = true;
            _LastUpdate = DateTime.Now;
        }
        #endregion
    }
}
