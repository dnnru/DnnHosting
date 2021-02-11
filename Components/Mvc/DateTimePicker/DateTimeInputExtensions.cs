#region Usings

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web.Mvc;
using DotNetNuke.Web.Mvc.Helpers;

#endregion

namespace Italliance.Modules.DnnHosting.Components.Mvc.DateTimePicker
{
    public static class DateTimeInputExtensions
    {
        public static MvcHtmlString DateTimePickerFor<TModel, TProperty>(this DnnHtmlHelper<TModel> htmlHelper,
                                                                         Expression<Func<TModel, TProperty>> expression,
                                                                         DateTimeCategory category)
        {
            return DateTimePickerFor(htmlHelper, expression, category, null, null);
        }

        public static MvcHtmlString DateTimePickerFor<TModel, TProperty>(this DnnHtmlHelper<TModel> htmlHelper,
                                                                         Expression<Func<TModel, TProperty>> expression,
                                                                         DateTimeCategory category,
                                                                         object htmlAttributes)
        {
            return DateTimePickerFor(htmlHelper, expression, category, null, htmlAttributes);
        }

        public static MvcHtmlString DateTimePickerFor<TModel, TProperty>(this DnnHtmlHelper<TModel> htmlHelper,
                                                                         Expression<Func<TModel, TProperty>> expression,
                                                                         DateTimeCategory category,
                                                                         DateTimePickerSettings customize)
        {
            return DateTimePickerFor(htmlHelper, expression, category, customize, null);
        }

        public static MvcHtmlString DateTimePickerFor<TModel, TProperty>(this DnnHtmlHelper<TModel> htmlHelper,
                                                                         Expression<Func<TModel, TProperty>> expression,
                                                                         DateTimeCategory category,
                                                                         DateTimePickerSettings customize,
                                                                         object htmlAttributes)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            var metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);

            return DateTimePickerHelper(htmlHelper,
                                        metadata,
                                        ExpressionHelper.GetExpressionText(expression),
                                        category,
                                        customize,
                                        HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static MvcHtmlString DateTimePicker(this DnnHtmlHelper htmlHelper, string name, DateTimeCategory category)
        {
            return DateTimePicker(htmlHelper, name, category, null, null);
        }

        public static MvcHtmlString DateTimePicker(this DnnHtmlHelper htmlHelper, string name, DateTimeCategory category, object htmlAttributes)
        {
            return DateTimePicker(htmlHelper, name, category, null, htmlAttributes);
        }

        public static MvcHtmlString DateTimePicker(this DnnHtmlHelper htmlHelper, string name, DateTimeCategory category, DateTimePickerSettings customize)
        {
            return DateTimePicker(htmlHelper, name, category, customize, null);
        }

        public static MvcHtmlString DateTimePicker(this DnnHtmlHelper htmlHelper, string name, DateTimeCategory category, DateTimePickerSettings customize, object htmlAttributes)
        {
            return DateTimePickerHelper(htmlHelper, null, name, category, customize, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        private static MvcHtmlString DateTimePickerHelper(DnnHtmlHelper htmlHelper,
                                                          ModelMetadata metadata,
                                                          string name,
                                                          DateTimeCategory category,
                                                          DateTimePickerSettings customize,
                                                          IDictionary<string, object> htmlAttributes)
        {
            string fullName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name);
            if (string.IsNullOrEmpty(fullName))
            {
                throw new ArgumentNullException(nameof(name));
            }

            var value = metadata == null ? HtmlHelpers.GetModelStateValue(htmlHelper.ViewContext, fullName, typeof(object)) : metadata.Model;

            var datePickerIdentity = $"div_{GetId()}";
            var divTag = GenerateDateTimePickerDiv(htmlHelper, datePickerIdentity, name, value, category, customize ?? new DateTimePickerSettings(), htmlAttributes);

            return new MvcHtmlString(divTag);
        }

        private static string GetId()
        {
            return Guid.NewGuid().ToString().Replace("-", "").Substring(0, 7);
        }

        private static string GenerateDateTimePickerDiv(DnnHtmlHelper htmlHelper,
                                                        string datePickerIdentity,
                                                        string name,
                                                        object value,
                                                        DateTimeCategory category,
                                                        DateTimePickerSettings customize,
                                                        IDictionary<string, object> htmlAttributes)
        {
            var divTag = new TagBuilder("div");
            divTag.MergeAttribute("id", datePickerIdentity);
            divTag.MergeAttribute("data-provide", "datepicker");
            divTag.AddCssClass(DateTimePickerSettings.GetDateTimePickerCssClass(category));

            var dateTextBox = htmlHelper.TextBox(name, DateTimePickerSettings.ConvertValueFormat(value, category), customize.GetDateTextBoxAttributes(htmlAttributes));
            var groupSpan = new TagBuilder("span");
            groupSpan.AddCssClass(DateTimePickerSettings.ICON_SPAN_CSS);
            var addSpan = new TagBuilder("span");
            addSpan.AddCssClass(DateTimePickerSettings.AddIconsDictionary[category]);
            groupSpan.InnerHtml = addSpan.ToString(TagRenderMode.Normal);

            var script = GenerateDateTimePickerScript(datePickerIdentity, category, customize);

            divTag.InnerHtml = dateTextBox.ToHtmlString() + groupSpan.ToString(TagRenderMode.Normal) + script;

            return divTag.ToString(TagRenderMode.Normal);
        }

        private static string GenerateDateTimePickerScript(string datePickerIdentity, DateTimeCategory category, DateTimePickerSettings settings)
        {
            var script = new TagBuilder("script");
            script.MergeAttribute("type", "text/javascript");

            string[] idArr = datePickerIdentity.Split(new[] {'_'}, StringSplitOptions.RemoveEmptyEntries);
            string id = idArr.Length == 2 ? idArr[1] : GetId();

            var setting = settings.GetDateTimePickerSetting(category, datePickerIdentity);
            script.InnerHtml =
                string.Format("var datetimePickerInit{2} = function (){{$('#{0}').datepicker({{ {1} }});$('#{0}').on('remove', function(){{$('#{0}_picker').remove();}});}};if($('#{0}').datetimepicker){{datetimePickerInit{2}();}}else{{$(function(){{datetimePickerInit{2}();}});}}",
                              datePickerIdentity,
                              setting,
                              id);

            return script.ToString(TagRenderMode.Normal);
        }
    }
}
