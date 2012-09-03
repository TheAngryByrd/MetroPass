using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Windows.Storage.Streams;

namespace MetroPass.Core.Model.Keys
{
    public interface IUserKey
    {
        IBuffer KeyData { get; }
    }
}
