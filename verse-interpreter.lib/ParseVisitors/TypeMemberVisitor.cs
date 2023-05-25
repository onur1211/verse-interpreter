using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using verse_interpreter.lib.Data.ResultObjects;
using verse_interpreter.lib.Grammar;

namespace verse_interpreter.lib.ParseVisitors
{
    public class TypeMemberVisitor : AbstractVerseVisitor<TypeMemberAccessResult>
    {
        private readonly ApplicationState _applicationState;
        private readonly ValueDefinitionVisitor _valueDefinitionVisitor;

        public TypeMemberVisitor(ApplicationState applicationState,
                                 ValueDefinitionVisitor valueDefinitionVisitor) : base(applicationState)
        {
            _applicationState = applicationState;
            _valueDefinitionVisitor = valueDefinitionVisitor;
        }

        public override TypeMemberAccessResult VisitType_member_definition([NotNull] Verse.Type_member_definitionContext context)
        {
            var identfier = context.type_member_access().Accept(this);

            var value = Converter.VariableConverter.Convert(context.value_definition().Accept(_valueDefinitionVisitor));
            value.Name = identfier.PropertyName;

            var instance = _applicationState.CurrentScope.LookupManager.GetVariable(identfier.VariableName).Value.DynamicType;
            instance.LookupManager.UpdateVariable(value);
            return base.VisitType_member_definition(context);
        }

        public override TypeMemberAccessResult VisitType_member_access([NotNull] Verse.Type_member_accessContext context)
        {
            TypeMemberAccessResult accessResult = new TypeMemberAccessResult();
            var propertyIdentfier = context.GetText().Split('.');

            return FetchChildrenRecursivly(propertyIdentfier, accessResult);
        }

        private TypeMemberAccessResult FetchChildrenRecursivly(string[] children, TypeMemberAccessResult finalResult)
        {
            // Can't happen I think
            if (children.Length == 0)
                return finalResult;
            // Only the property itself is accessed
            if (children.Length == 2)
            {
                finalResult.VariableName = children[0];
                finalResult.PropertyName = children[1];
                return finalResult;
            }
            // only the variable itself is accessed
            if (children.Length == 1)
            {
                finalResult.VariableName = children[0];
                return finalResult;
            }

            // If there is a property access of a child object within a class then go recursivly deeper to build the actual access object
            // Example: x.neighbour.age
            // Where neighbour is of type "Person"
            finalResult.VariableName = children[0];
            finalResult.PropertyName = children[1];
            finalResult.ChildResult = FetchChildrenRecursivly(children.Skip(2).ToArray(), new TypeMemberAccessResult());

            return finalResult;
        }
    }
}
