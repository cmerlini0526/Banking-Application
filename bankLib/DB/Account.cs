using System;
using System.Collections.Generic;

namespace bankLib.DB;

public partial class Account
{
    public int AccId { get; set; }

    public int AccOwnerId { get; set; }

    public string? AccName { get; set; }

    public double? AccBalance { get; set; }

    public string? AccType { get; set; }

    public string? AccBranch { get; set; }

    public bool AccIsActive { get; set; }

    public virtual User AccOwner { get; set; } = null!;

    public virtual ICollection<CheckRequest> CheckRequests { get; set; } = new List<CheckRequest>();

    public virtual ICollection<TransactionHistory> TransactionHistories { get; set; } = new List<TransactionHistory>();
}
