using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace InformaticaWSH
{
    public class InformaticaWebRequestController:IDisposable
    {
        WebRequestsExecutor _integrationExecutor;
        WebRequestsExecutor _metadataExecutor;
        DIServiceInfo _serviceInfo;
        private string _sessionId;


        public InformaticaWebRequestController(string url,DIServiceInfo serviceInfo)
        {
            _integrationExecutor = new WebRequestsExecutor(url+ @"/wsh/services/BatchServices/DataIntegration?WSDL");
            _metadataExecutor = new WebRequestsExecutor(url + @"/wsh/services/BatchServices/Metadata?WSDL");
            _serviceInfo = serviceInfo;
        }

        public void ChangeServiceInfo(DIServiceInfo serviceInfo)
        {
            this._serviceInfo = serviceInfo;
        }

        public async Task<string> Login(string domain, string repository, string login, string password)
        {
            Dispose();
            string result=await _integrationExecutor.ExecuteRequest(InformaticaWebRequestsTemplates.GetLoginTemplate(domain, repository, login, password));
            string sessionId = ValuesSoapXml.GetValueOnElement(result, "SessionId");
            _sessionId = sessionId;
            return sessionId;
        }
        public async Task Logout(string sessionId)
        {
            await _integrationExecutor.ExecuteRequest(InformaticaWebRequestsTemplates.GetLogoutTemplate(sessionId));
        }

        public async void Dispose()
        {
            if (!string.IsNullOrEmpty(_sessionId))
            {
                await Logout(_sessionId);
                _sessionId = null;
            }
        }

        #region METADATA WEBSERVICE
        public async Task<List<string>> AllFolders(string sessionId)
        {
            var result = await _metadataExecutor.ExecuteRequest(InformaticaWebRequestsTemplates.GetAllFoldersTemplate(sessionId));
            return ValuesSoapXml.GetAllValuesOnElement(result, "Name");
        }
        public async Task<List<string>> AllDIServers(string sessionId)
        {
            var result = await _metadataExecutor.ExecuteRequest(InformaticaWebRequestsTemplates.GetAllDIServersTemplate(sessionId));
            return ValuesSoapXml.GetAllValuesOnElement(result, "Name");
        }

        public async Task<List<string>> AllRepositories()
        {
            var result = await _metadataExecutor.ExecuteRequest(InformaticaWebRequestsTemplates.GetAllRepositoriesTemplate());
            return ValuesSoapXml.GetAllValuesOnElement(result, "Name");
        }
        public async Task<List<WorkflowItem>> AllWorkflows(string sessionId, string folderName)
        {
            var result = await _metadataExecutor.ExecuteRequest(InformaticaWebRequestsTemplates.GetAllWorkflowsTemplate(sessionId, folderName));
            //may be optimized
            var names = ValuesSoapXml.GetAllValuesOnElement(result, "Name");
            var isValids = ValuesSoapXml.GetAllValuesOnElement(result, "IsValid");
            List<WorkflowItem> wfItems = new List<WorkflowItem>(names.Count);
            for (int i = 0; i < names.Count; i++)
            {
                wfItems.Add(new WorkflowItem
                {
                    Name = names[i],
                    FolderName = folderName,
                    IsValid = isValids[i].Equals("true") ? true : false
                });
            }
            return wfItems;
        }
        public async Task<List<TaskItem>> AllTaskInstances(string sessionId, string folderName, string workflowName, int depth = 1, bool isValid = true)
        {
            var result = await _metadataExecutor.ExecuteRequest(InformaticaWebRequestsTemplates.GetAllTaskInstancesTemplate(sessionId, folderName, workflowName, depth, isValid));
            //may be optimized
            var names = ValuesSoapXml.GetAllValuesOnElement(result, "Name");
            var type = ValuesSoapXml.GetAllValuesOnElement(result, "Type");
            var isValids = ValuesSoapXml.GetAllValuesOnElement(result, "IsValid");
            List<TaskItem> taskItem = new List<TaskItem>(names.Count);
            for (int i = 0; i < names.Count; i++)
            {
                taskItem.Add(new TaskItem
                {
                    Name = names[i],
                    Type = type[i],
                    IsValid = isValids[i].Equals("true") ? true : false
                });
            }
            return taskItem;
        }
        #endregion
        #region INTEGRATION WEBSERVICE
        #region WORKFLOW

        public async Task WorkflowDetail(string sessionId, string folderName, string workflowName)
        {
        }
        public async Task WorkflowDetail(string sessionId, string folderName, string workflowName, RequestMode requestMode)
        {
        }
        public async Task WorkflowDetail(string sessionId, string folderName, string workflowName, RequestMode requestMode, string parameterFileName)
        {
        }
        public async Task WorkflowDetail(string sessionId, string folderName, string workflowName, RequestMode requestMode, List<TaskParam> param)
        {
        }
        public async Task WorkflowDetail(string sessionId, List<TaskAttribute> attribute, List<TaskKey> key, List<TaskParam> param, RequestMode requestMode, WorkflowInformParams workflowInfo, DIServiceInfo serviceInfo, string parameterFileName, string taskInstancePath, bool isAbort, string osUser, string reason)
        {
        }
        public async Task WorkflowDetailEx(string sessionId, string folderName)
        {
        }
        public async Task<List<LogMessage>> GetWorkflowLog(string sessionId, string folderName, string workflowName, int workflowRunId, int timeout = 60)
        {
            var result = await _integrationExecutor.ExecuteRequest(InformaticaWebRequestsTemplates.GetWorkflowLogTemplate(sessionId,
                new WorkflowInformParams
                {
                    FolderName= folderName,
                    WorkflowName=workflowName,
                    WorkflowRunId=workflowRunId
                }, _serviceInfo, timeout));
            return InfaLogParser.ParseWorkflowLogs(ValuesSoapXml.GetValueOnElement(result, "Buffer"));
        }

        public async Task<List<LogMessage>> GetWorkflowLog(string sessionId, WorkflowInformParams workflowInfo, int timeout = 60)
        {
            var result = await _integrationExecutor.ExecuteRequest(InformaticaWebRequestsTemplates.GetWorkflowLogTemplate(sessionId,
                workflowInfo, _serviceInfo, timeout));
            return InfaLogParser.ParseWorkflowLogs(ValuesSoapXml.GetValueOnElement(result, "Buffer"));
        }

        public async Task StartWorkflow(string sessionId, string folderName,string workflowName)
        {
            var result = await _integrationExecutor.ExecuteRequest(InformaticaWebRequestsTemplates.GetStartWorkflowTemplate(
                sessionId:sessionId, 
                requestMode:RequestMode.NORMAL,
                workflowInfo: new WorkflowInformParams {
                    FolderName=folderName,
                    WorkflowName=workflowName
                },
                serviceInfo: _serviceInfo,
                isAbort: false,
                key: null, attribute: null, param:null,
                osUser:"",
                reason: "",
                parameterFileName:"",
                taskInstancePath:""));
        }
        public async Task StartWorkflow(string sessionId, string folderName, string workflowName, RequestMode requestMode)
        {
            var result = await _integrationExecutor.ExecuteRequest(InformaticaWebRequestsTemplates.GetStartWorkflowTemplate(
                sessionId: sessionId,
                requestMode: requestMode,
                workflowInfo: new WorkflowInformParams
                {
                    FolderName = folderName,
                    WorkflowName = workflowName
                },
                serviceInfo: _serviceInfo,
                isAbort: false,
                key: null, attribute: null, param: null,
                osUser: "",
                reason: "",
                parameterFileName: "",
                taskInstancePath: ""));
        }
        public async Task StartWorkflow(string sessionId, string folderName, string workflowName, RequestMode requestMode,string parameterFileName)
        {
            var result = await _integrationExecutor.ExecuteRequest(InformaticaWebRequestsTemplates.GetStartWorkflowTemplate(
                sessionId: sessionId,
                requestMode: requestMode,
                workflowInfo: new WorkflowInformParams
                {
                    FolderName = folderName,
                    WorkflowName = workflowName
                },
                serviceInfo: _serviceInfo,
                isAbort: false,
                key: null, attribute: null, param: null,
                osUser: "",
                reason: "",
                parameterFileName: parameterFileName,
                taskInstancePath: ""));
        }
        public async Task StartWorkflow(string sessionId, string folderName, string workflowName, RequestMode requestMode, List<TaskParam> param)
        {
            var result = await _integrationExecutor.ExecuteRequest(InformaticaWebRequestsTemplates.GetStartWorkflowTemplate(
                sessionId: sessionId,
                requestMode: requestMode,
                workflowInfo: new WorkflowInformParams
                {
                    FolderName = folderName,
                    WorkflowName = workflowName
                },
                serviceInfo: _serviceInfo,
                isAbort: false,
                key: null, attribute: null, param: param,
                osUser: "",
                reason: "",
                parameterFileName: "",
                taskInstancePath: ""));
        }
        public async Task StartWorkflow(string sessionId, List<TaskAttribute> attribute, List<TaskKey> key, List<TaskParam> param, RequestMode requestMode, WorkflowInformParams workflowInfo, DIServiceInfo serviceInfo, string parameterFileName, string taskInstancePath, bool isAbort, string osUser, string reason)
        {
            var result = await _integrationExecutor.ExecuteRequest(InformaticaWebRequestsTemplates.GetStartWorkflowTemplate(
                sessionId: sessionId,
                requestMode: requestMode,
                workflowInfo: workflowInfo,
                serviceInfo: _serviceInfo,
                isAbort: isAbort,
                key: key, attribute: attribute, param: param,
                osUser: osUser,
                reason: reason,
                parameterFileName: parameterFileName,
                taskInstancePath: taskInstancePath));
        }

        public async Task<int> StartWorkflowEx(string sessionId, string folderName, string workflowName)
        {
            var result = await _integrationExecutor.ExecuteRequest(InformaticaWebRequestsTemplates.GetStartWorkflowExTemplate(
                sessionId: sessionId,
                requestMode: RequestMode.NORMAL,
                workflowInfoEx: new WorkflowInforExParams
                {
                    FolderName = folderName,
                    WorkflowName = workflowName
                },
                serviceInfo: _serviceInfo,
                key: null, attribute: null, param: null,
                osUser: "",
                reason: "",
                parameterFileName: "",
                taskInstancePath: ""));
            return  int.Parse(ValuesSoapXml.GetValueOnElement(result, "RunId"));
        }
        public async Task<int> StartWorkflowEx(string sessionId, string folderName, string workflowName, RequestMode requestMode)
        {
            var result = await _integrationExecutor.ExecuteRequest(InformaticaWebRequestsTemplates.GetStartWorkflowExTemplate(
                sessionId: sessionId,
                requestMode: requestMode,
                workflowInfoEx: new WorkflowInforExParams
                {
                    FolderName = folderName,
                    WorkflowName = workflowName
                },
                serviceInfo: _serviceInfo,
                key: null, attribute: null, param: null,
                osUser: "",
                reason: "",
                parameterFileName: "",
                taskInstancePath: ""));
            return int.Parse(ValuesSoapXml.GetValueOnElement(result, "RunId"));
        }
        public async Task<int> StartWorkflowEx(string sessionId, string folderName, string workflowName, RequestMode requestMode, string parameterFileName)
        {
            var result = await _integrationExecutor.ExecuteRequest(InformaticaWebRequestsTemplates.GetStartWorkflowExTemplate(
                sessionId: sessionId,
                requestMode: requestMode,
                workflowInfoEx: new WorkflowInforExParams
                {
                    FolderName = folderName,
                    WorkflowName = workflowName
                },
                serviceInfo: _serviceInfo,
                key: null, attribute: null, param: null,
                osUser: "",
                reason: "",
                parameterFileName: parameterFileName,
                taskInstancePath: ""));
            return int.Parse(ValuesSoapXml.GetValueOnElement(result, "RunId"));
        }
        public async Task<int> StartWorkflowEx(string sessionId, string folderName, string workflowName, RequestMode requestMode, List<TaskParam> param)
        {
            var result = await _integrationExecutor.ExecuteRequest(InformaticaWebRequestsTemplates.GetStartWorkflowExTemplate(
                sessionId: sessionId,
                requestMode: requestMode,
                workflowInfoEx: new WorkflowInforExParams
                {
                    FolderName = folderName,
                    WorkflowName = workflowName
                },
                serviceInfo: _serviceInfo,
                key: null, attribute: null, param: param,
                osUser: "",
                reason: "",
                parameterFileName: "",
                taskInstancePath: ""));
            return int.Parse(ValuesSoapXml.GetValueOnElement(result, "RunId"));
        }
        public async Task<int> StartWorkflowEx(string sessionId, List<TaskAttribute> attribute, List<TaskKey> key, List<TaskParam> param, RequestMode requestMode, WorkflowInforExParams workflowInfoEx, DIServiceInfo serviceInfo, string parameterFileName, string taskInstancePath, string osUser, string reason)
        {
            var result = await _integrationExecutor.ExecuteRequest(InformaticaWebRequestsTemplates.GetStartWorkflowExTemplate(
                sessionId: sessionId,
                requestMode: requestMode,
                workflowInfoEx: workflowInfoEx,
                serviceInfo: _serviceInfo,
                key: key, attribute: attribute, param: param,
                osUser: osUser,
                reason: reason,
                parameterFileName: parameterFileName,
                taskInstancePath: taskInstancePath));
            return int.Parse(ValuesSoapXml.GetValueOnElement(result, "RunId"));
        }

        public async Task StartWorkflowFromTask(string sessionId, string folderName, string workflowName,string taskInstancePath)
        {
            var result = await _integrationExecutor.ExecuteRequest(InformaticaWebRequestsTemplates.GetStartWorkflowFromTaskTemplate(
                sessionId: sessionId,
                requestMode: RequestMode.NORMAL,
                workflowInfo: new WorkflowInformParams
                {
                    FolderName = folderName,
                    WorkflowName = workflowName
                },
                serviceInfo: _serviceInfo,
                isAbort: false,
                key: null, attribute: null, param: null,
                osUser: "",
                reason: "",
                parameterFileName: "",
                taskInstancePath: taskInstancePath));
        }
        public async Task StartWorkflowFromTask(string sessionId, string folderName, string workflowName, string taskInstancePath, RequestMode requestMode)
        {
            var result = await _integrationExecutor.ExecuteRequest(InformaticaWebRequestsTemplates.GetStartWorkflowFromTaskTemplate(
                sessionId: sessionId,
                requestMode: requestMode,
                workflowInfo: new WorkflowInformParams
                {
                    FolderName = folderName,
                    WorkflowName = workflowName
                },
                serviceInfo: _serviceInfo,
                isAbort: false,
                key: null, attribute: null, param: null,
                osUser: "",
                reason: "",
                parameterFileName: "",
                taskInstancePath: taskInstancePath));
        }
        public async Task StartWorkflowFromTask(string sessionId, string folderName, string workflowName, string taskInstancePath, RequestMode requestMode, string parameterFileName)
        {
            var result = await _integrationExecutor.ExecuteRequest(InformaticaWebRequestsTemplates.GetStartWorkflowFromTaskTemplate(
                sessionId: sessionId,
                requestMode: requestMode,
                workflowInfo: new WorkflowInformParams
                {
                    FolderName = folderName,
                    WorkflowName = workflowName
                },
                serviceInfo: _serviceInfo,
                isAbort: false,
                key: null, attribute: null, param: null,
                osUser: "",
                reason: "",
                parameterFileName: parameterFileName,
                taskInstancePath: taskInstancePath));
        }
        public async Task StartWorkflowFromTask(string sessionId, string folderName, string workflowName, string taskInstancePath, RequestMode requestMode, List<TaskParam> param)
        {
            var result = await _integrationExecutor.ExecuteRequest(InformaticaWebRequestsTemplates.GetStartWorkflowFromTaskTemplate(
                sessionId: sessionId,
                requestMode: requestMode,
                workflowInfo: new WorkflowInformParams
                {
                    FolderName = folderName,
                    WorkflowName = workflowName
                },
                serviceInfo: _serviceInfo,
                isAbort: false,
                key: null, attribute: null, param: param,
                osUser: "",
                reason: "",
                parameterFileName: "",
                taskInstancePath: taskInstancePath));
        }
        public async Task StartWorkflowFromTask(string sessionId, List<TaskAttribute> attribute, List<TaskKey> key, List<TaskParam> param, RequestMode requestMode, WorkflowInformParams workflowInfo, DIServiceInfo serviceInfo, string parameterFileName, string taskInstancePath, bool isAbort, string osUser, string reason)
        {
            var result = await _integrationExecutor.ExecuteRequest(InformaticaWebRequestsTemplates.GetStartWorkflowFromTaskTemplate(
                sessionId: sessionId,
                requestMode: requestMode,
                workflowInfo: workflowInfo,
                serviceInfo: _serviceInfo,
                isAbort: isAbort,
                key: key, attribute: attribute, param: param,
                osUser: osUser,
                reason: reason,
                parameterFileName: parameterFileName,
                taskInstancePath: taskInstancePath));
        }


        public async Task StartWorkflowLogFetch(string sessionId, string folderName)
        {
        }
        public async Task RecoverWorkflow(string sessionId, string folderName)
        {
        }

        public async Task ResumeWorkflow(string sessionId, string folderName)
        {
        }

        public async Task ScheduleWorkflow(string sessionId, string folderName)
        {
        }

        public async Task StopWorkflow(string sessionId, string folderName)
        {
        }

        public async Task UncheduleWorkflow(string sessionId, string folderName)
        {
        }
        public async Task WaitTillWorkflowComplete(string sessionId, string folderName)
        {
        }
        #endregion
#region TASK
        public async Task TaskDetail(string sessionId, string folderName)
        {
        }

        public async Task TaskDetailEx(string sessionId, string folderName)
        {
        }
        public async Task StartTask(string sessionId, string folderName)
        {
        }

        public async Task StopTask(string sessionId, string folderName)
        {
        }

        public async Task WaitTillTaskComplete(string sessionId, string folderName)
        {
        }

#endregion

#region SESSION
        public async Task SessionPerfomanceData(string sessionId, string folderName, string workflowName, string taskInstancePath, DIServiceInfo serviceInfo)
        {
        }

        public async Task SessionStatistic(string sessionId, string folderName, string workflowName, string taskInstancePath, DIServiceInfo serviceInfo)
        {
        }

        public async Task StartSessLogFetch(string sessionId, string folderName)
        {
        }

        public async Task<List<LogMessage>> GetSessionLog(string sessionId, string folderName, string workflowName, string taskInstancePath, int timeout = 60)
        {
            var result = await _integrationExecutor.ExecuteRequest(InformaticaWebRequestsTemplates.GetSessionLogTemplate(sessionId, folderName, workflowName, taskInstancePath, _serviceInfo, timeout));
            return InfaLogParser.ParseWorkflowLogs(ValuesSoapXml.GetValueOnElement(result, "Buffer"));
        }
        #endregion

        #region SERVER
        public async Task DeinitServerConnection(string sessionId)
        {
            await _integrationExecutor.ExecuteRequest(InformaticaWebRequestsTemplates.GetDeinitServerConnectionTemplate(sessionId));
        }

        public async Task<string> DIServerProp(string sessionId)
        {
            string result = await _integrationExecutor.ExecuteRequest(InformaticaWebRequestsTemplates.GetDIServerPropTemplate(sessionId, _serviceInfo));
            return result;
        }

        public async Task<string> NextLogSegment(string sessionId, int logHandle, int timeOut = 60)
        {
           string result = await _integrationExecutor.ExecuteRequest(InformaticaWebRequestsTemplates.GetNextLogSegmentTemplate(sessionId, logHandle, timeOut));
           return result;
        }

        public async Task InitServerConnection(string sessionId, string loginHandle="", string serverName = "", string domainName = "")
        {
            await _integrationExecutor.ExecuteRequest(InformaticaWebRequestsTemplates.GetInitServerConnectionTemplate(sessionId,loginHandle,serverName,domainName));
        }

        public async Task MonitorDIServer(string sessionId, string folderName)
        {
        }

        public async Task<bool> PingDIServer(string sessionId, int timeOut = 10)
        {
            string result = "";
            try
            {
                result = await _integrationExecutor.ExecuteRequest(InformaticaWebRequestsTemplates.GetPingDIServerTemplate(sessionId, _serviceInfo, timeOut));
                if (ValuesSoapXml.GetValueOnElement(result, "PingDIServerReturn").Equals("ALIVE"))
                    return true;

            }
            // should fixed later
            catch (WebException e)
            {
                if (e.Message.Contains("Unable to establish connection"))
                    return false;
            }
            return false;
        }
        #endregion

        #endregion
    }
}
