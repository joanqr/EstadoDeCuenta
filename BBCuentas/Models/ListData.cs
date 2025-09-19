using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BBCuentas.Models
{
    public class ListData
    {
     
        public List<string> Files { get; set; }
        public List<FileByYearAndMonth> FileYearAndMonth { get; set; }
        public List<YearMonth> YearMonth { get; set; }

        public ListData(List<string> files, List<FileByYearAndMonth> fileYearAndMonth, List<YearMonth> yearMonth)
        {
            Files = files;
            FileYearAndMonth = fileYearAndMonth;
            YearMonth = yearMonth;
        }


    }
}