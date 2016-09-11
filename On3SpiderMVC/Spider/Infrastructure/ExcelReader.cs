using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinqToExcel;
using Spider.Interface;

namespace On3SpiderMVC.Infrastructure
{
    /// <summary>
    /// Class for reading an Excel 2007 spreadsheet using LinqToExcel
    /// </summary>
    public class ExcelReader<T> where T: IExcelSheet
    {
        private readonly string _fileName;

        public ExcelReader(string fileName)
        {
            _fileName = fileName;
        }

        /// <summary>
        /// Reads rows from an Excel spreadsheet using the schema laid out in the RosterSheet class.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<T> ReadSheet()
        {
            if (String.IsNullOrWhiteSpace(_fileName))
            {
                throw new InvalidOperationException("Attempted to read an invalid Excel file.");
            }

            var excel = new ExcelQueryFactory(_fileName);
            var worksheet = excel.Worksheet<T>().ToList();
            return worksheet;
        } 


    }
}
