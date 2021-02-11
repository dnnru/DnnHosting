#region Usings

using System;
using System.Drawing;

#endregion

namespace Italliance.Modules.DnnHosting.Components.Mvc.Excel
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ExcelSheetStyleAttribute : Attribute
    {
        public ExcelSheetStyleAttribute()
        {
            HeaderFontSize = 14;
            BodyFontSize = 12;
            FontFamily = "Times New Roman";
            IsHeaderBold = true;
            HeaderBackgroundColor = Color.CadetBlue;
        }

        public float HeaderFontSize { get; set; }

        public float BodyFontSize { get; set; }

        public string FontFamily { get; set; }

        public bool IsHeaderBold { get; set; }

        public Color HeaderBackgroundColor { get; set; }
    }
}
