using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Daemon.Impl;
#if R9
using JetBrains.ReSharper.Feature.Services.CSharp.Daemon;
#else
using JetBrains.ReSharper.Feature.Services.Daemon;
#endif

namespace ReSharper.Reflection.Highlightings
{

#if R9

  public interface IHighlightingWithRange : IHighlighting
  {
    DocumentRange CalculateRange();
  }

#endif

    public abstract class ReflectionHighlightingBase : CSharpHighlightingBase, IHighlighting, IHighlightingWithRange
    {
        protected ReflectionHighlightingBase()
        {
        }

        public int NavigationOffsetPatch
        {
            get
            {
                return 0;
            }
        }

        public abstract DocumentRange CalculateRange();
    }
}
