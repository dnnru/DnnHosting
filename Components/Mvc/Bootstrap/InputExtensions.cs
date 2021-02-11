#region Usings

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using DotNetNuke.Web.Mvc.Helpers;

#endregion

namespace Italliance.Modules.DnnHosting.Components.Mvc.Bootstrap
{
    public static class InputExtensions
    {
        private const string FORM_ACTION_CLASS = "form-actions";
        private const string INVERSE_BUTTON_CLASS = "btn-inverse";
        private const string DANGER_BUTTON_CLASS = "btn-danger";
        private const string WARNING_BUTTON_CLASS = "btn-warning";
        private const string SUCCESS_BUTTON_CLASS = "btn-success";
        private const string INFO_BUTTON_CLASS = "btn-info";
        private const string PRIMARY_BUTTON_CLASS = "btn-primary";
        private const string BUTTON_CLASS = "btn";
        private const string LABEL_CLASS = "control-label";
        private const string HELPER_CLASS = "help-block";
        private const string CONTROL_GROUP_CLASS = "form-group";
        private const string CONTROL_GROUP_ERROR_CLASS = "has-error";
        private const string FORM_CONTROL_CLASS = "form-control";

        public static MvcHtmlString BootstrapButton(this DnnHtmlHelper dnnHtmlHelper, string label, int type)
        {
            return MvcHtmlString.Create(CreateButton(label, type).ToString());
        }

