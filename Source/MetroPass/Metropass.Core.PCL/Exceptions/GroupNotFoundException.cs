using System;

namespace Metropass.Core.PCL.Exceptions
{
    public class GroupNotFoundException : ArgumentException
    {
        public GroupNotFoundException(string message, string paramName) : base(message, paramName) { }
    }
}
