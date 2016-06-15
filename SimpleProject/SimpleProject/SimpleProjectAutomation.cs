using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Project;
using Microsoft.VisualStudio.Project.Automation;

namespace VSXArcana.SimpleProject
{
    [ComVisible(true)]
    [Guid("32FD7C8A-D246-4BA3-AB30-AE1892024FAB")]
    public class OASimpleProject : OAProject
    {
        public OASimpleProject(SimpleProjectNode project) : base(project) {}
    }

    [ComVisible(true)]
    [Guid("94FECE3D-D571-4FA6-9A3D-5361AA8653EC")]
    public class OASimpleFileItem : OAFileItem
    {
        public OASimpleFileItem(OAProject project, FileNode node) : base(project, node) {}
    }
}
