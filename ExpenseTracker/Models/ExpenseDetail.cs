using System;
using System.Collections.Generic;

namespace ExpenseTracker.Models;

public partial class ExpenseDetail
{
    public int Id { get; set; }

    public int? IncomeDateAndAmountId { get; set; }

    public int? PayableId { get; set; }

    public decimal? Amount { get; set; }

    public string? Comment { get; set; }
    public bool? IsPaid { get; set; }
}
