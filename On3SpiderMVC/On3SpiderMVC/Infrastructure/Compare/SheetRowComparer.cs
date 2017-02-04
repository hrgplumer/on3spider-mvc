using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Spider.Interface;

namespace On3SpiderMVC.Infrastructure.Compare
{

    /// <summary>
    /// Class used to compare ISheetRow objects for equality. Two ISheetRows are considered equal if their Urls match.
    /// </summary>
    public class SheetRowComparer : IEqualityComparer<ISheetRow>
    {
        public bool Equals(ISheetRow x, ISheetRow y)
        {
            return x.Url.Equals(y.Url);
        }

        public int GetHashCode(ISheetRow obj)
        {
            return obj.Url.GetHashCode();
        }
    }
}