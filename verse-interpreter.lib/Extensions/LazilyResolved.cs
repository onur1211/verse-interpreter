using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
