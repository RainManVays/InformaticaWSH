using System;
using System.Collections.Generic;
using System.Globalization;
namespace InformaticaWSH
{
   internal class InfaLogParser
    {
        internal static List<LogMessage> ParseWorkflowLogs(string log)
        {
            var logsArray = log.Split('\n');
            for (int i = 0; i < logsArray.Length; i++)
            {
                if (!string.IsNullOrEmpty(logsArray[i]) && !CheckStringItsOneMessage(logsArray[i]))
                {
                    logsArray[i - 1] += logsArray[i];
                    logsArray[i] = null;
                    i = 0;
                }
            }

            List<LogMessage> logModels = new List<LogMessage>();
            foreach (string item in logsArray)
                if(!string.IsNullOrEmpty(item))
                   logModels.Add(ConvertRowToMessageModel(item));

            return logModels;
        }
        private static LogMessage ConvertRowToMessageModel(string row)
        {
            return new LogMessage
                {
                    Severity = GetRow(" :", ref row),
                    LogTime = DateTime.ParseExact(GetRow("[", ref row).Trim(), "ddd MMM dd HH:mm:ss yyyy", CultureInfo.InvariantCulture),
                    Code = GetRow("] : ", ref row),
                    Thread = GetRow(" ", ref row),
                    Message = row
                };
        }

        private static string GetRow(string indexValue,ref string row)
        {
            int index = row.IndexOf(indexValue);
            string result = row.Substring(0, index);
            int concatIndex = index + indexValue.Length;
            row = row.Substring(concatIndex);
            return result;
        }

        private static string[] messagesStatus = { "INFO : ", "ERROR : ", "FATAL : ", "WARNING : ", "TRACE : ", "DEBUG : " };

        private static bool CheckStringItsOneMessage(string text)
        {
            if (text.Contains(messagesStatus[0]))
                return true;

            for (int i = 1; i < messagesStatus.Length; i++)
                if (text.Contains(messagesStatus[i]))
                    return true;
            return false;
        }

    }
}
