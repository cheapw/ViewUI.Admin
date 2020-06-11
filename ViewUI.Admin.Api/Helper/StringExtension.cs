using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ViewUI.Admin.Api.Helper
{
    public static class StringExtension
    {
        private static string EmailRegexPattern = "(['\"]{1,}.+['\"]{1,}\\s+)?<?[\\w\\.\\-]+@[^\\.][\\w\\.\\-]+\\.[a-z]{2,}>?";
        public static bool IsEmail(this string email)
        {
            // Define a regular expression for repeated words.
            Regex rx = new Regex(EmailRegexPattern,
              RegexOptions.Compiled | RegexOptions.IgnoreCase);

            return rx.IsMatch(email);
        }
    }
}
