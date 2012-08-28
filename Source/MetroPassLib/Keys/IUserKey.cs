using MetroPassLib.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Windows.Storage.Streams;

namespace MetroPassLib.Keys
{
    public interface IUserKey
    {
        IBuffer KeyData
        {
            get;
        }
    }
}
