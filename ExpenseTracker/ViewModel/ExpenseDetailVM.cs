using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseTracker.ViewModel
{
    public class ExpenseDetailVM
    {
        public string PayableName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Comment { get; set; } = string.Empty;
    }
}