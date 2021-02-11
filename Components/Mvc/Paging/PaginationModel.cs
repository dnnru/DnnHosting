#region Usings

using System.Collections.Generic;
using System.Web.Mvc.Ajax;

#endregion

namespace Italliance.Modules.DnnHosting.Components.Mvc.Paging
{
    public class PaginationModel
    {
        public PaginationModel()
        {
            PaginationLinks = new List<PaginationLink>();
            AjaxOptions = null;
            Options = null;
        }

        public int PageSize { get; internal set; }

        public int CurrentPage { get; internal set; }

        public int PageCount { get; internal set; }

        public int TotalItemCount { get; internal set; }

        public IList<PaginationLink> PaginationLinks { get; }

        public AjaxOptions AjaxOptions { get; internal set; }

        public PagerOptions Options { get; internal set; }
    }
}
