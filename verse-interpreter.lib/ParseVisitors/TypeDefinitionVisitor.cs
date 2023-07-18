﻿using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using verse_interpreter.lib.Grammar;

namespace verse_interpreter.lib.ParseVisitors
{
    public class TypeDefinitionVisitor : AbstractVerseVisitor<CustomType>
    {
        private DeclarationVisitor _declarationVisitor;
        private CustomType _customType;

        public TypeDefinitionVisitor(ApplicationState applicationState,
                                     DeclarationVisitor declarationVisitor) : base(applicationState)
        {
            _declarationVisitor = declarationVisitor;
            _customType = null!;
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

        public override CustomType VisitType_body([NotNull] Verse.Type_bodyContext context)
        {
            // Gets all the variables and adds it to the classes scope
            var res = context.declaration().Accept(this._declarationVisitor);
            _customType.AddScopedVariable(res);
            return this.VisitChildren(context);
        }
    }
}
