using System;
using System.ComponentModel;

namespace VSXArcana.SimpleProject
{
    [AttributeUsage(AttributeTargets.All)]
    internal sealed class ResourcesCategoryAttribute : CategoryAttribute
    {
        public ResourcesCategoryAttribute(string categoryName) : base(categoryName) {}

        protected override string GetLocalizedString(string categoryName)
        {
            return Resources.GetString(categoryName);
        }
    }
}
