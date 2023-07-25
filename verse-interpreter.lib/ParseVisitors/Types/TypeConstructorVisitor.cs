using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using verse_interpreter.lib.Converter;
using verse_interpreter.lib.Data;
using verse_interpreter.lib.Data.CustomTypes;
using verse_interpreter.lib.Grammar;
using verse_interpreter.lib.Visitors;

namespace verse_interpreter.lib.ParseVisitors.Types
{
    // EXAM_UPDATED
    public class TypeConstructorVisitor : AbstractVerseVisitor<CustomType>
    {
        private readonly Lazy<ValueDefinitionVisitor> _valueDefinitionVisitor;
        private readonly CustomTypeFactory factory;

        public TypeConstructorVisitor(ApplicationState applicationState,
                                      Lazy<ValueDefinitionVisitor> valueDefinitionVisitor,
                                      CustomTypeFactory factory) : base(applicationState)
        {
            _valueDefinitionVisitor = valueDefinitionVisitor;
            this.factory = factory;
        }

        public override CustomType VisitConstructor_body([NotNull] Verse.Constructor_bodyContext context)
        {
            var constructorName = context.ID().GetText();
            var constructorElements = context.param_def_item();

            if (!ApplicationState.Types.Any(x => x.Value.ConstructorName == constructorName))
            {
                throw new InvalidOperationException($"The specified constructor \"{constructorName}\" is unknown!");
            }
            var fetchedType = factory.GetCustomType(constructorName);
            FetchParameters(constructorElements, fetchedType);

            return fetchedType;
        }

        private void FetchParameters(Verse.Param_def_itemContext context, CustomType baseType)
        {
            if (context == null)
            {
                return;
            }
            var name = context.declaration().ID().GetText();
            var value = context.declaration().value_definition().Accept(_valueDefinitionVisitor.Value)!;
            value.Name = name;

            baseType.LookupManager.UpdateVariable(VariableConverter.Convert(value));
            var child = context.param_def_item();
            if (child != null)
            {
                FetchParameters(child, baseType);
            }
        }
    }
}
