using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spider.Interface
{
    public interface IExcelSheet
    {
        string School { get; set; }
        string Url { get; set; }
        string Sport { get; set; }
    }
}
