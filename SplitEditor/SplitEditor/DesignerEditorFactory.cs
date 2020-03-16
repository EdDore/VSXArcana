using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Runtime.InteropServices;

namespace SplitEditor
{
    [Guid("859FDCDF-1561-4085-B265-BCD747F5E691")]
    internal class DesignerEditorFactory : SplitEditorFactoryBase
    {
        internal DesignerEditorFactory(SplitEditorPackage package) : base(package) { }

        public override int MapLogicalView(ref Guid rguidLogicalView, out string pbstrPhysicalView)
        {
            pbstrPhysicalView = null;

            if (rguidLogicalView.Equals(VSConstants.LOGVIEWID_Any) ||
                rguidLogicalView.Equals(VSConstants.LOGVIEWID_Primary) ||
                rguidLogicalView.Equals(VSConstants.LOGVIEWID_Designer))
            {
                pbstrPhysicalView = null;
            }
            else
            {
                return VSConstants.E_NOTIMPL;
            }

            return VSConstants.S_OK;
        }

        protected override object CreateDocView(IServiceProvider serviceProvider, IVsHierarchy hierarchy, uint itemid, string fileName, object docData, out Guid cmdUIGuid)
        {
            return new DesignerPane(serviceProvider, hierarchy, itemid, fileName, docData, out cmdUIGuid);
        }
    }
}
