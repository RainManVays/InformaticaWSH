using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Xml;

namespace InformaticaWSH
{
    class WebRequestsExecutor
    {
        private string _url;
        public WebRequestsExecutor(string url)
        {
            _url = url;
        }
        private static HttpWebRequest CreateWebRequest(string url, string action)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.Headers.Add("SOAPAction", action);
            webRequest.ContentType = "text/xml;charset=\"utf-8\"";
            webRequest.Accept = "text/xml";
            webRequest.Method = "POST";
            return webRequest;
        }
        private static HttpWebRequest CreateWebRequest(string url)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.Headers.Add("SOAPAction", "SOAPAction");
            webRequest.ContentType = "text/xml;charset=\"utf-8\"";
            webRequest.Accept = "text/xml";
            webRequest.Method = "POST";
            return webRequest;
        }
        private static async Task InsertSoapEnvelopeIntoWebRequest(XmlDocument soapEnvelopeXml, HttpWebRequest webRequest)
        {
            using (Stream stream = await webRequest.GetRequestStreamAsync())
            {
                soapEnvelopeXml.Save(stream);
            }
        }

        public async Task<string> ExecuteRequest(XmlDocument soapEnvelopeXml)
        {
            HttpWebRequest webRequest = CreateWebRequest(_url);
            try
            {
                await InsertSoapEnvelopeIntoWebRequest(soapEnvelopeXml, webRequest);

                HttpWebResponse response = (HttpWebResponse)await webRequest.GetResponseAsync();
                 
                using (StreamReader stream = new StreamReader(response.GetResponseStream()))
                    return await stream.ReadToEndAsync();
            }
            catch(WebException ex)
            {
                var resp = await new StreamReader(ex.Response.GetResponseStream()).ReadToEndAsync();
                throw new WebException(ValuesSoapXml.GetValueOnElement(resp, "ErrorCode") +"\n"+ValuesSoapXml.GetValueOnElement(resp, "faultstring"));
            }
            
        }
               
    }
}
