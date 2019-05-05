using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Windows.Forms;

namespace SimpleEditor
{
    public partial class SimpleWinFormEditor : UserControl
    {
        private SimpleEditorPane parentPane;
        public SimpleWinFormEditor(SimpleEditorPane editorPane)
        {
            this.parentPane = editorPane;
            InitializeComponent();
        }

        private void OnGetAppName_Click(object sender, EventArgs e)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            string appName;
            IVsUIShell vsUIShell = (IVsUIShell)SimpleEditorPackage.GetGlobalService(typeof(SVsUIShell));
            vsUIShell.GetAppName(out appName);
            VsShellUtilities.ShowMessageBox(parentPane, appName, "Winform Based Editor", OLEMSGICON.OLEMSGICON_INFO, OLEMSGBUTTON.OLEMSGBUTTON_OK, OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
        }
    }
}
