﻿using System.Collections.Generic;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;

public interface ISourceFileReader {
    IList<IPosting> ReadPostings(string fileName, IErrorsAndInfos errorsAndInfos);
}