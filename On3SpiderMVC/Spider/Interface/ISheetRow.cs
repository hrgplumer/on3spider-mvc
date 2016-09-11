using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spider.Models.Domain;
using Spider.Models.Result;

namespace Spider.Interface
{
    public interface ISheetRow
    {
        string School { get; set; }
        string Sport { get; set; }
        string Url { get; set; }
        RosterResult Results { get; set; } 
    }
}
