using System;
using System.ComponentModel;

namespace VSXArcana.SimpleProject
{
    [AttributeUsage(AttributeTargets.All)]
    internal sealed class ResourcesDescriptionAttribute : DescriptionAttribute
    {
        private bool replaced;

        public ResourcesDescriptionAttribute(string description) : base(description) {}

        public override string Description
        {
            get
            {
                if (!replaced)
                {
                    replaced = true;
                    DescriptionValue = Resources.GetString(base.Description);
                }
                return base.Description;
            }
        }
    }
}
