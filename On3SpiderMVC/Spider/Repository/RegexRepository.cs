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

        public static Regex ClassColumnRegex => new Regex(@"^\s*(YR.?|CL.?)\s*$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static Regex PositionColumnRegex => new Regex(@"^\s*(POS.?)\s*$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static Regex MajorColumnRegex => new Regex(@"^\s*(MAJOR.?)\s*$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    }
}
