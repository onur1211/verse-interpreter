using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using verse_interpreter.lib.ParseVisitors;

namespace verse_interpreter.lib.EventArguments
{
	
	public class FunctionRequestedExecutionEventArgs : EventArgs
	{
        public FunctionRequestedExecutionEventArgs(FunctionCall functionCall)
        {
                this.FunctionCall = functionCall;
        }

        public FunctionCall FunctionCall { get; private set; }  
    }
}
