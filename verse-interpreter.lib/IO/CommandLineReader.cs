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
        [Option('p', "Path", HelpText = "Set the path to the verse file")]
        public string? Path { get; set; }

        [Option('c', "Code", Required = false, HelpText = "Set the verse code directly!")]
        public string? Code { get; set; }

        [Option('d', "Debug", Required = false, HelpText = "Debug Mode will print all variables")]
        public bool Debug { get; set; }
    }
}
