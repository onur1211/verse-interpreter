using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using verse_interpreter.lib.ParseVisitors;
using verse_interpreter.lib.Visitors;

namespace verse_interpreter.lib.Wrapper
{
    public class TypeHandlingWrapper
    {
        public TypeDefinitionVisitor TypeDefinitionVisitor { get; }

        public TypeConstructorVisitor TypeConstructorVisitor { get; }

        public TypeInferencer TypeInferencer { get; }

        public TypeMemberVisitor TypeMemberVisitor { get; internal set; }

        public TypeHandlingWrapper(TypeInferencer typeInferencer, 
            TypeConstructorVisitor typeConstructorVisitor, 
            TypeDefinitionVisitor typeDefinitionVisitor,
            TypeMemberVisitor typeMemberVisitor)
        {
            TypeInferencer = typeInferencer;
            TypeConstructorVisitor = typeConstructorVisitor;
            TypeDefinitionVisitor = typeDefinitionVisitor;
            TypeMemberVisitor = typeMemberVisitor;
        }
    }
}
