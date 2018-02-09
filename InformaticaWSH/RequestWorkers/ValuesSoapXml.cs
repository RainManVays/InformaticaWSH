using System.Xml;

namespace InformaticaWSH
{
    internal static class ValuesSoapXml
    {
        internal static string GetValueOnElement(string result, string elementName)
        {
            XmlDocument xmlResult = new XmlDocument();
            xmlResult.LoadXml(result);
            var nodes = xmlResult.GetElementsByTagName(elementName);
            if (nodes != null && nodes.Count > 0)
                return nodes[0].InnerText;
            return null;
        }
    }
}
