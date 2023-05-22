﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace verse_interpreter.lib.Evaluators
{
    public interface IEvaluator<TOutput, WInput>
    {
        TOutput Evaluate(WInput input);

        bool AreVariablesBoundToValue(WInput input);
    }
}
