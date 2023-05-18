using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using verse_interpreter.lib.Data;
using verse_interpreter.lib.Data.ResultObjects;
using verse_interpreter.lib.Evaluators;
using verse_interpreter.lib.Factories;
using verse_interpreter.lib.Grammar;
using verse_interpreter.lib.Visitors;

namespace verse_interpreter.lib
{
    public class DeclarationParser
    {
        private ApplicationState _state;
        private TypeInferencer _inferencer;
        private readonly ValueDefinitionVisitor _valueDefinitionVisitor;

        public DeclarationParser(ApplicationState applicationState,
                                 TypeInferencer typeInferencer,
                                 ValueDefinitionVisitor valueDefinitionVisitor)
        {
            _state = applicationState;
            _inferencer = typeInferencer;
            _valueDefinitionVisitor = valueDefinitionVisitor;
        }

        public DeclarationResult ParseDeclaration(Verse.DeclarationContext context)
        {
            var assignmentOperatorKind = context.children[1].GetText();
            return assignmentOperatorKind switch
            {
                ":" => ParseBringToScopeOperator(context),
                "=" => ParseAssignValueToExistingVariable(context),
                ":=" => ParseGiveValueOperator(context),
                _ => throw new NotImplementedException(),
            };
        }

        private DeclarationResult ParseBringToScopeOperator(Verse.DeclarationContext context)
        {
            string name = context.ID().GetText();
            string type = context.type().GetText();

            if (!(_state.Types.ContainsKey(type) || _state.WellKnownTypes.Contains(type)))
            {
                throw new InvalidOperationException($"The specified type \"{type}\" does not exist!");
            }

            return new DeclarationResult()
            {
                Name = name,
                TypeName = type,
            };
        }

        private DeclarationResult ParseGiveValueOperator(Verse.DeclarationContext context)
        {
            var variable = ParseValueAssignment(context);
            return _inferencer.InferGivenType(variable);
        }

        private DeclarationResult ParseAssignValueToExistingVariable(Verse.DeclarationContext context)
        {
            var variableName = context.ID().GetText();
            if (!_state.CurrentScope.LookupManager.IsVariable(variableName))
            {
                throw new InvalidOperationException($"Invalid usage of out of scope variable {nameof(variableName)}");
            }

            return ParseValueAssignment(context);
        }

        /// <summary>
        /// Calls a <paramref type="ValueDefinitionVisitor"/> which fetches the values out of the parse tree.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private DeclarationResult ParseValueAssignment(Verse.DeclarationContext context)
        {
            DeclarationResult declarationResult = _valueDefinitionVisitor.Visit(context);
            declarationResult.Name = context.ID().GetText();

            if (declarationResult.CollectionVariable != null) 
            {
                declarationResult.CollectionVariable.Name = declarationResult.Name;
            }

            return declarationResult;
        }
    }
}
