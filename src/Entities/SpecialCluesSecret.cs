using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities {
    public class SpecialCluesSecret : ISecret<SpecialClues> {
        private SpecialClues PrivateDefaultValue;

        public SpecialClues DefaultValue => PrivateDefaultValue ??= new SpecialClues { new() { Clue = "a special clue" } };

        public string Guid => "ABA78D4F-6571-2A44-E793-E9BF113D8B86";
    }
}