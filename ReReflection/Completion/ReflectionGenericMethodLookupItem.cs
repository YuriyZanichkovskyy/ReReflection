using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Application;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.CSharp.CodeCompletion.Infrastructure;
using JetBrains.ReSharper.Feature.Services.LiveTemplates.Hotspots;
using JetBrains.ReSharper.Feature.Services.LiveTemplates.LiveTemplates;
using JetBrains.ReSharper.Feature.Services.LiveTemplates.Settings;
using JetBrains.ReSharper.Feature.Services.LiveTemplates.Util;
using JetBrains.ReSharper.Feature.Services.Lookup;
using JetBrains.ReSharper.LiveTemplates;
using JetBrains.ReSharper.LiveTemplates.Templates;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.ExpectedTypes;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Resx.Utils;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Refactorings.CSharp.ExtractMethod2.Common;
using JetBrains.TextControl;
using JetBrains.TextControl.Util;
using JetBrains.Util;

namespace ReSharper.Reflection.Completion
{
    public class ReflectionGenericMethodLookupItem : ReflectionMemberLookupItem
    {
        private DeclaredElementInstance _instance;

        public ReflectionGenericMethodLookupItem(string name, DeclaredElementInstance instance, CSharpCodeCompletionContext context, ILookupItemsOwner owner) 
            : base(name, string.Empty, instance, context, owner)
        {
            _instance = instance;
        }

        public override void Accept(JetBrains.TextControl.ITextControl textControl, JetBrains.Util.TextRange nameRange, LookupItemInsertType lookupItemInsertType, Suffix suffix, JetBrains.ProjectModel.ISolution solution, bool keepCaretStill)
        {
            Solution.GetPsiServices().Files.CommitAllDocuments();

            IDisposable changeUnit = Shell.Instance.GetComponent<TextControlChangeUnitFactory>().CreateChangeUnit(textControl, "Expand live template");
            try
            {
                var method = (IMethod)_instance.Element;
                var invocationExpression = ((IInvocationExpression)Context.NodeInFile.Parent); //target.GetMethod()


                string target = ((IReferenceExpression)invocationExpression.InvokedExpression).QualifierExpression.GetText();
                if (!method.IsStatic)
                {
                    target = string.Format("Expression.Default({0})", target);
                }


                textControl.Document.DeleteText(invocationExpression.GetDocumentRange().TextRange);

                string[] parameters;
                string[] arguments;
                BuildArguments(method, out parameters, out arguments);

                var template = new Template("GetMethodTemplate", string.Empty,
                    string.Format("Expression.Call({0}, \"{1}\", new Type[] {{ {2} }}, {3}).Method", target, method.ShortName, 
                    string.Join(", ", parameters.Select(p => string.Format("typeof({0})", p))),
                    string.Join(", ", arguments)),
                    false, true, false, TemplateApplicability.Live);

                for (int i = 0; i < parameters.Length; i++)
                {
                    template.Fields.Add(new TemplateField(parameters[i].Replace("$", string.Empty), parameters[i], 0));
                }

                HotspotSession sessionFromTemplate = LiveTemplatesManager.Instance.CreateHotspotSessionFromTemplate(
                    template, 
                    solution, 
                    textControl, (Action<IHotspotSession>)null);
                if (sessionFromTemplate == null)
                    return;
                sessionFromTemplate.Execute(changeUnit);
            }
            catch
            {
                changeUnit.Dispose();
                throw;
            }
        }

        private void BuildArguments(IMethod method, out string[] typeParameters, out string[] arguments)
        {
            typeParameters = new string[method.TypeParameters.Count];
            for (int i = 0; i < method.TypeParameters.Count; i++)
            {
                typeParameters[i] = string.Format("$typeParameter{0}$", i);
            }

            arguments = new string[method.Parameters.Count];
            for (int i = 0; i < method.Parameters.Count; i++)
            {
                arguments[i] = string.Format("Expression.Default(typeof({0}))", BuildType(method, method.Parameters[i]));
            }
        }

        private string BuildType(IMethod method, IParameter parameter)
        {
            var parameterType = parameter.Type;
            string presentableName = parameterType.GetPresentableName(CSharpLanguage.Instance);
            for (int i = 0; i < method.TypeParameters.Count; i++)
            {
                presentableName = presentableName.Replace(method.TypeParameters[0].ShortName, string.Format("$typeParameter{0}$", i));
            }
            return presentableName;
        }

        protected override void OnAfterComplete(ITextControl textControl, ref TextRange nameRange, ref TextRange decorationRange, TailType tailType,
            ref Suffix suffix, ref IRangeMarker caretPositionRangeMarker)
        {
        }
    }
}
