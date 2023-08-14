﻿using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using verse_interpreter.lib.Grammar;

namespace verse_interpreter.lib.ParseVisitors.Choice
{
	public class ChoiceArrayIndexingVisitor : AbstractVerseVisitor<ArrayIndexingResult>
	{
		public ChoiceArrayIndexingVisitor(ApplicationState applicationState) : base(applicationState)
		{
		}

		public override ArrayIndexingResult VisitNumericArrayIndex([NotNull] Verse.NumericArrayIndexContext context)
		{
			var identifiers = context.ID();
			var index = context.INT();
			if (identifiers == null || index == null)
			{
				throw new NotImplementedException("The specified way of indexing is not supported!");
			}

			return new ArrayIndexingResult()
			{
				ArrayIdentifier = identifiers.GetText(),
				Indexer = index.ToString()!
			};
		}

		public override ArrayIndexingResult VisitVariableNameArrayIndex([NotNull] Verse.VariableNameArrayIndexContext context)
		{
			var identifiers = context.ID();
			if (identifiers == null || identifiers.Count() != 2)
			{
				throw new NotImplementedException("The specified way of indexing is not supported!");
			}

			return new ArrayIndexingResult()
			{
				ArrayIdentifier = identifiers![0].GetText(),
				Indexer = identifiers![1].GetText()
			};
		}
	}
}
