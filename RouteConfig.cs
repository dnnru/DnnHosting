#region Usings

using DotNetNuke.Web.Mvc.Routing;

#endregion

namespace Italliance.Modules.DnnHosting
{
    public class RouteConfig : IMvcRouteMapper
    {
        public void RegisterRoutes(IMapRoute mapRouteManager)
        {
            mapRouteManager.MapRoute("DnnHosting", "DnnHosting", "{controller}/{action}", new[] {"Italliance.Modules.DnnHosting.Controllers"});
        }
    }
}
