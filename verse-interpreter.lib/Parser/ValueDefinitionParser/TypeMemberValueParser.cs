using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using verse_interpreter.lib.Data;
using verse_interpreter.lib.Data.ResultObjects;

namespace verse_interpreter.lib.Parser.ValueDefinitionParser
{
	public class TypeMemberValueParser
	{
		private readonly Lazy<PropertyResolver> _resolver;

		public TypeMemberValueParser(Lazy<PropertyResolver> resolver)
        {
			_resolver = resolver;
		}

        public void ParseMemberAccess(TypeMemberAccessResult memberAccessResult)
		{
			DeclarationResult declarationResult = new DeclarationResult();

			var variable = _resolver.Value.ResolveProperty(memberAccessResult.AbsoluteCall);

			declarationResult.TypeName = variable.Value.TypeData.Name;
			declarationResult.CollectionVariable = variable.Value.CollectionVariable;
			declarationResult.CustomType = variable.Value.CustomType;
			declarationResult.LiteralValue =
				variable!.Value.TypeData.Name == "int" ? variable!.Value.IntValue.ToString() : variable!.Value.StringValue;
		}
	}
}
