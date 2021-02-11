#region Usings

using System;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;

#endregion

namespace Italliance.Modules.DnnHosting.Components.Mvc
{
    public class HtmlHelpers
    {
        public static MvcHtmlString Concat(params MvcHtmlString[] items)
        {
            var builder = new StringBuilder();
            foreach (var item in items.Where(i => i != null))
            {
                builder.Append(item.ToHtmlString());
            }

            return MvcHtmlString.Create(builder.ToString());
        }

        public static int GetMaxLength(ViewDataDictionary<object> viewData)
        {
            var additionalValues = viewData.ModelMetadata.AdditionalValues;
            return additionalValues.ContainsKey("StringLength") ? (int) additionalValues["StringLength"] : -1;
        }

        public static MvcHtmlString AnonymousObjectToHtmlString(object htmlAttributes)
        {
            var dictionary = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            return DictionaryToHtmlString(dictionary);
        }

        public static MvcHtmlString DictionaryToHtmlString(RouteValueDictionary dictionary)
        {
            return new MvcHtmlString(dictionary.Aggregate("", (current, item) => $"{current}{item.Key}=\"{item.Value}\" "));
        }

        public static double ToJavascriptDate(DateTime? dateTime)
        {
            return dateTime?.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds ?? -1;
        }

        public static object GetModelStateValue(ViewContext viewContext, string key, Type destinationType)
        {
            ModelState modelState;
            if (viewContext.ViewData.ModelState.TryGetValue(key, out modelState))
            {
                if (modelState.Value != null)
                {
                    return modelState.Value.ConvertTo(destinationType, null /* culture */);
                }
            }

            return null;
        }
    }
}
