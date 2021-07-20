using System;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Components {
    public class WindowsConsole : IConsole {
        public void WriteLine() {
            Console.WriteLine();
        }

        public void WriteLine(string s) {
            Console.WriteLine(s);
        }
    }
}
