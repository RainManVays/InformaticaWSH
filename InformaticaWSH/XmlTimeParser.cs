using System;

namespace InformaticaWSH
{
    internal static class XmlTimeParser
    {
        public static DateTime ParseXmlTime(string xml)
        {
            //WA
            xml = "<xml>" + xml + "</xml>";
            return new DateTime(
                day: int.Parse(ValuesSoapXml.GetValueOnElement(xml, "Date")),
                minute: int.Parse(ValuesSoapXml.GetValueOnElement(xml, "Minutes")),
               hour: int.Parse(ValuesSoapXml.GetValueOnElement(xml, "Hours")),
               month: int.Parse(ValuesSoapXml.GetValueOnElement(xml, "Month")),
               year: int.Parse(ValuesSoapXml.GetValueOnElement(xml, "Year")),
               second: int.Parse(ValuesSoapXml.GetValueOnElement(xml, "Seconds")),
               millisecond: int.Parse(ValuesSoapXml.GetValueOnElement(xml, "NanoSeconds"))
               );
        }
        public static int ParseXmlUTCTime(string xml)
        {
            //WA
            xml = "<xml>" + xml + "</xml>";
            return int.Parse(ValuesSoapXml.GetValueOnElement(xml, "UTCTime"));
        }
    }
}