        public static MvcHtmlString BootstrapButton(this DnnHtmlHelper dnnHtmlHelper, string label, int type, object htmlAttributes)
        {
            return CreateButton(label, type, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static MvcHtmlString ButtonControlGroup(this DnnHtmlHelper dnnHtmlHelper, string label, int type)
        {
            return BootstrapifyControlGroupButton(BootstrapButton(dnnHtmlHelper, label, type));
        }

        public static MvcHtmlString ButtonControlGroup(this DnnHtmlHelper dnnHtmlHelper, string label, int type, object htmlAttributes)
        {
            return BootstrapifyControlGroupButton(BootstrapButton(dnnHtmlHelper, label, type, htmlAttributes));
        }

        public static MvcHtmlString ButtonFormAction(this DnnHtmlHelper dnnHtmlHelper, string label, int type)
        {
            return BootstrapifyFormActionButton(BootstrapButton(dnnHtmlHelper, label, type));
        }

        public static MvcHtmlString ButtonFormAction(this DnnHtmlHelper dnnHtmlHelper, string label, int type, object htmlAttributes)
        {
            return BootstrapifyFormActionButton(BootstrapButton(dnnHtmlHelper, label, type, htmlAttributes));
        }

        public static MvcHtmlString TextBoxControlGroupFor<TModel, TProperty>(this DnnHtmlHelper<TModel> dnnHtmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            return dnnHtmlHelper.TextBoxControlGroupFor(expression, format: null);
        }

        public static MvcHtmlString TextBoxControlGroupFor<TModel, TProperty>(this DnnHtmlHelper<TModel> dnnHtmlHelper,
                                                                              Expression<Func<TModel, TProperty>> expression,
                                                                              string format)
        {
            return dnnHtmlHelper.TextBoxControlGroupFor(expression, format, null);
        }

        public static MvcHtmlString TextBoxControlGroupFor<TModel, TProperty>(this DnnHtmlHelper<TModel> dnnHtmlHelper,
                                                                              Expression<Func<TModel, TProperty>> expression,
                                                                              object htmlAttributes)
        {
            return dnnHtmlHelper.TextBoxControlGroupFor(expression, null, htmlAttributes);
        }

        public static MvcHtmlString TextBoxControlGroupFor<TModel, TProperty>(this DnnHtmlHelper<TModel> dnnHtmlHelper,
                                                                              Expression<Func<TModel, TProperty>> expression,
                                                                              string format,
                                                                              object htmlAttributes)
        {
            return dnnHtmlHelper.TextBoxControlGroupFor(expression, format, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static MvcHtmlString TextBoxControlGroupFor<TModel, TProperty>(this DnnHtmlHelper<TModel> dnnHtmlHelper,
                                                                              Expression<Func<TModel, TProperty>> expression,
                                                                              IDictionary<string, object> htmlAttributes)
        {
            return dnnHtmlHelper.TextBoxControlGroupFor(expression, null, htmlAttributes);
        }

        public static MvcHtmlString TextBoxControlGroupFor<TModel, TProperty>(this DnnHtmlHelper<TModel> dnnHtmlHelper,
                                                                              Expression<Func<TModel, TProperty>> expression,
                                                                              string format,
                                                                              IDictionary<string, object> htmlAttributes)
        {
            AddInputAttributes(ref htmlAttributes);
            var coreControl = dnnHtmlHelper.TextBoxFor(expression, htmlAttributes);
            var coreLabel = dnnHtmlHelper.LabelFor(expression, LabelHtmlAttributes());

            return Bootstrapify(coreControl, coreLabel);
        }

        private static IDictionary<string, object> LabelHtmlAttributes()
        {
            return new Dictionary<string, object> {{"class", LABEL_CLASS}};
        }

        public static MvcHtmlString PasswordControlGroupFor<TModel, TProperty>(this DnnHtmlHelper<TModel> dnnHtmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            return PasswordControlGroupFor(dnnHtmlHelper, expression, null);
        }

        public static MvcHtmlString PasswordControlGroupFor<TModel, TProperty>(this DnnHtmlHelper<TModel> dnnHtmlHelper,
                                                                               Expression<Func<TModel, TProperty>> expression,
                                                                               object htmlAttributes)
        {
            return PasswordControlGroupFor(dnnHtmlHelper, expression, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static MvcHtmlString PasswordControlGroupFor<TModel, TProperty>(this DnnHtmlHelper<TModel> dnnHtmlHelper,
                                                                               Expression<Func<TModel, TProperty>> expression,
                                                                               IDictionary<string, object> htmlAttributes)
        {
            AddInputAttributes(ref htmlAttributes);
            var coreControl = dnnHtmlHelper.PasswordFor(expression, htmlAttributes);
            var coreLabel = dnnHtmlHelper.LabelFor(expression, LabelHtmlAttributes());

            return Bootstrapify(coreControl, coreLabel);
        }

        public static MvcHtmlString TextAreaControlGroupFor<TModel, TProperty>(this DnnHtmlHelper<TModel> dnnHtmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            return TextAreaControlGroupFor(dnnHtmlHelper, expression, null);
        }

        public static MvcHtmlString TextAreaControlGroupFor<TModel, TProperty>(this DnnHtmlHelper<TModel> dnnHtmlHelper,
                                                                               Expression<Func<TModel, TProperty>> expression,
                                                                               object htmlAttributes)
        {
            return TextAreaControlGroupFor(dnnHtmlHelper, expression, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static MvcHtmlString TextAreaControlGroupFor<TModel, TProperty>(this DnnHtmlHelper<TModel> dnnHtmlHelper,
                                                                               Expression<Func<TModel, TProperty>> expression,
                                                                               IDictionary<string, object> htmlAttributes)
        {
            AddInputAttributes(ref htmlAttributes);
            var coreControl = dnnHtmlHelper.TextAreaFor(expression, htmlAttributes);
            var coreLabel = dnnHtmlHelper.LabelFor(expression, LabelHtmlAttributes());

            return Bootstrapify(coreControl, coreLabel);
        }

        public static MvcHtmlString TextAreaControlGroupFor<TModel, TProperty>(this DnnHtmlHelper<TModel> dnnHtmlHelper,
                                                                               Expression<Func<TModel, TProperty>> expression,
                                                                               int rows,
                                                                               int columns,
                                                                               object htmlAttributes)
        {
            return TextAreaControlGroupFor(dnnHtmlHelper, expression, rows, columns, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static MvcHtmlString TextAreaControlGroupFor<TModel, TProperty>(this DnnHtmlHelper<TModel> dnnHtmlHelper,
                                                                               Expression<Func<TModel, TProperty>> expression,
                                                                               int rows,
                                                                               int columns,
                                                                               IDictionary<string, object> htmlAttributes)
        {
            AddInputAttributes(ref htmlAttributes);
            var coreControl = dnnHtmlHelper.TextAreaFor(expression, rows, columns, htmlAttributes);
            var coreLabel = dnnHtmlHelper.LabelFor(expression, LabelHtmlAttributes());

            return Bootstrapify(coreControl, coreLabel);
        }

        public static MvcHtmlString DropDownListControlGroupFor<TModel, TProperty>(this DnnHtmlHelper<TModel> dnnHtmlHelper,
                                                                                   Expression<Func<TModel, TProperty>> expression,
                                                                                   IEnumerable<SelectListItem> selectList)
        {
            return DropDownListControlGroupFor(dnnHtmlHelper, expression, selectList, null /* optionLabel */, null /* htmlAttributes */);
        }

        public static MvcHtmlString DropDownListControlGroupFor<TModel, TProperty>(this DnnHtmlHelper<TModel> dnnHtmlHelper,
                                                                                   Expression<Func<TModel, TProperty>> expression,
                                                                                   IEnumerable<SelectListItem> selectList,
                                                                                   object htmlAttributes)
        {
            return DropDownListControlGroupFor(dnnHtmlHelper, expression, selectList, null /* optionLabel */, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static MvcHtmlString DropDownListControlGroupFor<TModel, TProperty>(this DnnHtmlHelper<TModel> dnnHtmlHelper,
                                                                                   Expression<Func<TModel, TProperty>> expression,
                                                                                   IEnumerable<SelectListItem> selectList,
                                                                                   IDictionary<string, object> htmlAttributes)
        {
            return DropDownListControlGroupFor(dnnHtmlHelper, expression, selectList, null /* optionLabel */, htmlAttributes);
        }

        public static MvcHtmlString DropDownListControlGroupFor<TModel, TProperty>(this DnnHtmlHelper<TModel> dnnHtmlHelper,
                                                                                   Expression<Func<TModel, TProperty>> expression,
                                                                                   IEnumerable<SelectListItem> selectList,
                                                                                   string optionLabel)
        {
            return DropDownListControlGroupFor(dnnHtmlHelper, expression, selectList, optionLabel, null /* htmlAttributes */);
        }

        public static MvcHtmlString DropDownListControlGroupFor<TModel, TProperty>(this DnnHtmlHelper<TModel> dnnHtmlHelper,
                                                                                   Expression<Func<TModel, TProperty>> expression,
                                                                                   IEnumerable<SelectListItem> selectList,
                                                                                   string optionLabel,
                                                                                   object htmlAttributes)
        {
            return DropDownListControlGroupFor(dnnHtmlHelper, expression, selectList, optionLabel, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static MvcHtmlString DropDownListControlGroupFor<TModel, TProperty>(this DnnHtmlHelper<TModel> dnnHtmlHelper,
                                                                                   Expression<Func<TModel, TProperty>> expression,
                                                                                   IEnumerable<SelectListItem> selectList,
                                                                                   string optionLabel,
                                                                                   IDictionary<string, object> htmlAttributes)
        {
            AddInputAttributes(ref htmlAttributes);
            var coreControl = dnnHtmlHelper.DropDownListFor(expression, selectList, optionLabel, htmlAttributes);
            var coreLabel = dnnHtmlHelper.LabelFor(expression, LabelHtmlAttributes());

            return Bootstrapify(coreControl, coreLabel);
        }

        private static MvcHtmlString Bootstrapify(IHtmlString coreControl, MvcHtmlString coreLabel)
        {
            var controlGroupDiv = ControlGroupDiv();
            var coreHtml = coreControl.ToHtmlString();
            var textBox = Bootstrapify(coreHtml);

            var label = coreLabel;

            var errorMessage = HandleErrors(textBox, controlGroupDiv);

            controlGroupDiv.InnerHtml = label.ToHtmlString() + coreControl.ToHtmlString() + errorMessage;

            return MvcHtmlString.Create(controlGroupDiv.ToString());
        }

        private static MvcHtmlString BootstrapifyControlGroupButton(MvcHtmlString bootstrapButton)
        {
            var group = ControlGroupDiv();
            group.InnerHtml = bootstrapButton.ToHtmlString();

            return MvcHtmlString.Create(group.ToString());
        }

        private static MvcHtmlString BootstrapifyFormActionButton(MvcHtmlString bootstrapButton)
        {
            var control = FormActionDiv();
            control.InnerHtml = bootstrapButton.ToHtmlString();

            return MvcHtmlString.Create(control.ToString());
        }

        private static BootstrapControl Bootstrapify(string html)
        {
            var cssClass = GetClass(html);
            var errorMessage = GetErrorMessage(html);
            var id = GetId(html);

            return new BootstrapControl {Id = id, ErrorMessage = errorMessage, Class = cssClass};
        }

        private static void AddInputAttributes(ref IDictionary<string, object> htmlAttributes)
        {
            if (htmlAttributes == null)
            {
                htmlAttributes = new Dictionary<string, object>();
            }

            string classes;
            if (htmlAttributes.ContainsKey("class"))
            {
                classes = (string) htmlAttributes["class"];
                classes += " " + FORM_CONTROL_CLASS;
                htmlAttributes.Remove("class");
            }
            else
            {
                classes = FORM_CONTROL_CLASS;
            }

            htmlAttributes.Add("class", classes);
        }

        private static string HandleErrors(BootstrapControl textBox, TagBuilder controlGroupDiv)
        {
            var errorMessage = "";

            if (!textBox.IsValid)
            {
                controlGroupDiv.AddCssClass(CONTROL_GROUP_ERROR_CLASS);
                if (textBox.ErrorMessage.Length > 0)
                {
                    var errorBox = new TagBuilder("span");
                    errorBox.AddCssClass(HELPER_CLASS);
                    errorBox.InnerHtml = textBox.ErrorMessage;
                    errorMessage = errorBox.ToString();
                }
            }

            return errorMessage;
        }

        private static TagBuilder ControlLabel(BootstrapControl control)
        {
            var label = new TagBuilder("label");
            label.AddCssClass(LABEL_CLASS);
            label.MergeAttribute("for", control.Id);
            label.InnerHtml = control.Id;

            return label;
        }

        private static TagBuilder ControlGroupDiv()
        {
            var controlGroupDiv = new TagBuilder("div");
            controlGroupDiv.AddCssClass(CONTROL_GROUP_CLASS);

            return controlGroupDiv;
        }

        private static TagBuilder FormActionDiv()
        {
            var formActionDiv = new TagBuilder("div");
            formActionDiv.AddCssClass(FORM_ACTION_CLASS);

            return formActionDiv;
        }

        private static string GetId(string html)
        {
            return GetAttribute("id", html);
        }

        private static string GetErrorMessage(string html)
        {
            return GetAttribute("data-val-required", html);
        }

        private static string GetClass(string html)
        {
            return GetAttribute("class", html);
        }

        private static string GetAttribute(string attribute, string html)
        {
            var value = "";
            var match = Regex.Match(html, attribute + @"=""([^""]*)""", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                value = match.Groups[1].Value;
            }

            return value;
        }

        private static TagBuilder CreateButton(string label, int type)
        {
            var button = new TagBuilder("button");
            button.AddCssClass(GetButtonClass(type));
            button.InnerHtml = label;
            return button;
        }

        private static string GetButtonClass(int type)
        {
            var cssClass = "";
            switch (type)
            {
                case Buttons.PRIMARY:
                    cssClass = PRIMARY_BUTTON_CLASS;
                    break;

                case Buttons.INFO:
                    cssClass = INFO_BUTTON_CLASS;
                    break;

                case Buttons.SUCCESS:
                    cssClass = SUCCESS_BUTTON_CLASS;
                    break;

                case Buttons.WARNING:
                    cssClass = WARNING_BUTTON_CLASS;
                    break;

                case Buttons.DANGER:
                    cssClass = DANGER_BUTTON_CLASS;
                    break;

                case Buttons.INVERSE:
                    cssClass = INVERSE_BUTTON_CLASS;
                    break;
            }

            if (cssClass.Length > 0)
            {
                cssClass = BUTTON_CLASS + " " + cssClass;
            }
            else
            {
                cssClass = BUTTON_CLASS;
            }

            return cssClass;
        }

        private static MvcHtmlString CreateButton(string label, int type, IDictionary<string, object> htmlAttributes)
        {
            var button = CreateButton(label, type);
            button.MergeAttributes(htmlAttributes);
            return MvcHtmlString.Create(button.ToString());
        }
    }
}
