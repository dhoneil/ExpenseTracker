using System;
using System.Collections.Generic;

namespace ExpenseTracker.Models;

public partial class Payable
{
    public int Payableid { get; set; }
    public string? Payablename { get; set; }
    public bool? IsRecuring { get; set; }
    public decimal? Amount { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool? IsComplete { get; set; }
}
