using System;
// ReSharper disable UnusedMember.Global

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities.Fundamental;

public class Transaction {
    public DateTime Date { get; set; }
    public Security Security { get; set; }
    public TransactionType TransactionType { get; set; }
    public double Nominal { get; set; }
    public double PriceInEuro { get; set; }
    public double ExpensesInEuro { get; set; }
    public double IncomeInEuro { get; set; }
}