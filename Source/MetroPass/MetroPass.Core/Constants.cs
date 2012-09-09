﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetroPass.Core
{
    public class KdbConstants
    {
        // KeePass 2.x signatures
        public const uint FileSignature1 = 0x9AA2D903;
        public const uint FileSignature2 = 0xB54BFB67;
        public const uint Kdb4Version = 0x00030000;

        // KeePass 1.x signatures
        public const uint FileSignatureOld1 = 0x9AA2D903;
        public const uint FileSignatureOld2 = 0xB54BFB65;


    }
}