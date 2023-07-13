using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace verse_interpreter.lib.Data.Variables
{
	public class TypeData
	{
		public TypeData(string typeName)
		{
			this.Name = typeName;
		}

		public string Name { get; set; }
	}
}
