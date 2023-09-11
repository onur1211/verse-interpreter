﻿using Antlr4.Runtime.Misc;
using verse_interpreter.lib.Data.ResultObjects;
using verse_interpreter.lib.Grammar;

namespace verse_interpreter.lib.ParseVisitors
{
    public class ChoiceVisitor : AbstractVerseVisitor<ChoiceResult>
    {
        private readonly ChoiceArrayIndexingVisitor _indexingVisitor;
        private readonly ValueDefinitionVisitor valueDefinitionVisitor;

        public ChoiceVisitor(ApplicationState applicationState,
                             ChoiceArrayIndexingVisitor indexingVisitor,
                             ValueDefinitionVisitor valueDefinitionVisitor) : base(applicationState)
        {
            _indexingVisitor = indexingVisitor;
            this.valueDefinitionVisitor = valueDefinitionVisitor;
        }

        public override ChoiceResult VisitChoice_rule([NotNull] Verse.Choice_ruleContext context)
        {
            return GenerateChoiceResult(context, new());
        }

        public override ChoiceResult VisitChoice([NotNull] Verse.ChoiceContext context)
        {
            return GenerateChoiceResult(context, new());
        }

        private ChoiceResult GenerateChoiceResult(Verse.Choice_ruleContext context, ChoiceResult result)
        {
            ParseChoices(context.value_definition(), result);
            ParseLiterals(context.value_definition(), result);

            foreach (var childNodes in context.choice_rule())
            {
                result.Next = new ChoiceResult();
                GenerateChoiceResult(childNodes, result.Next);
            }

            return result;
        }

        private ChoiceResult GenerateChoiceResult(Verse.ChoiceContext context, ChoiceResult result)
        {
            var current = result;

            // later used in order to determine if a new "next" object is needed within the choice;
            var last = context.value_definition().Last();
            foreach (var element in context.value_definition())
            {
                while (current.Next != null)
                {
                    current = current.Next;
                }

                current = ParseChoices(element, current);
                current = ParseLiterals(element, current);
                if (element != last)
                {
                    current.Next = new ChoiceResult();
                }
            }

            return result;
        }

        private ChoiceResult ParseChoices(Verse.Value_definitionContext context, ChoiceResult result)
        {
            if (context == null)
            {
                return result;
            }

            var arrayIndex = _indexingVisitor.Visit(context);
            if (arrayIndex == null)
            {
                return result;
            }
            result.IndexingResults.Add(arrayIndex);

            return result;
        }

        private ChoiceResult ParseLiterals(Verse.Value_definitionContext context, ChoiceResult result)
        {
            if (context == null)
            {
                return result;
            }
            var declarationResult = valueDefinitionVisitor.Visit(context);
#pragma warning disable CS8602 // Dereferenzierung eines möglichen Nullverweises.
            if (declarationResult.TypeName == "false?")
            {
                return result;
            }
#pragma warning restore CS8602 // Dereferenzierung eines möglichen Nullverweises.

            result.Literals.Add(Converter.VariableConverter.Convert(declarationResult));
            return result;
        }
    }
}
