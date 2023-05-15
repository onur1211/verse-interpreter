using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using verse_interpreter.lib.Data;
using verse_interpreter.lib.Data.ResultObjects;

namespace verse_interpreter.lib.Evaluators
{
    public class StringExpressionEvaluator : IEvaluator<string, List<List<ExpressionResult>>>
    {
        private ApplicationState _applicationState;

        public StringExpressionEvaluator(ApplicationState applicationState)
        {
            _applicationState = applicationState;
        }

        public string Evaluate(List<List<ExpressionResult>> input)
        {
            StringBuilder concatinatedString = new StringBuilder();
            foreach (var subString in input)
            {
                for (int i = 0; i < subString.Count; i++)
                {
                    try
                    {
                        if(subString[i].Operator != null && OperatorLookUp.IsOperator(subString[i].Operator) && i + 1 == subString.Count - 1)
                        {
                            concatinatedString.Append(FetchValueFromLookUp(subString[i + 1].ValueIdentifier).First().Replace('\"', ' '));
                            continue;
                        }
                        if (subString[i].Operator != null && OperatorLookUp.IsOperator(subString[i].Operator))
                        {
                            concatinatedString.Append(FetchValueFromLookUp(subString[i - 1].ValueIdentifier).First().Replace('\"', ' '));
                            concatinatedString.Append(FetchValueFromLookUp(subString[i + 1].ValueIdentifier).First().Replace('\"', ' '));
                            i += 1;
                        }
                    } catch (Exception ex)
                    {
                        throw new InvalidOperationException("An error occured with the specified input. Check the operators!");
                    }

                }
            }
            return concatinatedString.ToString();
        }

        private List<string> FetchValueFromLookUp(string identifier)
        {
            return _applicationState.Scopes[_applicationState.CurrentScopeLevel].LookupManager.GetVariableStrings(identifier);
        }
    }
}
