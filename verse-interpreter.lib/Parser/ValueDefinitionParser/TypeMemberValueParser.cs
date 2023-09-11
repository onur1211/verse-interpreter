using verse_interpreter.lib.Data;
using verse_interpreter.lib.Data.ResultObjects;

namespace verse_interpreter.lib.Parser.ValueDefinitionParser
{
    public class TypeMemberValueParser
    {
        private readonly PropertyResolver _resolver;

        public TypeMemberValueParser(PropertyResolver resolver)
        {
            _resolver = resolver;
        }

        public void ParseMemberAccess(TypeMemberAccessResult memberAccessResult)
        {
            DeclarationResult declarationResult = new DeclarationResult();

#pragma warning disable CS8604 // Mögliches Nullverweisargument.
            var variable = _resolver.ResolveProperty(memberAccessResult.AbsoluteCall);
#pragma warning restore CS8604 // Mögliches Nullverweisargument.

            declarationResult.TypeName = variable.Value.TypeData.Name;
            declarationResult.CollectionVariable = variable.Value.CollectionVariable;
            declarationResult.CustomType = variable.Value.CustomType;
            declarationResult.LiteralValue =
                variable!.Value.TypeData.Name == "int" ? variable!.Value.IntValue.ToString() : variable!.Value.StringValue;
        }
    }
}
