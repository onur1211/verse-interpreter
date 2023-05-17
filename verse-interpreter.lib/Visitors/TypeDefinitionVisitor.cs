using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using verse_interpreter.lib.Grammar;

namespace verse_interpreter.lib.Visitors
{
    // EXAM_UPDATED
    public class TypeDefinitionVisitor : AbstractVerseVisitor<DynamicType>
    {
        private DeclarationVisitor _declarationVisitor;
        private DynamicType _dynamicType;

        public TypeDefinitionVisitor(ApplicationState applicationState,
                                     DeclarationVisitor declarationVisitor) : base(applicationState)
        {
            _declarationVisitor = declarationVisitor;
            _dynamicType = null!;
        }


        public override DynamicType VisitType_header([NotNull] Verse.Type_headerContext context)
        {
            _dynamicType = new DynamicType();
            var identfiers = context.ID();

            _dynamicType.Name = identfiers[0].GetText();
            _dynamicType.ConstructorName = identfiers[1].GetText();

            base.VisitType_header(context);

            return _dynamicType;
        }

        public override DynamicType VisitType_body([NotNull] Verse.Type_bodyContext context)
        {
            var res = context.declaration().Accept(this._declarationVisitor);
            _dynamicType.AddScopedVariable(res);
            return this.VisitChildren(context);
        }
    }
}
