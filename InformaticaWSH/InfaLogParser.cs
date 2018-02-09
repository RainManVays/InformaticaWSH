using System;
using System.Collections.Generic;
using System.Globalization;
namespace InformaticaWSH
{
   internal class InfaLogParser
    {
        internal static List<LogMessage> ParseWorkflowLogs(string log)
        {
            string[] splitValues = { "INFO", "ERROR", "FATAL", "WARNING", "TRACE", "DEBUG" };


            var logArray = log.Split('\n');
            for (int i = 0; i < logArray.Length; i++)
            {
                if (!string.IsNullOrEmpty(logArray[i]) && !CheckStringContainsValues(logArray[i], splitValues))
                {
                    logArray[i - 1] += logArray[i];
                    logArray[i] = null;
                    i = 0;
                }

            }
            List<LogMessage> logModels = new List<LogMessage>();
            foreach(string item in logArray)
            {
                var model = ConvertRowToMessageModel(item);
                if (model != null)
                    logModels.Add(model);
            }
            return logModels;
        }

        private static CultureInfo provider = CultureInfo.InvariantCulture;
        private static LogMessage ConvertRowToMessageModel(string row)
        {
            
            if (!string.IsNullOrEmpty(row))
            {
                LogMessage lmm = new LogMessage();
                lmm.Severity = GetRow(" :",ref row);
                lmm.LogTime= DateTime.ParseExact(GetRow("[", ref row).Trim(), "ddd MMM dd HH:mm:ss yyyy", provider);
                lmm.Code = GetRow("] : ", ref row);
                lmm.Thread = GetRow(" ", ref row);
                lmm.Message = row;

                return lmm;
            }
            return null;
        }

        private static string GetRow(string indexValue,ref string row)
        {
            int index = row.IndexOf(indexValue);
            string result = row.Substring(0, index);
            int concatindex = index + indexValue.Length;
            row = row.Substring(concatindex);
            return result;
        }

        private static bool CheckStringContainsValues(string text,string[] splitVal)
        {
            for (int i = 0; i < splitVal.Length; i++)
                if (text.Contains(splitVal[i]+ " : "))
                    return true;
            return false;
        }


    }
}
