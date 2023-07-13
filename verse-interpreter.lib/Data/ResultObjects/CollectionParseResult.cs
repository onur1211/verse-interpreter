using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using verse_interpreter.lib.Grammar;

namespace verse_interpreter.lib.Data.ResultObjects
{
    public class CollectionParseResult
    {
        public CollectionParseResult() 
        {
            this.ValueElements = new List<Verse.Value_definitionContext>();
            this.DeclarationElements = new List<Verse.DeclarationContext>();
            this.VariableElements = new List<string>();
        }

        public List<Verse.Value_definitionContext> ValueElements { get; set; }

        public List<Verse.DeclarationContext> DeclarationElements { get; set; }

        public List<string> VariableElements { get; set; }
    }
}
