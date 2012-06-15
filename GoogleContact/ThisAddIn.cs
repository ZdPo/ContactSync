using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Outlook = Microsoft.Office.Interop.Outlook;
using Office = Microsoft.Office.Core;
using Microsoft.Office.Tools;

namespace GoogleContact
{
    public partial class ThisAddIn
    {
        Outlook.Inspectors inspectors;

        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            inspectors = this.Application.Inspectors;
            LoggerProvider.Instance.Logger.Debug("******\r\n\t\tProgram start");
            inspectors.NewInspector += new Outlook.InspectorsEvents_NewInspectorEventHandler(inspectors_NewInspector);
            OutlookProvider.Instance.NameSpace = this.Application.Session;
            //GoogleProvider.GetProvider.Logon(SettingsProvider.Instance.UserName, SettingsProvider.Instance.UserPassword);
        }

        protected override Office.IRibbonExtensibility CreateRibbonExtensibilityObject()
        {
            LoggerProvider.Instance.Logger.Debug("Create Ribbon");
            return new Microsoft.Office.Tools.Ribbon.RibbonManager(
                new Microsoft.Office.Tools.Ribbon.OfficeRibbon[] { new GCRibbon() });

        }

        void inspectors_NewInspector(Outlook.Inspector Inspector)
        {
        }

        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {
        }

        #region VSTO generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InternalStartup()
        {
            this.Startup += new System.EventHandler(ThisAddIn_Startup);
            this.Shutdown += new System.EventHandler(ThisAddIn_Shutdown);
        }
        
        #endregion
    }
}
