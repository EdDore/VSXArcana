using Microsoft;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextManager.Interop;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

using IObjectWithSite = Microsoft.VisualStudio.OLE.Interop.IObjectWithSite;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

namespace SplitEditor
{
    public abstract class SplitEditorFactoryBase : IVsEditorFactory
    {
        protected ServiceProvider serviceProvider;
        private IOleServiceProvider site;
        private SplitEditorPackage package;

        protected SplitEditorFactoryBase(SplitEditorPackage package)
        {
            this.package = package;
        }

        // derived classes will implement this to create the actual editor/designer views.
        protected abstract object CreateDocView(System.IServiceProvider serviceProvider, IVsHierarchy hierarchy, uint itemid, string fileName, object docData, out Guid cmdUIGuid);

        #region IVsEditorFactory

        public int CreateEditorInstance(uint vsCreateEditorFlags, string fileName, string physicalView,
                                         IVsHierarchy hierarchy, uint itemid, IntPtr existingDocData,
                                         out IntPtr docView, out IntPtr docData,
                                         out string caption, out Guid cmdUIGuid, out int flags)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            object docViewObject = null;
            docView = IntPtr.Zero;
            docData = IntPtr.Zero;
            caption = null;
            cmdUIGuid = Guid.Empty;
            flags = 0;

            if ((vsCreateEditorFlags & (VSConstants.CEF_OPENFILE | VSConstants.CEF_SILENT)) == 0)
            {
                return VSConstants.E_INVALIDARG;
            }

            try
            {
                object docDataObject;

                // if needed create our DocData object
                if (existingDocData == IntPtr.Zero)
                {
                    ILocalRegistry localRegistry = serviceProvider.GetService(typeof(SLocalRegistry)) as ILocalRegistry;
                    Assumes.Present(localRegistry);

                    Guid guidUnknown = VSConstants.IID_IUnknown;
                    IntPtr newDocDataPtr;
                    ErrorHandler.ThrowOnFailure(localRegistry.CreateInstance(typeof(VsTextBufferClass).GUID, null, ref guidUnknown, (uint)CLSCTX.CLSCTX_INPROC_SERVER, out newDocDataPtr));

                    try
                    {
                        docDataObject = Marshal.GetObjectForIUnknown(newDocDataPtr);
                    }
                    finally
                    {
                        Marshal.Release(newDocDataPtr);
                    }

                    IObjectWithSite ows = docDataObject as IObjectWithSite;
                    if (ows != null)
                    {
                        ows.SetSite(this.site);
                    }
                }
                else
                {
                    // if document is already open
                    docDataObject = Marshal.GetObjectForIUnknown(existingDocData);
                    IVsTextLines textLines = docDataObject as IVsTextLines;
                    Marshal.Release(existingDocData);

                    if (textLines == null)
                    {
                        return VSConstants.VS_E_INCOMPATIBLEDOCDATA;
                    }
                }

                docViewObject = this.CreateDocView(this.serviceProvider, hierarchy, itemid, fileName, docDataObject, out cmdUIGuid);
                Debug.Assert(docViewObject != null);

                // todo: maybe check to see if we need to add readonly to caption?
                caption = "";

                docView = Marshal.GetIUnknownForObject(docViewObject);
                docData = Marshal.GetIUnknownForObject(docDataObject);
                docViewObject = null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            finally
            {
                // if we have a disposable view here, clean it up
                IDisposable disposableView;
                if ((disposableView = docViewObject as IDisposable) != null)
                {
                    disposableView.Dispose();
                }
            }

            return VSConstants.S_OK;
        }

        public int SetSite(IOleServiceProvider psp)
        {
            this.site = psp;
            this.serviceProvider = new ServiceProvider(this.site, false);
            return VSConstants.S_OK;
        }

        public int Close()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (this.serviceProvider != null)
            {
                this.serviceProvider.Dispose();
                this.serviceProvider = null;
            }
            this.site = null;
            return VSConstants.S_OK; ;
        }

        public abstract int MapLogicalView(ref Guid rguidLogicalView, out string pbstrPhysicalView);

        #endregion


    }
}
