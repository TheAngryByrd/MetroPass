using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetroPass.Core.Helpers
{
    public class NullableProgress<T> : IProgress<T>
    {
        public void Report(T value)
        {

        }
    }
}
