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

        [Option('d', "Debug", Required = false, HelpText = "Debug Mode will print all variables")]
        public bool Debug { get; set; }
    }
}
