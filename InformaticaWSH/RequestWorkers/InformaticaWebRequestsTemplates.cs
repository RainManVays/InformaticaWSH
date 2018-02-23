using System.Collections.Generic;
using System.Xml;

namespace InformaticaWSH
{
   internal static class InformaticaWebRequestsTemplates
    {
        private static string _envelopeHeader = "<soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\" " +
            "xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" " +
            "xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">";
        private static string _informaticaWshLink = "\"http://www.informatica.com/wsh\"";
        private static string ConvertDiServiceInfoToXMLText(DIServiceInfo serviceInfo)
        {
            return "<DIServiceInfo>" +
                        "<DomainName>" + serviceInfo.DomainName + "</DomainName>" +
                        "<ServiceName>" + serviceInfo.ServiceName + "</ServiceName>" +
                   "</DIServiceInfo>";
        }
        private static string ConvertParametersToXmlText(List<TaskParam> param)
        {
            if (param == null) return "";
            string result = "";
            foreach (TaskParam item in param)
                result += "<Parameters>" +
                    "<Scope>" + item.Scope + "</Scope>" +
                    "<Name>" + item.Name + "</Name>" +
                    "<Value>" + item.Value + "</Value>"
                    + "</Parameters>";

            return result;
        }

        private static string ConvertWorkflowInformToXmlText(WorkflowInformParams workflowInfo)
        {
            return
                ConvertElementAndValueToXmlText("FolderName", workflowInfo.FolderName) +
                ConvertElementAndValueToXmlText("WorkflowName", workflowInfo.WorkflowName) +
                ConvertElementAndValueToXmlText("WorkflowRunId", workflowInfo.WorkflowRunId==0?"":workflowInfo.WorkflowRunId.ToString()) +
                ConvertElementAndValueToXmlText("WorkflowRunInstanceName", workflowInfo.WorkflowInstanceName);
        }
        private static string ConvertWorkflowInformExToXmlText(WorkflowInforExParams workflowInfoEx)
        {
            return ConvertElementAndValueToXmlText("FolderName", workflowInfoEx.FolderName) +
                ConvertElementAndValueToXmlText("WorkflowName", workflowInfoEx.WorkflowName) +
                ConvertElementAndValueToXmlText("WorkflowRunInstanceName", workflowInfoEx.WorkflowInstanceName);
        }

        private static string ConvertElementAndValueToXmlText(string elementName,string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return "<"+ elementName + " xsi:nil=\"true\"/>";
            return "<" + elementName + ">" + value + "</" + elementName + ">";
        }

        private static string ConvertAttributeToXmlText(List<TaskAttribute> attribute)
        {
            if (attribute == null) return "";
            string result = "";
            foreach (TaskAttribute item in attribute)
                result += "<Attribute>" +
                    "<Name>" + item.Name + "</Name>" +
                    "<Value>" + item.Value + "</Value>"
                    + "</Attribute>";

            return result;
        }

        private static string ConvertKeyToXmlText(List<TaskKey> key)
        {
            if (key == null)
                return "<Key>" +
                    "<Key></Key>" +
                    "<mustUse>false</mustUse>"
                    + "</Key>";
            string result = "";
            foreach (TaskKey item in key)
                result += "<Key>" +
                    "<Key>" + item.Key + "</Key>" +
                    "<mustUse>" + item.MustUse + "</mustUse>"
                    + "</Key>";
            return result;
        }

 
        internal static XmlDocument GetLoginTemplate(string domain,string repository,string login,string password)
        {
            XmlDocument template = new XmlDocument();
            template.LoadXml(_envelopeHeader +
                   "<soap:Header/>" +
                    "<soap:Body xmlns:ns0="+_informaticaWshLink+"> " +
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
                    "<ns0:Context xmlns:ns0 = "+_informaticaWshLink+">" +
                      "<SessionId>" + sessionId + " </SessionId>" +
                      "</ns0:Context>" +
                       "</soap:Header>" +
                       "<soap:Body>" +
                      "<ns0:Logout xmlns:ns0 = "+_informaticaWshLink+"/>" +
                   "</soap:Body>" +
                "</soap:Envelope>");
            return template;
        }

