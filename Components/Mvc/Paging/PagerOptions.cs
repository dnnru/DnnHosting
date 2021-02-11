#region Usings

using System.Web.Mvc.Ajax;
using System.Web.Routing;

#endregion

namespace Italliance.Modules.DnnHosting.Components.Mvc.Paging
{
    public class PagerOptions
    {
        public PagerOptions()
        {
            RouteValues = new RouteValueDictionary();
            DisplayTemplate = Defaults.DisplayTemplate;
            MaxNrOfPages = Defaults.MaxNrOfPages;
            AlwaysAddFirstPageNumber = Defaults.AlwaysAddFirstPageNumber;
            PageRouteValueKey = Defaults.DefaultPageRouteValueKey;
            PreviousPageText = Defaults.PreviousPageText;
            PreviousPageTitle = Defaults.PreviousPageTitle;
            NextPageText = Defaults.NextPageText;
            NextPageTitle = Defaults.NextPageTitle;
            FirstPageText = Defaults.FirstPageText;
            FirstPageTitle = Defaults.FirstPageTitle;
            LastPageText = Defaults.LastPageText;
            LastPageTitle = Defaults.LastPageTitle;
            DisplayFirstPage = Defaults.DisplayFirstPage;
            DisplayLastPage = Defaults.DisplayLastPage;
            HideFirstPageNumber = Defaults.HideFirstPageNumber;
            HideLastPageNumber = Defaults.HideLastPageNumber;
            UseItemCountAsPageCount = Defaults.UseItemCountAsPageCount;
            HidePreviousAndNextPage = Defaults.HidePreviousAndNextPage;
            CustomRouteName = Defaults.CustomRouteName;
        }

        public RouteValueDictionary RouteValues { get; internal set; }

        public string DisplayTemplate { get; internal set; }

        public int MaxNrOfPages { get; internal set; }

        public AjaxOptions AjaxOptions { get; internal set; }

        public bool AlwaysAddFirstPageNumber { get; internal set; }

        public string Action { get; internal set; }

        public string Controller { get; internal set; }

        public string PageRouteValueKey { get; set; }

        public string PreviousPageText { get; set; }

        public string PreviousPageTitle { get; set; }

        public string NextPageText { get; set; }

        public string NextPageTitle { get; set; }

        public string FirstPageText { get; set; }

        public string FirstPageTitle { get; set; }

        public string LastPageText { get; set; }

        public string LastPageTitle { get; set; }

        public bool DisplayFirstAndLastPage => DisplayFirstPage && DisplayLastPage;

        public bool DisplayFirstPage { get; set; }

        public bool DisplayLastPage { get; set; }

        public bool HideFirstPageNumber { get; set; }

        public bool HideLastPageNumber { get; set; }

        public bool HidePreviousAndNextPage { get; internal set; }

        public bool UseItemCountAsPageCount { get; internal set; }

        public string CustomRouteName { get; set; }

        public static class DefaultDefaults
        {
            public const int MAX_NR_OF_PAGES = 10;
            public const string DISPLAY_TEMPLATE = null;
            public const bool ALWAYS_ADD_FIRST_PAGE_NUMBER = false;
            public const string DEFAULT_PAGE_ROUTE_VALUE_KEY = "page";
            public const string PREVIOUS_PAGE_TEXT = "«";
            public const string PREVIOUS_PAGE_TITLE = "Previous page";
            public const string NEXT_PAGE_TEXT = "»";
            public const string NEXT_PAGE_TITLE = "Next page";
            public const string FIRST_PAGE_TEXT = "<";
            public const string FIRST_PAGE_TITLE = "First page";
            public const string LAST_PAGE_TEXT = ">";
            public const string LAST_PAGE_TITLE = "Last page";
            public const bool DISPLAY_FIRST_PAGE = false;
            public const bool DISPLAY_LAST_PAGE = false;
            public const bool HIDE_FIRST_PAGE_NUMBER = false;
            public const bool HIDE_LAST_PAGE_NUMBER = false;
            public const bool USE_ITEM_COUNT_AS_PAGE_COUNT = false;
            public const string CUSTOM_ROUTE_NAME = null;
            public static bool HidePreviousAndNextPage = false;
        }

