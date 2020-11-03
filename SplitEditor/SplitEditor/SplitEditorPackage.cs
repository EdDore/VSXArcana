using System;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Task = System.Threading.Tasks.Task;

namespace SplitEditor
{

    //-------------------------------------------------------------------------------------------------//
    [ProvideEditorFactory(typeof(SplitViewEditorFactory), 100)]
    [ProvideEditorExtension(typeof(SplitViewEditorFactory), ".split", 0x100)]
    [ProvideEditorLogicalView(typeof(SplitViewEditorFactory), VSConstants.LOGVIEWID.Designer_string)]
    [ProvideEditorLogicalView(typeof(SplitViewEditorFactory), VSConstants.LOGVIEWID.Code_string)]
    [ProvideEditorLogicalView(typeof(SplitViewEditorFactory), VSConstants.LOGVIEWID.TextView_string)]
    [ProvideEditorLogicalView(typeof(SplitViewEditorFactory), VSConstants.LOGVIEWID.Debugging_string)]
    // TODO: Do we need something like this???  [ProvideXmlEditorChooserDesignerView("SplitEditor Example", ".split", LogicalViewID.Designer, 0x5000)]
    //-------------------------------------------------------------------------------------------------//
    [ProvideEditorFactory(typeof(DesignerEditorFactory), 100)]
    //-------------------------------------------------------------------------------------------------//
    [ProvideEditorFactory(typeof(SourceEditorFactory), 100)]
    [ProvideEditorLogicalView(typeof(SourceEditorFactory), VSConstants.LOGVIEWID.Designer_string)]
    [ProvideEditorLogicalView(typeof(SourceEditorFactory), VSConstants.LOGVIEWID.Code_string)]
    [ProvideEditorLogicalView(typeof(SourceEditorFactory), VSConstants.LOGVIEWID.TextView_string)]
    [ProvideEditorLogicalView(typeof(SourceEditorFactory), VSConstants.LOGVIEWID.Debugging_string)]
    //-------------------------------------------------------------------------------------------------//
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [Guid(PackageGuids.guidSplitEditorPackageString)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    public sealed class SplitEditorPackage : AsyncPackage
    {

        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            // When initialized asynchronously, the current thread may be a background thread at this point.
            // Do any initialization that requires the UI thread after switching to the UI thread.
            await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            // TODO: Set up package level menu commands here (if we have any)
            OleMenuCommandService commandService = await this.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Assumes.Present(commandService);
            var menuCommandID = new CommandID(PackageGuids.guidSplitEditorPackageCmdSet, PackageIds.DoNothingCommand);
            var menuItem = new MenuCommand(this.OnDoNothing, menuCommandID);
            commandService.AddCommand(menuItem);

            this.RegisterEditorFactory(new SplitViewEditorFactory(this));
            this.RegisterEditorFactory(new DesignerEditorFactory(this));
            this.RegisterEditorFactory(new SourceEditorFactory(this));

        }

        private void OnDoNothing(object sender, EventArgs e)
        {
            // todo:
        }

    }
}
