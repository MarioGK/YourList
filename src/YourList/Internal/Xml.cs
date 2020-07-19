using System.Xml.Linq;
using YourList.Internal.Extensions;

namespace YourList.Internal
{
    internal static class Xml
    {
        public static XElement Parse(string source)
        {
            return XElement.Parse(source, LoadOptions.PreserveWhitespace).StripNamespaces();
        }
    }
}