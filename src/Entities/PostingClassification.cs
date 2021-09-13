﻿using System.Xml.Serialization;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities {
    public class PostingClassification : IPostingClassification {
        [XmlIgnore]
        public bool IgnoreCredit { get; init; }

        [XmlAttribute("credit")]
        public bool Credit { get; init; }

        [XmlAttribute("clue")]
        public string Clue { get; init;  }

        [XmlAttribute("classification")]
        public string Classification { get; init; }

        [XmlIgnore]
        public int Month { get; init; }

        [XmlIgnore]
        public int Year { get; init; }
    }
}
