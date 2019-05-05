using Microsoft.VisualStudio.Shell;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using Task = System.Threading.Tasks.Task;

namespace SimpleEditor
{
    [ProvideEditorFactory(typeof(SimpleEditorFactory), 1000)]
    [ProvideEditorExtension(typeof(SimpleEditorFactory), ".simple", 1000)]
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [Guid(SimpleEditorPackage.PackageGuidString)]
    public sealed class SimpleEditorPackage : AsyncPackage
    {
        public const string PackageGuidString = "11660be0-377f-489e-9af7-da034eccb6af";
        public SimpleEditorPackage() { }
        
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await base.InitializeAsync(cancellationToken, progress);
            this.RegisterEditorFactory(new SimpleEditorFactory());
        }
    }
}
