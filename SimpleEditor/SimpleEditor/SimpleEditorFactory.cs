using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Runtime.InteropServices;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;


namespace SimpleEditor
{
    [Guid("68105032-854E-4926-9E17-0BFB0CDD4E68")]
    class SimpleEditorFactory : IVsEditorFactory
    {
        private IOleServiceProvider oleServiceProvider;

        #region IVSEditorFactory

        public int CreateEditorInstance(uint grfCreateDoc, string pszMkDocument, string pszPhysicalView, IVsHierarchy pvHier, uint itemid, IntPtr punkDocDataExisting, out IntPtr ppunkDocView, out IntPtr ppunkDocData, out string pbstrEditorCaption, out Guid pguidCmdUI, out int pgrfCDW)
        {
            ppunkDocData = IntPtr.Zero;
            ppunkDocView = IntPtr.Zero;
            pguidCmdUI = typeof(SimpleEditorFactory).GUID;
            pbstrEditorCaption = null;
            pgrfCDW = 0;

            // Validate Inputs
            if ((grfCreateDoc & (VSConstants.CEF_OPENFILE | VSConstants.CEF_SILENT)) == 0)
            {
                return VSConstants.E_INVALIDARG;
            }

            if (punkDocDataExisting != IntPtr.Zero)
            {
                return VSConstants.VS_E_INCOMPATIBLEDOCDATA;
            }

            SimpleEditorPane docView = new SimpleEditorPane();
            ppunkDocView = Marshal.GetIUnknownForObject(docView);
            ppunkDocData = Marshal.GetIUnknownForObject(docView);
            return VSConstants.S_OK;
        }

        public int SetSite(IOleServiceProvider psp)
        {
            oleServiceProvider = psp;
            return VSConstants.S_OK;
        }

        public int Close()
        {
            return VSConstants.S_OK;
        }

        public int MapLogicalView(ref Guid rguidLogicalView, out string pbstrPhysicalView)
        {
            // we only support the one view
            pbstrPhysicalView = null;
            return VSConstants.S_OK;
        }

        #endregion
    }
}
