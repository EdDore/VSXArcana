using System;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.Shell.TableControl;
using Microsoft.VisualStudio.Shell.TableManager;

namespace ErrorFilterPackage
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class ToggleErrorFilterCommand : IEntryFilter
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 0x0100;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("be4452eb-74a7-47ae-84bb-c541330a3864");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly Package package;


        /// <summary>
        /// boolean to track whether we want to filter or not
        /// </summary>
        private bool bFiltering = false;


        /// <summary>
        /// Initializes a new instance of the <see cref="ToggleErrorFilterCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        private ToggleErrorFilterCommand(Package package)
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
                var menuItem = new OleMenuCommand(ToggleErrorFilter, null, QueryToggleErrorFilter, menuCommandID);
                commandService.AddCommand(menuItem);
            }
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static ToggleErrorFilterCommand Instance
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
            Instance = new ToggleErrorFilterCommand(package);
        }

        /// <summary>
        /// This function is the callback used to update the command when the menu item is displayed.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and OleMenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void QueryToggleErrorFilter(object sender, EventArgs e)
        {
            OleMenuCommand menuCommand = (OleMenuCommand)sender;

            // set the menu text (tooltip) and latch/toggle the button to reflect whether we are filtering or not
            if (bFiltering)
            {
                menuCommand.Text = "Reenable Project Reference Warnings";
                menuCommand.Checked = true;
            }
            else
            {
                menuCommand.Text = "Disable Project Reference Warnings";
                menuCommand.Checked = false;
            }
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and OleMenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void ToggleErrorFilter(object sender, EventArgs e)
        {
            if(bFiltering)
            {
                // Turn off filtering
                IErrorList errorList = ServiceProvider.GetService(typeof(SVsErrorList)) as IErrorList;
                errorList.TableControl.SetFilter("MyFilter", null);
            }
            else
            {
                // Turn on filtering
                IErrorList errorList = ServiceProvider.GetService(typeof(SVsErrorList)) as IErrorList;
                errorList.TableControl.FiltersChanged += OnFiltersChanged;
                errorList.TableControl.SetFilter("MyFilter", this);
            }
        }

        // We need to account for removal of our filter via the "Clear All Filters" command, so we'll
        // use the OnFiltersChanged event to set our bFiltering flag, as well as unhook the event whenever
        // the filter isn't present.
        private void OnFiltersChanged(object sender, FiltersChangedEventArgs e)
        {
            if (e.NewFilter == this)
            {
                bFiltering = true;
            }
            else if (e.OldFilter == this)
            {
                bFiltering = false;
                IErrorList errorList = ServiceProvider.GetService(typeof(SVsErrorList)) as IErrorList;
                errorList.TableControl.FiltersChanged -= OnFiltersChanged;
            }

        }

        #region IEntryFilter

        // Custom IEntryFilter.Match method, invoked when this packages's filtering feature is enabled.
        // This method filters out any warnings with the description matching the regex query below.
        public bool Match(ITableEntryHandle entry)
        {
            if (entry==null)
            {
                throw new ArgumentNullException("entry is null");
            }

            string description;
            if (entry.TryGetValue(StandardTableKeyNames.Text, out description))
            {
                // if the description matches the unwanted warning, filter it out by returning false.
                Regex regex = new Regex("The project '.*' cannot be referenced.");
                Match m = regex.Match(description);
                if (m.Success)
                    return false;
            }

            return true;
        }

        #endregion
    }
}
