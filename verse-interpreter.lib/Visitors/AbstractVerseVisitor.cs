using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using verse_interpreter.lib.Grammar;

namespace verse_interpreter.lib.Visitors
{
    public abstract class AbstractVerseVisitor<T> : VerseBaseVisitor<T>
    {
        private ApplicationState _state;

        public AbstractVerseVisitor(ApplicationState applicationState)
        {
            _state = applicationState;
        }

        public ApplicationState ApplicationState { get { return _state; } }
    }
}
