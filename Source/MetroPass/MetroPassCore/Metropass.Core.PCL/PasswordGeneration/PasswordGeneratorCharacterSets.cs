using System.Collections.Generic;
using System.Reflection;

namespace Metropass.Core.PCL.PasswordGeneration
{
    public class PasswordGeneratorCharacterSets
    {
        public const string Uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        public const string Lowercase = "abcdefhijklmnopqrstuvwxyz";
        public const string Digit = "0123456789";
        public const string Minus = "-";
        public const string Underscore = "_";
        public const string Space = " ";
        public const string Special = "!@#$%^&*+=?,.";
        public const string Bracket = "(){}[]<>";


        public static Dictionary<string, string> CharacterMap
        {
            get
            {
                var thisType = typeof(PasswordGeneratorCharacterSets);
                var map = new Dictionary<string, string>();
       
                var props = thisType.GetRuntimeFields();

                foreach (var item in props)
                {
                    map.Add(item.Name, (string)item.GetValue(thisType));
                }

                return map;
            }
        }
    }
}
