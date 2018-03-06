using System;

namespace InformaticaWSH
{
    public class DIServerDetail
    {
        public string DIServerStatus { get; set; }
        public DateTime CurrentTime { get; set; }
        public int CurrentUTCTime { get; set; }
        public DateTime StartupTime { get; set; }
        public int StartupUTCTime { get; set; }
        public DateTime ReferenceTime { get; set; }
        public int ReferenceUTCTime { get; set; }
    }
}
