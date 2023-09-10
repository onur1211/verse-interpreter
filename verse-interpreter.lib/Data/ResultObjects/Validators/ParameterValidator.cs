using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using verse_interpreter.lib.Data.Interfaces;
using verse_interpreter.lib.EventArguments;
using verse_interpreter.lib.ParseVisitors;

namespace verse_interpreter.lib.Data.ResultObjects.Validators
{
    public class ParameterValidator : IValidator<FunctionCall>
    {
        public event EventHandler<ExpressionWithNoValueFoundEventArgs>? FalseDetected;

        public bool IsTypeConformityGiven(FunctionCall value)
        {
            for (int i = 0; i < value.Function.ParameterCount; i++)
            {
                var parameterTypeName = value.Function.Parameters[i].Value.TypeData.Name;
                var passedValueTypeName = value.Parameters.Parameters[i].Value.TypeData.Name;

                if (value.Parameters.Parameters[i].Value == ValueObject.False)
                {
                    FalseDetected?.Invoke(this, new ExpressionWithNoValueFoundEventArgs());
                    continue;
                }

				if (parameterTypeName != passedValueTypeName)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
