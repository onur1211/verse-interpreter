namespace verse_interpreter.lib.Data.Interfaces
{
    public interface IExpression<TOut>
    {
        Func<TOut>? PostponedExpression { get; set; }
    }
}
