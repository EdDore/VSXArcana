//#define _WINFORM_BASED

using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Windows.Forms;

namespace SimpleEditor
{
    public class SimpleEditorPane : WindowPane, IVsPersistDocData
    {
        private uint docCookie;
        private uint itemId;
        private IVsHierarchy vsHierarchy;
        private string filePath;

#if _WINFORM_BASED
        private SimpleWinFormEditor editorControl;

        public SimpleEditorPane()
        {
            this.editorControl = new SimpleWinFormEditor(this);
        }

        public override IWin32Window Window
        {
            get { return this.editorControl; }
        }
#else
        public SimpleEditorPane()
        {
            this.Content = new SimpleWPFEditor(this);
        }
#endif

#region IVsPersistDocData

        public int GetGuidEditorType(out Guid pClassID)
        {
            pClassID = typeof(SimpleEditorPane).GUID;
            return VSConstants.S_OK;
        }

        public int IsDocDataDirty(out int pfDirty)
        {
            pfDirty = 0;
            return VSConstants.S_OK;
        }

        public int SetUntitledDocPath(string pszDocDataPath)
        {
            pszDocDataPath = "Untitled";
            return VSConstants.S_OK;
        }

        public int LoadDocData(string pszMkDocument)
        {
            this.filePath = pszMkDocument;

            // todo:
            return VSConstants.S_OK;
        }

        public int SaveDocData(VSSAVEFLAGS dwSave, out string pbstrMkDocumentNew, out int pfSaveCanceled)
        {
            // todo:
            pbstrMkDocumentNew = this.filePath;
            pfSaveCanceled = 0;
            return VSConstants.S_OK;
        }

        public int Close()
        {
            return VSConstants.S_OK;
        }

        public int OnRegisterDocData(uint docCookie, IVsHierarchy pHierNew, uint itemidNew)
        {
            this.docCookie = docCookie;
            this.vsHierarchy = pHierNew;
            this.itemId = itemidNew;

            return VSConstants.S_OK;
        }

        public int RenameDocData(uint grfAttribs, IVsHierarchy pHierNew, uint itemidNew, string pszMkDocumentNew)
        {
            throw new NotImplementedException();
        }

        public int IsDocDataReloadable(out int pfReloadable)
        {
            pfReloadable = 1;
            return VSConstants.S_OK;
        }

        public int ReloadDocData(uint grfFlags)
        {
            // todo:
            return VSConstants.S_OK;
        }

#endregion
    }
}
