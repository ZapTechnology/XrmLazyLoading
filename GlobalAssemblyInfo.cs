using System.Reflection;

[assembly: AssemblyCompany("Zap Technology Pty Ltd")]
[assembly: AssemblyCopyright("Copyright © Zap Technology Pty. Ltd. 2017")]
[assembly: AssemblyCulture("")]
[assembly: AssemblyVersion(ThisAssembly.Git.SemVer.Major
    + "." + ThisAssembly.Git.SemVer.Minor
    + "." + ThisAssembly.Git.SemVer.Patch)]
[assembly: AssemblyInformationalVersion(ThisAssembly.Git.SemVer.Major
    + "." + ThisAssembly.Git.SemVer.Minor
    + "." + ThisAssembly.Git.SemVer.Patch
    + "+" + ThisAssembly.Git.Commit)]