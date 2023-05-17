using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using verse_interpreter.lib.Data.DataVisitors;
using verse_interpreter.lib.Data.ResultObjects;
using verse_interpreter.lib.Grammar;

namespace verse_interpreter.lib.Visitors
{
    public class TypeMemberVisitor : AbstractVerseVisitor<TypeMemberAccessResult>
    {
        private readonly ApplicationState _applicationState;
        private readonly ValueDefinitionVisitor _valueDefinitionVisitor;
        private readonly VariableVisitor _variableVisitor;

        public TypeMemberVisitor(ApplicationState applicationState,
                                 ValueDefinitionVisitor valueDefinitionVisitor,
                                 VariableVisitor variableVisitor) : base(applicationState)
        {
            _applicationState = applicationState;
            _valueDefinitionVisitor = valueDefinitionVisitor;
            _variableVisitor = variableVisitor;
        }

        public override TypeMemberAccessResult VisitType_member_definition([NotNull] Verse.Type_member_definitionContext context)
        {
            var identfier = context.type_member_access().Accept(this);
            var value = Converter.VariableConverter.Convert(context.value_definition().Accept(_valueDefinitionVisitor));
            var instance = _applicationState.CurrentScope.LookupManager.GetVariable(identfier.VariableName).AcceptDynamicType(_variableVisitor);
            instance.LookupManager.UpdateVariable(value);
            return base.VisitType_member_definition(context);
        }

        public override TypeMemberAccessResult VisitType_member_access([NotNull] Verse.Type_member_accessContext context)
        {
            var propertyIdentfier = context.GetText().Split('.');

            return new TypeMemberAccessResult(propertyIdentfier[0], propertyIdentfier[1]);
        }
    }
}
