﻿#region Usings

using System.Web;
using DotNetNuke.Web.Mvc.Helpers;

#endregion

namespace Italliance.Modules.DnnHosting.Components.Mvc.FlashMessages
{
    /// <summary>
    ///     Twitter Bootstrap based HTML renderer for Flash Messages rendering the messages as alerts.
    /// </summary>
    public static class FlashMessageHtmlHelper
    {
        /// <summary>
        ///     Renders any queued flash messages as a Twitter Bootstrap alerta and returns the html code.
        /// </summary>
        /// <param name="html"></param>
        /// ///
        /// <param name="dismissable">Indicates if the messages should be dismissable</param>
        /// <returns></returns>
        public static IHtmlString RenderFlashMessages(this DnnHtmlHelper html, bool dismissable = true)
        {
            // Retrieve queued messages.
            var messages = FlashMessage.Retrieve(html.ViewContext.HttpContext);
            var output = "";

            foreach (var message in messages)
            {
                output += RenderFlashMessage(message, dismissable);
            }

            return html.Raw(output);
        }

        /// <summary>
        ///     Renders the passed flash message as a Twitter Bootstrap alert component.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="dismissable">Indicates if this message should be dismissable</param>
        /// <returns></returns>
        public static string RenderFlashMessage(FlashMessageModel message, bool dismissable = true)
        {
            var cssClasses = message.Type.GetCssStyle();
            if (dismissable)
            {
                cssClasses += " alert-dismissible";
            }

            string result = $"<div class=\"{cssClasses}\" role=\"alert\">\r\n";

            if (dismissable)
            {
                result += "<button type=\"button\" class=\"close\" data-dismiss=\"alert\" aria-label=\"Close\"><span aria-hidden=\"true\">&times;</span></button>\r\n";
            }

            if (!string.IsNullOrWhiteSpace(message.Title))
            {
                result += "<strong>" + HttpUtility.HtmlEncode(message.Title) + "</strong> ";
            }

            if (message.IsHtml)
            {
                result += message.Message;
            }
            else
            {
                result += HttpUtility.HtmlEncode(message.Message);
            }

            result += "</div>";
            return result;
        }

        /// <summary>
        ///     Returns the Twitter bootstrap css style for the passed message type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static string GetCssStyle(this FlashMessageType type)
        {
            switch (type)
            {
                case FlashMessageType.Danger:
                    return "alert alert-danger";
                default:
                case FlashMessageType.Info:
                    return "alert alert-info";
                case FlashMessageType.Warning:
                    return "alert alert-warning";
                case FlashMessageType.Confirmation:
                    return "alert alert-success";
            }
        }

        /*
         * http://forums.asp.net/t/1752236.aspx/1?ASP+NET+MVC+Multiple+Forms+Validation+Summaries
        public static void RenderValidationFlashMessage(this HtmlHelper html, string message)
        {
            if(html.ViewData.ModelState.IsValid
        }*/
    }
}
