#region Usings

using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using DotNetNuke.Web.Mvc.Helpers;

#endregion

namespace Italliance.Modules.DnnHosting.Components.Mvc
{
    public static class Extensions
    {
        internal static HtmlHelper HtmlHelper(this DnnHtmlHelper htmlHelper)
        {
            PropertyInfo prop = typeof(DnnHtmlHelper).GetProperty("HtmlHelper", BindingFlags.NonPublic | BindingFlags.Instance);

            MethodInfo getter = prop?.GetGetMethod(true);
            return getter?.Invoke(htmlHelper, null) as HtmlHelper;
        }

        public static MvcHtmlString ReadOnlyFor<TModel, TValue>(this DnnHtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression)
        {
            return html.EditorFor(expression,
                                  new
                                  {
                                      @readonly = "readonly"
                                  });
        }

        /* Nl2br */
        public static MvcHtmlString Nl2Br(this DnnHtmlHelper helper, string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return new MvcHtmlString(text);
            }

            // Replace \r\n with \n, then \n with <br/>
            var html = helper.Encode(text);
            html = Regex.Replace(html, Environment.NewLine, "\n");
            html = Regex.Replace(html, "\n", "<br/>");
            return new MvcHtmlString(html);
        }

        public static MvcHtmlString ValidationErrorFor<TModel, TProperty>(this DnnHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, string error)
        {
            return HasError(htmlHelper, ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData), ExpressionHelper.GetExpressionText(expression))
                       ? new MvcHtmlString(error)
                       : null;
        }

        private static bool HasError(this DnnHtmlHelper htmlHelper, ModelMetadata modelMetadata, string expression)
        {
            string modelName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(expression);
            FormContext formContext = htmlHelper.ViewContext.FormContext;
            if (formContext == null)
            {
                return false;
            }

            if (!htmlHelper.ViewData.ModelState.ContainsKey(modelName))
            {
                return false;
            }

            ModelState modelState = htmlHelper.ViewData.ModelState[modelName];

            ModelErrorCollection modelErrors = modelState?.Errors;
            if (modelErrors == null)
            {
                return false;
            }

            return modelErrors.Count > 0;
        }
    }
}
