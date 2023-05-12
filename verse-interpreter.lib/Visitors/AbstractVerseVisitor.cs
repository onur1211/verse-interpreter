using verse_interpreter.lib.Data;
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

        protected void AddScopedVariable(string scopeName, IScope<int> scope)
        {
            _state.Scopes.Add(scopeName, scope);
        }
    }
}
