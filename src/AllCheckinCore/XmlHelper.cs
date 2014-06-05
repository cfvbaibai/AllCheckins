using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AllCheckin.Core
{
    public static class XmlHelper
    {
        public static string GetOuterXml(XElement xElement)
        {
            using (var reader = xElement.CreateReader())
            {
                return reader.ReadOuterXml();
            }
        }
    }
}
