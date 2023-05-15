using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace verse_interpreter.lib.Visitors
{
    public class TypeMemberVisitor : AbstractVerseVisitor<object>
    {
        public TypeMemberVisitor(ApplicationState applicationState) : base(applicationState)
        {
        }

    }
}
