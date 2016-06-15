using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using Microsoft.Win32;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Project;

namespace VSXArcana.SimpleProject
{
    [ProvideObject(typeof(GeneralPropertyPage))]
    [ProvideProjectFactory(typeof(SimpleProjectFactory), "Simple Project", "Simple Project Files (*.sproj);*.sproj", "sproj", "sproj", "NullPath", LanguageVsTemplate = "Simple Project", NewProjectRequireNewFolderVsTemplate = false)]
    [ProvideProjectItem(typeof(SimpleProjectFactory), "Simple Items", "NullPath", 100)]
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(GuidList.guidSimpleProjectPkgString)]
    public sealed class SimpleProjectPackage : ProjectPackage
    {
        public SimpleProjectPackage() {}

        protected override void Initialize()
        {
            base.Initialize();
            this.RegisterProjectFactory(new SimpleProjectFactory(this));

            // Add our command handlers for menu (commands must exist in the .vsct file)
            OleMenuCommandService mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if ( null != mcs )
            {
                CommandID menuCommandID = new CommandID(GuidList.guidSimpleProjectCmdSet, (int)PkgCmdIDList.cmdidMyCommand);
                MenuCommand menuItem = new MenuCommand(MenuItemCallback, menuCommandID );
                mcs.AddCommand( menuItem );
            }
        }

        public override string ProductUserContext
        {
            get { return "CustomProj"; }
        }

        private void MenuItemCallback(object sender, EventArgs e)
        {
            // todo:
        }

    }
}
