using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetroPassLib.Utils
{
    public class StrUtil
    {
        public static bool IsHexString(string str, bool bStrict)
        {
            if (str == null) throw new ArgumentNullException("str");
            if (str.Length == 0) return true;

            foreach (char ch in str)
            {
                if ((ch >= '0') && (ch <= '9')) continue;
                if ((ch >= 'a') && (ch <= 'z')) continue;
                if ((ch >= 'A') && (ch <= 'Z')) continue;

                if (bStrict) return false;

                if ((ch == ' ') || (ch == '\t') || (ch == '\r') || (ch == '\n'))
                    continue;

                return false;
            }

            return true;
        }
    }
}
