using Antlr4.Runtime.Misc;
using System.Reflection.Metadata.Ecma335;
using verse_interpreter.lib.Data;
using verse_interpreter.lib.Data.ResultObjects;
using verse_interpreter.lib.Grammar;

namespace verse_interpreter.lib.Visitors
{
    public class FunctionDeclarationVisitor : AbstractVerseVisitor<FunctionDeclarationResult>
    {
        private DeclarationVisitor declarationVisitor;

        public FunctionDeclarationVisitor(ApplicationState applicationState,
                                          DeclarationVisitor declarationVisitor) : base(applicationState)
        {
            this.declarationVisitor = declarationVisitor;
        }

        public override FunctionDeclarationResult VisitFunction_body([NotNull] Verse.Function_bodyContext context)
        {
            throw new NotImplementedException();
        }

        public override FunctionDeclarationResult VisitFunction_definition([NotNull] Verse.Function_definitionContext context)
        {
            this.ApplicationState.CurrentScopeLevel += 1;
            var name = context.ID();
            var type = context.type();
            var parameter = context.function_param();
            var functionResult = new FunctionDeclarationResult()
            {
                FunctionName = name.GetText(),
            };
            foreach (var param in parameter.children)
            {
                if (param.GetText() == "(" || param.GetText() == ")")
                {
                    continue;
                }
                // Fetches all the parameters
                var parameterSet = param.Accept(this).VariableDeclarations;
                // Filters all the duplicate / null values without requiring an IEqualityComparer
                parameterSet.RemoveAll(x => x == null);
                var finalParameterSet = parameterSet.GroupBy(x => x.Name)
                                                    .Select(g => g.First());
            }

            VisitChildren(context);
            this.ApplicationState.CurrentScopeLevel -= 1;
            return null;
        }

        public override FunctionDeclarationResult VisitParam_def_item([NotNull] Verse.Param_def_itemContext context)
        {
            FunctionDeclarationResult functionDeclarationResult = new();
            List<DeclarationResult> children = new List<DeclarationResult>();
            foreach (var child in context.children)
            {
                // Adds all the parameters in the current node
                children.Add(child.Accept(this.declarationVisitor));
            }

            functionDeclarationResult.VariableDeclarations.AddRange(children);

            // Recursivly checks the children whether or not there are additional parameter_definiton nodes.
            var recursiveCallResult = VisitChildren(context);
            if (recursiveCallResult != null)
                functionDeclarationResult.VariableDeclarations.AddRange(recursiveCallResult.VariableDeclarations);

            return functionDeclarationResult;
        }
    }
}
