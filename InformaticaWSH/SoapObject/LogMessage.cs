using System;

namespace InformaticaWSH
{
   /// <summary>
   /// 
   /// </summary>
   public class LogMessage
    {
        /// <summary>
        /// 
        /// </summary>
        public string Severity { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime LogTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Thread { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Message { get; set; }

    }
}
