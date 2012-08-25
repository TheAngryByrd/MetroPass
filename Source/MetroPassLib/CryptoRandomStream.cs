using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetroPassLib
{
    /// <summary>
    /// Algorithms supported by <c>CryptoRandomStream</c>.
    /// </summary>
    public enum CrsAlgorithm
    {
        /// <summary>
        /// Not supported.
        /// </summary>
        Null = 0,

        /// <summary>
        /// A variant of the ARCFour algorithm (RC4 incompatible).
        /// </summary>
        ArcFourVariant = 1,

        /// <summary>
        /// Salsa20 stream cipher algorithm.
        /// </summary>
        Salsa20 = 2,

        Count = 3
    }
}
