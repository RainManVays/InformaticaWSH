using System;

namespace InformaticaWSH
{
   /// <summary>
   /// Object contained workflow and session log message values
   /// </summary>
   public class LogMessage
    {
        /// <summary>
        /// message status can be : INFO, ERROR, FATAL, WARNING, TRACE, DEBUG
        /// </summary>
        public string Severity { get; set; }
        /// <summary>
        /// timestamp message
        /// </summary>
        public DateTime LogTime { get; set; }
        /// <summary>
        /// Thread
        /// </summary>
        public string Thread { get; set; }
        /// <summary>
        /// message code on informatica inner codes
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// message text
        /// </summary>
        public string Message { get; set; }

    }
}
