using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace verse_interpreter.lib
{
    // EXAM_UPDATED

    public static class OperatorLookUp
    {
        private static Dictionary<string, string> _operators;

        static OperatorLookUp()
        {
            _operators = new Dictionary<string, string>()
            {
                { "+", "ADD" },
            };
        }


        public static bool IsOperator(string operatorName)
        {
            return _operators.ContainsKey(operatorName);    
        }
    }
}
