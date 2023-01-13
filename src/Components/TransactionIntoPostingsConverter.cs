using System;
using System.Collections.Generic;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities.Fundamental;
namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Components;

public class TransactionIntoPostingsConverter : ITransactionIntoPostingsConverter {
    public IList<IPosting> Convert(Transaction transaction) {
        var result = new List<IPosting> {
            new Posting {
                Date = transaction.Date,
                Amount = Math.Max(0, transaction.IncomeInEuro - transaction.ExpensesInEuro),
                Remark = "Capital income"
            },
            new Posting {
                Date = transaction.Date,
                Amount = - Math.Max(0, transaction.IncomeInEuro - transaction.ExpensesInEuro),
                Remark = "Saved capital income"
            }
        };
        return result;
    }
}