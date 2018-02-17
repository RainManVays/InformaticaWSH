using System;

namespace InformaticaWSH
{
   public class LogMessage
    {
        public string Severity { get; set; }
        public DateTime LogTime { get; set; }
        public string Thread { get; set; }
        public string Code { get; set; }
        public string Message { get; set; }

    }
}
