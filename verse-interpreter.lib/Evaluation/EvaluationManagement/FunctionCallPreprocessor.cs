﻿using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using verse_interpreter.lib.Data.Interfaces;
using verse_interpreter.lib.Exceptions;
using verse_interpreter.lib.ParseVisitors;

namespace verse_interpreter.lib.Evaluation.EvaluationManagement
{
    public class FunctionCallPreprocessor
    {
        private readonly IValidator<FunctionCallItem> _functionCallValidator;

        public FunctionCallPreprocessor(IValidator<FunctionCallItem> functionCallValidator)
        {
            _functionCallValidator = functionCallValidator;
        }

        public void BuildExecutableFunction(FunctionCallItem item)
        {
            if(!IsArityEqual(item))
            {
                throw new NotEqualArityException($"The arity of the function is {item.Function.ParameterCount}, but only {item.Parameters.ParameterCount} parameters were given!");
            }
            if(!_functionCallValidator.IsTypeConformityGiven(item))
            {
                throw new InvalidTypeCombinationException("The given parameters do not match the signature of the function!");
            }

            BindValues(item);
        }

        private bool IsArityEqual(FunctionCallItem item)
        {
            return item.Parameters.ParameterCount == item.Function.ParameterCount;
        }

        private void BindValues(FunctionCallItem item)
        {
            for(int i = 0; i < item.Function.ParameterCount; i++)
            {
                var variable  = item.Function.Parameters[i];
                variable.Value = item.Parameters.Parameters[i].Value;
                item.Function.AddScopedVariable(variable);
            }
        }
    }
}