using System;
using System.Collections.Generic;

namespace bankLib.DB;

public partial class CheckRequest
{
    public int ReqId { get; set; }

    public int? UserId { get; set; }

    public int? AccId { get; set; }

    public DateTime? ReqOpenDate { get; set; }

    public DateTime? ReqRespondDate { get; set; }

    public bool? ReqAccepted { get; set; }

    public virtual Account? Acc { get; set; }

    public virtual User? User { get; set; }
}
