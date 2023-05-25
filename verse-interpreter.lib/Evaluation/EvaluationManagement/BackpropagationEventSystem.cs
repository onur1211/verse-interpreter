using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using verse_interpreter.lib.Data;
using verse_interpreter.lib.Data.Expressions;
using verse_interpreter.lib.Data.Interfaces;
using verse_interpreter.lib.Factories;
using verse_interpreter.lib.IO;
using verse_interpreter.lib.Lookup.EventArguments;

namespace verse_interpreter.lib.Evaluation.EvaluationManagement
{
    public class BackpropagationEventSystem
    {
        private readonly ApplicationState _applicationState;
        private List<IExpression<ArithmeticExpression>> _arithmeticExpressions;
        private List<IExpression<StringExpression>> _stringExpressions;

        private Dictionary<string, IExpression<ArithmeticExpression>> _associatedArithmeticExpressions;
        private Dictionary<string, IExpression<StringExpression>> _associatedStringExpressions;

        // Keeps track of Expressions which are not yet ready to execute
        // When they are ready to execute, execute them
        // Return their given result
        public BackpropagationEventSystem(ApplicationState applicationState)
        {
            _applicationState = applicationState;

            _arithmeticExpressions = new List<IExpression<ArithmeticExpression>>();
            _stringExpressions = new List<IExpression<StringExpression>>();

            _associatedArithmeticExpressions = new Dictionary<string, IExpression<ArithmeticExpression>>();
            _associatedStringExpressions = new Dictionary<string, IExpression<StringExpression>>();
        }

        public void AddExpression(IExpression<ArithmeticExpression> expression)
        {
            _arithmeticExpressions.Add(expression);
        }

        public void AddExpression(string identfier, IExpression<ArithmeticExpression> expression)
        {
            _associatedArithmeticExpressions.Add(identfier, expression);
        }

        public void AddExpression(IExpression<StringExpression> expression)
        {
            _stringExpressions.Add(expression);
        }

        public void AddExpression(string identfier, IExpression<StringExpression> expression)
        {
            _associatedStringExpressions.Add(identfier, expression);
        }

        /// <summary>
        /// After a variable is bound, this callback checks if the expressions are ready to be evaluated or not by calling them 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        public void HandleVariableBound(object sender, VariableBoundEventArgs eventArgs)
        {
            foreach (var element in _arithmeticExpressions)
            {
                var res = element.PostponedExpression.Invoke();
                if (res.PostponedExpression == null)
                {
                    Printer.PrintResult(res.ResultValue.ToString()!);
                }
            }

            foreach (var expression in _associatedArithmeticExpressions)
            {
                var evaluatedExpression = expression.Value.PostponedExpression.Invoke();
                if (evaluatedExpression.PostponedExpression == null)
                {
                    _associatedArithmeticExpressions.Remove(expression.Key);
                    _applicationState.CurrentScope.LookupManager.UpdateVariable(new Variable(expression.Key, new("int", expression.Value.PostponedExpression.Invoke().ResultValue)));
                }
            }
        }
    }
}
