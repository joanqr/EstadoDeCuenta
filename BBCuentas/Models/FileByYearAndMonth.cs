using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BBCuentas.Models
{
    public class FileByYearAndMonth
    {
        public string Year { get; set; }
        public string Month { get; set; }
        public string FilePath { get; set; }
        public string Carpeta { get; set; }
        public string Empresa { get; set; }

        public FileByYearAndMonth(string year, string month, string filePath,string carpeta, string empresa)
        {
            Year = year;
            Month = month;
            FilePath = filePath;
            Carpeta = carpeta;
            Empresa = empresa;
        }
    }
}