using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using verse_interpreter.lib.Data.ResultObjects;
using Z.Expressions;

namespace verse_interpreter.lib.Evaluators
{
    public class ArithmeticEvaluator : IEvaluator<int, List<List<ExpressionResult>>>
    {
        private readonly ApplicationState _applicationState;

        public ArithmeticEvaluator(ApplicationState applicationState) 
        {
            _applicationState = applicationState;
        }

        public int Evaluate(List<List<ExpressionResult>> input)
        {
            StringBuilder stringBuilder = new StringBuilder();
       

            foreach (var expressionList in input)
            {
                stringBuilder.Append('(');

                foreach (var value in expressionList) 
                {
                    if (this._applicationState.Scopes[1].LookupManager.IsVariable(value.ValueIdentifier.ToString()))
                    {
                        var variableValue = this._applicationState.Scopes[1].LookupManager.GetVariableInts(value.ValueIdentifier.ToString());
                        stringBuilder.Append(variableValue.First());
                        continue;
                    }

                    if (!String.IsNullOrEmpty(value.Operator))
                    {
                        stringBuilder.Append(value.Operator.ToString());
                        continue;
                    }

                    if (value.Value != null)
                    {
                        stringBuilder.Append(value.Value.ToString());
                        continue;
                    }
                }
            }

            for (int i = 0; i < input.Count; i++)
            {
                stringBuilder.Append(')');
            }

            var result = Eval.Execute<int>(stringBuilder.ToString());
            return result;
        }
    }
}
