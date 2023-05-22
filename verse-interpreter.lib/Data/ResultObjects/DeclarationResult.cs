using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using verse_interpreter.lib.Data.Expressions;
using verse_interpreter.lib.Data.Interfaces;
using verse_interpreter.lib.Data.ResultObjects;
using verse_interpreter.lib.Factories;

namespace verse_interpreter.lib.Data
{
    public class DeclarationResult
    {
        public string Value { get; set; } = null!;

        public DynamicType? DynamicType { get; set; } = null!;

        public CollectionVariable? CollectionVariable { get; set; } = null!;

        public string Name { get; set; } = null!;

        public string TypeName { get; set; } = null!;

        public List<List<ExpressionResult>>? ExpressionResults { get; set; } = null!;

        public DeclarationResult()
        {
            TypeName = "undefined";
        }
    }
}
