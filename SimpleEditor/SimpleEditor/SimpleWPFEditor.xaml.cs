using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System.Windows;
using System.Windows.Controls;

namespace SimpleEditor
{
    /// <summary>
    /// Interaction logic for SimpleWPFEditor.xaml
    /// </summary>
    public partial class SimpleWPFEditor : UserControl
    {
        private SimpleEditorPane parentPane;

        public SimpleWPFEditor()
        {
            InitializeComponent();
        }

        public SimpleWPFEditor(SimpleEditorPane editorPane)
        {
            this.parentPane = editorPane;
            InitializeComponent();
        }

        private void OnGetAppName_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            string appName;
            IVsUIShell vsUIShell = (IVsUIShell)SimpleEditorPackage.GetGlobalService(typeof(SVsUIShell));
            vsUIShell.GetAppName(out appName);
            VsShellUtilities.ShowMessageBox(parentPane, appName, "WPF Based Editor", OLEMSGICON.OLEMSGICON_INFO, OLEMSGBUTTON.OLEMSGBUTTON_OK, OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
        }
    }
}
