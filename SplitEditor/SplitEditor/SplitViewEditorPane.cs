﻿using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using IWin32Window = System.Windows.Forms.IWin32Window;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;
using Microsoft.VisualStudio.ProjectSystem.VS;
using System.Diagnostics;
using System.Drawing;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Package;

namespace SplitEditor
{
    class SplitViewEditorPane : WindowPane, IVsMultiViewDocumentView, IVsDeferredDocView, IVsCodeWindow
    {
        private IVsHierarchy vsHierarchy;
        private uint itemid;
        private string fileName;
        private object docData;

        private SplitViewEditorControl splitViewEditorControl;
        private IVsWindowFrame designerFrame = null;
        private IVsWindowFrame sourceFrame = null;

        internal SplitViewEditorPane(IServiceProvider serviceProvider, IVsHierarchy hierarchy, uint itemid, string filename, object docData, out Guid cmdUIGuid) : base(serviceProvider)
        {
            this.vsHierarchy = hierarchy;
            this.itemid = itemid;
            this.fileName = filename;
            this.docData = docData;
            cmdUIGuid = typeof(SplitViewEditorFactory).GUID;
        }

        protected override void Initialize()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            base.Initialize();
        }
        
        public override IWin32Window Window
        {
            get { return this.splitViewEditorControl; }
        }

        protected override void OnCreate()
        {
            base.OnCreate();
            this.splitViewEditorControl = new SplitViewEditorControl(this);

            designerFrame = CreateChildEditorFrame(typeof(DesignerEditorFactory).GUID, splitViewEditorControl.DesignerPanel);
            //sourceFrame = CreateChildEditorFrame(typeof(SourceEditorFactory).GUID, splitViewEditorControl.SourcePanel);

            // TODO: use XML Editor guid instead of our custom SourceEditorFactory ?
            Guid guidXMLEditor = new Guid("FA3CD31E-987B-443A-9B81-186104E8DAC1");
            sourceFrame = CreateChildEditorFrame(guidXMLEditor, splitViewEditorControl.SourcePanel);

            this.splitViewEditorControl.DesignerPanel.SizeChanged += DesignerPanel_SizeChanged;
            this.splitViewEditorControl.SourcePanel.SizeChanged += SourcePanel_SizeChanged;
        }
        protected override bool PreProcessMessage(ref Message m)
        {
            // TODO: May need to forward messages to active designer or source pane here.
            return base.PreProcessMessage(ref m);
        }

        protected override void OnClose()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            // BUG!!!: we should get a prompt to save if dirty, even though
            // we close the child frames out with NoSave, but the RDT isn't finding the 
            // prompt to save along with save if dirty flags for some reason...
            sourceFrame.CloseFrame((uint) __FRAMECLOSE.FRAMECLOSE_NoSave);
            designerFrame.CloseFrame((uint)__FRAMECLOSE.FRAMECLOSE_NoSave);
            
