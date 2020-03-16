using System;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;


namespace SplitEditor
{
    internal class DesignerPane : WindowPane
    {
        private IVsHierarchy vsHierarchy;
        private uint itemid;
        private string fileName;
        private object docData;

        internal DesignerPane(IServiceProvider serviceProvider, IVsHierarchy hierarchy, uint itemid, string filename, object docData, out Guid cmdUIGuid) : base(serviceProvider)
        {
            this.vsHierarchy = hierarchy;
            this.itemid = itemid;
            this.fileName = filename;
            this.docData = docData;
            cmdUIGuid = typeof(DesignerEditorFactory).GUID;
        }

        protected override void Initialize()
        {
            base.Initialize();
            this.Content = new DesignerControl();
        }

        protected override void OnClose()
        {
            base.OnClose();
        }

    }
}
