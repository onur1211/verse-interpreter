using Antlr4.Runtime.Misc;
using verse_interpreter.lib.Converter;
using verse_interpreter.lib.Data.Variables;
using verse_interpreter.lib.Grammar;

namespace verse_interpreter.lib.ParseVisitors
{
    public class ChoiceConversionVisitor : AbstractVerseVisitor<Choice>
    {
        private readonly PropertyResolver _propertyResolver;
        private readonly Lazy<ValueDefinitionVisitor> _valueDefinitionVisitor;

        public ChoiceConversionVisitor(ApplicationState applicationState,
                                       PropertyResolver resolver,
                                       Lazy<ValueDefinitionVisitor> valueDefinitionVisitor) : base(applicationState)
        {
            _propertyResolver = resolver;
            _valueDefinitionVisitor = valueDefinitionVisitor;
        }

        public override Choice VisitQuestionmark_operator([NotNull] Verse.Questionmark_operatorContext context)
        {
            var variableIdentifier = context.ID();
            var arrayLiteral = context.array_literal();

            if (variableIdentifier != null)
            {
                var variable = _propertyResolver.ResolveProperty(variableIdentifier.GetText());
                return ChoiceConverter.Convert(variable.Value.CollectionVariable);
            }
            if (arrayLiteral != null)
            {
                var literal = _valueDefinitionVisitor.Value.Visit(arrayLiteral);
#pragma warning disable CS8604 // Mögliches Nullverweisargument.
                return ChoiceConverter.Convert(VariableConverter.Convert(literal).Value.CollectionVariable);
#pragma warning restore CS8604 // Mögliches Nullverweisargument.
            }
            else
            {
                throw new NotImplementedException("The specified conversion to a choice is currently not supported!");
            }
        }
    }
}
