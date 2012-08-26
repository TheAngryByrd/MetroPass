using MetroPassLib.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetroPassLib.Keys
{
    public interface IUserKey
    {
        ProtectedBuffer KeyData
        {
            get;
        }
    }
}
