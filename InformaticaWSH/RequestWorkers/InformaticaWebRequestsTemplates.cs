using System.Xml;

namespace InformaticaWSH
{
   internal static class InformaticaWebRequestsTemplates
    {
        private static string _envelopeHeader = "<soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">";
        private static string _informaticaWshLink = "\"http://www.informatica.com/wsh\"";
        private static string ConvertDiServiceInfoToXMLText(DIServiceInfo serviceInfo)
        {
            return "<DIServiceInfo>" +
                        "<DomainName>" + serviceInfo.DomainName + "</DomainName>" +
                        "<ServiceName>" + serviceInfo.ServiceName + "</ServiceName>" +
                   "</DIServiceInfo>";
        }

        internal static XmlDocument GetLoginTemplate(string domain,string repository,string login,string password)
        {
            XmlDocument template = new XmlDocument();
            template.LoadXml(_envelopeHeader +
                   "<soap:Header/>" +
                    "<soap:Body xmlns:ns0=\"http://www.informatica.com/wsh\">" +
                    "<ns0:Login>" +
                     "<RepositoryDomainName>"+ domain + "</RepositoryDomainName>" +
                     "<RepositoryName>"+ repository + "</RepositoryName>" +
                     "<UserName>"+ login + "</UserName>" +
                     "<Password>"+ password + "</Password>" +
                     "<UserNameSpace/>" +
                     "</ns0:Login>" +
                   "</soap:Body>" +
                "</soap:Envelope>");
            return template;
        }
        internal static XmlDocument GetLogoutTemplate(string sessionId)
        {
            XmlDocument template = new XmlDocument();
            template.LoadXml(_envelopeHeader +
                   "<soap:Header>" +
                    "<ns0:Context xmlns:ns0 = \"http://www.informatica.com/wsh\">" +
                      "<SessionId>" + sessionId + " </SessionId>" +
                      "</ns0:Context>" +
                       "</soap:Header>" +
                       "<soap:Body>" +
                      "<ns0:Logout xmlns:ns0 = \"http://www.informatica.com/wsh\"/>" +
                   "</soap:Body>" +
                "</soap:Envelope>");
            return template;
        }

