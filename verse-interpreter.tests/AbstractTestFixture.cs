namespace verse_interpreter.tests
{
    [TestFixture]
    public abstract class AbstractTestFixture
    {
        public string BasePathString { get; set; }

        public AbstractTestFixture()
        {
            BasePathString = "-p ../../../../../../verse-interpreter.tests/Snippets/";
        }
    }
}

