using Microsoft.Extensions.DependencyInjection;

namespace verse_interpreter.lib.Extensions
{
	public class LazilyResolved<T> : Lazy<T>
	{
		public LazilyResolved(IServiceProvider serviceProvider)
							 : base(serviceProvider.GetRequiredService<T>)
		{
		}
	}
}
