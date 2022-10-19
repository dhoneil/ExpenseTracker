using System;
using System.Collections.Generic;

namespace ExpenseTracker.Models;

public partial class IncomeDateAndAmount
{
    public int Id { get; set; }
    public DateTime? IncomeDate { get; set; }
    public decimal? IncomeAmount { get; set; }
}
