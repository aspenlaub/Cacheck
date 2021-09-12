using System;
using System.Xml.Serialization;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities {
    public class PostingAdjustment : IPostingAdjustment, IEquatable<PostingAdjustment> {
        [XmlIgnore]
        public string Guid { get; set; } = System.Guid.NewGuid().ToString();

        [XmlAttribute("date")]
        public DateTime Date { get; init; }

        [XmlAttribute("clue")]
        public string Clue { get; init; }

        [XmlAttribute("amount")]
        public double Amount { get; init; }

        [XmlElement("adjustedamount")]
        public double AdjustedAmount { get; init; }

        [XmlIgnore]
        public string FormattedDate => $"{Date.Day:D2}.{Date.Month:D2}.{Date.Year}";

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

            return obj.GetType() == GetType() && Equals((PostingAdjustment)obj);
        }

        public override int GetHashCode() {
            return HashCode.Combine(Date, Clue, Amount, AdjustedAmount);
        }
    }
}