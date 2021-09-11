using System;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities {
    public class PostingAdjustment : IPostingAdjustment, IEquatable<PostingAdjustment> {
        public DateTime Date { get; init; }
        public string Clue { get; init; }
        public double Amount { get; init; }
        public double AdjustedAmount { get; init; }

        public PostingAdjustment() {}

        public PostingAdjustment(IPostingAdjustment postingAdjustment) {
            Date = postingAdjustment.Date;
            Clue = postingAdjustment.Clue;
            Amount = postingAdjustment.Amount;
            AdjustedAmount = postingAdjustment.AdjustedAmount;
        }

        public bool Equals(PostingAdjustment other) {
            if (other == null) { return false; }

            return Date.Equals(other.Date) && Clue == other.Clue && Amount.Equals(other.Amount) && AdjustedAmount.Equals(other.AdjustedAmount);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) { return false; }
            if (ReferenceEquals(this, obj)) { return true; }
            if (obj.GetType() != GetType()) { return false; }

            return Equals((PostingAdjustment)obj);
        }

        public override int GetHashCode() {
            return HashCode.Combine(Date, Clue, Amount, AdjustedAmount);
        }
    }
}