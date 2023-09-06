using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using verse_interpreter.lib.Converter;
using verse_interpreter.lib.Data.ResultObjects.Validators;
using verse_interpreter.lib.Data;
using verse_interpreter.lib.Evaluation.EvaluationManagement;
using verse_interpreter.lib.Grammar;
using System.Diagnostics.CodeAnalysis;

namespace verse_interpreter.lib.ParseVisitors.Unification
{
    public class EqualityVisitor
    {
        private readonly ApplicationState _state;
        private readonly TypeInferencer _inferencer;
        private readonly GeneralEvaluator _generalEvaluator;
        private readonly PropertyResolver _propertyResolver;
        private readonly ValueDefinitionVisitor _valueDefinitionVisitor;


        public EqualityVisitor(ApplicationState applicationState,
                                 TypeInferencer typeInferencer,
                                 GeneralEvaluator generalEvaluator,
                                 PropertyResolver propertyResolver,
                                 ValueDefinitionVisitor valueDefinitionVisitor)

        {
            _state = applicationState;
            _inferencer = typeInferencer;
            _generalEvaluator = generalEvaluator;
            _propertyResolver = propertyResolver;
            _state.CurrentScope.LookupManager.VariableBound += _generalEvaluator.Propagator.HandleVariableBound!;
            _valueDefinitionVisitor = valueDefinitionVisitor;
        }

        public DeclarationResult ParseEquality(Verse.DeclarationContext context)
        {
            // Get the name of the variable
            // Example: x=2 or x=y then get 'x'
            var variableName = context.ID().GetText();

            // Check if the name is a variable
            if (!_state.CurrentScope.LookupManager.IsVariable(variableName))
            {
                throw new InvalidOperationException($"Invalid usage of out-of-scope variable {variableName}");
            }

            Variable actualVariable = _propertyResolver.ResolveProperty(variableName);

            if (actualVariable.Value.CollectionVariable != null) 
            {
                return null!;
            }

            Variable value = GetValueFromContext(context);

            // If there is no value then return null and let the
            // declaration parser handle the value assignement.
            if (value == null)
            {
                return null!;
            }

            // Try to unify the variable with the given value
            // If the unification fails then return false?
            if (!TryUnification(actualVariable, value))
            {
                return new DeclarationResult
                {
                    Name = variableName,
                    TypeName = "false?"
                };
            }

            return null!;
        }

        private bool TryUnification(Variable variable, Variable secondVariable)
        {
            if (variable == null || secondVariable == null)
            {
                return false;
            }

            if (variable.Value.TypeData.Name != secondVariable.Value.TypeData.Name) 
            { 
                return false; 
            }

            switch (variable.Value)
            {
                case object when variable.Value!.IntValue != null && secondVariable.Value.IntValue != null:
                    return variable.Value.IntValue.Value == secondVariable.Value.IntValue.Value;

                case object when variable.Value!.StringValue != null && secondVariable.Value.StringValue != null:
                    return variable.Value.StringValue == secondVariable.Value.StringValue;

                case object when variable.Value.TypeData.Name == "false?" && secondVariable.Value.TypeData.Name == "false?":
                    return true;

                case object when variable.Value.CollectionVariable != null && secondVariable.Value.CollectionVariable != null:
                    if (variable.Value.CollectionVariable.Values.Count != secondVariable.Value.CollectionVariable.Values.Count)
                    {
                        return false;
                    }
                    return true;

                default:
                    return false;
            }
        }

        private Variable GetValueFromContext(Verse.DeclarationContext context)
        {
            if (context.value_definition() != null)
            {
                Variable anonymVariable = VariableConverter.Convert(context.value_definition().Accept(_valueDefinitionVisitor)!);
                string value = context.value_definition().GetText();

                // Check if the value is a variable and if true get the value from it
                // Example: x=y then y is the value as a variable
                if (_state.CurrentScope.LookupManager.IsVariable(value))
                {
                    return _propertyResolver.ResolveProperty(value);
                }
                else
                {
                    return anonymVariable;
                }
            }
            else if (context.choice_rule() != null)
            {
                return null!;
            }

            return null!;
        }
    }
}