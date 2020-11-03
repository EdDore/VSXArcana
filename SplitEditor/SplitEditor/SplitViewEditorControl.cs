using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft;

namespace SplitEditor
{
    internal partial class SplitViewEditorControl : UserControl
    {
        private IServiceProvider serviceProvider;

        public SplitterPanel DesignerPanel { get; private set; }

        public SplitterPanel SourcePanel { get; private set; }

        public SplitViewEditorControl(IServiceProvider sp)
        {
            this.serviceProvider = sp;

            InitializeComponent();
            DesignerPanel = splitContainer.Panel1;
            SourcePanel = splitContainer.Panel2;
        }

        //protected override CreateParams CreateParams
        //{
        //    get
        //    {
        //        Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
        //        CreateParams createParams = base.CreateParams;

        //        // set the parent HWND to prevent WinForms from using a "parking window" as the parent.
        //        // if the parking window is system DPI aware and we are per - monitor aware, creation will fail.
        //        if (this.serviceProvider != null && createParams.Parent == IntPtr.Zero)
        //        {
        //            IVsWindowFrame frame = (IVsWindowFrame)this.serviceProvider.GetService(typeof(SVsWindowFrame));
        //            Assumes.Present(frame);
        //            EnvDTE.Window window = VsShellUtilities.GetWindowObject(frame);
        //            Assumes.Present(window);
        //            createParams.Parent = new IntPtr(window.HWnd);
        //        }
        //        return createParams;
        //    }
        //}

    }
}
