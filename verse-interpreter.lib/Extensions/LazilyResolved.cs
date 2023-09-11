using Microsoft.Extensions.DependencyInjection;

namespace verse_interpreter.lib.Extensions
{
    public class LazilyResolved<T> : Lazy<T>
    {
        public LazilyResolved(IServiceProvider serviceProvider)
#pragma warning disable CS8714 // Der Typ kann nicht als Typparameter im generischen Typ oder in der generischen Methode verwendet werden. Die NULL-Zulässigkeit des Typarguments entspricht nicht der notnull-Einschränkung.
                             : base(serviceProvider.GetRequiredService<T>)
#pragma warning restore CS8714 // Der Typ kann nicht als Typparameter im generischen Typ oder in der generischen Methode verwendet werden. Die NULL-Zulässigkeit des Typarguments entspricht nicht der notnull-Einschränkung.
        {
        }
    }
}
