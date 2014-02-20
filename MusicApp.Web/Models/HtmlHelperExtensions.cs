using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SocialApp.Models
{
    public static class HtmlHelperExtensions
    {
        public static string Truncate(this HtmlHelper helper, string input, int toLength)
        {
            if (input.Length <= toLength)
            {
                return input;
            }
            return input.Substring(0, toLength) + "...";
        }
    }
}