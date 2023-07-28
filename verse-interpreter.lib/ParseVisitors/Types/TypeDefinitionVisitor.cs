using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using verse_interpreter.lib.Data.CustomTypes;
using verse_interpreter.lib.Grammar;

namespace verse_interpreter.lib.ParseVisitors.Types
{
    public class TypeDefinitionVisitor : AbstractVerseVisitor<CustomType>
    {
        private DeclarationVisitor _declarationVisitor;
        private CustomType _customType;

        public TypeDefinitionVisitor(ApplicationState applicationState,
                                     DeclarationVisitor declarationVisitor) : base(applicationState)
        {
            _declarationVisitor = declarationVisitor;
            _customType = default!;
        }


        public override CustomType VisitType_header([NotNull] Verse.Type_headerContext context)
        {
            _customType = new CustomType();
            var identfiers = context.ID();

            // Fetches the name of the class and it's constructor
            _customType.Name = identfiers[0].GetText();
            _customType.ConstructorName = identfiers[1].GetText();

            base.VisitType_header(context);

            return _customType;
        }

		public override CustomType VisitMulti_declaration([NotNull] Verse.Multi_declarationContext context)
		{
			// Gets all the variables and adds it to the classes scope
			var res = context.declaration().Accept(_declarationVisitor);
			_customType.AddScopedVariable(res);
			return VisitChildren(context);
		}
    }
}
