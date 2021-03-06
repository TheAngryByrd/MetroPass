﻿using System;
using System.Collections.Generic;

namespace Metropass.Core.PCL.Model.Kdb4.Keys
{
    public class CompositeKey
    {
        public IProgress<double> PercentComplete { get; set; }
        public IList<IUserKey> UserKeys { get; set; }

        public CompositeKey(IList<IUserKey> userKeys)
        {
            Init(userKeys, new NullableProgress<double>());
        }

        public CompositeKey(IList<IUserKey> userKeys, IProgress<double> percentComplete)
        {
            Init(userKeys, percentComplete);            
        }

        private void Init(IList<IUserKey> userKeys, IProgress<double> percentComplete)
        {
            UserKeys = userKeys;
            PercentComplete = percentComplete;
        }     
    }
}
