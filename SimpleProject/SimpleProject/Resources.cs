using System;
using System.Reflection;
using System.Globalization;
using System.Resources;
using System.Threading;

namespace VSXArcana.SimpleProject
{
    internal sealed class Resources
    {
		// Constants
        internal const string Application = "Application";
        internal const string ApplicationCaption = "ApplicationCaption";
        internal const string GeneralCaption = "GeneralCaption";
        internal const string AssemblyName = "AssemblyName";
        internal const string AssemblyNameDescription = "AssemblyNameDescription";
        internal const string OutputType = "OutputType";
        internal const string OutputTypeDescription = "OutputTypeDescription";
        internal const string DefaultNamespace = "DefaultNamespace";
        internal const string DefaultNamespaceDescription = "DefaultNamespaceDescription";
        internal const string StartupObject = "StartupObject";
        internal const string StartupObjectDescription = "StartupObjectDescription";
        internal const string ApplicationIcon = "ApplicationIcon";
        internal const string ApplicationIconDescription = "ApplicationIconDescription";
        internal const string Project = "Project";
        internal const string ProjectFile = "ProjectFile";
        internal const string ProjectFileDescription = "ProjectFileDescription";
        internal const string ProjectFolder = "ProjectFolder";
        internal const string ProjectFolderDescription = "ProjectFolderDescription";
        internal const string OutputFile = "OutputFile";
        internal const string OutputFileDescription = "OutputFileDescription";
        internal const string TargetFrameworkMoniker = "TargetFrameworkMoniker";
        internal const string TargetFrameworkMonikerDescription = "TargetFrameworkMonikerDescription";
        internal const string NestedProjectFileAssemblyFilter = "NestedProjectFileAssemblyFilter";
        //internal const string MsgFailedToLoadTemplateFile = "Failed to add template file to project";

        private static Resources loader;
        private ResourceManager resourceManager;
        private static Object internalSyncObjectInstance;

        internal Resources()
        {
            resourceManager = new System.Resources.ResourceManager("Microsoft.VisualStudio.Project.Samples.CustomProject.Resources",
                Assembly.GetExecutingAssembly());
        }

        private static Object InternalSyncObject
        {
            get
            {
                if (internalSyncObjectInstance == null)
                {
                    Object o = new Object();
                    Interlocked.CompareExchange(ref internalSyncObjectInstance, o, null);
                }
                return internalSyncObjectInstance;
            }
        }

        private static CultureInfo Culture
        {
            get { return null/*use ResourceManager default, CultureInfo.CurrentUICulture*/; }
        }

        public static ResourceManager ResourceManager
        {
            get
            {
                return GetLoader().resourceManager;
            }
        }

        public static string GetString(string name, params object[] args)
        {
            Resources resourcesInstance = GetLoader();
            if (resourcesInstance == null)
            {
                return null;
            }
        
            string res = resourcesInstance.resourceManager.GetString(name, Resources.Culture);

            if (args != null && args.Length > 0)
            {
                return String.Format(CultureInfo.CurrentCulture, res, args);
            }
            else
            {
                return res;
            }
        }

        public static string GetString(string name)
        {
            Resources resourcesInstance = GetLoader();
            if (resourcesInstance == null)
            {
                return null;
            }
            return resourcesInstance.resourceManager.GetString(name, Resources.Culture);
        }

        public static object GetObject(string name)
        {
            Resources resourcesInstance = GetLoader();

            if (resourcesInstance == null)
            {
                return null;
            }
            return resourcesInstance.resourceManager.GetObject(name, Resources.Culture);
        }

        private static Resources GetLoader()
        {
            if (loader == null)
            {
                lock (InternalSyncObject)
                {
                    if (loader == null)
                    {
                        loader = new Resources();
                    }
                }
            }
            return loader;
        }
    }
}
