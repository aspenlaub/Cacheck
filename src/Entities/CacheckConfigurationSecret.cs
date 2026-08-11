using System.IO;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities;

public class CacheckConfigurationSecret : ISecret<CacheckConfiguration> {
    public CacheckConfiguration DefaultValue => field ??= new CacheckConfiguration { SourceFolder = Path.GetTempPath(), JsonFolder = Path.GetTempPath() };

    public string Guid => "AE9AA8B3-C9C1-3489-5DE4-8D3C86CF17E6";
}