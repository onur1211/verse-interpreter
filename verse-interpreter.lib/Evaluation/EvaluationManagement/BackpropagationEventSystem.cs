using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using verse_interpreter.lib.Data;
using verse_interpreter.lib.Data.Expressions;
using verse_interpreter.lib.Data.Interfaces;
using verse_interpreter.lib.Evaluation.Evaluators;
using verse_interpreter.lib.EventArguments;
using verse_interpreter.lib.Factories;
using verse_interpreter.lib.IO;
using verse_interpreter.lib.Lookup.EventArguments;

namespace verse_interpreter.lib.Evaluation.EvaluationManagement
{
    public class BackpropagationEventSystem
    {
        private readonly ApplicationState _applicationState;
        private readonly List<IExpression<ArithmeticExpression>> _arithmeticExpressions;
        private readonly List<IExpression<StringExpression>> _stringExpressions;
        private readonly List<IExpression<ComparisonExpression>> _comparisonExpressions;

        private readonly Dictionary<string, IExpression<ArithmeticExpression>> _associatedArithmeticExpressions;
        private readonly Dictionary<string, IExpression<StringExpression>> _associatedStringExpressions;

        // Keeps track of Expressions which are not yet ready to execute
        // When they are ready to execute, execute them
        // Return their given result
        public BackpropagationEventSystem(ApplicationState applicationState)
        {
            _applicationState = applicationState;

            _arithmeticExpressions = new List<IExpression<ArithmeticExpression>>();
            _stringExpressions = new List<IExpression<StringExpression>>();
            _comparisonExpressions = new List<IExpression<ComparisonExpression>>();

            _associatedArithmeticExpressions = new Dictionary<string, IExpression<ArithmeticExpression>>();
            _associatedStringExpressions = new Dictionary<string, IExpression<StringExpression>>();
        }

        public event EventHandler<StringExpressionResolvedEventArgs> StringExpressionResolved;
        public event EventHandler<ArithmeticExpressionResolvedEventArgs> ArithmeticExpressionResolved;
        public event EventHandler<ComparisonExpressionResolvedEventArgs> ComparisonExpressionResolved;

        public void AddExpression(IExpression<ComparisonExpression> expression)
        {
            _comparisonExpressions.Add(expression);
        }

        public void AddExpression(IExpression<ArithmeticExpression> expression)
        {
            _arithmeticExpressions.Add(expression);
        }

        public void AddExpression(string identifier, IExpression<ArithmeticExpression> expression)
        {
            _associatedArithmeticExpressions.Add(identifier, expression);
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
            for (int i = 0; i < _arithmeticExpressions.Count; i++)
            {
                var res = _arithmeticExpressions[i].PostponedExpression!.Invoke();
                if (res.PostponedExpression == null)
                {
                    ArithmeticExpressionResolved?.Invoke(this, new ArithmeticExpressionResolvedEventArgs(res));
                    _arithmeticExpressions.Remove(_arithmeticExpressions[i]);
                }
            }

            foreach (var expression in _associatedArithmeticExpressions)
            {
                var evaluatedExpression = expression.Value.PostponedExpression!.Invoke();
                if (evaluatedExpression.PostponedExpression == null)
                {
                    _associatedArithmeticExpressions.Remove(expression.Key);
                    _applicationState.CurrentScope.LookupManager.UpdateVariable(new Variable(expression.Key, new ValueObject("int", expression.Value.PostponedExpression.Invoke().ResultValue)));
                }
            }

            for (var i = 0; i < _stringExpressions.Count; i++)
            {
                var expression = _stringExpressions[i];
                var res = expression.PostponedExpression!.Invoke();
                if (res.PostponedExpression == null)
                {
                    StringExpressionResolved?.Invoke(this, new StringExpressionResolvedEventArgs(res));
                    _stringExpressions.Remove(_stringExpressions[i]);
                }
            }

            for(int i = 0; i < _comparisonExpressions.Count ; i++)
            {
                var res = _comparisonExpressions[i].PostponedExpression!.Invoke();
                if (res.PostponedExpression == null)
                {
                    ComparisonExpressionResolved?.Invoke(this, new ComparisonExpressionResolvedEventArgs(res));
                    _comparisonExpressions.Remove(_comparisonExpressions[i]);
                }
            }
        }
    }
}
