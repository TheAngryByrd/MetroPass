using System;
using Metropass.Core.PCL.PasswordGeneration;

namespace MetroPass.WP8.Infrastructure.PasswordGeneration
{
    public class PasswordGeneratorWP8 : PasswordGeneratorBase
    {
        public override int GenerateRandomNumber(int max) {
            return new Random().Next(0,max);            
        }
    }
}
