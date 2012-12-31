﻿using System;

namespace MetroPass.Core.Exceptions
{
    public class GroupNotFoundException : ArgumentException
    {
        public GroupNotFoundException(string message, string paramName) : base(message, paramName) { }
    }
}
