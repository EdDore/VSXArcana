using System;
using System.ComponentModel.Design;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.VisualStudio.Shell;
using Task = System.Threading.Tasks.Task;

namespace SimplePackage
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // Info on this package for Help/About
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(SimplePackage.SimplePackageGuidString)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    public sealed class SimplePackage : AsyncPackage
    {
        public const string SimplePackageGuidString = "7ea1a428-17be-4f01-83e4-fe5e5d382427";
        public static readonly Guid SimplePackageCommandSet = new Guid("eb7cb3a4-f89c-4791-9742-83c313992e39");
        public const int SimpleCommandId = 0x0100;

        public SimplePackage() { }

        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            // wait for UI thread
            await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            // initialize menu commands
            OleMenuCommandService commandService = await GetServiceAsync((typeof(IMenuCommandService))) as OleMenuCommandService;
            var menuCommandID = new CommandID(SimplePackageCommandSet, SimpleCommandId);
            var menuItem = new MenuCommand(this.OnSimpleCommand, menuCommandID);
            commandService.AddCommand(menuItem);
        }

        private void OnSimpleCommand(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Hello Simple Package!!!");
        }
    }
}

