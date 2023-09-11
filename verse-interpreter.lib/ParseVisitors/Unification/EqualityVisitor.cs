using verse_interpreter.lib.Converter;
using verse_interpreter.lib.Data;
using verse_interpreter.lib.Evaluation.EvaluationManagement;

namespace verse_interpreter.lib.ParseVisitors.Unification
{
    public class EqualityVisitor
    {
        private readonly ApplicationState _state;
        private readonly Lazy<TypeInferencer> _inferencer;
        private readonly Lazy<PropertyResolver> _propertyResolver;
        private readonly Lazy<ValueDefinitionVisitor> _valueDefinitionVisitor;
        private readonly GeneralEvaluator _generalEvaluator;

        public EqualityVisitor(ApplicationState applicationState,
                                 Lazy<TypeInferencer> typeInferencer,
                                 Lazy<PropertyResolver> propertyResolver,
                                 Lazy<ValueDefinitionVisitor> valueDefinitionVisitor,
                                 GeneralEvaluator generalEvaluator
            )

        {
            _state = applicationState;
            _inferencer = typeInferencer;
            _generalEvaluator = generalEvaluator;
            _propertyResolver = propertyResolver;
            _state.CurrentScope.LookupManager.VariableBound += _generalEvaluator.Propagator.HandleVariableBound!;
            _valueDefinitionVisitor = valueDefinitionVisitor;
        }

        public DeclarationResult ParseEquality(DeclarationResult declarationResult)
        {
            // Get the name of the variable
            // Example: x=2 or x=y then get 'x'

            // Check if the name is a declared variable.
            if (!_state.CurrentScope.LookupManager.IsVariable(declarationResult.Name))
            {
                throw new InvalidOperationException($"Invalid usage of out-of-scope variable {declarationResult.Name}");
            }

            // Get the variable on the left side.
            Variable actualVariable = _propertyResolver.Value.ResolveProperty(declarationResult.Name);

            // Get the value from the right side as an anonym variable.
            Variable value = VariableConverter.Convert(declarationResult);


            // Due to the fact that the both variable currently may still get unified at later point,
            // leave it there and wait for it to bind
            if (!actualVariable.HasValue() && !value.HasValue())
            {
                return declarationResult;
            }

            // If there is no variable or value then return null and let the
            // declaration parser handle the value assignement.
            if (actualVariable == null || value == null)
            {
                return null!;
            }

            // Try to unify the variable with the given value
            // If the unification fails then return false?
            if (!TryUnification(actualVariable, value))
            {
                return new DeclarationResult
                {
                    Name = declarationResult.Name,
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

            switch (true)
            {
                case true when (!variable.HasValue() || !secondVariable.HasValue()) && variable.Value.TypeData.Name == secondVariable.Value.TypeData.Name:
                    return true;

                // Int value equality
                case true when variable.Value!.IntValue != null && secondVariable.Value.IntValue != null:
                    return variable.Value.IntValue.Value == secondVariable.Value.IntValue.Value;

                // String value equality
                case true when variable.Value!.StringValue != null && secondVariable.Value.StringValue != null:
                    return variable.Value.StringValue == secondVariable.Value.StringValue;

                // false? equality
                case true when variable.Value.TypeData.Name == "false?" && secondVariable.Value.TypeData.Name == "false?":
                    return true;

                // Collection equality
                case true when variable.Value.CollectionVariable != null && secondVariable.Value.CollectionVariable != null:
                    return TryUnificationWithArray(variable.Value.CollectionVariable, secondVariable.Value.CollectionVariable);

                // Choice equality
                case true when variable.Value!.Choice != null && secondVariable.Value.Choice != null:
                    return TryUnificationWithChoice(variable.Value.Choice.AllChoices(), secondVariable.Value.Choice.AllChoices());

                default:
                    return false;
            }
        }

        private bool TryUnificationWithArray(VerseCollection collection, VerseCollection secondCollection)
        {
            // Check if the collections have the same count.
            // If not then unifications fails and return false.
            if (collection.Values.Count != secondCollection.Values.Count)
            {
                return false;
            }

            // Check if the values of the elements are the same.
            // If there is at least one element which doesnt match
            // then unification fails and return false.
            for (int i = 0; i < collection.Values.Count; i++)
            {
                Variable variable = collection.Values[i];
                Variable secondVariable = secondCollection.Values[i];

                switch (true)
                {
                    case true when variable.Value!.IntValue != null && secondVariable.Value.IntValue != null:
                        if (variable.Value.IntValue.Value != secondVariable.Value.IntValue.Value)
                        {
                            return false;
                        }
                        break;

                    case true when (!variable.HasValue() || !secondVariable.HasValue()) && variable.Value.TypeData.Name == secondVariable.Value.TypeData.Name:
                        break;

                    case true when variable.Value!.StringValue != null && secondVariable.Value.StringValue != null:
                        if (variable.Value.StringValue != secondVariable.Value.StringValue)
                        {
                            return false;
                        }
                        break;

                    case true when variable.Value.TypeData.Name == "false?" && secondVariable.Value.TypeData.Name == "false?":
                        break;

                    case true when variable.Value.CollectionVariable != null && secondVariable.Value.CollectionVariable != null:
                        if (!TryUnificationWithArray(variable.Value.CollectionVariable, secondVariable.Value.CollectionVariable))
                        {
                            return false;
                        }
                        break;

                    case true when variable.Value!.Choice != null && secondVariable.Value.Choice != null:
                        if (!TryUnificationWithChoice(variable.Value.Choice.AllChoices(), secondVariable.Value.Choice.AllChoices()))
                        {
                            return false;
                        }
                        break;

                    default:
                        return false;
                }
            }

            return true;
        }

        private bool TryUnificationWithChoice(IEnumerable<Data.Variables.Choice> choice, IEnumerable<Data.Variables.Choice> secondChoice)
        {
            return false;
        }

        private Variable GetValueFromContext(DeclarationResult declarationResult)
        {

            //if (context.value_definition() != null)
            //{
            //    Variable anonymVariable = VariableConverter.Convert(context.value_definition().Accept(_valueDefinitionVisitor.Value)!);
            //    string value = context.value_definition().GetText();
            //    if (anonymVariable.Value.Choice != null)
            //    {
            //        return null!;
            //    }

            //    // Check if the value is a variable and if true get the value from it
            //    // Example: x=y then y is the value as a variable
            //    if (_state.CurrentScope.LookupManager.IsVariable(value))
            //    {
            //        return _propertyResolver.Value.ResolveProperty(value);
            //    }
            //    else
            //    {
            //        return anonymVariable;
            //    }
            //}

            return null!;
        }
    }
}