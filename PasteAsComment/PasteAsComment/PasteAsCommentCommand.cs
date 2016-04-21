using System;
using System.ComponentModel.Design;
using System.Text;
using System.Windows;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.ComponentModelHost;
using EnvDTE;
using EnvDTE80;


namespace PasteAsComment
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class PasteAsCommentCommand
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 0x0100;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("66e242c8-9ffd-4bff-860a-946ccc1b1f97");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly Package package;

        private DTE2 _dte = null;
        DTE2 GetDTE()
        {
            if (_dte == null)
            {
                _dte = Package.GetGlobalService(typeof(SDTE)) as DTE2;
            }
            return _dte;
        }

        private IVsEditorAdaptersFactoryService _adapterFactoryService = null;
        IVsEditorAdaptersFactoryService GetAdapterFactoryService()
        {
            if (_adapterFactoryService == null)
            {
                IComponentModel componentModel = (IComponentModel)Package.GetGlobalService(typeof(SComponentModel));
                _adapterFactoryService = componentModel.GetService<IVsEditorAdaptersFactoryService>();
            }
            return _adapterFactoryService;
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="PasteAsCommentCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        private PasteAsCommentCommand(Package package)
        {
            if (package == null)
            {
                throw new ArgumentNullException("package");
            }

            this.package = package;

            OleMenuCommandService commandService = this.ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null)
            {
                var menuCommandID = new CommandID(CommandSet, CommandId);
                var menuItem = new OleMenuCommand(this.MenuItemCallback, menuCommandID);
                menuItem.BeforeQueryStatus += MenuItem_BeforeQueryStatus;
                commandService.AddCommand(menuItem);
            }
        }

        private void MenuItem_BeforeQueryStatus(object sender, EventArgs e)
        {
            OleMenuCommand pasteCommand = (OleMenuCommand)sender;

            // hidden by default
            pasteCommand.Visible = false;
            pasteCommand.Enabled = false;

            Document activeDoc = GetDTE().ActiveDocument;
            if (activeDoc != null && activeDoc.ProjectItem != null && activeDoc.ProjectItem.ContainingProject != null)
            {
                string lang = activeDoc.Language;
                if (activeDoc.Language.Equals("CSharp"))
                {
                    // show command if active document is a csharp file.
                    pasteCommand.Visible = true;
                }

                // enable commmand if command is visible and there is text on the clipboard
                pasteCommand.Enabled = (pasteCommand.Visible) ? Clipboard.ContainsText() : false;
            }
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static PasteAsCommentCommand Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private IServiceProvider ServiceProvider
        {
            get
            {
                return this.package;
            }
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static void Initialize(Package package)
        {
            Instance = new PasteAsCommentCommand(package);
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void MenuItemCallback(object sender, EventArgs e)
        {
            string clipboardText = Clipboard.GetText();
            if (clipboardText.Length == 0)
                return;

            // parse the clipboard text
            StringBuilder sb = new StringBuilder();
            var separators = new string[] {"\r\n","\n"};
            var lines = clipboardText.Split(separators, StringSplitOptions.None);

            // format each line into a comment
            foreach (string line in lines)
                sb.AppendFormat("// {0}\r\n", line);

            DTE2 dte = GetDTE();
            try
            {
                dte.UndoContext.Open("Paste Text as Comment");

                // insert at current cursor position
                var selection = (TextSelection)dte.ActiveDocument.Selection;
                if (selection != null)
                {
                    selection.Insert(sb.ToString());
                    dte.ActiveDocument.Activate();
                    dte.ExecuteCommand("Edit.FormatDocument");
                }
            }
            finally
            {
                dte.UndoContext.Close();
            }
        }
    }
}
