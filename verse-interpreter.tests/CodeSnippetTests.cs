using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using verse_interpreter.lib;

namespace verse_interpreter.tests
{
	public class CodeSnippetTests : AbstractTestFixture
	{
		private Application _application = null!;

		[SetUp]
		public void StartUp()
		{
			_application = new Application();
		}

		[Test]
		public void Should_Calculate_Variables_Bound_At_Different_Times()
		{
			using (var writer = new StringWriter())
			{
				Console.SetOut(writer);
				_application.Run(new[]
				{
					BasePathString + "Test1.verse"
				});

				// Uses lenience to evaluate the subexpressions
				Assert.That(writer.ToString(), Contains.Substring("16"));
			}
		}

		[Test]
		public void Should_Concatenate_Two_Strings()
		{
			using (var writer = new StringWriter())
			{
				Console.SetOut(writer);
				_application.Run(new[]
				{
					BasePathString + "Test2.verse"
				});

				// Takes A and B and adds them two together
				Assert.That(writer.ToString(), Contains.Substring("ABA"));
			}
		}

		[Test]
		public void Should_Concatenate_Strings_Inline()
		{
			using (var writer = new StringWriter())
			{
				Console.SetOut(writer);
				_application.Run(new[]
				{
					BasePathString + "Test8.verse"
				});

				// Takes xyz and abc and adds them two together --> inline
				Assert.That(writer.ToString(), Contains.Substring("xyzabc"));
			}
		}

		[Test]
		public void Should_Call_Simple_Function()
		{
			using (var writer = new StringWriter())
			{
				Console.SetOut(writer);
				_application.Run(new[]
				{
					BasePathString + "Test3.verse"
				});

				// The function takes 1 2 3 and adds 6 + 6
				Assert.That(writer.ToString(), Contains.Substring("12"));
			}
		}

		[Test]
		public void Should_Access_Type_Property()
		{
			using (var writer = new StringWriter())
			{
				Console.SetOut(writer);
				_application.Run(new[]
				{
					BasePathString + "Test4.verse"
				});

				// Accesses the member of the custom made triangle type and adds 25 to it
				Assert.That(writer.ToString(), Contains.Substring("50"));
			}
		}

		[Test]
		public void Should_Add_Two_Local_Variables()
		{
			using (var writer = new StringWriter())
			{
				Console.SetOut(writer);
				_application.Run(new[]
				{
					   BasePathString + "Test10.verse"
				});

				Assert.That(writer.ToString(), Contains.Substring("50"));
			}
		}

		[Test]
		[TestCase("Hallo Welt")]
		[TestCase("4")]
		[TestCase("HalloWelt")]
		public void Should_Print_Arbitrary_Values(string expected)
		{
			using (var writer = new StringWriter())
			{
				Console.SetOut(writer);
				_application.Run(new[]
				{
				   BasePathString + "Test9.verse"
				});

				Assert.That(writer.ToString(), Contains.Substring(expected));
			}
		}

		[Test]
		public void Should_Print_Member_Access()
		{
			using (var writer = new StringWriter())
			{
				Console.SetOut(writer);
				_application.Run(new[]
				{
					BasePathString + "Test5.verse"
				});

				Assert.That(writer.ToString(), Contains.Substring("20"));
			}
		}

		[Test]
		public void Should_Add_Value_OutOf_Array()
		{
			using (var writer = new StringWriter())
			{
				Console.SetOut(writer);
				_application.Run(new[]
				{
					BasePathString + "Test6.verse"
				});

				Assert.That(writer.ToString(), Contains.Substring("6"));
			}
		}
	}
}
