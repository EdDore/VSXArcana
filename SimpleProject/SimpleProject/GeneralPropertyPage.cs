using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Windows.Forms;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Project;

namespace VSXArcana.SimpleProject
{
    [ComVisible(true)]
    [Guid("67DBF3E5-11E3-40BA-B25A-F4C5EBBB4ED0")]
    class GeneralPropertyPage : SettingsPage
    {
        private string assemblyName;
        private OutputType outputType;
        private string defaultNamespace;
        private string startupObject;
        private string applicationIcon;
        private FrameworkName targetFrameworkMoniker;

        public GeneralPropertyPage()
        {
            this.Name = Resources.GetString(Resources.GeneralCaption);
        }

        [ResourcesCategoryAttribute(Resources.AssemblyName)]
        [LocDisplayName(Resources.AssemblyName)]
        [ResourcesDescriptionAttribute(Resources.AssemblyNameDescription)]
        public string AssemblyName
        {
            get { return this.assemblyName; }
            set { this.assemblyName = value; this.IsDirty = true; }
        }

        [ResourcesCategoryAttribute(Resources.Application)]
        [LocDisplayName(Resources.OutputType)]
        [ResourcesDescriptionAttribute(Resources.OutputTypeDescription)]
        public OutputType OutputType
        {
            get { return this.outputType; }
            set { this.outputType = value; this.IsDirty = true; }
        }

        [ResourcesCategoryAttribute(Resources.Application)]
        [LocDisplayName(Resources.DefaultNamespace)]
        [ResourcesDescriptionAttribute(Resources.DefaultNamespaceDescription)]
        public string DefaultNamespace
        {
            get { return this.defaultNamespace; }
            set { this.defaultNamespace = value; this.IsDirty = true; }
        }

        [ResourcesCategoryAttribute(Resources.Application)]
        [LocDisplayName(Resources.StartupObject)]
        [ResourcesDescriptionAttribute(Resources.StartupObjectDescription)]
        public string StartupObject
        {
            get { return this.startupObject; }
            set { this.startupObject = value; this.IsDirty = true; }
        }

        [ResourcesCategoryAttribute(Resources.Application)]
        [LocDisplayName(Resources.ApplicationIcon)]
        [ResourcesDescriptionAttribute(Resources.ApplicationIconDescription)]
        public string ApplicationIcon
        {
            get { return this.applicationIcon; }
            set { this.applicationIcon = value; this.IsDirty = true; }
        }

        [ResourcesCategoryAttribute(Resources.Project)]
        [LocDisplayName(Resources.ProjectFile)]
        [ResourcesDescriptionAttribute(Resources.ProjectFileDescription)]
        public string ProjectFile
        {
            get { return Path.GetFileName(this.ProjectMgr.ProjectFile); }
        }

        [ResourcesCategoryAttribute(Resources.Project)]
        [LocDisplayName(Resources.ProjectFolder)]
        [ResourcesDescriptionAttribute(Resources.ProjectFolderDescription)]
        public string ProjectFolder
        {
            get { return Path.GetDirectoryName(this.ProjectMgr.ProjectFolder); }
        }

        [ResourcesCategoryAttribute(Resources.Project)]
        [LocDisplayName(Resources.OutputFile)]
        [ResourcesDescriptionAttribute(Resources.OutputFileDescription)]
        public string OutputFile
        {
            get
            {
                switch (this.outputType)
                {
                    case OutputType.Exe:
                    case OutputType.WinExe:
                        {
                            return this.assemblyName + ".exe";
                        }

                    default:
                        {
                            return this.assemblyName + ".dll";
                        }
                }
            }
        }

        [ResourcesCategoryAttribute(Resources.Project)]
        [LocDisplayName(Resources.TargetFrameworkMoniker)]
        [ResourcesDescriptionAttribute(Resources.TargetFrameworkMonikerDescription)]
        [PropertyPageTypeConverter(typeof(FrameworkNameConverter))]
        public FrameworkName TargetFrameworkMoniker
        {
            get { return this.targetFrameworkMoniker; }
            set { this.targetFrameworkMoniker = value; IsDirty = true; }
        }

        public override string GetClassName()
        {
            return this.GetType().FullName;
        }

        protected override void BindProperties()
        {
            if (this.ProjectMgr == null)
            {
                return;
            }

            this.assemblyName = this.ProjectMgr.GetProjectProperty("AssemblyName", true);
            string outputType = this.ProjectMgr.GetProjectProperty("OutputType", false);
            if (outputType != null && outputType.Length > 0)
            {
                try
                {
                    this.outputType = (OutputType)Enum.Parse(typeof(OutputType), outputType);
                }
                catch (ArgumentException)
                {
                }
            }

            this.defaultNamespace = this.ProjectMgr.GetProjectProperty("RootNamespace", false);
            this.startupObject = this.ProjectMgr.GetProjectProperty("StartupObject", false);
            this.applicationIcon = this.ProjectMgr.GetProjectProperty("ApplicationIcon", false);

            try
            {
                this.targetFrameworkMoniker = this.ProjectMgr.TargetFrameworkMoniker;
            }
            catch (ArgumentException)
            {
            }
        }

        protected override int ApplyChanges()
        {
            if (this.ProjectMgr == null)
            {
                return VSConstants.E_INVALIDARG;
            }

            IVsPropertyPageFrame propertyPageFrame = (IVsPropertyPageFrame)this.ProjectMgr.Site.GetService((typeof(SVsPropertyPageFrame)));
            bool reloadRequired = this.ProjectMgr.TargetFrameworkMoniker != this.targetFrameworkMoniker;

            this.ProjectMgr.SetProjectProperty("AssemblyName", this.assemblyName);
            this.ProjectMgr.SetProjectProperty("OutputType", this.outputType.ToString());
            this.ProjectMgr.SetProjectProperty("RootNamespace", this.defaultNamespace);
            this.ProjectMgr.SetProjectProperty("StartupObject", this.startupObject);
            this.ProjectMgr.SetProjectProperty("ApplicationIcon", this.applicationIcon);

            if (reloadRequired)
            {
                if (MessageBox.Show(SR.GetString(SR.ReloadPromptOnTargetFxChanged), SR.GetString(SR.ReloadPromptOnTargetFxChangedCaption), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    this.ProjectMgr.TargetFrameworkMoniker = this.targetFrameworkMoniker;
                }
            }

            this.IsDirty = false;

            if (reloadRequired)
            {
                // This prevents the property page from displaying bad data from the zombied (unloaded) project
                propertyPageFrame.HideFrame();
                propertyPageFrame.ShowFrame(this.GetType().GUID);
            }

            return VSConstants.S_OK;
        }
    }
}
