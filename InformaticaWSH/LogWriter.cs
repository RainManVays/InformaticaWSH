using System;
using System.IO;
namespace InformaticaWSH
{
    class LogWriter
    {
        internal static void WriteLog(string logPath,string fileName,string type,string message)
        {
            if (!string.IsNullOrEmpty(logPath)&& !string.IsNullOrEmpty(fileName))
                File.AppendAllText(logPath + "//" + fileName, DateTime.Now + ";"+type+";" + message+"\n");
        }
    }
}