        internal static XmlDocument GetAllFoldersTemplate(string sessionId)
        {
            XmlDocument template = new XmlDocument();
            template.LoadXml(_envelopeHeader +
                   "<soap:Header>" +
                    "<ns0:Context xmlns:ns0 = \"http://www.informatica.com/wsh\">"+
                      "<SessionId>"+ sessionId + " </SessionId>"+
                      "</ns0:Context>"+
                       "</soap:Header>"+
                       "<soap:Body>"+
                      "<ns0:GetAllFolders xmlns:ns0 = \"http://www.informatica.com/wsh\"/>" +
                   "</soap:Body>" +
                "</soap:Envelope>");
            return template;
        }
        internal static XmlDocument GetAllRepositoriesTemplate(string sessionId)
        {
            XmlDocument template = new XmlDocument();
            template.LoadXml(_envelopeHeader +
                   "<soap:Header/>" +
                     "<soap:Body>" +
                      "<ns0:GetAllRepositories xmlns:ns0 = \"http://www.informatica.com/wsh\"/>" +
                     "</soap:Body>" +
                "</soap:Envelope>");
            return template;
        }
        internal static XmlDocument GetAllWorkflowsTemplate(string sessionId,string folderName)
        {
            XmlDocument template = new XmlDocument();
            template.LoadXml(_envelopeHeader +
                   "<soap:Header>" +
                    "<ns0:Context xmlns:ns0 = \"http://www.informatica.com/wsh\">" +
                      "<SessionId>" + sessionId + " </SessionId>" +
                      "</ns0:Context>" +
                       "</soap:Header>" +
                       "<soap:Body>" +
                      "<ns0:GetAllWorkflows xmlns:ns0 = \"http://www.informatica.com/wsh\">" +
                      "<Name>"+ folderName + "</Name>" +
                      "</ns0:GetAllWorkflows>" +
                   "</soap:Body>" +
                "</soap:Envelope>");
            return template;
        }
        internal static XmlDocument GetWorkflowLogTemplate(string sessionId, string folderName,string workflowName,int workflowRunId, DIServiceInfo serviceInfo,int timeout=60)
        {
            XmlDocument template = new XmlDocument();
            template.LoadXml(_envelopeHeader +
                   "<soap:Header>" +
                    "<ns0:Context xmlns:ns0 = \"http://www.informatica.com/wsh\">" +
                      "<SessionId>" + sessionId + "</SessionId>" +
                      "</ns0:Context>" +
                       "</soap:Header>" +
                       "<soap:Body>" +
                        "<ns0:GetWorkflowLog xmlns:ns0=\"http://www.informatica.com/wsh\">"+
                         ConvertDiServiceInfoToXMLText(serviceInfo) +
                         "<FolderName>" +folderName+"</FolderName>" +
                         "<WorkflowName>" +workflowName+"</WorkflowName>" +
                         "<WorkflowRunId>" +workflowRunId+"</WorkflowRunId>" +
                         "<WorkflowRunInstanceName>" +workflowName+"</WorkflowRunInstanceName>" +
                         "<Timeout>" +timeout+"</Timeout>" +
                      "</ns0:GetWorkflowLog>" +
                   "</soap:Body>" +
                "</soap:Envelope>");
            return template;
        }
        internal static XmlDocument GetWorkflowLogTemplate(string sessionId, string folderName, string workflowName,string workflowInstanceName, int workflowRunId, DIServiceInfo serviceInfo, int timeout = 60)
        {
            XmlDocument template = new XmlDocument();
            template.LoadXml(_envelopeHeader +
                   "<soap:Header>" +
                    "<ns0:Context xmlns:ns0 = \"http://www.informatica.com/wsh\">" +
                      "<SessionId>" + sessionId + "</SessionId>" +
                      "</ns0:Context>" +
                       "</soap:Header>" +
                       "<soap:Body>" +
                        "<ns0:GetWorkflowLog xmlns:ns0=\"http://www.informatica.com/wsh\">" +
                         ConvertDiServiceInfoToXMLText(serviceInfo) +
                         "<FolderName>" + folderName + "</FolderName>" +
                         "<WorkflowName>" + workflowName + "</WorkflowName>" +
                         "<WorkflowRunId>" + workflowRunId + "</WorkflowRunId>" +
                         "<WorkflowRunInstanceName>" + workflowInstanceName + "</WorkflowRunInstanceName>" +
                         "<Timeout>" + timeout + "</Timeout>" +
                      "</ns0:GetWorkflowLog>" +
                   "</soap:Body>" +
                "</soap:Envelope>");
            return template;
        }
        internal static XmlDocument GetSessionLogTemplate(string sessionId, string folderName, string workflowName,string taskInstancePath, DIServiceInfo serviceInfo, int timeout = 60)
        {
            XmlDocument template = new XmlDocument();
            template.LoadXml(_envelopeHeader +
                   "<soap:Header>" +
                    "<ns0:Context xmlns:ns0 = \"http://www.informatica.com/wsh\">" +
                      "<SessionId>" + sessionId + "</SessionId>" +
                      "</ns0:Context>" +
                       "</soap:Header>" +
                       "<soap:Body>" +
                        "<ns0:GetSessionLog xmlns:ns0=\"http://www.informatica.com/wsh\">" +
                         ConvertDiServiceInfoToXMLText(serviceInfo) +
                         "<FolderName>" + folderName + "</FolderName>" +
                         "<WorkflowName>" + workflowName + "</WorkflowName>" +
                         "<TaskInstancePath>" + taskInstancePath + "</TaskInstancePath>" +
                         "<Timeout>" + timeout + "</Timeout>" +
                      "</ns0:GetSessionLog>" +
                   "</soap:Body>" +
                "</soap:Envelope>");
            return template;
        }

