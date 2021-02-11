#region Usings

using System;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using DotNetNuke.Web.Mvc.Helpers;

#endregion

namespace Italliance.Modules.DnnHosting.Components.Mvc.Paging
{
    public class Pager : IHtmlString
    {
        private readonly int _currentPage;
        private readonly DnnHtmlHelper _htmlHelper;
        private readonly DnnUrlHelper _urlHelper;
        private readonly int _pageSize;
        protected readonly PagerOptions PagerOptions;
        private int _totalItemCount;

        public Pager(DnnHtmlHelper htmlHelper, int pageSize, int currentPage, int totalItemCount)
        {
            _htmlHelper = htmlHelper;
            _pageSize = pageSize;
            _currentPage = currentPage;
            _totalItemCount = totalItemCount;
            PagerOptions = new PagerOptions();
            _urlHelper = new DnnUrlHelper(_htmlHelper.ViewContext, _htmlHelper.RouteCollection);
        }

        public virtual string ToHtmlString()
        {
            var model = BuildPaginationModel(GeneratePageUrl);

            if (!string.IsNullOrEmpty(PagerOptions.DisplayTemplate))
            {
                string templatePath = PagerOptions.DisplayTemplate.StartsWith("~") ? PagerOptions.DisplayTemplate : $"DisplayTemplates/{PagerOptions.DisplayTemplate}";

                return _htmlHelper.Partial(templatePath, model).ToHtmlString();
            }

            var sb = new StringBuilder();

            foreach (var paginationLink in model.PaginationLinks)
            {
                if (paginationLink.Active)
                {
                    if (paginationLink.IsCurrent)
                    {
                        sb.AppendFormat("<span class=\"current\">{0}</span>", paginationLink.DisplayText);
                    }
                    else if (!paginationLink.PageIndex.HasValue)
                    {
                        sb.AppendFormat(paginationLink.DisplayText);
                    }
                    else
                    {
                        var linkBuilder = new StringBuilder("<a");

                        if (PagerOptions.AjaxOptions != null)
                        {
                            foreach (var ajaxOption in PagerOptions.AjaxOptions.ToUnobtrusiveHtmlAttributes())
                            {
                                linkBuilder.AppendFormat(" {0}=\"{1}\"", ajaxOption.Key, ajaxOption.Value);
                            }
                        }

                        linkBuilder.AppendFormat(" href=\"{0}\" title=\"{1}\">{2}</a>", paginationLink.Url, paginationLink.DisplayTitle, paginationLink.DisplayText);

                        sb.Append(linkBuilder);
                    }
                }
                else
                {
                    sb.AppendFormat(!paginationLink.IsSpacer ? "<span class=\"disabled\">{0}</span>" : "<span class=\"spacer\">{0}</span>", paginationLink.DisplayText);
                }
            }

            return sb.ToString();
        }

        public Pager Options(Action<PagerOptionsBuilder> buildOptions)
        {
            buildOptions(new PagerOptionsBuilder(PagerOptions));
            return this;
        }

