using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using verse_interpreter.lib.Lookup;

namespace verse_interpreter.lib
{
    public class ValueFetcher
    {
        public T FetchValue<T>(string variableName, LookupManager lookUpManager)
        {
            if (!lookUpManager.IsVariable(variableName))
            {
                throw new ArgumentException();
            }

            throw new NotImplementedException();
        }
    }
}
