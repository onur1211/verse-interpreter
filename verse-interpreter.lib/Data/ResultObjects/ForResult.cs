using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace verse_interpreter.lib.Data.ResultObjects
{
	public class ForResult
	{
		public List<Variable> LocalVariables { get; set; }

		public ForResult()
		{
			LocalVariables = new List<Variable>();
		}
	}
}
