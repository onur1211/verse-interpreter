using Antlr4.Runtime;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using verse_interpreter.lib.Data;
using verse_interpreter.lib.Exceptions;

namespace verse_interpreter.lib.Lookup
{
    public class LookupManager
    {
        private ILookupTable<int?> intLookupTable;

        private ILookupTable<string> stringLookupTable;

        private readonly ILookupTable<DynamicType> typeTable;

        private Dictionary<string, Action<DeclarationResult>> addCommands;

        private Dictionary<string, Action<string, DeclarationResult>> updateCommands;

        public LookupManager(ILookupTable<int?> intTable,
                             ILookupTable<string> stringTable,
                             ILookupTable<DynamicType> typeTable)
        {
            this.intLookupTable = intTable;
            this.stringLookupTable = stringTable;
            this.typeTable = typeTable;
            this.addCommands = new Dictionary<string, Action<DeclarationResult>>();
            this.updateCommands = new Dictionary<string, Action<string, DeclarationResult>>();
            this.SetAddCommands();
            this.SetUpdateCommands();
        }

        public void Add(DeclarationResult declarationResult)
        {
            // Check if variable is already in a lookup table (which means the variable was already declared once).
            // If true then throw exception.
            if (this.intLookupTable.Table.ContainsKey(declarationResult.Name) 
               || this.stringLookupTable.Table.ContainsKey(declarationResult.Name)
               || this.typeTable.Table.ContainsKey(declarationResult.Name))
            {
                throw new VariableAlreadyExistException(declarationResult.Name);
            }

            foreach (var command in this.addCommands) 
            {
                if (declarationResult.TypeName == command.Key)
                {
                    command.Value.Invoke(declarationResult);
                }
            }
        }

        public void UpdateVariable(string variableName, DeclarationResult declarationResult)
        {
            if (!IsVariable(variableName))
            {
                throw new InvalidOperationException("The specified variable does not exist in this scope!");
            }

            foreach (var command in this.updateCommands)
            {
                if (declarationResult.TypeName == command.Key)
                {
                    command.Value.Invoke(variableName, declarationResult);
                }
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
                return new List<string>();
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
                return new List<int?>();
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
                throw new VariableDoesNotExistException(variableName);
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
                return this.intLookupTable.Table.ContainsKey(name) 
                    || this.stringLookupTable.Table.ContainsKey(name)
                    || this.typeTable.Table.ContainsKey(name);
            }
        }

        private void SetAddCommands()
        {
            Action<DeclarationResult> addInt = new Action<DeclarationResult>(AddInt);
            Action<DeclarationResult> addString = new Action<DeclarationResult>(AddString);
            Action<DeclarationResult> addDynamic = new Action<DeclarationResult>(AddDynamic);

            this.addCommands.Add("int", addInt);
            this.addCommands.Add("string", addString);
            this.addCommands.Add("dynamic", addDynamic);
        }

        private void SetUpdateCommands()
        {
            Action<string, DeclarationResult> updateInt = new Action<string, DeclarationResult>(UpdateInt);
            Action<string, DeclarationResult> updateString = new Action<string, DeclarationResult>(UpdateString);
            Action<string, DeclarationResult> updateDynamic = new Action<string, DeclarationResult>(UpdateDynamic);

            this.updateCommands.Add("int", updateInt);
            this.updateCommands.Add("string", updateString);
            this.updateCommands.Add("dynamic", updateDynamic);
        }

        private void AddInt(DeclarationResult declarationResult)
        {
            List<int?> intValues = new List<int?>();
            int parsedValue;

            bool hasValues = int.TryParse(declarationResult.Value, out parsedValue);
            
            if (hasValues)
            {
                this.intLookupTable.Table.Add(declarationResult.Name, intValues);
                intValues.Add(parsedValue);
                return;
            }

            this.intLookupTable.Table.Add(declarationResult.Name, null);
        }

        private void AddString(DeclarationResult declarationResult) 
        {
            List<string> stringValues = new List<string>();
            stringValues.Add(declarationResult.Value);
            this.stringLookupTable.Table.Add(declarationResult.Name, stringValues);
        }

        private void AddDynamic(DeclarationResult declarationResult) 
        {
            this.typeTable.Table.Add(declarationResult.Name, new() { declarationResult.DynamicType });
        }

        private void UpdateInt(string variableName, DeclarationResult declarationResult)
        {
            int parsedValue;
            bool hasValues = int.TryParse(declarationResult.Value, out parsedValue);
            
            if (!hasValues)
            {
                return;
            }

            this.intLookupTable.Table.Remove(variableName);
            this.intLookupTable.Table.Add(variableName, new() { parsedValue });
        }

        private void UpdateString(string variableName, DeclarationResult declarationResult) 
        {
            this.stringLookupTable.Table.Remove(variableName);
            this.stringLookupTable.Table.Add(variableName, new() { declarationResult.Value });
        }

        private void UpdateDynamic(string variableName, DeclarationResult declarationResult)
        {
            if (declarationResult.DynamicType == null) 
            {
                return;
            }

            throw new NotImplementedException();
        }
    }
}
