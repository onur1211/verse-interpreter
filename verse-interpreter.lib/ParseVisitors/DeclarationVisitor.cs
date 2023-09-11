﻿using verse_interpreter.lib.Converter;
using verse_interpreter.lib.Data;
using verse_interpreter.lib.Grammar;
using verse_interpreter.lib.Parser;

namespace verse_interpreter.lib.ParseVisitors
{
    public class DeclarationVisitor : AbstractVerseVisitor<Variable>
    {
        private DeclarationParser _parser;

        public DeclarationVisitor(ApplicationState applicationState,
                                  DeclarationParser declarationParser) : base(applicationState)
        {
            _parser = declarationParser;
        }

        public override Variable VisitDeclaration([Antlr4.Runtime.Misc.NotNull] Verse.DeclarationContext context)
        {
            return VariableConverter.Convert(_parser.ParseDeclaration(context));
        }
    }
}
