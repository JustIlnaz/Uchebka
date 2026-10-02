using System;

namespace AutoService.Data;

public class Payment
{
    public int Id { get; set; }
    public int? InvoiceId { get; set; }
    public DateTime Date { get; set; }
    public decimal Amount { get; set; }
    public string? Method { get; set; }
    public string? Status { get; set; }
    public string? TransactionNumber { get; set; }

    public virtual Invoice? Invoice { get; set; }
}
