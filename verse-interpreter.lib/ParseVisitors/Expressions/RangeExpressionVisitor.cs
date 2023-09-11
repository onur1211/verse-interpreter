using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using verse_interpreter.lib.Converter;
using verse_interpreter.lib.Data;
using verse_interpreter.lib.Grammar;

namespace verse_interpreter.lib.ParseVisitors.Expressions
{
    public class RangeExpressionVisitor : AbstractVerseVisitor<DeclarationResult>
    {
        private readonly ApplicationState _state;
        private readonly TypeInferencer _typeInferencer;

        public RangeExpressionVisitor(ApplicationState applicationState,
                                      TypeInferencer typeInferencer) : base(applicationState)
        {
            _state = applicationState;
            _typeInferencer = typeInferencer;
        }

        public override DeclarationResult VisitRange_expression([NotNull] Verse.Range_expressionContext context)
        {
            // Get the numbers from the range expression
            // Example: 1..10 -> Get 1 and 10
            List<int> rangeNumbers = new List<int>();
            List<int> result = new List<int>();
            int start;
            int step;
            int end;

            for (int i = 0; i < context.INT().Length; i++)
            {
                int num = int.Parse(context.INT(i).GetText());
                rangeNumbers.Add(num);
            }

            // Check if a step like 1,3..10 or just 1..10 is given.
            if (rangeNumbers.Count > 2)
            {
                start = rangeNumbers.First();
                step = rangeNumbers.ElementAt(1) - start;
                end = rangeNumbers.Last();
            }
            else
            {
                start = rangeNumbers.First();
                step = 1;
                end = rangeNumbers.Last();
            }

            // Add the numbers within the range to the list.
            for (int i = start; i <= end; i += step)
            {
                result.Add(i);
            }

            // Create anonym variables from the values of the result.
            List<Variable> anonymVariables = new List<Variable>();

            foreach (var value in result)
            {
                anonymVariables.Add(new Variable("undefined", new ValueObject("int", value)));
            }

            DeclarationResult declarationResult = new DeclarationResult();
            declarationResult.CollectionVariable = new VerseCollection(anonymVariables);
            declarationResult.CollectionVariable.Values = anonymVariables;

            var choice = ChoiceConverter.Convert(declarationResult.CollectionVariable!);
            var choiceResult = ChoiceConverter.Convert(choice);

            declarationResult = new DeclarationResult();
            declarationResult.ChoiceResult = choiceResult;
            return _typeInferencer.InferGivenType(declarationResult);
        }
    }
}
