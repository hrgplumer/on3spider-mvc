using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinqToExcel.Attributes;
using Spider.Interface;
using Spider.Models.Result;

namespace Spider.Models
{
    public class RosterSheet : IExcelSheet, ISheetRow
    {
        [ExcelColumn("Roster URL")]
        public string Url { get; set; }

        [ExcelColumn("School")]
        public string School { get; set; }

        [ExcelColumn("Sport")]
        public string Sport { get; set; }

        public RosterResult Results { get; set; }
    }
}
