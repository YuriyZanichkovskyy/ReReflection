using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Occurences;
#if !R9
using JetBrains.ReSharper.Feature.Services.Occurences.OccurenceKindProviders;
using JetBrains.ReSharper.Feature.Services.Navigation.Search;

#endif
using JetBrains.Util;

namespace ReSharper.Reflection.Search
{
    [SolutionComponent]
    public class ReflectedMemberOccurenceKindProvider : IOccurenceKindProvider
    {
        public static readonly OccurenceKind ReflectionOccurenceKind = new OccurenceKind("Reflection Access", false);

        public ICollection<OccurenceKind> GetOccurenceKinds(IOccurence occurence)
        {
            if (occurence is ReflectedMemberOccurence)
            {
                return new[] { ReflectionOccurenceKind };
            }

            return EmptyList<OccurenceKind>.InstanceList;
        }

        public IEnumerable<OccurenceKind> GetAllPossibleOccurenceKinds()
        {
            yield return ReflectionOccurenceKind;
        }
    }
}
