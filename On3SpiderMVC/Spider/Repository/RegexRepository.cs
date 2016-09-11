using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Spider.Repository
{
    /// <summary>
    /// Contains Regex objects needed for parsing Html
    /// </summary>
    public static class RegexRepository
    {
        /// <summary>
        /// Regex used to determine if a column contains player number.
        /// </summary>
        /// <returns></returns>
        public static Regex NumberColumnRegex => new Regex(@"\s*(?:No.?|#)\s*", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static Regex NameColumnRegex => new Regex(@"^\s*(Full\s+)?Name\s*$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static Regex MajorRegex => new Regex(@"<td\b[^>]*>\s*major:?\s*</td>", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

        public static Regex MajorMinorRegex => new Regex(@">\s*major(/minor)?:?\s*(<[^>]+>)*(?<major>[^<]+)\s*<", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

    }
}
