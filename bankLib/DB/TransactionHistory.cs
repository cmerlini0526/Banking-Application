using System;
using System.Collections.Generic;

namespace bankLib.DB;

public partial class TransactionHistory
{
    public int? UserId { get; set; }

    public int? AccId { get; set; }

    public int TransId { get; set; }

    public double TransChange { get; set; }

    public double TransOldBal { get; set; }

    public DateTime TransDate { get; set; }

    public string? TransType { get; set; }

    public virtual Account? Acc { get; set; }

    public virtual User? User { get; set; }
}
