#region Usings

using System;

#endregion

namespace Italliance.Modules.DnnHosting.Components.Mvc.Excel
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class ExcelDisplayNameAttribute : Attribute
    {
        public ExcelDisplayNameAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}