        public virtual PaginationModel BuildPaginationModel(Func<int, string> generateUrl)
        {
            int pageCount;
            if (PagerOptions.UseItemCountAsPageCount)
            {
                // Set page count directly from total item count instead of calculating. Then calculate totalItemCount based on pageCount and pageSize;
                pageCount = _totalItemCount;
                _totalItemCount = pageCount * _pageSize;
            }
            else
            {
                pageCount = (int) Math.Ceiling(_totalItemCount / (double) _pageSize);
            }

            var model = new PaginationModel {PageSize = _pageSize, CurrentPage = _currentPage, TotalItemCount = _totalItemCount, PageCount = pageCount};

            // First page
            if (PagerOptions.DisplayFirstPage)
            {
                model.PaginationLinks.Add(new PaginationLink
                                          {
                                              Active = _currentPage > 1, DisplayText = PagerOptions.FirstPageText,
                                              DisplayTitle = PagerOptions.FirstPageTitle,
                                              PageIndex = 1, Url = generateUrl(1)
                                          });
            }

            // Previous page
            if (!PagerOptions.HidePreviousAndNextPage)
            {
                var previousPageText = PagerOptions.PreviousPageText;
                model.PaginationLinks.Add(_currentPage > 1
                                              ? new PaginationLink
                                                {
                                                    Active = true, DisplayText = previousPageText, DisplayTitle = PagerOptions.PreviousPageTitle,
                                                    PageIndex = _currentPage - 1,
                                                    Url = generateUrl(_currentPage - 1)
                                                }
                                              : new PaginationLink {Active = false, DisplayText = previousPageText});
            }

            var start = 1;
            var end = pageCount;
            var nrOfPagesToDisplay = PagerOptions.MaxNrOfPages;

            if (pageCount > nrOfPagesToDisplay)
            {
                var middle = (int) Math.Ceiling(nrOfPagesToDisplay / 2d) - 1;
                var below = _currentPage - middle;
                var above = _currentPage + middle;

                if (below < 2)
                {
                    above = nrOfPagesToDisplay;
                    below = 1;
                }
                else if (above > pageCount - 2)
                {
                    above = pageCount;
                    below = pageCount - nrOfPagesToDisplay + 1;
                }

                start = below;
                end = above;
            }

            if (!PagerOptions.HideFirstPageNumber && start > 1)
            {
                model.PaginationLinks.Add(new PaginationLink {Active = true, PageIndex = 1, DisplayText = "1", Url = generateUrl(1)});
                if (start > 3)
                {
                    model.PaginationLinks.Add(new PaginationLink {Active = true, PageIndex = 2, DisplayText = "2", Url = generateUrl(2)});
                }

                if (start > 2)
                {
                    model.PaginationLinks.Add(new PaginationLink {Active = false, DisplayText = "...", IsSpacer = true});
                }
            }

            for (var i = start; i <= end; i++)
            {
                if (i == _currentPage || _currentPage <= 0 && i == 1)
                {
                    model.PaginationLinks.Add(new PaginationLink {Active = true, PageIndex = i, IsCurrent = true, DisplayText = i.ToString()});
                }
                else
                {
                    model.PaginationLinks.Add(new PaginationLink {Active = true, PageIndex = i, DisplayText = i.ToString(), Url = generateUrl(i)});
                }
            }

            if (!PagerOptions.HideLastPageNumber && end < pageCount)
            {
                if (end < pageCount)
                {
                    model.PaginationLinks.Add(new PaginationLink {Active = false, DisplayText = "...", IsSpacer = true});
                }

                if (end < pageCount - 2)
                {
                    model.PaginationLinks.Add(new PaginationLink {Active = true, PageIndex = pageCount, DisplayText = pageCount.ToString(), Url = generateUrl(pageCount)});
                }
            }

            // Next page
            if (!PagerOptions.HidePreviousAndNextPage)
            {
                var nextPageText = PagerOptions.NextPageText;
                model.PaginationLinks.Add(_currentPage < pageCount
                                              ? new PaginationLink
                                                {
                                                    Active = true, PageIndex = _currentPage + 1, DisplayText = nextPageText,
                                                    DisplayTitle = PagerOptions.NextPageTitle,
                                                    Url = generateUrl(_currentPage + 1)
                                                }
                                              : new PaginationLink {Active = false, DisplayText = nextPageText});
            }

            // Last page
            if (PagerOptions.DisplayLastPage)
            {
                model.PaginationLinks.Add(new PaginationLink
                                          {
                                              Active = _currentPage < pageCount, DisplayText = PagerOptions.LastPageText,
                                              DisplayTitle = PagerOptions.LastPageTitle,
                                              PageIndex = pageCount, Url = generateUrl(pageCount)
                                          });
            }

            // AjaxOptions
            if (PagerOptions.AjaxOptions != null)
            {
                model.AjaxOptions = PagerOptions.AjaxOptions;
            }

            model.Options = PagerOptions;
            return model;
        }

        protected virtual string GeneratePageUrl(int pageNumber)
        {
            var viewContext = _htmlHelper.ViewContext;
            var routeDataValues = viewContext.RequestContext.RouteData.Values;
            RouteValueDictionary pageLinkValueDictionary;

            // Avoid canonical errors when pageNumber is equal to 1.
            if (pageNumber == 1 && !PagerOptions.AlwaysAddFirstPageNumber)
            {
                pageLinkValueDictionary = new RouteValueDictionary(PagerOptions.RouteValues);

                if (routeDataValues.ContainsKey(PagerOptions.PageRouteValueKey))
                {
                    routeDataValues.Remove(PagerOptions.PageRouteValueKey);
                }
            }
            else
            {
                pageLinkValueDictionary = new RouteValueDictionary(PagerOptions.RouteValues) {{PagerOptions.PageRouteValueKey, pageNumber}};
            }

            // To be sure we get the right route, ensure the controller and action are specified.
            if (!pageLinkValueDictionary.ContainsKey("controller") && routeDataValues.ContainsKey("controller"))
            {
                pageLinkValueDictionary.Add("controller", routeDataValues["controller"]);
            }

            if (!pageLinkValueDictionary.ContainsKey("action") && routeDataValues.ContainsKey("action"))
            {
                pageLinkValueDictionary.Add("action", routeDataValues["action"]);
            }

            // Fix the dictionary if there are arrays in it.
            pageLinkValueDictionary = pageLinkValueDictionary.FixListRouteDataValues();
            return _urlHelper.Action(pageLinkValueDictionary["action"].ToString(), pageLinkValueDictionary["controller"].ToString(), pageLinkValueDictionary);
        }
    }

    public class Pager<TModel> : Pager
    {
        private readonly DnnHtmlHelper<TModel> _htmlHelper;

        public Pager(DnnHtmlHelper<TModel> htmlHelper, int pageSize, int currentPage, int totalItemCount) : base(htmlHelper, pageSize, currentPage, totalItemCount)
        {
            _htmlHelper = htmlHelper;
        }

        public Pager<TModel> Options(Action<PagerOptionsBuilder<TModel>> buildOptions)
        {
            buildOptions(new PagerOptionsBuilder<TModel>(PagerOptions, _htmlHelper));
            return this;
        }
    }
}
