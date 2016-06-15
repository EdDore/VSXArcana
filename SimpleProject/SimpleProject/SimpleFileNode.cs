using System;
using Microsoft.VisualStudio.Project.Automation;
using Microsoft.VisualStudio.Project;

namespace VSXArcana.SimpleProject
{
    public class SimpleFileNode : FileNode
    {
        private OASimpleFileItem automationObject;

        internal SimpleFileNode(ProjectNode root, ProjectElement e) : base(root, e)
        {
        }

        public override object GetAutomationObject()
        {
            if(automationObject == null)
            {
                automationObject = new OASimpleFileItem(this.ProjectMgr.GetAutomationObject() as OAProject, this);
            }
            return automationObject;
        }

        internal OleServiceProvider.ServiceCreatorCallback ServiceCreator
        {
            get { return new OleServiceProvider.ServiceCreatorCallback(this.CreateServices); }
        }

        private object CreateServices(Type serviceType)
        {
            object service = null;
            if(typeof(EnvDTE.ProjectItem) == serviceType)
            {
                service = GetAutomationObject();
            }
            return service;
        }
    }
}
