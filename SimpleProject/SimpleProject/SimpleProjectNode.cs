using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using EnvDTE;
using VSLangProj;

using Microsoft.VisualStudio.Project.Automation;
using Microsoft.VisualStudio.Project;

namespace VSXArcana.SimpleProject
{
    [Guid(GuidList.guidSimpleProjectNodeString)]
    public class SimpleProjectNode : ProjectNode
    {
        internal enum MyCustomProjectImageName
        {
            Project = 0,
        }

        internal const string ProjectTypeName = "MyCustomProject";
        internal static int imageOffset;        
        private SimpleProjectPackage package;
        private static ImageList imageList;
        private VSLangProj.VSProject vsProject;

        static SimpleProjectNode()
        {
            imageList = Utilities.GetImageList(typeof(SimpleProjectNode).Assembly.GetManifestResourceStream("Microsoft.VisualStudio.Project.Samples.CustomProject.Resources.MyCustomProjectImageList.bmp"));
        }

        public SimpleProjectNode(SimpleProjectPackage package)
        {
            this.package = package;
            InitializeImageList();
            this.CanProjectDeleteItems = true;
        }

        public static ImageList ImageList
        {
            get
            {
                return imageList;
            }
            set
            {
                imageList = value;
            }
        }

        protected internal VSLangProj.VSProject VSProject
        {
            get
            {
                if(vsProject == null)
                {
                    vsProject = new OAVSProject(this);
                }
                return vsProject;
            }
        }

        public override Guid ProjectGuid
        {
            get { return typeof(SimpleProjectFactory).GUID; }
        }

        public override string ProjectType
        {
            get { return ProjectTypeName; }
        }

        public override int ImageIndex
        {
            get
            {
                return imageOffset + (int)MyCustomProjectImageName.Project;
            }
        }

        public override object GetAutomationObject()
        {
            return new OASimpleProject(this);
        }

        /// <summary>
        /// Creates the file node.
        /// </summary>
        /// <param name="item">The project element item.</param>
        /// <returns></returns>
        public override FileNode CreateFileNode(ProjectElement item)
        {
            SimpleFileNode node = new SimpleFileNode(this, item);
            node.OleServiceProvider.AddService(typeof(EnvDTE.Project), new OleServiceProvider.ServiceCreatorCallback(this.CreateServices), false);
            node.OleServiceProvider.AddService(typeof(ProjectItem), node.ServiceCreator, false);
            node.OleServiceProvider.AddService(typeof(VSProject), new OleServiceProvider.ServiceCreatorCallback(this.CreateServices), false);
            return node;
        }

        protected override Guid[] GetConfigurationIndependentPropertyPages()
        {
            Guid[] result = new Guid[1];
            result[0] = typeof(GeneralPropertyPage).GUID;
            return result;
        }

        protected override Guid[] GetPriorityProjectDesignerPages()
        {
            Guid[] result = new Guid[1];
            result[0] = typeof(GeneralPropertyPage).GUID;
            return result;
        }

        public override void AddFileFromTemplate(string source, string target)
        {
            if(!File.Exists(source))
            {
                throw new FileNotFoundException(string.Format("Template file not found: {0}", source));
            }

            // The class name is based on the new file name
            string className = Path.GetFileNameWithoutExtension(target);
            string namespce = this.FileTemplateProcessor.GetFileNamespace(target, this);

            this.FileTemplateProcessor.AddReplace("%className%", className);
            this.FileTemplateProcessor.AddReplace("%namespace%", namespce);
            try
            {
                this.FileTemplateProcessor.UntokenFile(source, target);
                this.FileTemplateProcessor.Reset();
            }
            catch(Exception e)
            {
                throw new FileLoadException("Failed to add template file to project", target, e);
            }
        }

        private void InitializeImageList()
        {
            imageOffset = this.ImageHandler.ImageList.Images.Count;
            foreach(Image img in ImageList.Images)
            {
                this.ImageHandler.AddImage(img);
            }
        }

        private object CreateServices(Type serviceType)
        {
            object service = null;
            if(typeof(VSLangProj.VSProject) == serviceType)
            {
                service = this.VSProject;
            }
            else if(typeof(EnvDTE.Project) == serviceType)
            {
                service = this.GetAutomationObject();
            }
            return service;
        }
    }
}
