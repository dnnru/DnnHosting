#region Usings

using System.Collections;
using System.Web.Routing;

#endregion

namespace Italliance.Modules.DnnHosting.Components.Mvc.Paging
{
    public static class RouteValueDictionaryExtensions
    {
        /// <summary>
        ///     Fix RouteValueDictionaries that contain arrays.
        ///     Source: http://stackoverflow.com/a/5208050/691965
        /// </summary>
        /// <param name="routes"></param>
        /// <returns></returns>
        public static RouteValueDictionary FixListRouteDataValues(this RouteValueDictionary routes)
        {
            var newRv = new RouteValueDictionary();
            foreach (var key in routes.Keys)
            {
                var value = routes[key];
                if (value is IEnumerable && !(value is string))
                {
                    var index = 0;
                    foreach (var val in (IEnumerable) value)
                    {
                        newRv.Add($"{key}[{index}]", val);
                        index++;
                    }
                }
                else
                {
                    newRv.Add(key, value);
                }
            }

            return newRv;
        }
    }
}