        /// <summary>
        ///     The static Defaults class allows you to set Pager defaults for the entire application.
        ///     Set values at application startup.
        /// </summary>
        public static class Defaults
        {
            public static int MaxNrOfPages = DefaultDefaults.MAX_NR_OF_PAGES;
            public static string DisplayTemplate = DefaultDefaults.DISPLAY_TEMPLATE;
            public static bool AlwaysAddFirstPageNumber = DefaultDefaults.ALWAYS_ADD_FIRST_PAGE_NUMBER;
            public static string DefaultPageRouteValueKey = DefaultDefaults.DEFAULT_PAGE_ROUTE_VALUE_KEY;
            public static string PreviousPageText = DefaultDefaults.PREVIOUS_PAGE_TEXT;
            public static string PreviousPageTitle = DefaultDefaults.PREVIOUS_PAGE_TITLE;
            public static string NextPageText = DefaultDefaults.NEXT_PAGE_TEXT;
            public static string NextPageTitle = DefaultDefaults.NEXT_PAGE_TITLE;
            public static string FirstPageText = DefaultDefaults.FIRST_PAGE_TEXT;
            public static string FirstPageTitle = DefaultDefaults.FIRST_PAGE_TITLE;
            public static string LastPageText = DefaultDefaults.LAST_PAGE_TEXT;
            public static string LastPageTitle = DefaultDefaults.LAST_PAGE_TITLE;
            public static bool DisplayFirstPage = DefaultDefaults.DISPLAY_FIRST_PAGE;
            public static bool DisplayLastPage = DefaultDefaults.DISPLAY_LAST_PAGE;
            public static bool HideFirstPageNumber = DefaultDefaults.HIDE_FIRST_PAGE_NUMBER;
            public static bool HideLastPageNumber = DefaultDefaults.HIDE_LAST_PAGE_NUMBER;
            public static bool UseItemCountAsPageCount = DefaultDefaults.USE_ITEM_COUNT_AS_PAGE_COUNT;
            public static bool HidePreviousAndNextPage = DefaultDefaults.HidePreviousAndNextPage;
            public static string CustomRouteName = DefaultDefaults.CUSTOM_ROUTE_NAME;

            public static void Reset()
            {
                MaxNrOfPages = DefaultDefaults.MAX_NR_OF_PAGES;
                DisplayTemplate = DefaultDefaults.DISPLAY_TEMPLATE;
                AlwaysAddFirstPageNumber = DefaultDefaults.ALWAYS_ADD_FIRST_PAGE_NUMBER;
                DefaultPageRouteValueKey = DefaultDefaults.DEFAULT_PAGE_ROUTE_VALUE_KEY;
                PreviousPageText = DefaultDefaults.PREVIOUS_PAGE_TEXT;
                PreviousPageTitle = DefaultDefaults.PREVIOUS_PAGE_TITLE;
                NextPageText = DefaultDefaults.NEXT_PAGE_TEXT;
                NextPageTitle = DefaultDefaults.NEXT_PAGE_TITLE;
                FirstPageText = DefaultDefaults.FIRST_PAGE_TEXT;
                FirstPageTitle = DefaultDefaults.FIRST_PAGE_TITLE;
                LastPageText = DefaultDefaults.LAST_PAGE_TEXT;
                LastPageTitle = DefaultDefaults.LAST_PAGE_TITLE;
                DisplayFirstPage = DefaultDefaults.DISPLAY_FIRST_PAGE;
                DisplayLastPage = DefaultDefaults.DISPLAY_LAST_PAGE;
                HideFirstPageNumber = DefaultDefaults.HIDE_FIRST_PAGE_NUMBER;
                HideLastPageNumber = DefaultDefaults.HIDE_LAST_PAGE_NUMBER;
                UseItemCountAsPageCount = DefaultDefaults.USE_ITEM_COUNT_AS_PAGE_COUNT;
                HidePreviousAndNextPage = DefaultDefaults.HidePreviousAndNextPage;
                CustomRouteName = DefaultDefaults.CUSTOM_ROUTE_NAME;
            }
        }
    }
}
