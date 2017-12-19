
namespace ShowDebugHelpContext
{
    using System;
    using System.ComponentModel.Design;
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Shell.Interop;

    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(DebugHelpContextPackage.PackageGuidString)]
    public sealed class DebugHelpContextPackage : Package
    {
        public const string PackageGuidString = "0926e9f5-ceb6-4333-8b34-37de7acb8e94";
        public static  Guid CommandSetGuid = new Guid("36c60ac4-07c8-4133-909c-1616dbfa662f");
        public static  Guid DebugHelpContextToolWindowGuid = new Guid("66dba47c-61df-11d2-aa79-00c04f990343");
        public const int CommandId = 0x0100;

        public DebugHelpContextPackage() {}

        protected override void Initialize()
        {
            base.Initialize();

            OleMenuCommandService mcs = (OleMenuCommandService)GetService(typeof(IMenuCommandService));
            if (null != mcs)
            {
                CommandID menuCommandID = new CommandID(CommandSetGuid, (int)CommandId);
                MenuCommand menuItem = new MenuCommand(ShowDebugHelpContextToolWindow, menuCommandID);
                mcs.AddCommand(menuItem);
            }
        }

        private void ShowDebugHelpContextToolWindow(object sender, EventArgs e)
        {
            IVsUIShell vsUIShell = (IVsUIShell)GetService(typeof(SVsUIShell));
            int hr = vsUIShell.FindToolWindow((uint)__VSFINDTOOLWIN.FTW_fForceCreate, ref DebugHelpContextToolWindowGuid, out IVsWindowFrame vsWindowFrame);
            if (null != vsWindowFrame)
            {
                hr = vsWindowFrame.Show();
            }
        }
    }
}
