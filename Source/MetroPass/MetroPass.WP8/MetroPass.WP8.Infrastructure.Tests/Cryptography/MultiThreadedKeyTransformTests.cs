using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetroPass.WP8.Infrastructure.Cryptography;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace MetroPass.WP8.Infrastructure.Tests.Cryptography
{
    [TestClass]
    public class MultiThreadedKeyTransformTests
    {
        [TestMethod]
        [Ignore]
        public async Task MyTestMethod()
        {
            var data = Convert.FromBase64String("F1k5AYmDq8qkApuoHmGtuqY/P6vji+2Eqe3QO+pYet4=");
            var key = Convert.FromBase64String("6JZef3f/5ojLKqbEVcNswAGy0HXNSeNPk4JYQrUD+fY=");

            var s = new Stopwatch();
            var bouncy = new MultiThreadedBouncyCastleCrypto();

            s.Start();
            int rounds = 500000;
            await bouncy.Transform(data, key, rounds, new NullableProgress<double>());
            s.Stop();
            var timeInBouncy = s.ElapsedMilliseconds;

            s.Reset();
            var managed = new MultithreadedManagedCrypto();
            s.Start();
            await managed.Transform(data, key, rounds, new NullableProgress<double>());
            s.Stop();

            var timeInManaged= s.ElapsedMilliseconds;
            
            Assert.Inconclusive("{0} in bouncy and {1} in managed", timeInBouncy, timeInManaged);
        }
    }
}
