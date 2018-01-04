using System.Collections.Generic;
using System.IO;
using EnvDTE;
using Microsoft.VisualStudio.TemplateWizard;
using Microsoft.VisualStudio.VCProjectEngine;

namespace SimpleCppDLLWizard
{
    public class Wizard : IWizard
    {
        private DTE _dte;

        public void BeforeOpeningFile(ProjectItem projectItem) { }

        public void ProjectFinishedGenerating(Project project)
        {
            // use VCProject.LatestTargetPlatformVersion property, which is what the stock wizards use.
            VCProject vcProject = (VCProject)project.Object;
            string wtpv = vcProject.LatestTargetPlatformVersion;

            if (wtpv != null)
            {
                // we only have to do this for a single config, as the property in question is global.
                IVCCollection configs = (IVCCollection)vcProject.Configurations;
                VCConfiguration firstConfig = (VCConfiguration)configs.Item(1);
                IVCRulePropertyStorage rule = (IVCRulePropertyStorage)firstConfig.Rules.Item("ConfigurationGeneral");
                rule?.SetPropertyValue("WindowsTargetPlatformVersion", wtpv);
            }
        }

        public void ProjectItemFinishedGenerating(ProjectItem projectItem) { }
        public void RunFinished() { }

        public void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary, WizardRunKind runKind, object[] customParams)
        {
            _dte = (DTE)automationObject;

            // manually copy over the SimpleDLL.vcxproj.filters file because it isn't
            // referenced by the .vcxproj, and the wizard engine doesn't copy it to
            // the project's destination directory.

            // note, customParams[0] is the full path to the .vstemplate file, so we'll
            // use it's folder to find the needed SimpleDLL.vcxproj.filters file.

            string sourceFile = Path.Combine(Path.GetDirectoryName((string)customParams[0]), "SimpleDll.vcxproj.filters");
            string destFile = Path.Combine(replacementsDictionary["$destinationdirectory$"], string.Format("{0}.vcxproj.filters", replacementsDictionary["$projectname$"]));
            File.Copy(sourceFile, destFile);
        }

        public bool ShouldAddProjectItem(string filePath) { return true; }
    }
}