            base.OnClose();
        }

        private void DesignerPanel_SizeChanged(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (designerFrame != null)
            {
                Guid guidEmpty = Guid.Empty;
                Rectangle rcBounds = ((SplitterPanel)sender).Bounds;
                designerFrame.SetFramePos(VSSETFRAMEPOS.SFP_fSize | VSSETFRAMEPOS.SFP_fMove, ref guidEmpty, 0, 0, rcBounds.Width, rcBounds.Height);
            }
        }

        private void SourcePanel_SizeChanged(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (sourceFrame != null)
            {
                Guid guidEmpty = Guid.Empty;
                Rectangle rcBounds = ((SplitterPanel)sender).Bounds;
                sourceFrame.SetFramePos(VSSETFRAMEPOS.SFP_fSize | VSSETFRAMEPOS.SFP_fMove, ref guidEmpty, 0, 0, rcBounds.Width, rcBounds.Height);
            }
        }
                
        private IVsWindowFrame CreateChildEditorFrame(Guid guidEditorFactory, SplitterPanel panel)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            IVsUIShellOpenDocument vsUISHOD = GetService(typeof(SVsUIShellOpenDocument)) as IVsUIShellOpenDocument;

            IVsWindowFrame childFrame = null;
            IVsUIHierarchy vsUIHier;
            uint itemid;
            int fOpen;
            Guid guidLogEditorView = Guid.Empty;
            IOleServiceProvider oleServiceProvider;

            int hr = vsUISHOD.IsSpecificDocumentViewOpen((IVsUIHierarchy)this.vsHierarchy, this.itemid, this.fileName, ref guidEditorFactory, null, 0, out vsUIHier, out itemid, out childFrame, out fOpen);
            if (ErrorHandler.Succeeded(hr) && fOpen != 0)
            {
                // if already open, create a copy
                hr = vsUISHOD.OpenCopyOfStandardEditor(childFrame, ref guidLogEditorView, out childFrame);
            }
            else
            {
                hr = vsUISHOD.OpenDocumentViaProjectWithSpecific(fileName,
                    (uint)__VSSPECIFICEDITORFLAGS.VSSPECIFICEDITOR_UseEditor | (uint)__VSSPECIFICEDITORFLAGS.VSSPECIFICEDITOR_DoOpen,
                    ref guidEditorFactory, null, ref guidLogEditorView, out oleServiceProvider, out vsUIHier, out itemid, out childFrame);
            }

            if (ErrorHandler.Failed(hr))
            {
                Debug.Fail("Failed to create/open editor!!!");
                Marshal.ThrowExceptionForHR(hr);
            }

            IVsWindowFrame parentFrame = GetService(typeof(SVsWindowFrame)) as IVsWindowFrame;
            if (parentFrame == null)
            {
                throw new System.InvalidOperationException("could not retrieve the parent Window Frame");
            }

            childFrame.SetProperty((int)__VSFPROPID2.VSFPROPID_ParentFrame, parentFrame);
            childFrame.SetProperty((int)__VSFPROPID.VSFPROPID_ViewHelper, this);
            childFrame.SetProperty((int)__VSFPROPID3.VSFPROPID_NotifyOnActivate, true);
            childFrame.SetProperty((int)__VSFPROPID5.VSFPROPID_DontAutoOpen, true);
            childFrame.SetProperty((int)__VSFPROPID2.VSFPROPID_ParentHwnd, panel.Handle);

            Guid cmdUITextEditorGuid = VSConstants.GUID_TextEditorFactory;
            childFrame.SetGuidProperty((int)__VSFPROPID.VSFPROPID_InheritKeyBindings, ref cmdUITextEditorGuid);

            return childFrame;
        }

        private IVsCodeWindow _vsCodeWindow = null;
        private IVsCodeWindow SourceCodeWindow
        {
            get
            {
                ThreadHelper.ThrowIfNotOnUIThread();
                if (_vsCodeWindow == null)
                {
                    if (sourceFrame != null)
                    {
                        object docView;
                        sourceFrame.GetProperty((int)__VSFPROPID.VSFPROPID_DocView, out docView);
                        _vsCodeWindow = docView as IVsCodeWindow;
                    }
                }
                return _vsCodeWindow;
            }
        }
       
        #region IVsMulitiViewDocumentView

        public int ActivateLogicalView(ref Guid rguidLogicalView)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (rguidLogicalView.Equals(VSConstants.LOGVIEWID_Any) || rguidLogicalView.Equals(VSConstants.LOGVIEWID_Primary))
            {
                // keep whatever's active, active
                return VSConstants.S_OK;
            }
            else
            {
                // TODO: Activate either the code or designer frame.
                if (this.sourceFrame!=null)
                    this.sourceFrame.Show();

                return VSConstants.S_OK;
            }
        }

        public int GetActiveLogicalView(out Guid pguidLogicalView)
        {
            // always return Guid.Empty to prevent Toolbox changing
            pguidLogicalView = Guid.Empty;
            return VSConstants.S_OK;
        }

        public int IsLogicalViewActive(ref Guid rguidLogicalView, out int pIsActive)
        {
            // TODO: Check which pane is active returning pIsActive accordingly
            pIsActive = 1;
            return VSConstants.S_OK;
        }


        #endregion

        #region IVsDeferredDocView

        public int get_DocView(out IntPtr ppUnkDocView)
        {
            ppUnkDocView = Marshal.GetIUnknownForObject(this);
            return VSConstants.S_OK;
        }

        public int get_CmdUIGuid(out Guid pGuidCmdId)
        {
            pGuidCmdId = typeof(SplitViewEditorFactory).GUID;
            return VSConstants.S_OK;
        }

        #endregion

        #region IVsCodeWindow

        public int SetBuffer(IVsTextLines pBuffer)
        {
            IVsCodeWindow vsCodeWindow = SourceCodeWindow;
            if (vsCodeWindow != null)
            {
                return vsCodeWindow.SetBuffer(pBuffer);
            }
            return VSConstants.E_NOTIMPL;
        }

        public int GetBuffer(out IVsTextLines ppBuffer)
        {
            IVsCodeWindow vsCodeWindow = SourceCodeWindow;
            if (vsCodeWindow != null)
            {
                return vsCodeWindow.GetBuffer(out ppBuffer);
            }
            ppBuffer = null;
            return VSConstants.E_NOTIMPL;
        }

        public int GetPrimaryView(out IVsTextView ppView)
        {
            IVsCodeWindow vsCodeWindow = SourceCodeWindow;
            if (vsCodeWindow != null)
            {
                return vsCodeWindow.GetPrimaryView(out ppView);
            }
            ppView = null;
            return VSConstants.E_NOTIMPL;
        }

        public int GetSecondaryView(out IVsTextView ppView)
        {
            IVsCodeWindow vsCodeWindow = SourceCodeWindow;
            if (vsCodeWindow != null)
            {
                return vsCodeWindow.GetSecondaryView(out ppView);
            }
            ppView = null;
            return VSConstants.E_NOTIMPL;
        }

        public int SetViewClassID(ref Guid clsidView)
        {
            IVsCodeWindow vsCodeWindow = SourceCodeWindow;
            if (vsCodeWindow != null)
            {
                return vsCodeWindow.SetViewClassID(ref clsidView);
            }
            return VSConstants.E_NOTIMPL;
        }

        public int GetViewClassID(out Guid pclsidView)
        {
            IVsCodeWindow vsCodeWindow = SourceCodeWindow;
            if (vsCodeWindow != null)
            {
                return vsCodeWindow.GetViewClassID(out pclsidView);
            }
            pclsidView = Guid.Empty;
            return VSConstants.E_NOTIMPL;
        }

        public int SetBaseEditorCaption(string[] pszBaseEditorCaption)
        {
            IVsCodeWindow vsCodeWindow = SourceCodeWindow;
            if (vsCodeWindow != null)
            {
                return vsCodeWindow.SetBaseEditorCaption(pszBaseEditorCaption);
            }
            return VSConstants.E_NOTIMPL;
        }

        public int GetEditorCaption(READONLYSTATUS dwReadOnly, out string pbstrEditorCaption)
        {
            IVsCodeWindow vsCodeWindow = SourceCodeWindow;
            if (vsCodeWindow != null)
            {
                return vsCodeWindow.GetEditorCaption(dwReadOnly, out pbstrEditorCaption);
            }
            pbstrEditorCaption = null;
            return VSConstants.E_NOTIMPL;
        }

        public int Close()
        {
            IVsCodeWindow vsCodeWindow = SourceCodeWindow;
            if (vsCodeWindow != null)
            {
                return vsCodeWindow.Close();
            }
            return VSConstants.E_NOTIMPL;
        }

        public int GetLastActiveView(out IVsTextView ppView)
        {
            IVsCodeWindow vsCodeWindow = SourceCodeWindow;
            if (vsCodeWindow != null)
            {
                return vsCodeWindow.GetLastActiveView(out ppView);
            }
            ppView = null;
            return VSConstants.E_NOTIMPL;
        }

        #endregion

    }
}