using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace verse_interpreter.lib.IO
{

	public class CommandLineOptions
	{
		[Option('p', "Path", Required =true,  HelpText = "Set the path to the verse file", SetName = "path")]
		public string? Path { get; set; }

		[Option('c', "Code", Required = false, HelpText = "Set the verse code directly!", SetName = "code")]
		public string? Code { get; set; }
	}
}
