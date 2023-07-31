using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using verse_interpreter.lib.ParseVisitors.Choice;

namespace verse_interpreter.lib.Data.ResultObjects
{
	public class ChoiceResult
	{
		public List<ArrayIndexingResult> IndexingResults { get; set; }
		public List<Variable> Literals { get; set; }

        public ChoiceResult()
        {
			this.IndexingResults = new List<ArrayIndexingResult>();
			this.Literals = new List<Variable>();
        }
    }
}
