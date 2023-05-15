using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using verse_interpreter.lib.Data;

namespace verse_interpreter.lib
{
    public class ApplicationState
    {
        public Dictionary<int, IScope<int>> Scopes { get; set; }

        // EXAM_UPDATED
        public Dictionary<string, DynamicType> Types { get; set; }


        public ApplicationState()
        {
            Scopes = new Dictionary<int, IScope<int>>
            {
                { 1, new CurrentScope() }
            };
            Types = new Dictionary<string, DynamicType>();
            CurrentScopeLevel = 1;
        }

        public int CurrentScopeLevel { get; set; }  



    }
}
