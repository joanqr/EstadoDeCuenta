using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BBCuentas.Models
{
    public class YearMonth
    {
        public string Year { get; set; }
        public List<string> Months { get; set; }

        public YearMonth(string year, List<string> months)
        {
            Year = year;
            Months = months;
        }
    }
}