using System;
using System.Runtime.CompilerServices;
using System.Xml.Linq;


public static class XmlEx
{
    public static XElement Element(this XElement element, [CallerMemberName]
                                    String propertyName = null)
    {
        return element.Element(propertyName);
    }
}
