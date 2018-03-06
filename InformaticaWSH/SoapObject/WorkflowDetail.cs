using System;

namespace InformaticaWSH
{
    public class WorkflowDetail
    {
        public DIServerDetail DIServerDetail { get; set; }

        public string FolderName { get; set; }
        public string WorkflowName { get; set; }
        public int WorkflowRunId { get; set; }
        public string WorkflowRunInstanceName { get; set; }
        public string WorkflowRunStatus { get; set; }
        public string WorkflowRunType { get; set; }
        public int RunErrorCode { get; set; }
        public string RunErrorMessage { get; set; }
        public DateTime StartTime { get; set; }
        public int StartUTCTime { get; set; }
        public DateTime EndTime { get; set; }
        public int EndUTCTime { get; set; }
        public string UserName { get; set; }
        public string LogFileName { get; set; }
        public string LogFileCodePage { get; set; }
        public string OSUser { get; set; }

    }
}
