using Antlr4.Runtime;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using verse_interpreter.lib.Data;

namespace verse_interpreter.lib.Lookup
{
    public class LookupManager
    {
        private ILookupTable<int?> intLookupTable;
        private ILookupTable<string> stringLookupTable;
        private readonly ILookupTable<DynamicType> typeTable;

        public LookupManager(ILookupTable<int?> intTable,
                             ILookupTable<string> stringTable,
                             ILookupTable<DynamicType> typeTable)
        {
            this.intLookupTable = intTable;
            this.stringLookupTable = stringTable;
            this.typeTable = typeTable;
        }



        public void Add(DeclarationResult declarationResult)
        {
            // Create a values list
            List<int?> intValues = new List<int?>();
            List<string> stringValues = new List<string>();

            // Check if variable is already in lookup table (which means the variable was already declared once).
            // If true then throw exception.
            if (this.intLookupTable.Table.ContainsKey(declarationResult.Name))
            {
                throw new Exception();
            }
            if (this.stringLookupTable.Table.ContainsKey(declarationResult.Name))
            {
                throw new Exception();
            }
            if (this.typeTable.Table.ContainsKey(declarationResult.Name))
            {
                throw new Exception();
            }

            if (declarationResult.TypeName == "int")
            {
                int parsedValue;
                var hasValues = int.TryParse(declarationResult.Value, out parsedValue);
                if (hasValues)
                {
                    this.intLookupTable.Table.Add(declarationResult.Name, intValues);
                    intValues.Add(parsedValue);
                    return;
                }
                this.intLookupTable.Table.Add(declarationResult.Name, null);

            }
            if (declarationResult.TypeName == "string")
            {
                stringValues.Add(declarationResult.Value);
                this.stringLookupTable.Table.Add(declarationResult.Name, stringValues);
            }
            if (declarationResult.DynamicType != null)
            {
                this.typeTable.Table.Add(declarationResult.Name, new() { declarationResult.DynamicType });
            }
            // Get the values from the declaration result and add to lookup.
        }

        public void UpdateVariable(string variableName, DeclarationResult declarationResult)
        {
            if (!IsVariable(variableName))
            {
                throw new InvalidOperationException("The specified variable does not exist in this scope!");
            }

            if (declarationResult.TypeName == "int")
            {
                int parsedValue;
                var hasValues = int.TryParse(declarationResult.Value, out parsedValue);
                if (hasValues)
                {
                    intLookupTable.Table.Remove(variableName);
                    intLookupTable.Table.Add(variableName, new() { parsedValue });
                    return;
                }
            }
            if (declarationResult.TypeName == "string")
            {
                stringLookupTable.Table.Remove(variableName);
                stringLookupTable.Table.Add(variableName, new() { declarationResult.Value });
            }
            if (declarationResult.DynamicType != null)
            {
                throw new NotImplementedException();
            }
        }

        public List<string> GetVariableStrings(string variableName)
        {
            if (string.IsNullOrEmpty(variableName))
            {
                throw new ArgumentNullException();
            }

            // Check if variable is in lookup.
            // If false then the variable does not exist (which means it was never declared).
            if (!this.stringLookupTable.Table.ContainsKey(variableName))
            {
                throw new InvalidOperationException($"The specified variable with the name \"{variableName}\" is not declared yet!");
            }

            // Get the values of the variable and return it.
            List<string> values;
            this.stringLookupTable.Table.TryGetValue(variableName, out values!);
            return values!;
        }

        public List<int?> GetVariableInts(string variableName)
        {
            if (string.IsNullOrEmpty(variableName))
            {
                throw new ArgumentNullException();
            }

            // Check if variable is in lookup.
            // If false then the variable does not exist (which means it was never declared).
            if (!this.intLookupTable.Table.ContainsKey(variableName))
            {
                throw new InvalidOperationException($"The specified variable with the name \"{variableName}\" is not declared yet!"); 
            }

            // Get the values of the variable and return it.
            List<int?> values;
            this.intLookupTable.Table.TryGetValue(variableName, out values!);
            return values!;
        }

        public DynamicType GetInstanceVariable(string variableName)
        {
            if (string.IsNullOrEmpty(variableName))
            {
                throw new ArgumentNullException();
            }

            if (!this.typeTable.Table.ContainsKey(variableName))
            {
                throw new InvalidOperationException($"The specified variable with the name \"{variableName}\" is not declared yet!");
            }

            return this.typeTable.Table[variableName].First();
        }

        public bool IsVariable(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return false;
            }
            else
            {
                return this.intLookupTable.Table.ContainsKey(name) || this.stringLookupTable.Table.ContainsKey(name);
            }
        }
    }
}
