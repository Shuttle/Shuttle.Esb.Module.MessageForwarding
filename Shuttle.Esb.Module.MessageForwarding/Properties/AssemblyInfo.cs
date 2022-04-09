using System.Reflection;
using System.Runtime.InteropServices;

#if NET461
[assembly: AssemblyTitle(".NET Framework 4.6.1")]
#endif

#if NETCOREAPP2_1
[assembly: AssemblyTitle(".NET Core 2.1")]
#endif

#if NETSTANDARD2_0
[assembly: AssemblyTitle(".NET Standard 2.0")]
#endif

[assembly: AssemblyVersion("12.0.1.0")]
[assembly: AssemblyCopyright("Copyright © Eben Roux 2019")]
[assembly: AssemblyProduct("Shuttle.Esb.Module.MessageForwarding")]
[assembly: AssemblyCompany("Shuttle")]
[assembly: AssemblyConfiguration("Release")]
[assembly: AssemblyInformationalVersion("12.0.1")]
[assembly: ComVisible(false)]
