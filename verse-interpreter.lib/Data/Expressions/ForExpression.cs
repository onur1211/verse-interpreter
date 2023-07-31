using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using verse_interpreter.lib.Data.Interfaces;

namespace verse_interpreter.lib.Data.Expressions
{
	public class ForExpression : IExpression<ForExpression>
	{
		public Func<ForExpression>? PostponedExpression { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		public VerseCollection Collection { get; internal set; }
	}
}
