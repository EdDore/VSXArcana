using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.TemplateWizard;

namespace VSXArcana.VSXWizards
{
    class VSXSimplePackageWizard : IWizard
    {
        private DTE2 dte;

        public void BeforeOpeningFile(ProjectItem projectItem)
        {
        }

        public void ProjectFinishedGenerating(Project project)
        {
        }

        public void ProjectItemFinishedGenerating(ProjectItem projectItem)
        {
        }

        public void RunFinished()
        {
        }

        public void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary, WizardRunKind runKind, object[] customParams)
        {
            dte = (DTE2)automationObject;

            //foreach (string key in replacementsDictionary.Keys)
            //{
            //    System.Diagnostics.Debug.Write(key + " : ");
            //    System.Diagnostics.Debug.WriteLine(replacementsDictionary[key]);
            //}

            string safeprojectname = replacementsDictionary["$safeprojectname$"];

            safeprojectname.IndexOf("Package", StringComparison.CurrentCultureIgnoreCase);

            if (safeprojectname.Contains("Package",,StringComparison.CurrentCultureIgnoreCase))
            {
                // remove Package from $safeprojectname$
                replacementsDictionary["$safeprojectname$"] = safeprojectname.Substring(0, safeprojectname.Length - 7);
            }

        }

        public bool ShouldAddProjectItem(string filePath)
        {
            return true;
        }
    }
}
