using verse_interpreter.lib.Grammar;

namespace verse_interpreter.lib.EventArguments
{
	public class DeclarationInArrayFoundEventArgs : EventArgs
	{
		public DeclarationInArrayFoundEventArgs(Verse.DeclarationContext declarationContext)
		{
			this.declarationContext = declarationContext;
		}

		public Verse.DeclarationContext declarationContext { get; private set; }
	}
}
