using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SplitEditor
{
    [Guid("BD43DB55-7721-4727-B2FB-C5884C45E6D3")]
    internal class SplitViewEditorFactory : SplitEditorFactoryBase
    {
        internal SplitViewEditorFactory(SplitEditorPackage package) : base(package) { }

        protected override object CreateDocView(IServiceProvider serviceProvider, IVsHierarchy hierarchy, uint itemid, string fileName, object docData, out Guid cmdUIGuid)
        {
            return new SplitViewEditorPane(serviceProvider, hierarchy, itemid, fileName, docData, out cmdUIGuid);
        }

        public override int MapLogicalView(ref Guid rguidLogicalView, out string pbstrPhysicalView)
        {
            // This editor factory supports any view. So always return a null for the physical view here.
            pbstrPhysicalView = null;

            if (rguidLogicalView.Equals(VSConstants.LOGVIEWID_Any) ||
                rguidLogicalView.Equals(VSConstants.LOGVIEWID_Primary) ||
                rguidLogicalView.Equals(VSConstants.LOGVIEWID_Debugging) ||
                rguidLogicalView.Equals(VSConstants.LOGVIEWID_Code) ||
                rguidLogicalView.Equals(VSConstants.LOGVIEWID_Designer) ||
                rguidLogicalView.Equals(VSConstants.LOGVIEWID_TextView))
            {
                return VSConstants.S_OK;
            }

            return VSConstants.E_NOTIMPL;
        }
    }
}
