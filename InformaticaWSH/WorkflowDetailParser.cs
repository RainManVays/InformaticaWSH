namespace InformaticaWSH
{
   static class WorkflowDetailParser
    {

        internal static WorkflowDetail ConvertStringToWorkflowDetail(string result)
        {
            DIServerDetail disDetail = new DIServerDetail
            {
                DIServerStatus = ValuesSoapXml.GetValueOnElement(result, "DIServerStatus"),
                CurrentTime = XmlTimeParser.ParseXmlTime(ValuesSoapXml.GetNodeOnElementName(result, "CurrentTime")),
                CurrentUTCTime = XmlTimeParser.ParseXmlUTCTime(ValuesSoapXml.GetNodeOnElementName(result, "CurrentTime")),

                StartupTime = XmlTimeParser.ParseXmlTime(ValuesSoapXml.GetNodeOnElementName(result, "StartupTime")),
                StartupUTCTime = XmlTimeParser.ParseXmlUTCTime(ValuesSoapXml.GetNodeOnElementName(result, "StartupTime")),

                ReferenceTime = XmlTimeParser.ParseXmlTime(ValuesSoapXml.GetNodeOnElementName(result, "ReferenceTime")),
                ReferenceUTCTime = XmlTimeParser.ParseXmlUTCTime(ValuesSoapXml.GetNodeOnElementName(result, "ReferenceTime"))
            };


            WorkflowDetail wfDetail = new WorkflowDetail
            {
                DIServerDetail = disDetail,

                FolderName = ValuesSoapXml.GetValueOnElement(result, "FolderName"),
                WorkflowName = ValuesSoapXml.GetValueOnElement(result, "WorkflowName"),
                WorkflowRunId = int.Parse(ValuesSoapXml.GetValueOnElement(result, "WorkflowRunId")),
                WorkflowRunInstanceName = ValuesSoapXml.GetValueOnElement(result, "WorkflowRunInstanceName"),
                WorkflowRunStatus = ValuesSoapXml.GetValueOnElement(result, "WorkflowRunStatus"),
                WorkflowRunType = ValuesSoapXml.GetValueOnElement(result, "WorkflowRunType"),
                RunErrorCode = int.Parse(ValuesSoapXml.GetValueOnElement(result, "RunErrorCode")),
                RunErrorMessage = ValuesSoapXml.GetValueOnElement(result, "RunErrorMessage"),
                UserName = ValuesSoapXml.GetValueOnElement(result, "UserName"),
                LogFileName = ValuesSoapXml.GetValueOnElement(result, "LogFileName"),
                LogFileCodePage = ValuesSoapXml.GetValueOnElement(result, "LogFileCodePage"),
                OSUser = ValuesSoapXml.GetValueOnElement(result, "OSUser"),

                StartTime = XmlTimeParser.ParseXmlTime(ValuesSoapXml.GetNodeOnElementName(result, "StartTime")),
                StartUTCTime = XmlTimeParser.ParseXmlUTCTime(ValuesSoapXml.GetNodeOnElementName(result, "StartTime")),
                EndTime = XmlTimeParser.ParseXmlTime(ValuesSoapXml.GetNodeOnElementName(result, "EndTime")),
                EndUTCTime = XmlTimeParser.ParseXmlUTCTime(ValuesSoapXml.GetNodeOnElementName(result, "EndTime"))
            };

            return wfDetail;

        }
    }
}
