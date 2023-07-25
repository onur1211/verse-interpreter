using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace verse_interpreter.lib.Extensions
{
    public static class GenericExtensions
    {
        public static T DeepClone<T>(this T obj) where T : class
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);
                ms.Position = 0;

                return (T)formatter.Deserialize(ms);
            }
        }

        public static IServiceCollection AddLazyResolution(this IServiceCollection services)
        {
            return services.AddTransient(
                typeof(Lazy<>),
                typeof(LazilyResolved<>));
        }

        public static bool ContainsWhere<T>(this IEnumerable<T> values, Func<T, bool> predicate)
        {
            foreach(var value in values)
            {
                if (predicate(value))
                {
                    return true;
                }
            }

            return false;
        }
    }
}