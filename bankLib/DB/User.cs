using System;
using System.Collections.Generic;

namespace bankLib.DB;

public partial class User
{
    public int UserId { get; set; }

    public string? UserName { get; set; }

    public string? UserPass { get; set; }

    public bool? UserIsAdmin { get; set; }

    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();

    public virtual ICollection<CheckRequest> CheckRequests { get; set; } = new List<CheckRequest>();

    public virtual ICollection<TransactionHistory> TransactionHistories { get; set; } = new List<TransactionHistory>();
}
