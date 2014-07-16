using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.ActionManagement;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Navigation.Search;
using JetBrains.ReSharper.Feature.Services.Occurences;
using JetBrains.ReSharper.Feature.Services.Occurences.OccurenceKindProviders;
using JetBrains.ReSharper.Features.Common.Occurences;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Tree;

namespace ReSharper.Reflection.Search
{
    public class ReflectedMemberOccurence : ReferenceOccurence
    {
        public ReflectedMemberOccurence(IReference reference, IDeclaredElement target, OccurenceType occurenceType, IProjectFile projectFile = null) 
            : base(reference, target, occurenceType, projectFile)
        {
            Kinds.Clear();
            Kinds.Add(ReflectedMemberOccurenceKindProvider.ReflectionOccurenceKind);
        }
    }

    [ActionHandler(new string[] { "OccurenceBrowser.Filter.ShowReflection" })]
    public class ShowReflectionAccessAction : ShowOccurenceKindBaseAction
    {
        public override OccurenceKind OccurenceKind
        {
            get
            {
                return ReflectedMemberOccurenceKindProvider.ReflectionOccurenceKind;
            }
        }
    }
}
