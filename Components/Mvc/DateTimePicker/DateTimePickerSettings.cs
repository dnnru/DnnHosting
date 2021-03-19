#region

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

#endregion

namespace Italliance.Modules.DnnHosting.Components.Mvc.DateTimePicker
{
    public class DateTimePickerSettings
    {
        public enum PickerViewCategory
        {
            Day = 0,
            Month = 1,
            Year = 2,
            Decade = 3,
            Millenium = 4
        }

        public const string ICON_SPAN_CSS = "input-group-addon";
        public const string REMOVE_ICON = "glyphicon glyphicon-remove";
        public const string DATA_TIME_TEXT_BOX_CSS = "form-control background-white pointer ";

        public const string DATE_TIME_PICKER_LINK_FORMAT = "dd.mm.yyyy";
        public const string DATE_TIME_FORMAT = "dd.MM.yyyy";

        public const string DATE_TIME_PICKER_CSS_FORMAT = "pointer input-group date {0}";

        public const string PICKER_VIEW_SETTING_TEMPLATE = "startView: {0}, minViewMode: {1}, maxViewMode: {2}";

        public static IDictionary<DateTimeCategory, string> AddIconsDictionary = new Dictionary<DateTimeCategory, string>
                                                                                 {
                                                                                     {DateTimeCategory.Date, "glyphicon glyphicon-calendar"},
                                                                                     {DateTimeCategory.Month, "glyphicon glyphicon-th"},
                                                                                     {DateTimeCategory.Year, "glyphicon glyphicon-th-list"}
                                                                                 };

        public static IDictionary<DateTimeCategory, string> DateTimePickerFormatsDictionary = new Dictionary<DateTimeCategory, string>
                                                                                              {
                                                                                                  {DateTimeCategory.Date, "dd.mm.yyyy"},
                                                                                                  {DateTimeCategory.Month, "mm.yyyy"},
                                                                                                  {DateTimeCategory.Year, "yyyy"}
                                                                                              };

        public static IDictionary<DateTimeCategory, string> DateTimeFormatsDictionary = new Dictionary<DateTimeCategory, string>
                                                                                        {
                                                                                            {DateTimeCategory.Date, "dd.MM.yyyy"},
                                                                                            {DateTimeCategory.Month, "MM.yyyy"},
                                                                                            {DateTimeCategory.Year, "yyyy"}
                                                                                        };

        public static IDictionary<DateTimeCategory, string> CssClassDictionary = new Dictionary<DateTimeCategory, string>
                                                                                 {
                                                                                     {DateTimeCategory.Date, "form_date"},
                                                                                     {DateTimeCategory.Month, "form_month"},
                                                                                     {DateTimeCategory.Year, "form_year"}
                                                                                 };

        public static IDictionary<DateTimeCategory, string> PickerViewSettingsDictionary = new Dictionary<DateTimeCategory, string>
                                                                                           {
                                                                                               {
                                                                                                   DateTimeCategory.Date,
                                                                                                   string.Format(CultureInfo.InvariantCulture,
                                                                                                                 PICKER_VIEW_SETTING_TEMPLATE,
                                                                                                                 (int) PickerViewCategory.Day,
                                                                                                                 (int) PickerViewCategory.Day,
                                                                                                                 (int) PickerViewCategory.Year)
                                                                                               },
                                                                                               {
                                                                                                   DateTimeCategory.Month,
                                                                                                   string.Format(CultureInfo.InvariantCulture,
                                                                                                                 PICKER_VIEW_SETTING_TEMPLATE,
                                                                                                                 (int) PickerViewCategory.Month,
                                                                                                                 (int) PickerViewCategory.Month,
                                                                                                                 (int) PickerViewCategory.Millenium)
                                                                                               },
                                                                                               {
                                                                                                   DateTimeCategory.Year,
                                                                                                   string.Format(CultureInfo.InvariantCulture,
                                                                                                                 PICKER_VIEW_SETTING_TEMPLATE,
                                                                                                                 (int) PickerViewCategory.Year,
                                                                                                                 (int) PickerViewCategory.Year,
                                                                                                                 (int) PickerViewCategory.Millenium)
                                                                                               }
                                                                                           };

        public DateTimePickerSettings() : this(null, null, false)
        { }

        public DateTimePickerSettings(DateTime? minDate, DateTime? maxDate, bool selectOnly) : this(minDate, maxDate, true, selectOnly)
        { }

        public DateTimePickerSettings(DateTime? minDate, DateTime? maxDate, bool clearBtn, bool selectOnly)
        {
            MinDateTime = minDate;
            MaxDateTime = maxDate;
            ClearBtn = clearBtn;
            SelectOnly = selectOnly;
        }

        public DateTime? MinDateTime { get; set; }

        public DateTime? MaxDateTime { get; set; }

        public bool ClearBtn { get; set; }

        public bool SelectOnly { get; set; }

        public IDictionary<string, object> GetDateTextBoxAttributes(IDictionary<string, object> htmlAttributes)
        {
            var textHtmlAttributes = new Dictionary<string, object>();
            if (SelectOnly)
            {
                textHtmlAttributes.Add("readonly", "readonly");
            }

            var cssClass = DATA_TIME_TEXT_BOX_CSS;
            if (htmlAttributes != null && htmlAttributes.ContainsKey("class"))
            {
                cssClass = DATA_TIME_TEXT_BOX_CSS + htmlAttributes["class"];
            }

            textHtmlAttributes.Add("class", cssClass);

            return textHtmlAttributes;
        }

        public string GetDateTimePickerSetting(DateTimeCategory category, string datePickerIdentity)
        {
            var viewSetting = PickerViewSettingsDictionary[category];
            var commonSetting = string.Format(CultureInfo.CurrentCulture,
                                              "language: 'ru', forceParse: true, autoclose: true, clearBtn: {0}, todayHighlight: {1}, todayBtn: {1}, format: '{2}' ",
                                              ClearBtn.ToString().ToLowerInvariant(),
                                              category == DateTimeCategory.Date ? "true" : "false",
                                              DateTimePickerFormatsDictionary[category]);
            var dateLimitSetting = DateLimitSetting();

            return string.Format(CultureInfo.CurrentCulture, "{0},{1}{2}", viewSetting, commonSetting, dateLimitSetting);
        }

        private string DateLimitSetting()
        {
            var builder = new StringBuilder();
            if (MinDateTime != null)
            {
                builder.AppendFormat(", startDate: '{0}'", MinDateTime.Value.ToString(DATE_TIME_FORMAT, CultureInfo.InvariantCulture));
            }

            if (MaxDateTime != null)
            {
                builder.AppendFormat(", endDate: '{0}'", MaxDateTime.Value.ToString(DATE_TIME_FORMAT, CultureInfo.InvariantCulture));
            }

            return builder.ToString();
        }

        public static string GetDateTimePickerCssClass(DateTimeCategory category)
        {
            return string.Format(CultureInfo.CurrentCulture, DATE_TIME_PICKER_CSS_FORMAT, CssClassDictionary[category]);
        }

        public static object ConvertValueFormat(object value, DateTimeCategory category)
        {
            if (value != null)
            {
                var strVal = value as string;
                if (!string.IsNullOrWhiteSpace(strVal))
                {
                    value = DateTime.Parse(strVal);
                }

                var dateVal = value as DateTime?;
                if (dateVal != null)
                {
                    value = dateVal.Value.ToString(DateTimeFormatsDictionary[category]);
                }
            }

            return value;
        }
    }
}