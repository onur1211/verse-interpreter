using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace verse_interpreter.lib.Data.ResultObjects
{
	public class ChoiceResult
	{
		public List<Variable> PossibleValues { get; set; }
		public string Target { get; set; }

        public ChoiceResult(string target)
        {
			this.Target = target;
			this.PossibleValues = new List<Variable>();
        }
    }
}
