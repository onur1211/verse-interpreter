using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using verse_interpreter.lib.ParseVisitors.Types;
using verse_interpreter.lib.Visitors;

namespace verse_interpreter.lib.Wrapper
{
    /// <summary>
    /// Provides a wrapper class for handling various type-related visitors and inferencers.
    /// </summary>
    public class TypeHandlingWrapper
	{
		/// <summary>
		/// Gets the visitor responsible for handling type definitions.
		/// </summary>
		public TypeDefinitionVisitor TypeDefinitionVisitor { get; }

		/// <summary>
		/// Gets the visitor responsible for handling type constructors.
		/// </summary>
		public TypeConstructorVisitor TypeConstructorVisitor { get; }

		/// <summary>
		/// Gets the type inferencer for performing type inference operations.
		/// </summary>
		public TypeInferencer TypeInferencer { get; }

		/// <summary>
		/// Gets or sets the visitor responsible for handling type members.
		/// </summary>
		public TypeMemberVisitor TypeMemberVisitor { get; internal set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="TypeHandlingWrapper"/> class.
		/// </summary>
		/// <param name="typeInferencer">The type inferencer for type inference operations.</param>
		/// <param name="typeConstructorVisitor">The visitor for handling type constructors.</param>
		/// <param name="typeDefinitionVisitor">The visitor for handling type definitions.</param>
		/// <param name="typeMemberVisitor">The visitor for handling type members.</param>
		public TypeHandlingWrapper(TypeInferencer typeInferencer,
			TypeConstructorVisitor typeConstructorVisitor,
			TypeDefinitionVisitor typeDefinitionVisitor,
			TypeMemberVisitor typeMemberVisitor)
		{
			TypeInferencer = typeInferencer;
			TypeConstructorVisitor = typeConstructorVisitor;
			TypeDefinitionVisitor = typeDefinitionVisitor;
			TypeMemberVisitor = typeMemberVisitor;
		}
	}
}
