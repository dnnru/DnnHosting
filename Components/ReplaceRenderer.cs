#region Usings

using System.Globalization;
using System.Reflection;
using System.Threading.Tasks;

#endregion

namespace Italliance.Modules.DnnHosting.Components
{
    public class ReplaceRenderer
    {
        public static string Parse<T>(string template, T model, bool isHtml = true)
        {
            foreach (PropertyInfo pi in model.GetType().GetRuntimeProperties())
            {
                template = template.Replace($"##{pi.Name}##", string.Format(new CultureInfo("ru-RU"), "{0}", pi.GetValue(model, null)));
            }

            return template;
        }

        public static Task<string> ParseAsync<T>(string template, T model, bool isHtml = true)
        {
            return Task.FromResult(Parse(template, model, isHtml));
        }
    }
}