        internal static XmlDocument GetAllFoldersTemplate(string sessionId)
        {
            XmlDocument template = new XmlDocument();
            template.LoadXml(_envelopeHeader +
                   "<soap:Header>" +
                    "<ns0:Context xmlns:ns0="+_informaticaWshLink+">"+
                      "<SessionId>"+ sessionId + " </SessionId>"+
                      "</ns0:Context>"+
                       "</soap:Header>"+
                       "<soap:Body>"+
                      "<ns0:GetAllFolders xmlns:ns0=" + _informaticaWshLink+"/>" +
                   "</soap:Body>" +
                "</soap:Envelope>");
            return template;
        }
        internal static XmlDocument GetAllDIServersTemplate(string sessionId)
        {
            XmlDocument template = new XmlDocument();
            template.LoadXml(_envelopeHeader +
                   "<soap:Header>" +
                    "<ns0:Context xmlns:ns0=" + _informaticaWshLink + ">" +
                      "<SessionId>" + sessionId + " </SessionId>" +
                      "</ns0:Context>" +
                       "</soap:Header>" +
                       "<soap:Body>" +
                      "<ns0:GetAllDIServers xmlns:ns0=" + _informaticaWshLink + "/>" +
                   "</soap:Body>" +
                "</soap:Envelope>");
            return template;
        }
        internal static XmlDocument GetAllRepositoriesTemplate()
        {
            XmlDocument template = new XmlDocument();
            template.LoadXml(_envelopeHeader +
                   "<soap:Header/>" +
                     "<soap:Body>" +
                      "<ns0:GetAllRepositories xmlns:ns0 = "+_informaticaWshLink+"/>" +
                     "</soap:Body>" +
                "</soap:Envelope>");
            return template;
        }
        internal static XmlDocument GetAllWorkflowsTemplate(string sessionId,string folderName)
        {
            XmlDocument template = new XmlDocument();
            template.LoadXml(_envelopeHeader +
                   "<soap:Header>" +
                    "<ns0:Context xmlns:ns0 = "+_informaticaWshLink+">" +
                      "<SessionId>" + sessionId + " </SessionId>" +
                      "</ns0:Context>" +
                       "</soap:Header>" +
                       "<soap:Body>" +
                      "<ns0:GetAllWorkflows xmlns:ns0 = "+_informaticaWshLink+">" +
                      "<Name>"+ folderName + "</Name>" +
                      "</ns0:GetAllWorkflows>" +
                   "</soap:Body>" +
                "</soap:Envelope>");
            return template;
        }
        internal static XmlDocument GetAllTaskInstancesTemplate(string sessionId, string folderName,string workflowName,int depth,bool isValid)
        {
            XmlDocument template = new XmlDocument();
            template.LoadXml(_envelopeHeader +
                   "<soap:Header>" +
                    "<ns0:Context xmlns:ns0 = " + _informaticaWshLink + ">" +
                      "<SessionId>" + sessionId + " </SessionId>" +
                      "</ns0:Context>" +
                       "</soap:Header>" +
                       "<soap:Body>" +
                      "<ns0:GetAllTaskInstances xmlns:ns0 = " + _informaticaWshLink + ">" +
                      "<WorkflowInfo>" + 
                      "<Name>" + workflowName + "</Name>" +
                      "<IsValid>" + isValid + "</IsValid>" +
                      "<FolderName>" + folderName + "</FolderName>" +
                      "</WorkflowInfo>" +
                      "<Depth>" + depth + "</Depth>" +
                      "</ns0:GetAllTaskInstances>" +
                   "</soap:Body>" +
                "</soap:Envelope>");
            return template;
        }
        internal static XmlDocument GetWorkflowLogTemplate(string sessionId, WorkflowInformParams workflowInfo, DIServiceInfo serviceInfo,int timeout)
        {
            XmlDocument template = new XmlDocument();
            template.LoadXml(_envelopeHeader +
                   "<soap:Header>" +
                    "<ns0:Context xmlns:ns0 = "+_informaticaWshLink+">" +
                      "<SessionId>" + sessionId + "</SessionId>" +
                      "</ns0:Context>" +
                       "</soap:Header>" +
                       "<soap:Body>" +
                        "<ns0:GetWorkflowLog xmlns:ns0="+_informaticaWshLink+">"+
                         ConvertDiServiceInfoToXMLText(serviceInfo) +
                         ConvertWorkflowInformToXmlText(workflowInfo) +
                         "<Timeout>" +timeout+"</Timeout>" +
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
                    "<ns0:Context xmlns:ns0 = "+_informaticaWshLink+">" +
                      "<SessionId>" + sessionId + "</SessionId>" +
                      "</ns0:Context>" +
                       "</soap:Header>" +
                       "<soap:Body>" +
                        "<ns0:GetSessionLog xmlns:ns0="+_informaticaWshLink+">" +
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

        internal static XmlDocument GetDeinitServerConnectionTemplate(string sessionId)
        {
            XmlDocument template = new XmlDocument();
            template.LoadXml(_envelopeHeader +
                   "<soap:Header>" +
                    "<ns0:Context xmlns:ns0 = "+_informaticaWshLink+">" +
                      "<SessionId>" + sessionId + " </SessionId>" +
                      "</ns0:Context>" +
                       "</soap:Header>" +
                       "<soap:Body>" +
                      "<ns0:DeinitializeDIServerConnection xmlns:ns0= " + _informaticaWshLink+"/>" +
                   "</soap:Body>" +
                "</soap:Envelope>");
            return template;
        }
        internal static XmlDocument GetDIServerPropTemplate(string sessionId,DIServiceInfo serviceInfo)
        {
            XmlDocument template = new XmlDocument();
            template.LoadXml(_envelopeHeader +
                   "<soap:Header>" +
                    "<ns0:Context xmlns:ns0 = "+_informaticaWshLink+">" +
                      "<SessionId>" + sessionId + " </SessionId>" +
                      "</ns0:Context>" +
                       "</soap:Header>" +
                       "<soap:Body>" +
                      "<ns0:GetDIServerProperties  xmlns:ns0 = " + _informaticaWshLink+">" +
                      "<DomainName>"+ serviceInfo.DomainName+ "</DomainName>"+
                      "<ServiceName>"+serviceInfo.ServiceName+"</ServiceName>" +
                      "</ns0:GetDIServerProperties>" +
                   "</soap:Body>" +
                "</soap:Envelope>");
            return template;
        }
        internal static XmlDocument GetNextLogSegmentTemplate(string sessionId,int logHandle,int timeOut )
        {
            XmlDocument template = new XmlDocument();
            template.LoadXml(_envelopeHeader +
                   "<soap:Header>" +
                    "<ns0:Context xmlns:ns0 = "+_informaticaWshLink+">" +
                      "<SessionId>" + sessionId + " </SessionId>" +
                      "</ns0:Context>" +
                       "</soap:Header>" +
                       "<soap:Body>" +
                      "<ns0:GetNextLogSegment xmlns:ns0 = " + _informaticaWshLink + ">" +
                      "<LogHandle>" + logHandle + "</LogHandle>" +
                      "<TimeOut>" + timeOut + "</TimeOut>" +
                      "</ns0:GetNextLogSegment>" +
                   "</soap:Body>" +
                "</soap:Envelope>");
            return template;
        }
        internal static XmlDocument GetSessionPerfomanceDataTemplate(string sessionId, string folderName,string workflowName,string taskInstancePath,DIServiceInfo serviceInfo)
        {
            XmlDocument template = new XmlDocument();
            template.LoadXml(_envelopeHeader +
                   "<soap:Header>" +
                    "<ns0:Context xmlns:ns0 = "+_informaticaWshLink+">" +
                      "<SessionId>" + sessionId + " </SessionId>" +
                      "</ns0:Context>" +
                       "</soap:Header>" +
                       "<soap:Body>" +
                     "<ns0:GetSessionPerformanceData xmlns:ns0 = " + _informaticaWshLink + ">" +
                        ConvertDiServiceInfoToXMLText(serviceInfo) +
                         "<FolderName>" + folderName + "</FolderName>" +
                         "<WorkflowName>" + workflowName + "</WorkflowName>" +
                         "<TaskInstancePath>" + taskInstancePath + "</TaskInstancePath>" +
                      "</ns0:GetSessionPerformanceData>" +
                   "</soap:Body>" +
                "</soap:Envelope>");
            return template;
        }
        internal static XmlDocument GetSessionStatisticTemplate(string sessionId, string folderName, string workflowName, string taskInstancePath, DIServiceInfo serviceInfo)
        {
            XmlDocument template = new XmlDocument();
            template.LoadXml(_envelopeHeader +
                   "<soap:Header>" +
                    "<ns0:Context xmlns:ns0 = "+_informaticaWshLink+">" +
                      "<SessionId>" + sessionId + " </SessionId>" +
                      "</ns0:Context>" +
                       "</soap:Header>" +
                       "<soap:Body>" +
                      "<ns0:GetSessionStatistics xmlns:ns0 = " + _informaticaWshLink + ">" +
                        ConvertDiServiceInfoToXMLText(serviceInfo) +
                         "<FolderName>" + folderName + "</FolderName>" +
                         "<WorkflowName>" + workflowName + "</WorkflowName>" +
                         "<TaskInstancePath>" + taskInstancePath + "</TaskInstancePath>" +
                      "</ns0:GetSessionStatistics>" +
                   "</soap:Body>" +
                "</soap:Envelope>");
            return template;
        }
        internal static XmlDocument GetTaskDetailTemplate(string sessionId,
                                                            WorkflowInformParams workflowInfo,
                                                            string parameterFileName,
                                                            List<TaskParam> param,
                                                            RequestMode mode,
                                                            string taskInstancePath,
                                                            bool isAbort,
                                                            DIServiceInfo serviceInfo)
        {
            XmlDocument template = new XmlDocument();
            template.LoadXml(_envelopeHeader +
                   "<soap:Header>" +
                    "<ns0:Context xmlns:ns0 = " + _informaticaWshLink + ">" +
                      "<SessionId>" + sessionId + " </SessionId>" +
                      "</ns0:Context>" +
                       "</soap:Header>" +
                       "<soap:Body>" +
                       "<ns0:GetTaskDetails xmlns:ns0 = " + _informaticaWshLink + ">" +
                        ConvertDiServiceInfoToXMLText(serviceInfo) +
                         ConvertWorkflowInformToXmlText(workflowInfo) +
                         "<ParameterFileName>" + parameterFileName + "</ParameterFileName>"+
                         "<Parameters>" +
                           ConvertParametersToXmlText(param)+
                         "</Parameters>"+
                         "<RequestMode>"+ mode + "</RequestMode>" +
                         "<TaskInstancePath>"+ taskInstancePath + "</TaskInstancePath>" +
                         "<IsAbort>"+ isAbort + "</IsAbort>"+
                      "</ns0:GetTaskDetails>" +
                   "</soap:Body>" +
                "</soap:Envelope>");
            return template;
        }
        internal static XmlDocument GetTaskDetailExTemplate(string sessionId,string taskInstancePath, WorkflowInforExParams workflowInfoEx, DIServiceInfo serviceInfo)
        {
            XmlDocument template = new XmlDocument();
            template.LoadXml(_envelopeHeader +
                   "<soap:Header>" +
                    "<ns0:Context xmlns:ns0 = "+_informaticaWshLink+">" +
                      "<SessionId>" + sessionId + " </SessionId>" +
                      "</ns0:Context>" +
                       "</soap:Header>" +
                       "<soap:Body>" +
                      "<ns0:GetTaskDetailsEx  xmlns:ns0 = " + _informaticaWshLink+">" +
                        ConvertDiServiceInfoToXMLText(serviceInfo) +
                        ConvertWorkflowInformExToXmlText(workflowInfoEx) +
                         "<TaskInstancePath>" + taskInstancePath + "</TaskInstancePath>" +
                      "</ns0:GetTaskDetailsEx>" +
                   "</soap:Body>" +
                "</soap:Envelope>");
            return template;
        }
        internal static XmlDocument GetWorkflowDetailTemplate(string sessionId,List<TaskAttribute> attribute,List<TaskKey> key, List<TaskParam> param, RequestMode requestMode, WorkflowInformParams workflowInfo, DIServiceInfo serviceInfo, string parameterFileName, string taskInstancePath, bool isAbort, string osUser, string reason)
        {
            XmlDocument template = new XmlDocument();
            template.LoadXml(_envelopeHeader +
                   "<soap:Header>" +
                    "<ns0:Context xmlns:ns0 = "+_informaticaWshLink+">" +
                      "<SessionId>" + sessionId + " </SessionId>" +
                      "</ns0:Context>" +
                       "</soap:Header>" +
                       "<soap:Body>" +
                       "<ns0:GetWorkflowDetails xmlns:ns0 = " + _informaticaWshLink + ">" +
                        ConvertDiServiceInfoToXMLText(serviceInfo) +
                         ConvertWorkflowInformToXmlText(workflowInfo) +
                          "<Reason>" + reason + "</Reason>" +
                           ConvertAttributeToXmlText(attribute) +
                           ConvertKeyToXmlText(key) +
                         "<ParameterFileName>" + parameterFileName + "</ParameterFileName>" +
                         "<Parameters>" +
                           ConvertParametersToXmlText(param) +
                         "</Parameters>" +
                         "<RequestMode>" + requestMode + "</RequestMode>" +
                         "<TaskInstancePath>" + taskInstancePath + "</TaskInstancePath>" +
                         "<IsAbort>" + isAbort + "</IsAbort>" +
                         "<OSUser>" + osUser + "</OSUser>" +
                      "</ns0:GetWorkflowDetails>" +
                   "</soap:Body>" +
                "</soap:Envelope>");
            return template;
        }
        internal static XmlDocument GetWorkflowDetailExTemplate(string sessionId, WorkflowInformParams workflowInfo, DIServiceInfo serviceInfo)
        {
            XmlDocument template = new XmlDocument();
            template.LoadXml(_envelopeHeader +
                   "<soap:Header>" +
                    "<ns0:Context xmlns:ns0 = "+_informaticaWshLink+">" +
                      "<SessionId>" + sessionId + " </SessionId>" +
                      "</ns0:Context>" +
                       "</soap:Header>" +
                       "<soap:Body>" +
                       "<ns0:GetWorkflowDetailsEx xmlns:ns0 = " + _informaticaWshLink + ">" +
                        ConvertDiServiceInfoToXMLText(serviceInfo) +
                         ConvertWorkflowInformToXmlText(workflowInfo) +
                      "</ns0:GetWorkflowDetailsEx>" +
                   "</soap:Body>" +
                "</soap:Envelope>");
            return template;
        }
        internal static XmlDocument GetInitServerConnectionTemplate(string sessionId, string loginHandle, string serverName,string domainName)
        {
            XmlDocument template = new XmlDocument();
            template.LoadXml(_envelopeHeader +
                   "<soap:Header>" +
                    "<ns0:Context xmlns:ns0 = "+_informaticaWshLink+">" +
                      "<SessionId>" + sessionId + " </SessionId>" +
                      "</ns0:Context>" +
                       "</soap:Header>" +
                       "<soap:Body>" +
                      "<ns0:InitializeDIServerConnection  xmlns:ns0 = " + _informaticaWshLink+">" +
                      "<LoginHandle>" + loginHandle + "</LoginHandle>" +
                      "<DIServerName>" + serverName + "</DIServerName>" +
                      "<DIServerDomain>" + domainName + "</DIServerDomain>" +
                      "</ns0:InitializeDIServerConnection >" +
                   "</soap:Body>" +
                "</soap:Envelope>");
            return template;
        }
        internal static XmlDocument GetMonitorDIServerTemplate(string sessionId, MonitorMode mode,DIServiceInfo serviceInfo)
        {
            XmlDocument template = new XmlDocument();
            template.LoadXml(_envelopeHeader +
                   "<soap:Header>" +
                    "<ns0:Context xmlns:ns0 = "+_informaticaWshLink+">" +
                      "<SessionId>" + sessionId + " </SessionId>" +
                      "</ns0:Context>" +
                       "</soap:Header>" +
                       "<soap:Body>" +
                       "<ns0:MonitorDIServer  xmlns:ns0 = " + _informaticaWshLink + ">" +
                        ConvertDiServiceInfoToXMLText(serviceInfo) +
                         "<MonitorMode>" + mode + "</MonitorMode>" +
                      "</ns0:MonitorDIServer>" +
                   "</soap:Body>" +
                "</soap:Envelope>");
            return template;
        }
        internal static XmlDocument GetPingDIServerTemplate(string sessionId, DIServiceInfo serviceInfo,int timeOut)
        {
            XmlDocument template = new XmlDocument();
            template.LoadXml(_envelopeHeader +
                   "<soap:Header>" +
                    "<ns0:Context xmlns:ns0 = "+_informaticaWshLink+">" +
                      "<SessionId>" + sessionId + " </SessionId>" +
                      "</ns0:Context>" +
                       "</soap:Header>" +
                       "<soap:Body>" +
                       "<ns0:PingDIServer xmlns:ns0 = " + _informaticaWshLink + ">" +
                        ConvertDiServiceInfoToXMLText(serviceInfo) +
                         "<TimeOut>" + timeOut + "</TimeOut>" +
                      "</ns0:PingDIServer>" +
                   "</soap:Body>" +
                "</soap:Envelope>");
            return template;
        }
        internal static XmlDocument GetRecoverWorkflowTemplate(string sessionId, List<TaskAttribute> attribute, List<TaskKey> key, List<TaskParam> param, RequestMode requestMode, WorkflowInformParams workflowInfo, DIServiceInfo serviceInfo, string parameterFileName, string taskInstancePath, bool isAbort, string osUser, string reason = "")
        {
            XmlDocument template = new XmlDocument();
            template.LoadXml(_envelopeHeader +
                   "<soap:Header>" +
                    "<ns0:Context xmlns:ns0 = "+_informaticaWshLink+">" +
                      "<SessionId>" + sessionId + " </SessionId>" +
                      "</ns0:Context>" +
                       "</soap:Header>" +
                       "<soap:Body>" +
                      "<ns0:RecoverWorkflow xmlns:ns0 = " + _informaticaWshLink + ">" +
                        ConvertDiServiceInfoToXMLText(serviceInfo) +
                        ConvertWorkflowInformToXmlText(workflowInfo) +
                          "<Reason>" + reason + "</Reason>" +
                           ConvertAttributeToXmlText(attribute) +
                           ConvertKeyToXmlText(key) +
                         "<ParameterFileName>" + parameterFileName + "</ParameterFileName>" +
                         "<Parameters>" +
                           ConvertParametersToXmlText(param) +
                         "</Parameters>" +
                         "<RequestMode>" + requestMode + "</RequestMode>" +
                         "<TaskInstancePath>" + taskInstancePath + "</TaskInstancePath>" +
                         "<IsAbort>" + isAbort + "</IsAbort>" +
                         "<OSUser>" + osUser + "</OSUser>" +
                      "</ns0:RecoverWorkflow>" +
                   "</soap:Body>" +
                "</soap:Envelope>");
            return template;
        }
        internal static XmlDocument GetResumeWorkflowTemplate(string sessionId, List<TaskAttribute> attribute, List<TaskKey> key, List<TaskParam> param, RequestMode requestMode,  WorkflowInformParams workflowInfo, DIServiceInfo serviceInfo, string parameterFileName, string taskInstancePath, bool isAbort, string osUser, string reason)
        {
            XmlDocument template = new XmlDocument();
            template.LoadXml(_envelopeHeader +
                   "<soap:Header>" +
                    "<ns0:Context xmlns:ns0 = "+_informaticaWshLink+">" +
                      "<SessionId>" + sessionId + " </SessionId>" +
                      "</ns0:Context>" +
                       "</soap:Header>" +
                       "<soap:Body>" +
                      "<ns0:ResumeWorkflow xmlns:ns0 = " + _informaticaWshLink + ">" +
                        ConvertDiServiceInfoToXMLText(serviceInfo) +
                        ConvertWorkflowInformToXmlText(workflowInfo) +
                          "<Reason>" + reason + "</Reason>" +
                           ConvertAttributeToXmlText(attribute) +
                           ConvertKeyToXmlText(key) +
                         "<ParameterFileName>" + parameterFileName + "</ParameterFileName>" +
                         "<Parameters>" +
                           ConvertParametersToXmlText(param) +
                         "</Parameters>" +
                         "<RequestMode>" + requestMode + "</RequestMode>" +
                         "<TaskInstancePath>" + taskInstancePath + "</TaskInstancePath>" +
                         "<IsAbort>" + isAbort + "</IsAbort>" +
                         "<OSUser>" + osUser + "</OSUser>" +
                      "</ns0:ResumeWorkflow>" +
                   "</soap:Body>" +
                "</soap:Envelope>");
            return template;
        }
        internal static XmlDocument GetScheduleWorkflowTemplate(string sessionId, List<TaskAttribute> attribute, List<TaskKey> key, List<TaskParam> param, RequestMode requestMode,  WorkflowInformParams workflowInfo, DIServiceInfo serviceInfo, string parameterFileName, string taskInstancePath, bool isAbort, string osUser, string reason)
        {
            XmlDocument template = new XmlDocument();
            template.LoadXml(_envelopeHeader +
                   "<soap:Header>" +
                    "<ns0:Context xmlns:ns0 = "+_informaticaWshLink+">" +
                      "<SessionId>" + sessionId + " </SessionId>" +
                      "</ns0:Context>" +
                       "</soap:Header>" +
                       "<soap:Body>" +
                      "<ns0:ScheduleWorkflow xmlns:ns0 = " + _informaticaWshLink + ">" +
                        ConvertDiServiceInfoToXMLText(serviceInfo) +
                        ConvertWorkflowInformToXmlText(workflowInfo) +
                          "<Reason>" + reason + "</Reason>" +
                           ConvertAttributeToXmlText(attribute) +
                           ConvertKeyToXmlText(key) +
                         "<ParameterFileName>" + parameterFileName + "</ParameterFileName>" +
                         "<Parameters>" +
                           ConvertParametersToXmlText(param) +
                         "</Parameters>" +
                         "<RequestMode>" + requestMode + "</RequestMode>" +
                         "<TaskInstancePath>" + taskInstancePath + "</TaskInstancePath>" +
                         "<IsAbort>" + isAbort + "</IsAbort>" +
                         "<OSUser>" + osUser + "</OSUser>" +
                      "</ns0:ScheduleWorkflow>" +
                   "</soap:Body>" +
                "</soap:Envelope>");
            return template;
        }
        internal static XmlDocument GetStartSessLogFetchTemplate(string sessionId, string folderName, string workflowName, int workflowRunId, DIServiceInfo serviceInfo)
        {
            XmlDocument template = new XmlDocument();
            template.LoadXml(_envelopeHeader +
                   "<soap:Header>" +
                    "<ns0:Context xmlns:ns0 = "+_informaticaWshLink+">" +
                      "<SessionId>" + sessionId + " </SessionId>" +
                      "</ns0:Context>" +
                       "</soap:Header>" +
                       "<soap:Body>" +
                      "<ns0:StartSessionLogFetch xmlns:ns0 = " + _informaticaWshLink+">" +
                       ConvertDiServiceInfoToXMLText(serviceInfo) +
                         "<FolderName>" + folderName + "</FolderName>" +
                         "<WorkflowName>" + workflowName + "</WorkflowName>" +
                         "<TaskInstancePath>" + workflowRunId + "</TaskInstancePath>" +
                      "</ns0:StartSessionLogFetch>" +
                   "</soap:Body>" +
                "</soap:Envelope>");
            return template;
        }
        internal static XmlDocument GetStartTaskTemplate(string sessionId, RequestMode requestMode, List<TaskParam> param, WorkflowInformParams workflowInfo, string parameterFileName, string taskInstancePath, bool isAbort,  DIServiceInfo serviceInfo)
        {
            XmlDocument template = new XmlDocument();
            template.LoadXml(_envelopeHeader +
                   "<soap:Header>" +
                    "<ns0:Context xmlns:ns0 = "+_informaticaWshLink+">" +
                      "<SessionId>" + sessionId + " </SessionId>" +
                      "</ns0:Context>" +
                       "</soap:Header>" +
                       "<soap:Body>" +
                      "<ns0:StartTask xmlns:ns0 = " + _informaticaWshLink + ">" +
                        ConvertDiServiceInfoToXMLText(serviceInfo) +
                        ConvertWorkflowInformToXmlText(workflowInfo) +
                         "<ParameterFileName>" + parameterFileName + "</ParameterFileName>" +
                         "<Parameters>" +
                           ConvertParametersToXmlText(param) +
                         "</Parameters>" +
                         "<RequestMode>" + requestMode + "</RequestMode>" +
                         "<TaskInstancePath>" + taskInstancePath + "</TaskInstancePath>" +
                         "<IsAbort>" + isAbort + "</IsAbort>" +
                      "</ns0:StartTask>" +
                   "</soap:Body>" +
                "</soap:Envelope>");
            return template;
        }


        internal static XmlDocument GetStartWorkflowTemplate(string sessionId, List<TaskAttribute> attribute, List<TaskKey> key, List<TaskParam> param, RequestMode requestMode,  WorkflowInformParams workflowInfo, DIServiceInfo serviceInfo, string parameterFileName, string taskInstancePath, bool isAbort, string osUser, string reason)
        {
            XmlDocument template = new XmlDocument();
            template.LoadXml(_envelopeHeader +
                   "<soap:Header>" +
                    "<ns0:Context xmlns:ns0 = "+_informaticaWshLink+">" +
                      "<SessionId>" + sessionId + " </SessionId>" +
                      "</ns0:Context>" +
                       "</soap:Header>" +
                       "<soap:Body>" +
                       "<ns0:StartWorkflow xmlns:ns0 = " + _informaticaWshLink + ">" +
                        ConvertDiServiceInfoToXMLText(serviceInfo) +
                        ConvertWorkflowInformToXmlText(workflowInfo) +
                        ConvertElementAndValueToXmlText("Reason",reason)+
                        ConvertAttributeToXmlText(attribute) +
                        ConvertKeyToXmlText(key) +
                         ConvertElementAndValueToXmlText("ParameterFileName", parameterFileName) +
                         "<Parameters>" +
                           ConvertParametersToXmlText(param) +
                         "</Parameters>" +
                         "<RequestMode>" + requestMode + "</RequestMode>" +
                         ConvertElementAndValueToXmlText("TaskInstancePath", taskInstancePath) +
                         "<IsAbort>" + isAbort + "</IsAbort>" +
                         ConvertElementAndValueToXmlText("OSUser", osUser) +
                      "</ns0:StartWorkflow>" +
                   "</soap:Body>" +
                "</soap:Envelope>");
            return template;
        }
        internal static XmlDocument GetStartWorkflowExTemplate(string sessionId, List<TaskAttribute> attribute, List<TaskKey> key, List<TaskParam> param, RequestMode requestMode, WorkflowInforExParams workflowInfoEx, DIServiceInfo serviceInfo, string parameterFileName, string taskInstancePath, string osUser, string reason)
        {
            XmlDocument template = new XmlDocument();
            template.LoadXml(_envelopeHeader +
                   "<soap:Header>" +
                    "<ns0:Context xmlns:ns0 = "+_informaticaWshLink+">" +
                      "<SessionId>" + sessionId + " </SessionId>" +
                      "</ns0:Context>" +
                       "</soap:Header>" +
                       "<soap:Body>" +
                      "<ns0:StartWorkflowEx xmlns:ns0 = " + _informaticaWshLink + ">" +
                         ConvertDiServiceInfoToXMLText(serviceInfo) +
                        ConvertWorkflowInformExToXmlText(workflowInfoEx) +
                        ConvertElementAndValueToXmlText("Reason", reason) +
                        ConvertAttributeToXmlText(attribute) +
                        ConvertKeyToXmlText(key) +
                         ConvertElementAndValueToXmlText("ParameterFileName", parameterFileName) +
                         "<Parameters>" +
                           ConvertParametersToXmlText(param) +
                         "</Parameters>" +
                         "<RequestMode>" + requestMode + "</RequestMode>" +
                         ConvertElementAndValueToXmlText("TaskInstancePath", taskInstancePath) +
                         ConvertElementAndValueToXmlText("OSUser", osUser) +
                      "</ns0:StartWorkflowEx>" +
                   "</soap:Body>" +
                "</soap:Envelope>");
            return template;
        }
        internal static XmlDocument GetStartWorkflowFromTaskTemplate(string sessionId, List<TaskAttribute> attribute, List<TaskKey> key, List<TaskParam> param, RequestMode requestMode,  WorkflowInformParams workflowInfo, DIServiceInfo serviceInfo, string parameterFileName, string taskInstancePath, bool isAbort, string osUser, string reason )
        {
            XmlDocument template = new XmlDocument();
            template.LoadXml(_envelopeHeader +
                   "<soap:Header>" +
                    "<ns0:Context xmlns:ns0 = "+_informaticaWshLink+">" +
                      "<SessionId>" + sessionId + " </SessionId>" +
                      "</ns0:Context>" +
                       "</soap:Header>" +
                       "<soap:Body>" +
                      "<ns0:StartWorkflowFromTask xmlns:ns0 = " + _informaticaWshLink + ">" +
                        ConvertDiServiceInfoToXMLText(serviceInfo) +
                        ConvertWorkflowInformToXmlText(workflowInfo) +
                        ConvertElementAndValueToXmlText("Reason", reason) +
                        ConvertAttributeToXmlText(attribute) +
                        ConvertKeyToXmlText(key) +
                         ConvertElementAndValueToXmlText("ParameterFileName", parameterFileName) +
                         "<Parameters>" +
                           ConvertParametersToXmlText(param) +
                         "</Parameters>" +
                         "<RequestMode>" + requestMode + "</RequestMode>" +
                         ConvertElementAndValueToXmlText("TaskInstancePath", taskInstancePath) +
                         "<IsAbort>" + isAbort + "</IsAbort>" +
                         ConvertElementAndValueToXmlText("OSUser", osUser) +
                      "</ns0:StartWorkflowFromTask>" +
                   "</soap:Body>" +
                "</soap:Envelope>");
            return template;
        }
        internal static XmlDocument GetStartWorkflowLogFetchTemplate(string sessionId, WorkflowInformParams workflowInfo, DIServiceInfo serviceInfo)
        {
            XmlDocument template = new XmlDocument();
            template.LoadXml(_envelopeHeader +
                   "<soap:Header>" +
                    "<ns0:Context xmlns:ns0 = "+_informaticaWshLink+">" +
                      "<SessionId>" + sessionId + " </SessionId>" +
                      "</ns0:Context>" +
                       "</soap:Header>" +
                       "<soap:Body>" +
                      "<ns0:StartWorkflowLogFetch xmlns:ns0 = " + _informaticaWshLink+">" +
                       ConvertDiServiceInfoToXMLText(serviceInfo) +
                       ConvertWorkflowInformToXmlText(workflowInfo) +
                      "</ns0:StartWorkflowLogFetch>" +
                   "</soap:Body>" +
                "</soap:Envelope>");
            return template;
        }
        internal static XmlDocument GetStopTaskTemplate(string sessionId, RequestMode requestMode, List<TaskParam> param, WorkflowInformParams workflowInfo, string parameterFileName, string taskInstancePath, bool isAbort, DIServiceInfo serviceInfo)
        {
            XmlDocument template = new XmlDocument();
            template.LoadXml(_envelopeHeader +
                   "<soap:Header>" +
                    "<ns0:Context xmlns:ns0 = "+_informaticaWshLink+">" +
                      "<SessionId>" + sessionId + " </SessionId>" +
                      "</ns0:Context>" +
                       "</soap:Header>" +
                       "<soap:Body>" +
                      "<ns0:StopTask xmlns:ns0 = " + _informaticaWshLink + ">" +
                        ConvertDiServiceInfoToXMLText(serviceInfo) +
                        ConvertWorkflowInformToXmlText(workflowInfo) +
                         "<ParameterFileName>" + parameterFileName + "</ParameterFileName>" +
                         "<Parameters>" +
                           ConvertParametersToXmlText(param) +
                         "</Parameters>" +
                         "<RequestMode>" + requestMode + "</RequestMode>" +
                         "<TaskInstancePath>" + taskInstancePath + "</TaskInstancePath>" +
                         "<IsAbort>" + isAbort + "</IsAbort>" +
                      "</ns0:StopTask>" +
                   "</soap:Body>" +
                "</soap:Envelope>");
            return template;
        }
        internal static XmlDocument GetStopWorkflowTemplate(string sessionId, List<TaskAttribute> attribute, List<TaskKey> key, List<TaskParam> param, RequestMode requestMode,  WorkflowInformParams workflowInfo, DIServiceInfo serviceInfo, string parameterFileName, string taskInstancePath, bool isAbort, string osUser, string reason)
        {
            XmlDocument template = new XmlDocument();
            template.LoadXml(_envelopeHeader +
                   "<soap:Header>" +
                    "<ns0:Context xmlns:ns0 = "+_informaticaWshLink+">" +
                      "<SessionId>" + sessionId + " </SessionId>" +
                      "</ns0:Context>" +
                       "</soap:Header>" +
                       "<soap:Body>" +
                      "<ns0:StopWorkflow xmlns:ns0 = " + _informaticaWshLink + ">" +
                        ConvertDiServiceInfoToXMLText(serviceInfo) +
                        ConvertWorkflowInformToXmlText(workflowInfo) +
                          "<Reason>" + reason + "</Reason>" +
                           ConvertAttributeToXmlText(attribute) +
                           ConvertKeyToXmlText(key) +
                         "<ParameterFileName>" + parameterFileName + "</ParameterFileName>" +
                         "<Parameters>" +
                           ConvertParametersToXmlText(param) +
                         "</Parameters>" +
                         "<RequestMode>" + requestMode + "</RequestMode>" +
                         "<TaskInstancePath>" + taskInstancePath + "</TaskInstancePath>" +
                         "<IsAbort>" + isAbort + "</IsAbort>" +
                         "<OSUser>" + osUser + "</OSUser>" +
                      "</ns0:StopWorkflow>" +
                   "</soap:Body>" +
                "</soap:Envelope>");
            return template;
        }
        internal static XmlDocument GetUncheduleWorkflowTemplate(string sessionId, List<TaskAttribute> attribute, List<TaskKey> key, List<TaskParam> param, RequestMode requestMode,  WorkflowInformParams workflowInfo, DIServiceInfo serviceInfo, string parameterFileName, string taskInstancePath, bool isAbort, string osUser, string reason)
        {
            XmlDocument template = new XmlDocument();
            template.LoadXml(_envelopeHeader +
                   "<soap:Header>" +
                    "<ns0:Context xmlns:ns0 = "+_informaticaWshLink+">" +
                      "<SessionId>" + sessionId + " </SessionId>" +
                      "</ns0:Context>" +
                       "</soap:Header>" +
                       "<soap:Body>" +
                       "<ns0:UnscheduleWorkflow xmlns:ns0 = " + _informaticaWshLink + ">" +
                        ConvertDiServiceInfoToXMLText(serviceInfo) +
                        ConvertWorkflowInformToXmlText(workflowInfo) +
                          "<Reason>" + reason + "</Reason>" +
                           ConvertAttributeToXmlText(attribute) +
                           ConvertKeyToXmlText(key) +
                         "<ParameterFileName>" + parameterFileName + "</ParameterFileName>" +
                         "<Parameters>" +
                           ConvertParametersToXmlText(param) +
                         "</Parameters>" +
                         "<RequestMode>" + requestMode + "</RequestMode>" +
                         "<TaskInstancePath>" + taskInstancePath + "</TaskInstancePath>" +
                         "<IsAbort>" + isAbort + "</IsAbort>" +
                         "<OSUser>" + osUser + "</OSUser>" +
                      "</ns0:UnscheduleWorkflow>" +
                   "</soap:Body>" +
                "</soap:Envelope>");
            return template;
        }
        internal static XmlDocument GetWaitTillTaskCompleteTemplate(string sessionId, RequestMode requestMode, List<TaskParam> param, WorkflowInformParams workflowInfo, string parameterFileName, string taskInstancePath, bool isAbort, DIServiceInfo serviceInfo)
        {
            XmlDocument template = new XmlDocument();
            template.LoadXml(_envelopeHeader +
                   "<soap:Header>" +
                    "<ns0:Context xmlns:ns0 = "+_informaticaWshLink+">" +
                      "<SessionId>" + sessionId + " </SessionId>" +
                      "</ns0:Context>" +
                       "</soap:Header>" +
                       "<soap:Body>" +
                       "<ns0:WaitTillTaskComplete xmlns:ns0 = " + _informaticaWshLink + ">" +
                        ConvertDiServiceInfoToXMLText(serviceInfo) +
                        ConvertWorkflowInformToXmlText(workflowInfo) +
                         "<ParameterFileName>" + parameterFileName + "</ParameterFileName>" +
                         "<Parameters>" +
                           ConvertParametersToXmlText(param) +
                         "</Parameters>" +
                         "<RequestMode>" + requestMode + "</RequestMode>" +
                         "<TaskInstancePath>" + taskInstancePath + "</TaskInstancePath>" +
                         "<IsAbort>" + isAbort + "</IsAbort>" +
                      "</ns0:WaitTillTaskComplete>" +
                   "</soap:Body>" +
                "</soap:Envelope>");
            return template;
        }
        internal static XmlDocument GetWaitTillWorkflowCompleteTemplate(string sessionId, List<TaskAttribute> attribute, List<TaskKey> key, List<TaskParam> param, RequestMode requestMode,  WorkflowInformParams workflowInfo, DIServiceInfo serviceInfo, string parameterFileName, string taskInstancePath, bool isAbort, string osUser, string reason)
        {
            XmlDocument template = new XmlDocument();
            template.LoadXml(_envelopeHeader +
                   "<soap:Header>" +
                    "<ns0:Context xmlns:ns0 = "+_informaticaWshLink+">" +
                      "<SessionId>" + sessionId + " </SessionId>" +
                      "</ns0:Context>" +
                       "</soap:Header>" +
                       "<soap:Body>" +
                       "<ns0:WaitTillWorkflowComplete xmlns:ns0 = " + _informaticaWshLink + ">" +
                        ConvertDiServiceInfoToXMLText(serviceInfo) +
                        ConvertWorkflowInformToXmlText(workflowInfo) +
                          "<Reason>" + reason + "</Reason>" +
                           ConvertAttributeToXmlText(attribute) +
                           ConvertKeyToXmlText(key) +
                         "<ParameterFileName>" + parameterFileName + "</ParameterFileName>" +
                         "<Parameters>" +
                           ConvertParametersToXmlText(param) +
                         "</Parameters>" +
                         "<RequestMode>" + requestMode + "</RequestMode>" +
                         "<TaskInstancePath>" + taskInstancePath + "</TaskInstancePath>" +
                         "<IsAbort>" + isAbort + "</IsAbort>" +
                         "<OSUser>" + osUser + "</OSUser>" +
                      "</ns0:WaitTillWorkflowComplete>" +
                   "</soap:Body>" +
                "</soap:Envelope>");
            return template;
        }


    }
}
