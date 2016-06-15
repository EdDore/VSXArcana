using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Project;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

namespace VSXArcana.SimpleProject
{
    [Guid(GuidList.guidSimpleProjectFactoryString)]
    public class SimpleProjectFactory : ProjectFactory
    {
        private SimpleProjectPackage package;

        public SimpleProjectFactory(SimpleProjectPackage package) : base(package)
        {
            this.package = package;
        }

        protected override ProjectNode CreateProject()
        {
            SimpleProjectNode project = new SimpleProjectNode(this.package);
            project.SetSite((IOleServiceProvider)((IServiceProvider)this.package).GetService(typeof(IOleServiceProvider)));
            return project;
        }
    }
}
