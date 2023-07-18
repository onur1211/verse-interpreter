﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using verse_interpreter.lib.Data.Interfaces;
using verse_interpreter.lib.ParseVisitors;

namespace verse_interpreter.lib.Data.ResultObjects.Validators
{
    public class ParameterValidator : IValidator<FunctionCall>
    {
        public bool IsTypeConformityGiven(FunctionCall value)
        {
            for (int i = 0; i < value.Function.ParameterCount; i++)
            {
                if (value.Function.Parameters[i].Value.TypeData.Name != value.Parameters.Parameters[i].Value.TypeData.Name)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
