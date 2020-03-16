using Microsoft;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextManager.Interop;
using System;
using System.Runtime.InteropServices;

namespace SplitEditor
{
    [Guid("B140A729-276F-45BD-99AB-CDF97B0C0B9D")]
    class SourceEditorFactory : SplitEditorFactoryBase
    {
        internal SourceEditorFactory(SplitEditorPackage package) : base(package) { }

        public override int MapLogicalView(ref Guid rguidLogicalView, out string pbstrPhysicalView)
        {
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

        protected override object CreateDocView(System.IServiceProvider serviceProvider, IVsHierarchy hierarchy, uint itemid, string fileName, object docData, out Guid cmdUIGuid)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            // Instead of creating a WindowPane based object, we'll create an instance of the VS Code Editor, to serve
            // as our source editor view.

            ILocalRegistry localRegistry = serviceProvider.GetService(typeof(SLocalRegistry)) as ILocalRegistry;
            Assumes.Present(localRegistry);

            cmdUIGuid = typeof(SourceEditorFactory).GUID;
            Guid guidCodeWindow = typeof(IVsCodeWindow).GUID;
            IntPtr pUnkPtr;
            IVsCodeWindow vsCodeWindow;

            int hr = localRegistry.CreateInstance(typeof(VsCodeWindowClass).GUID, null, ref guidCodeWindow, (uint)CLSCTX.CLSCTX_INPROC_SERVER, out pUnkPtr);
            ErrorHandler.ThrowOnFailure(hr);

            try
            {
                vsCodeWindow = (IVsCodeWindow)Marshal.GetObjectForIUnknown(pUnkPtr);
            }
            finally
            {
                Marshal.Release(pUnkPtr);
            }

            IVsTextLines vsTextLines = docData as IVsTextLines;
            if (vsTextLines == null)
            {
                IVsTextBufferProvider provider = docData as IVsTextBufferProvider;
                if (provider != null)
                {
                    ErrorHandler.ThrowOnFailure(provider.GetTextBuffer(out vsTextLines));
                }
            }

            if (vsTextLines != null)
            {
                ErrorHandler.ThrowOnFailure(vsCodeWindow.SetBuffer(vsTextLines));
            }

            return vsCodeWindow;
        }
    }
}
