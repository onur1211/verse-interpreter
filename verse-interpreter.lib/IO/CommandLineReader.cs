using CommandLine;

namespace verse_interpreter.lib.IO
{
    public class CommandLineOptions
    {
        [Option('p', "Path", Required = true, HelpText = "Set the path to the verse file", SetName = "path")]
        public string? Path { get; set; }

        [Option('c', "Code", Required = false, HelpText = "Set the verse code directly!", SetName = "code")]
        public string? Code { get; set; }

        [Option('d', "Debug", Required = false, HelpText = "Debug Mode will print all variables")]
        public bool Debug { get; set; }
    }
}
