#if R9
using JetBrains.Application.BuildScript.Application.Zones;
using JetBrains.Application.Settings;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Daemon.CSharp.Stages;
using JetBrains.ReSharper.Feature.Services;
using JetBrains.ReSharper.Feature.Services.CSharp.Daemon;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Feature.Services.Navigation;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
#endif

namespace ReSharper.Reflection
{
#if R9
    [ZoneMarker]
    public class ZoneMarker : IPsiLanguageZone, IRequire<ILanguageCSharpZone>, IRequire<ICodeEditingZone>, IRequire<DaemonZone>, IRequire<NavigationZone>
    {
    }
#endif
}
