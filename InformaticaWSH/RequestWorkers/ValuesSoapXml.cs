using System.Collections.Generic;
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
        internal static string GetNodeOnElementName(string result, string elementName)
        {
            XmlDocument xmlResult = new XmlDocument();
            xmlResult.LoadXml(result);
            var nodes = xmlResult.GetElementsByTagName(elementName);
            if (nodes != null && nodes.Count > 0)
                return nodes[0].InnerXml;
            return null;
        }


        internal static List<string> GetAllValuesOnElement(string result, string elementName)
        {
            XmlDocument xmlResult = new XmlDocument();
            xmlResult.LoadXml(result);
            var nodes = xmlResult.GetElementsByTagName(elementName);
            if (nodes == null)
                return null;
            List<string> resultsItems = new List<string>(nodes.Count);
            foreach (XmlNode node in nodes)
                resultsItems.Add(node.InnerText);

            return resultsItems;
        }
    }
}
