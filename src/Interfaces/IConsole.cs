﻿using System.Threading.Tasks;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces {
    public interface IConsole {
        Task WriteLineAsync();
        Task WriteLineAsync(string s);
    }
}
