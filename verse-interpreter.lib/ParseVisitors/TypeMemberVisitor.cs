using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using verse_interpreter.lib.Data.ResultObjects;
using verse_interpreter.lib.Grammar;

namespace verse_interpreter.lib.Visitors
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
            var value = context.value_definition().Accept(_valueDefinitionVisitor);
            //var instance = _applicationState.CurrentScope.LookupManager.GetInstanceVariable(identfier.VariableName);
            throw new NotImplementedException();
            //instance.LookupManager.UpdateVariable(identfier.PropertyName, value);
            return base.VisitType_member_definition(context);
        }

        public override TypeMemberAccessResult VisitType_member_access([NotNull] Verse.Type_member_accessContext context)
        {
            var propertyIdentfier = context.GetText().Split('.');

            return new TypeMemberAccessResult(propertyIdentfier[0], propertyIdentfier[1]);
        }
    }
}
