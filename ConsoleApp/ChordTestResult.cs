using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp
{
    internal class ChordTestResult
    {
        public string FileName { get; set; }
        public bool Success { get; set; }
        public string RootsText { get; set; }
        public string ErrorMessage { get; set; }
    }
}
