using JetBrains.DocumentModel;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Daemon.Impl;

namespace ReReflection.Highlightings
{
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