        internal static XmlDocument GetDeinitServerConnectionTemplate(string sessionId, string folderName)
        {
            XmlDocument template = new XmlDocument();
            template.LoadXml(_envelopeHeader +
                   "<soap:Header>" +
                    "<ns0:Context xmlns:ns0 = \"http://www.informatica.com/wsh\">" +
                      "<SessionId>" + sessionId + " </SessionId>" +
                      "</ns0:Context>" +
                       "</soap:Header>" +
                       "<soap:Body>" +
                      "<ns0:GetAllWorkflows xmlns:ns0 = \"http://www.informatica.com/wsh\">" +
                      "<Name>" + folderName + "</Name>" +
                      "</ns0:GetAllWorkflows>" +
                   "</soap:Body>" +
                "</soap:Envelope>");
            return template;
        }
        internal static XmlDocument GetDIServerPropTemplate(string sessionId, string folderName)
        {
            XmlDocument template = new XmlDocument();
            template.LoadXml(_envelopeHeader +
                   "<soap:Header>" +
                    "<ns0:Context xmlns:ns0 = \"http://www.informatica.com/wsh\">" +
                      "<SessionId>" + sessionId + " </SessionId>" +
                      "</ns0:Context>" +
                       "</soap:Header>" +
                       "<soap:Body>" +
                      "<ns0:GetAllWorkflows xmlns:ns0 = \"http://www.informatica.com/wsh\">" +
                      "<Name>" + folderName + "</Name>" +
                      "</ns0:GetAllWorkflows>" +
                   "</soap:Body>" +
                "</soap:Envelope>");
            return template;
        }
        internal static XmlDocument GetNextLogSegmentTemplate(string sessionId, string folderName)
        {
            XmlDocument template = new XmlDocument();
            template.LoadXml(_envelopeHeader +
                   "<soap:Header>" +
                    "<ns0:Context xmlns:ns0 = \"http://www.informatica.com/wsh\">" +
                      "<SessionId>" + sessionId + " </SessionId>" +
                      "</ns0:Context>" +
                       "</soap:Header>" +
                       "<soap:Body>" +
                      "<ns0:GetAllWorkflows xmlns:ns0 = \"http://www.informatica.com/wsh\">" +
                      "<Name>" + folderName + "</Name>" +
                      "</ns0:GetAllWorkflows>" +
                   "</soap:Body>" +
                "</soap:Envelope>");
            return template;
        }
        internal static XmlDocument GetSessionPerfomanceDataTemplate(string sessionId, string folderName)
        {
            XmlDocument template = new XmlDocument();
            template.LoadXml(_envelopeHeader +
                   "<soap:Header>" +
                    "<ns0:Context xmlns:ns0 = \"http://www.informatica.com/wsh\">" +
                      "<SessionId>" + sessionId + " </SessionId>" +
                      "</ns0:Context>" +
                       "</soap:Header>" +
                       "<soap:Body>" +
                      "<ns0:GetAllWorkflows xmlns:ns0 = \"http://www.informatica.com/wsh\">" +
                      "<Name>" + folderName + "</Name>" +
                      "</ns0:GetAllWorkflows>" +
                   "</soap:Body>" +
                "</soap:Envelope>");
            return template;
        }
        internal static XmlDocument GetSessionStatisticTemplate(string sessionId, string folderName)
        {
            XmlDocument template = new XmlDocument();
            template.LoadXml(_envelopeHeader +
                   "<soap:Header>" +
                    "<ns0:Context xmlns:ns0 = \"http://www.informatica.com/wsh\">" +
                      "<SessionId>" + sessionId + " </SessionId>" +
                      "</ns0:Context>" +
                       "</soap:Header>" +
                       "<soap:Body>" +
                      "<ns0:GetAllWorkflows xmlns:ns0 = \"http://www.informatica.com/wsh\">" +
                      "<Name>" + folderName + "</Name>" +
                      "</ns0:GetAllWorkflows>" +
                   "</soap:Body>" +
                "</soap:Envelope>");
            return template;
        }
        internal static XmlDocument GetTaskDetailTemplate(string sessionId, string folderName)
        {
            XmlDocument template = new XmlDocument();
            template.LoadXml(_envelopeHeader +
                   "<soap:Header>" +
                    "<ns0:Context xmlns:ns0 = \"http://www.informatica.com/wsh\">" +
                      "<SessionId>" + sessionId + " </SessionId>" +
                      "</ns0:Context>" +
                       "</soap:Header>" +
                       "<soap:Body>" +
                      "<ns0:GetAllWorkflows xmlns:ns0 = \"http://www.informatica.com/wsh\">" +
                      "<Name>" + folderName + "</Name>" +
                      "</ns0:GetAllWorkflows>" +
                   "</soap:Body>" +
                "</soap:Envelope>");
            return template;
        }
        internal static XmlDocument GetTaskDetailExTemplate(string sessionId, string folderName)
        {
            XmlDocument template = new XmlDocument();
            template.LoadXml(_envelopeHeader +
                   "<soap:Header>" +
                    "<ns0:Context xmlns:ns0 = \"http://www.informatica.com/wsh\">" +
                      "<SessionId>" + sessionId + " </SessionId>" +
                      "</ns0:Context>" +
                       "</soap:Header>" +
                       "<soap:Body>" +
                      "<ns0:GetAllWorkflows xmlns:ns0 = \"http://www.informatica.com/wsh\">" +
                      "<Name>" + folderName + "</Name>" +
                      "</ns0:GetAllWorkflows>" +
                   "</soap:Body>" +
                "</soap:Envelope>");
            return template;
        }
        internal static XmlDocument GetWorkflowDetailTemplate(string sessionId, string folderName)
        {
            XmlDocument template = new XmlDocument();
            template.LoadXml(_envelopeHeader +
                   "<soap:Header>" +
                    "<ns0:Context xmlns:ns0 = \"http://www.informatica.com/wsh\">" +
                      "<SessionId>" + sessionId + " </SessionId>" +
                      "</ns0:Context>" +
                       "</soap:Header>" +
                       "<soap:Body>" +
                      "<ns0:GetAllWorkflows xmlns:ns0 = \"http://www.informatica.com/wsh\">" +
                      "<Name>" + folderName + "</Name>" +
                      "</ns0:GetAllWorkflows>" +
                   "</soap:Body>" +
                "</soap:Envelope>");
            return template;
        }
        internal static XmlDocument GetWorkflowDetailExTemplate(string sessionId, string folderName)
        {
            XmlDocument template = new XmlDocument();
            template.LoadXml(_envelopeHeader +
                   "<soap:Header>" +
                    "<ns0:Context xmlns:ns0 = \"http://www.informatica.com/wsh\">" +
                      "<SessionId>" + sessionId + " </SessionId>" +
                      "</ns0:Context>" +
                       "</soap:Header>" +
                       "<soap:Body>" +
                      "<ns0:GetAllWorkflows xmlns:ns0 = \"http://www.informatica.com/wsh\">" +
                      "<Name>" + folderName + "</Name>" +
                      "</ns0:GetAllWorkflows>" +
                   "</soap:Body>" +
                "</soap:Envelope>");
            return template;
        }
        internal static XmlDocument GetInitServerConnectionTemplate(string sessionId, string folderName)
        {
            XmlDocument template = new XmlDocument();
            template.LoadXml(_envelopeHeader +
                   "<soap:Header>" +
                    "<ns0:Context xmlns:ns0 = \"http://www.informatica.com/wsh\">" +
                      "<SessionId>" + sessionId + " </SessionId>" +
                      "</ns0:Context>" +
                       "</soap:Header>" +
                       "<soap:Body>" +
                      "<ns0:GetAllWorkflows xmlns:ns0 = \"http://www.informatica.com/wsh\">" +
                      "<Name>" + folderName + "</Name>" +
                      "</ns0:GetAllWorkflows>" +
                   "</soap:Body>" +
                "</soap:Envelope>");
            return template;
        }
        internal static XmlDocument GetMonitorDIServerTemplate(string sessionId, string folderName)
        {
            XmlDocument template = new XmlDocument();
            template.LoadXml(_envelopeHeader +
                   "<soap:Header>" +
                    "<ns0:Context xmlns:ns0 = \"http://www.informatica.com/wsh\">" +
                      "<SessionId>" + sessionId + " </SessionId>" +
                      "</ns0:Context>" +
                       "</soap:Header>" +
                       "<soap:Body>" +
                      "<ns0:GetAllWorkflows xmlns:ns0 = \"http://www.informatica.com/wsh\">" +
                      "<Name>" + folderName + "</Name>" +
                      "</ns0:GetAllWorkflows>" +
                   "</soap:Body>" +
                "</soap:Envelope>");
            return template;
        }
        internal static XmlDocument GetPingDIServerTemplate(string sessionId, string folderName)
        {
            XmlDocument template = new XmlDocument();
            template.LoadXml(_envelopeHeader +
                   "<soap:Header>" +
                    "<ns0:Context xmlns:ns0 = \"http://www.informatica.com/wsh\">" +
                      "<SessionId>" + sessionId + " </SessionId>" +
                      "</ns0:Context>" +
                       "</soap:Header>" +
                       "<soap:Body>" +
                      "<ns0:GetAllWorkflows xmlns:ns0 = \"http://www.informatica.com/wsh\">" +
                      "<Name>" + folderName + "</Name>" +
                      "</ns0:GetAllWorkflows>" +
                   "</soap:Body>" +
                "</soap:Envelope>");
            return template;
        }
        internal static XmlDocument GetRecoverWorkflowTemplate(string sessionId, string folderName)
        {
            XmlDocument template = new XmlDocument();
            template.LoadXml(_envelopeHeader +
                   "<soap:Header>" +
                    "<ns0:Context xmlns:ns0 = \"http://www.informatica.com/wsh\">" +
                      "<SessionId>" + sessionId + " </SessionId>" +
                      "</ns0:Context>" +
                       "</soap:Header>" +
                       "<soap:Body>" +
                      "<ns0:GetAllWorkflows xmlns:ns0 = \"http://www.informatica.com/wsh\">" +
                      "<Name>" + folderName + "</Name>" +
                      "</ns0:GetAllWorkflows>" +
                   "</soap:Body>" +
                "</soap:Envelope>");
            return template;
        }
        internal static XmlDocument GetResumeWorkflowTemplate(string sessionId, string folderName)
        {
            XmlDocument template = new XmlDocument();
            template.LoadXml(_envelopeHeader +
                   "<soap:Header>" +
                    "<ns0:Context xmlns:ns0 = \"http://www.informatica.com/wsh\">" +
                      "<SessionId>" + sessionId + " </SessionId>" +
                      "</ns0:Context>" +
                       "</soap:Header>" +
                       "<soap:Body>" +
                      "<ns0:GetAllWorkflows xmlns:ns0 = \"http://www.informatica.com/wsh\">" +
                      "<Name>" + folderName + "</Name>" +
                      "</ns0:GetAllWorkflows>" +
                   "</soap:Body>" +
                "</soap:Envelope>");
            return template;
        }
        internal static XmlDocument GetScheduleWorkflowTemplate(string sessionId, string folderName)
        {
            XmlDocument template = new XmlDocument();
            template.LoadXml(_envelopeHeader +
                   "<soap:Header>" +
                    "<ns0:Context xmlns:ns0 = \"http://www.informatica.com/wsh\">" +
                      "<SessionId>" + sessionId + " </SessionId>" +
                      "</ns0:Context>" +
                       "</soap:Header>" +
                       "<soap:Body>" +
                      "<ns0:GetAllWorkflows xmlns:ns0 = \"http://www.informatica.com/wsh\">" +
                      "<Name>" + folderName + "</Name>" +
                      "</ns0:GetAllWorkflows>" +
                   "</soap:Body>" +
                "</soap:Envelope>");
            return template;
        }
        internal static XmlDocument GetStartSessLogFetchTemplate(string sessionId, string folderName)
        {
            XmlDocument template = new XmlDocument();
            template.LoadXml(_envelopeHeader +
                   "<soap:Header>" +
                    "<ns0:Context xmlns:ns0 = \"http://www.informatica.com/wsh\">" +
                      "<SessionId>" + sessionId + " </SessionId>" +
                      "</ns0:Context>" +
                       "</soap:Header>" +
                       "<soap:Body>" +
                      "<ns0:GetAllWorkflows xmlns:ns0 = \"http://www.informatica.com/wsh\">" +
                      "<Name>" + folderName + "</Name>" +
                      "</ns0:GetAllWorkflows>" +
                   "</soap:Body>" +
                "</soap:Envelope>");
            return template;
        }
        internal static XmlDocument GetStartTaskTemplate(string sessionId, string folderName)
        {
            XmlDocument template = new XmlDocument();
            template.LoadXml(_envelopeHeader +
                   "<soap:Header>" +
                    "<ns0:Context xmlns:ns0 = \"http://www.informatica.com/wsh\">" +
                      "<SessionId>" + sessionId + " </SessionId>" +
                      "</ns0:Context>" +
                       "</soap:Header>" +
                       "<soap:Body>" +
                      "<ns0:GetAllWorkflows xmlns:ns0 = \"http://www.informatica.com/wsh\">" +
                      "<Name>" + folderName + "</Name>" +
                      "</ns0:GetAllWorkflows>" +
                   "</soap:Body>" +
                "</soap:Envelope>");
            return template;
        }
        internal static XmlDocument GetStartWorkflowTemplate(string sessionId, string folderName)
        {
            XmlDocument template = new XmlDocument();
            template.LoadXml(_envelopeHeader +
                   "<soap:Header>" +
                    "<ns0:Context xmlns:ns0 = \"http://www.informatica.com/wsh\">" +
                      "<SessionId>" + sessionId + " </SessionId>" +
                      "</ns0:Context>" +
                       "</soap:Header>" +
                       "<soap:Body>" +
                      "<ns0:GetAllWorkflows xmlns:ns0 = \"http://www.informatica.com/wsh\">" +
                      "<Name>" + folderName + "</Name>" +
                      "</ns0:GetAllWorkflows>" +
                   "</soap:Body>" +
                "</soap:Envelope>");
            return template;
        }
        internal static XmlDocument GetStartWorkflowExTemplate(string sessionId, string folderName)
        {
            XmlDocument template = new XmlDocument();
            template.LoadXml(_envelopeHeader +
                   "<soap:Header>" +
                    "<ns0:Context xmlns:ns0 = \"http://www.informatica.com/wsh\">" +
                      "<SessionId>" + sessionId + " </SessionId>" +
                      "</ns0:Context>" +
                       "</soap:Header>" +
                       "<soap:Body>" +
                      "<ns0:GetAllWorkflows xmlns:ns0 = \"http://www.informatica.com/wsh\">" +
                      "<Name>" + folderName + "</Name>" +
                      "</ns0:GetAllWorkflows>" +
                   "</soap:Body>" +
                "</soap:Envelope>");
            return template;
        }
        internal static XmlDocument GetStartWorkflowFromTaskTemplate(string sessionId, string folderName)
        {
            XmlDocument template = new XmlDocument();
            template.LoadXml(_envelopeHeader +
                   "<soap:Header>" +
                    "<ns0:Context xmlns:ns0 = \"http://www.informatica.com/wsh\">" +
                      "<SessionId>" + sessionId + " </SessionId>" +
                      "</ns0:Context>" +
                       "</soap:Header>" +
                       "<soap:Body>" +
                      "<ns0:GetAllWorkflows xmlns:ns0 = \"http://www.informatica.com/wsh\">" +
                      "<Name>" + folderName + "</Name>" +
                      "</ns0:GetAllWorkflows>" +
                   "</soap:Body>" +
                "</soap:Envelope>");
            return template;
        }
        internal static XmlDocument GetStartWorkflowLogFetchTemplate(string sessionId, string folderName)
        {
            XmlDocument template = new XmlDocument();
            template.LoadXml(_envelopeHeader +
                   "<soap:Header>" +
                    "<ns0:Context xmlns:ns0 = \"http://www.informatica.com/wsh\">" +
                      "<SessionId>" + sessionId + " </SessionId>" +
                      "</ns0:Context>" +
                       "</soap:Header>" +
                       "<soap:Body>" +
                      "<ns0:GetAllWorkflows xmlns:ns0 = \"http://www.informatica.com/wsh\">" +
                      "<Name>" + folderName + "</Name>" +
                      "</ns0:GetAllWorkflows>" +
                   "</soap:Body>" +
                "</soap:Envelope>");
            return template;
        }
        internal static XmlDocument GetStopTaskTemplate(string sessionId, string folderName)
        {
            XmlDocument template = new XmlDocument();
            template.LoadXml(_envelopeHeader +
                   "<soap:Header>" +
                    "<ns0:Context xmlns:ns0 = \"http://www.informatica.com/wsh\">" +
                      "<SessionId>" + sessionId + " </SessionId>" +
                      "</ns0:Context>" +
                       "</soap:Header>" +
                       "<soap:Body>" +
                      "<ns0:GetAllWorkflows xmlns:ns0 = \"http://www.informatica.com/wsh\">" +
                      "<Name>" + folderName + "</Name>" +
                      "</ns0:GetAllWorkflows>" +
                   "</soap:Body>" +
                "</soap:Envelope>");
            return template;
        }
        internal static XmlDocument GetStopWorkflowTemplate(string sessionId, string folderName)
        {
            XmlDocument template = new XmlDocument();
            template.LoadXml(_envelopeHeader +
                   "<soap:Header>" +
                    "<ns0:Context xmlns:ns0 = \"http://www.informatica.com/wsh\">" +
                      "<SessionId>" + sessionId + " </SessionId>" +
                      "</ns0:Context>" +
                       "</soap:Header>" +
                       "<soap:Body>" +
                      "<ns0:GetAllWorkflows xmlns:ns0 = \"http://www.informatica.com/wsh\">" +
                      "<Name>" + folderName + "</Name>" +
                      "</ns0:GetAllWorkflows>" +
                   "</soap:Body>" +
                "</soap:Envelope>");
            return template;
        }
        internal static XmlDocument GetUncheduleWorkflowTemplate(string sessionId, string folderName)
        {
            XmlDocument template = new XmlDocument();
            template.LoadXml(_envelopeHeader +
                   "<soap:Header>" +
                    "<ns0:Context xmlns:ns0 = \"http://www.informatica.com/wsh\">" +
                      "<SessionId>" + sessionId + " </SessionId>" +
                      "</ns0:Context>" +
                       "</soap:Header>" +
                       "<soap:Body>" +
                      "<ns0:GetAllWorkflows xmlns:ns0 = \"http://www.informatica.com/wsh\">" +
                      "<Name>" + folderName + "</Name>" +
                      "</ns0:GetAllWorkflows>" +
                   "</soap:Body>" +
                "</soap:Envelope>");
            return template;
        }
        internal static XmlDocument GetWaitTillTaskCompleteTemplate(string sessionId, string folderName)
        {
            XmlDocument template = new XmlDocument();
            template.LoadXml(_envelopeHeader +
                   "<soap:Header>" +
                    "<ns0:Context xmlns:ns0 = \"http://www.informatica.com/wsh\">" +
                      "<SessionId>" + sessionId + " </SessionId>" +
                      "</ns0:Context>" +
                       "</soap:Header>" +
                       "<soap:Body>" +
                      "<ns0:GetAllWorkflows xmlns:ns0 = \"http://www.informatica.com/wsh\">" +
                      "<Name>" + folderName + "</Name>" +
                      "</ns0:GetAllWorkflows>" +
                   "</soap:Body>" +
                "</soap:Envelope>");
            return template;
        }
        internal static XmlDocument GetWaitTillWorkflowCompleteTemplate(string sessionId, string folderName)
        {
            XmlDocument template = new XmlDocument();
            template.LoadXml(_envelopeHeader +
                   "<soap:Header>" +
                    "<ns0:Context xmlns:ns0 = \"http://www.informatica.com/wsh\">" +
                      "<SessionId>" + sessionId + " </SessionId>" +
                      "</ns0:Context>" +
                       "</soap:Header>" +
                       "<soap:Body>" +
                      "<ns0:GetAllWorkflows xmlns:ns0 = \"http://www.informatica.com/wsh\">" +
                      "<Name>" + folderName + "</Name>" +
                      "</ns0:GetAllWorkflows>" +
                   "</soap:Body>" +
                "</soap:Envelope>");
            return template;
        }


    }
}
