using System;
using System.ComponentModel;

namespace VSXArcana.SimpleProject
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    internal sealed class LocDisplayNameAttribute : DisplayNameAttribute
    {
        private string name;

        public LocDisplayNameAttribute(string name)
        {
            this.name = name;
        }

        public override string DisplayName
        {
            get
            {
                string result = Resources.GetString(this.name);
                if (result == null)
                {
                    result = this.name;
                }
                return result;
            }
        }
    }
}
