using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace InformaticaWSH
{
    /// <summary>
    /// 
    /// </summary>
    public class InformaticaSOAPController:IDisposable
    {
        WebRequestsExecutor _integrationExecutor;
        WebRequestsExecutor _metadataExecutor;
        DIServiceInfo _serviceInfo;
        private string _sessionId;
        private static string _lastResponse;
        private static string _lastRequest;
        private static string _lastErrowResponse;
        private static string _logFolderPath;
        private static string _instanceDT;

        internal static void SetLastResponse(string value)
        {
            _lastResponse = value;
            LogWriter.WriteLog(_logFolderPath, _instanceDT,"response", value);
        }
        /// <summary>
        /// get last XML response
        /// </summary>
        /// <returns>response xml text</returns>
        public string GetLastResponse()
        {
            return _lastResponse;
        }
        internal static void SetlastErrowResponse(string value)
        {
            _lastErrowResponse = value;
            LogWriter.WriteLog(_logFolderPath, _instanceDT, "error", value);
        }
        /// <summary>
        /// get last Error response
        /// </summary>
        /// <returns>response error xml text<</returns>
        public string GetlastErrowResponse()
        {
            return _lastErrowResponse;
        }
        internal static void SetlastRequest(string value)
        {
            _lastRequest = value;
             LogWriter.WriteLog(_logFolderPath,_instanceDT, "request", value);
        }
        /// <summary>
        /// get last request, sended to server
        /// </summary>
        /// <returns>request xml text</returns>
        public string GetlastRequest()
        {
            return _lastRequest;
        }

        /// <summary>
        /// create object controller without logging
        /// </summary>
        /// <param name="url">server url format as http://ip:port</param>
        /// <param name="serviceInfo">target domain and integration service</param>
        public InformaticaSOAPController(string url,DIServiceInfo serviceInfo)
        {
            _integrationExecutor = new WebRequestsExecutor(url+ @"/wsh/services/BatchServices/DataIntegration?WSDL");
            _metadataExecutor = new WebRequestsExecutor(url + @"/wsh/services/BatchServices/Metadata?WSDL");
            _serviceInfo = serviceInfo;
        }
        /// <summary>
        /// create object controller with logging
        /// </summary>
        /// <param name="url">server url format as http://ip:port</param>
        /// <param name="serviceInfo">target domain and integration service</param>
        /// <param name="logFolderPath">folder path where will be creating log files</param>
        public InformaticaSOAPController(string url, DIServiceInfo serviceInfo, string logFolderPath)
        {
            _instanceDT = DateTime.Now.ToString().Replace('.','_').Replace(':','-').Replace(' ','_');
            _logFolderPath = logFolderPath;
            _integrationExecutor = new WebRequestsExecutor(url + @"/wsh/services/BatchServices/DataIntegration?WSDL");
            _metadataExecutor = new WebRequestsExecutor(url + @"/wsh/services/BatchServices/Metadata?WSDL");
            _serviceInfo = serviceInfo;
        }

        /// <summary>
        /// you can change target IS without recreating constructor
        /// </summary>
        ///<param name="serviceInfo">target domain and integration service</param>
        public void ChangeServiceInfo(DIServiceInfo serviceInfo)
        {
            this._serviceInfo = serviceInfo;
        }

        /// <summary>
        /// Login to server and get sessionID
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="repository"></param>
        /// <param name="login"></param>
        /// <param name="password"></param>
        /// <returns>session ID</returns>
        public async Task<string> Login(string domain, string repository, string login, string password)
        {
            Dispose();
            string result=await _integrationExecutor.ExecuteRequest(InformaticaSOAPTemplates.GetLoginTemplate(domain, repository, login, password));
            string sessionId = ValuesSoapXml.GetValueOnElement(result, "SessionId");
            _sessionId = sessionId;
            return sessionId;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionId">login sessionId</param>
        /// <returns></returns>
        public async Task Logout(string sessionId)
        {
            await _integrationExecutor.ExecuteRequest(InformaticaSOAPTemplates.GetLogoutTemplate(sessionId));
        }

        /// <summary>
        /// logout from web service
        /// </summary>
        public async void Dispose()
        {
            if (!string.IsNullOrEmpty(_sessionId))
            {
                await Logout(_sessionId);
                _sessionId = null;
            }
        }

        #region METADATA WEBSERVICE
        /// <summary>
        /// Get all folders in repository
        /// </summary>
        /// <param name="sessionId">login sessionId</param>
        /// <returns>folders collection</returns>
        public async Task<List<string>> AllFolders(string sessionId)
        {
            var result = await _metadataExecutor.ExecuteRequest(InformaticaSOAPTemplates.GetAllFoldersTemplate(sessionId));
            return ValuesSoapXml.GetAllValuesOnElement(result, "Name");
        }
        /// <summary>
        /// get names all data integration services
        /// </summary>
        /// <param name="sessionId">login sessionId</param>
        /// <returns>collection data integration services names</returns>
        public async Task<List<string>> AllDIServers(string sessionId)
        {
            var result = await _metadataExecutor.ExecuteRequest(InformaticaSOAPTemplates.GetAllDIServersTemplate(sessionId));
            return ValuesSoapXml.GetAllValuesOnElement(result, "Name");
        }
        /// <summary>
        /// get names all repositories in domain
        /// </summary>
        /// <returns>collection repositories names</returns>
        public async Task<List<string>> AllRepositories()
        {
            var result = await _metadataExecutor.ExecuteRequest(InformaticaSOAPTemplates.GetAllRepositoriesTemplate());
            return ValuesSoapXml.GetAllValuesOnElement(result, "Name");
        }
        /// <summary>
        /// Get all workflows from folder
        /// </summary>
        /// <param name="sessionId">login sessionId</param>
        /// <param name="folderName">folder containing workflow</param>
        /// <returns>workflow item list</returns>
        public async Task<List<WorkflowItem>> AllWorkflows(string sessionId, string folderName)
        {
            var result = await _metadataExecutor.ExecuteRequest(InformaticaSOAPTemplates.GetAllWorkflowsTemplate(sessionId, folderName));
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
        /// <summary>
        /// Get all tasks\sessions at workflow
        /// </summary>
        /// <param name="sessionId">login sessionId</param>
        /// <param name="folderName">folder containing workflow</param>
        /// <param name="workflowName">workflow name which is need</param>
        /// <param name="depth"></param>
        /// <param name="isValid"></param>
        /// <returns>collection contains task items</returns>
        public async Task<List<TaskItem>> AllTaskInstances(string sessionId, string folderName, string workflowName, int depth = 1, bool isValid = true)
        {
            var result = await _metadataExecutor.ExecuteRequest(InformaticaSOAPTemplates.GetAllTaskInstancesTemplate(sessionId, folderName, workflowName, depth, isValid));
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

        /// <summary>
        /// get workflow details
        /// </summary>
        /// <param name="sessionId">login sessionId</param>
        /// <param name="folderName">folder containing workflow</param>
        /// <param name="workflowName">workflow name which is need</param>
        /// <returns></returns>
        public async Task<string> WorkflowDetail(string sessionId, string folderName, string workflowName)
        {
            var result = await _integrationExecutor.ExecuteRequest(InformaticaSOAPTemplates.GetWorkflowDetailTemplate(
                 sessionId: sessionId,
                 workflowInfo: new WorkflowInformParams
                 {
                     FolderName = folderName,
                     WorkflowName = workflowName
                 },
                 serviceInfo: _serviceInfo));
            return result;
        }
        /// <summary>
        /// get workflow details
        /// </summary>
        /// <param name="sessionId">login sessionId</param>
        /// <param name="folderName">folder containing workflow</param>
        /// <param name="workflowName">workflow name which is need</param>
        /// <param name="requestMode">request mode Normal or Recovery</param>
        /// <returns></returns>
        public async Task<string> WorkflowDetail(string sessionId, string folderName, string workflowName, RequestMode requestMode)
        {
            var result = await _integrationExecutor.ExecuteRequest(InformaticaSOAPTemplates.GetWorkflowDetailTemplate(
                sessionId: sessionId,
                requestMode: requestMode,
                workflowInfo: new WorkflowInformParams
                {
                    FolderName = folderName,
                    WorkflowName = workflowName
                },
                serviceInfo: _serviceInfo));
            return result;
        }
        /// <summary>
        /// get workflow details
        /// </summary>
        /// <param name="sessionId">login sessionId</param>
        /// <param name="folderName">folder containing workflow</param>
        /// <param name="workflowName">workflow name which is need</param>
        /// <param name="requestMode">request mode Normal or Recovery</param>
        /// <param name="parameterFileName">parameter file to use when running the workflow</param>
        /// <returns></returns>
        public async Task<string> WorkflowDetail(string sessionId, string folderName, string workflowName, RequestMode requestMode, string parameterFileName)
        {
            var result = await _integrationExecutor.ExecuteRequest(InformaticaSOAPTemplates.GetWorkflowDetailTemplate(
              sessionId: sessionId,
              requestMode: requestMode,
              workflowInfo: new WorkflowInformParams
              {
                  FolderName = folderName,
                  WorkflowName = workflowName
              },
              serviceInfo: _serviceInfo, parameterFileName: parameterFileName));
            return result;
        }
        /// <summary>
        /// get workflow details
        /// </summary>
        /// <param name="sessionId">login sessionId</param>
        /// <param name="folderName">folder containing workflow</param>
        /// <param name="workflowName">workflow name which is need</param>
        /// <param name="requestMode">request mode Normal or Recovery</param>
        /// <param name="param">optional parameters  as sends to worklow</param>
        /// <returns></returns>
        public async Task<string> WorkflowDetail(string sessionId, string folderName, string workflowName, RequestMode requestMode, List<TaskParam> param)
        {
            var result = await _integrationExecutor.ExecuteRequest(InformaticaSOAPTemplates.GetWorkflowDetailTemplate(
                sessionId: sessionId,
                requestMode: requestMode,
                workflowInfo: new WorkflowInformParams
                {
                    FolderName = folderName,
                    WorkflowName = workflowName
                },
                serviceInfo: _serviceInfo, param: param));
            return result;
        }
        /// <summary>
        /// get workflow details
        /// </summary>
        /// <param name="sessionId">login sessionId</param>
        /// <param name="attribute">attribute used to start or schedule a workflow or task</param>
        /// <param name="key">Key to use to start a workflow or task</param>
        /// <param name="param">optional parameters  as sends to worklow</param>
        /// <param name="requestMode">request mode Normal or Recovery</param>
        /// <param name="workflowInfo">workflow data  need to run</param>
        /// <param name="parameterFileName">parameter file to use when running the workflow</param>
        /// <param name="taskInstancePath">Path specifying the location of the task</param>
        /// <param name="isAbort">Indicate whether to abort a task</param>
        /// <param name="osUser">system profile assigned to the workflow</param>
        /// <param name="reason">request reason</param>
        /// <returns></returns>
        public async Task<string> WorkflowDetail(string sessionId, List<TaskAttribute> attribute, List<TaskKey> key, List<TaskParam> param, RequestMode requestMode, WorkflowInformParams workflowInfo, string parameterFileName, string taskInstancePath, bool isAbort, string osUser, string reason)
        {
            var result = await _integrationExecutor.ExecuteRequest(InformaticaSOAPTemplates.GetWorkflowDetailTemplate(
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
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionId">login sessionId</param>
        /// <param name="folderName">folder containing workflow</param>
        /// <param name="workflowName">workflow name which is need</param>
        /// <returns></returns>
        public async Task<WorkflowDetail> WorkflowDetailEx(string sessionId, string folderName,string workflowName)
        {
            var result=await _integrationExecutor.ExecuteRequest(InformaticaSOAPTemplates.GetWorkflowDetailExTemplate(
                    sessionId: sessionId,
                    workflowInfo: new WorkflowInformParams
                    {
                        FolderName = folderName,
                        WorkflowName = workflowName
                    },
                    serviceInfo: _serviceInfo));
            return WorkflowDetailParser.ConvertStringToWorkflowDetail(result);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionId">login sessionId</param>
        /// <param name="folderName">folder containing workflow</param>
        /// <param name="workflowName">workflow name which is need</param>
        /// <param name="workflowRunId">ID workflow run instance </param>
        /// <returns></returns>
        public async Task<WorkflowDetail> WorkflowDetailEx(string sessionId, string folderName, string workflowName,int workflowRunId)
        {
            var result = await _integrationExecutor.ExecuteRequest(InformaticaSOAPTemplates.GetWorkflowDetailExTemplate(
                    sessionId: sessionId,
                    workflowInfo: new WorkflowInformParams
                    {
                        FolderName = folderName,
                        WorkflowName = workflowName,
                        WorkflowRunId = workflowRunId
                    },
                    serviceInfo: _serviceInfo));
            return WorkflowDetailParser.ConvertStringToWorkflowDetail(result);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionId">login sessionId</param>
        /// <param name="workflowInfo">workflow data  need to run</param>
        /// <returns></returns>
        public async Task<WorkflowDetail> WorkflowDetailEx(string sessionId, WorkflowInformParams workflowInfo)
        {
            var result = await _integrationExecutor.ExecuteRequest(InformaticaSOAPTemplates.GetWorkflowDetailExTemplate(
                    sessionId: sessionId,
                    workflowInfo: workflowInfo,
                    serviceInfo: _serviceInfo));
            return WorkflowDetailParser.ConvertStringToWorkflowDetail(result);
        }

        /// <summary>
        /// get workflow run log
        /// </summary>
        /// <param name="sessionId">login sessionId</param>
        /// <param name="folderName">folder containing workflow</param>
        /// <param name="workflowName">workflow name which is need</param>
        /// <param name="workflowRunId">workflowRunId</param>
        /// <returns>collection contains workflow logs</returns>
        public async Task<List<LogMessage>> GetWorkflowLog(string sessionId, string folderName, string workflowName, int workflowRunId)
        {
            var result = await _integrationExecutor.ExecuteRequest(InformaticaSOAPTemplates.GetWorkflowLogTemplate(sessionId,
                new WorkflowInformParams
                {
                    FolderName= folderName,
                    WorkflowName=workflowName,
                    WorkflowRunId=workflowRunId
                }, _serviceInfo));
            return InfaLogParser.ParseWorkflowLogs(ValuesSoapXml.GetValueOnElement(result, "Buffer"));
        }
        /// <summary>
        /// get workflow run log
        /// </summary>
        /// <param name="sessionId">login sessionId</param>
        /// <param name="workflowInfo">workflow data  need to run</param>
        /// <returns>collection contains workflow logs</returns>
        public async Task<List<LogMessage>> GetWorkflowLog(string sessionId, WorkflowInformParams workflowInfo)
        {
            var result = await _integrationExecutor.ExecuteRequest(InformaticaSOAPTemplates.GetWorkflowLogTemplate(sessionId,
                workflowInfo, _serviceInfo));
            return InfaLogParser.ParseWorkflowLogs(ValuesSoapXml.GetValueOnElement(result, "Buffer"));
        }
        /// <summary>
        /// get workflow run log
        /// </summary>
        /// <param name="sessionId">login sessionId</param>
        /// <param name="workflowInfo">workflow data  need to run</param>
        /// <param name="timeout">manual timeout</param>
        /// <returns>collection contains workflow logs</returns>
        public async Task<List<LogMessage>> GetWorkflowLog(string sessionId, WorkflowInformParams workflowInfo,int timeout)
        {
            var result = await _integrationExecutor.ExecuteRequest(InformaticaSOAPTemplates.GetWorkflowLogTemplate(sessionId,
                workflowInfo, _serviceInfo,timeout));
            return InfaLogParser.ParseWorkflowLogs(ValuesSoapXml.GetValueOnElement(result, "Buffer"));
        }


        /// <summary>
        /// running workflow at Informatica repository
        /// </summary>
        /// <param name="sessionId">login sessionId</param>
        /// <param name="folderName">folder containing workflow</param>
        /// <param name="workflowName">workflow name which is need</param>
        /// <returns></returns>
        public async Task StartWorkflow(string sessionId, string folderName,string workflowName)
        {
            await _integrationExecutor.ExecuteRequest(InformaticaSOAPTemplates.GetStartWorkflowTemplate(
                 sessionId: sessionId,
                 workflowInfo: new WorkflowInformParams
                 {
                     FolderName = folderName,
                     WorkflowName = workflowName
                 },
                 serviceInfo: _serviceInfo));
        }
        /// <summary>
        /// running workflow at Informatica repository
        /// </summary>
        /// <param name="sessionId">login sessionId</param>
        /// <param name="folderName">folder containing workflow</param>
        /// <param name="workflowName">workflow name which is need</param>
        /// <param name="requestMode">request mode Normal or Recovery</param>
        /// <returns></returns>
        public async Task StartWorkflow(string sessionId, string folderName, string workflowName, RequestMode requestMode)
        {
            await _integrationExecutor.ExecuteRequest(InformaticaSOAPTemplates.GetStartWorkflowTemplate(
                sessionId: sessionId,
                requestMode: requestMode,
                workflowInfo: new WorkflowInformParams
                {
                    FolderName = folderName,
                    WorkflowName = workflowName
                },
                serviceInfo: _serviceInfo));
        }
        /// <summary>
        /// running workflow at Informatica repository
        /// </summary>
        /// <param name="sessionId">login sessionId</param>
        /// <param name="folderName">folder containing workflow</param>
        /// <param name="workflowName">workflow name which is need</param>
        /// <param name="requestMode">request mode Normal or Recovery</param>
        /// <param name="parameterFileName">parameter file to use when running the workflow</param>
        /// <returns></returns>
        public async Task StartWorkflow(string sessionId, string folderName, string workflowName, RequestMode requestMode,string parameterFileName)
        {
            await _integrationExecutor.ExecuteRequest(InformaticaSOAPTemplates.GetStartWorkflowTemplate(
              sessionId: sessionId,
              requestMode: requestMode,
              workflowInfo: new WorkflowInformParams
              {
                  FolderName = folderName,
                  WorkflowName = workflowName
              },
              serviceInfo: _serviceInfo, parameterFileName: parameterFileName));
        }
        /// <summary>
        /// running workflow at Informatica repository
        /// </summary>
        /// <param name="sessionId">login sessionId</param>
        /// <param name="folderName">folder containing workflow</param>
        /// <param name="workflowName">workflow name which is need</param>
        /// <param name="requestMode">request mode Normal or Recovery</param>
        /// <param name="param">optional parameters  as sends to worklow</param>
        /// <returns></returns>
        public async Task StartWorkflow(string sessionId, string folderName, string workflowName, RequestMode requestMode, List<TaskParam> param)
        {
            await _integrationExecutor.ExecuteRequest(InformaticaSOAPTemplates.GetStartWorkflowTemplate(
                sessionId: sessionId,
                requestMode: requestMode,
                workflowInfo: new WorkflowInformParams
                {
                    FolderName = folderName,
                    WorkflowName = workflowName
                },
                serviceInfo: _serviceInfo, param: param));
        }
        /// <summary>
        /// running workflow at Informatica repository
        /// </summary>
        /// <param name="sessionId">login sessionId</param>
        /// <param name="attribute">attribute used to start or schedule a workflow or task</param>
        /// <param name="key">Key to use to start a workflow or task</param>
        /// <param name="param">optional parameters  as sends to worklow</param>
        /// <param name="requestMode">request mode Normal or Recovery</param>
        /// <param name="workflowInfo">workflow data  need to run</param>
        /// <param name="parameterFileName">parameter file to use when running the workflow</param>
        /// <param name="taskInstancePath">Path specifying the location of the task</param>
        /// <param name="isAbort">Indicate whether to abort a task</param>
        /// <param name="osUser">system profile assigned to the workflow</param>
        /// <param name="reason">request reason</param>
        /// <returns></returns>
        public async Task StartWorkflow(string sessionId, List<TaskAttribute> attribute, List<TaskKey> key, List<TaskParam> param, RequestMode requestMode, WorkflowInformParams workflowInfo,string parameterFileName, string taskInstancePath, bool isAbort, string osUser, string reason)
        {
            var result = await _integrationExecutor.ExecuteRequest(InformaticaSOAPTemplates.GetStartWorkflowTemplate(
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

        /// <summary>
        /// running workflow at Informatica repository and get workflow run ID
        /// </summary>
        /// <param name="sessionId">login sessionId</param>
        /// <param name="folderName">folder containing workflow</param>
        /// <param name="workflowName">workflow name which is need</param>
        /// <returns>workflow run ID</returns>
        public async Task<int> StartWorkflowEx(string sessionId, string folderName, string workflowName)
        {
            var result = await _integrationExecutor.ExecuteRequest(InformaticaSOAPTemplates.GetStartWorkflowExTemplate(
                sessionId: sessionId,
                workflowInfoEx: new WorkflowInforExParams
                {
                    FolderName = folderName,
                    WorkflowName = workflowName
                },
                serviceInfo: _serviceInfo
                ));
            return  int.Parse(ValuesSoapXml.GetValueOnElement(result, "RunId"));
        }
        /// <summary>
        /// run workflow at Informatica repository and get workflow run ID
        /// </summary>
        /// <param name="sessionId">login sessionId</param>
        /// <param name="folderName">folder containing workflow</param>
        /// <param name="workflowName">workflow name which is need</param>
        /// <param name="requestMode">request mode Normal or Recovery</param>
        /// <returns>workflow run ID</returns>
        public async Task<int> StartWorkflowEx(string sessionId, string folderName, string workflowName, RequestMode requestMode)
        {
            var result = await _integrationExecutor.ExecuteRequest(InformaticaSOAPTemplates.GetStartWorkflowExTemplate(
                sessionId: sessionId,
                requestMode: requestMode,
                workflowInfoEx: new WorkflowInforExParams
                {
                    FolderName = folderName,
                    WorkflowName = workflowName
                },
                serviceInfo: _serviceInfo));
            return int.Parse(ValuesSoapXml.GetValueOnElement(result, "RunId"));
        }
        /// <summary>
        /// running workflow at Informatica repository and get workflow run ID
        /// </summary>
        /// <param name="sessionId">login sessionId</param>
        /// <param name="folderName">folder containing workflow</param>
        /// <param name="workflowName">workflow name which is need</param>
        /// <param name="requestMode">request mode Normal or Recovery</param>
        /// <param name="parameterFileName">parameter file to use when running the workflow</param>
        /// <returns></returns>
        public async Task<int> StartWorkflowEx(string sessionId, string folderName, string workflowName, RequestMode requestMode, string parameterFileName)
        {
            var result = await _integrationExecutor.ExecuteRequest(InformaticaSOAPTemplates.GetStartWorkflowExTemplate(
                sessionId: sessionId,
                requestMode: requestMode,
                workflowInfoEx: new WorkflowInforExParams
                {
                    FolderName = folderName,
                    WorkflowName = workflowName
                },
                serviceInfo: _serviceInfo,
                parameterFileName: parameterFileName));
            return int.Parse(ValuesSoapXml.GetValueOnElement(result, "RunId"));
        }
        /// <summary>
        /// running workflow at Informatica repository and get workflow run ID
        /// </summary>
        /// <param name="sessionId">login sessionId</param>
        /// <param name="folderName">folder containing workflow</param>
        /// <param name="workflowName">workflow name which is need</param>
        /// <param name="requestMode">request mode Normal or Recovery</param>
        /// <param name="param">optional parameters  as sends to worklow</param>
        /// <returns>workflow run ID</returns>
        public async Task<int> StartWorkflowEx(string sessionId, string folderName, string workflowName, RequestMode requestMode, List<TaskParam> param)
        {
            var result = await _integrationExecutor.ExecuteRequest(InformaticaSOAPTemplates.GetStartWorkflowExTemplate(
                sessionId: sessionId,
                requestMode: requestMode,
                workflowInfoEx: new WorkflowInforExParams
                {
                    FolderName = folderName,
                    WorkflowName = workflowName
                },
                serviceInfo: _serviceInfo, param: param
                ));
            return int.Parse(ValuesSoapXml.GetValueOnElement(result, "RunId"));
        }
        /// <summary>
        /// running workflow at Informatica repository and get workflow run ID
        /// </summary>
        /// <param name="sessionId">login sessionId</param>
        /// <param name="attribute">attribute used to start or schedule a workflow or task</param>
        /// <param name="key">Key to use to start a workflow or task</param>
        /// <param name="param">optional parameters  as sends to worklow</param>
        /// <param name="requestMode">request mode Normal or Recovery</param>
        /// <param name="workflowInfoEx"></param>
        /// <param name="parameterFileName">parameter file to use when running the workflow</param>
        /// <param name="taskInstancePath">Path specifying the location of the task</param>
        /// <param name="osUser">system profile assigned to the workflow</param>
        /// <param name="reason">request reason</param>
        /// <returns>workflow run ID</returns>
        public async Task<int> StartWorkflowEx(string sessionId, List<TaskAttribute> attribute, List<TaskKey> key, List<TaskParam> param, RequestMode requestMode, WorkflowInforExParams workflowInfoEx, string parameterFileName, string taskInstancePath, string osUser, string reason)
        {
            var result = await _integrationExecutor.ExecuteRequest(InformaticaSOAPTemplates.GetStartWorkflowExTemplate(
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

        /// <summary>
        /// running workflow at Informatica repository and get workflow run ID
        /// </summary>
        /// <param name="sessionId">login sessionId</param>
        /// <param name="folderName">folder containing workflow</param>
        /// <param name="workflowName">workflow name which is need</param>
        /// <param name="taskInstancePath">Path specifying the location of the task</param>
        /// <returns></returns>
        public async Task StartWorkflowFromTask(string sessionId, string folderName, string workflowName,string taskInstancePath)
        {
            await _integrationExecutor.ExecuteRequest(InformaticaSOAPTemplates.GetStartWorkflowFromTaskTemplate(
                sessionId: sessionId,
                workflowInfo: new WorkflowInformParams
                {
                    FolderName = folderName,
                    WorkflowName = workflowName
                },
                serviceInfo: _serviceInfo,
                taskInstancePath: taskInstancePath));
        }
        /// <summary>
        /// running workflow at Informatica repository on since session\task name
        /// </summary>
        /// <param name="sessionId">login sessionId</param>
        /// <param name="folderName">folder containing workflow</param>
        /// <param name="workflowName">workflow name which is need</param>
        /// <param name="taskInstancePath">session\task name</param>
        /// <param name="requestMode">request mode Normal or Recovery</param>
        /// <returns></returns>
        public async Task StartWorkflowFromTask(string sessionId, string folderName, string workflowName, string taskInstancePath, RequestMode requestMode)
        {
            await _integrationExecutor.ExecuteRequest(InformaticaSOAPTemplates.GetStartWorkflowFromTaskTemplate(
                sessionId: sessionId,
                requestMode: requestMode,
                workflowInfo: new WorkflowInformParams
                {
                    FolderName = folderName,
                    WorkflowName = workflowName
                },
                serviceInfo: _serviceInfo, 
                taskInstancePath: taskInstancePath));
        }
        /// <summary>
        /// running workflow at Informatica repository on since session\task name
        /// </summary>
        /// <param name="sessionId">login sessionId</param>
        /// <param name="folderName">folder containing workflow</param>
        /// <param name="workflowName">workflow name which is need</param>
        /// <param name="taskInstancePath">Path specifying the location of the task</param>
        /// <param name="requestMode">request mode Normal or Recovery</param>
        /// <param name="parameterFileName">parameter file to use when running the workflow</param>
        /// <returns></returns>
        public async Task StartWorkflowFromTask(string sessionId, string folderName, string workflowName, string taskInstancePath, RequestMode requestMode, string parameterFileName)
        {
            await _integrationExecutor.ExecuteRequest(InformaticaSOAPTemplates.GetStartWorkflowFromTaskTemplate(
                sessionId: sessionId,
                requestMode: requestMode,
                workflowInfo: new WorkflowInformParams
                {
                    FolderName = folderName,
                    WorkflowName = workflowName
                },
                serviceInfo: _serviceInfo, parameterFileName: parameterFileName,
                taskInstancePath: taskInstancePath));
        }
        /// <summary>
        /// running workflow at Informatica repository on since session\task name
        /// </summary>
        /// <param name="sessionId">login sessionId</param>
        /// <param name="folderName">folder containing workflow</param>
        /// <param name="workflowName">workflow name which is need</param>
        /// <param name="taskInstancePath">session\task name</param>
        /// <param name="requestMode">request mode Normal or Recovery</param>
        /// <param name="param">optional parameters  as sends to worklow</param>
        /// <returns></returns>
        public async Task StartWorkflowFromTask(string sessionId, string folderName, string workflowName, string taskInstancePath, RequestMode requestMode, List<TaskParam> param)
        {
            await _integrationExecutor.ExecuteRequest(InformaticaSOAPTemplates.GetStartWorkflowFromTaskTemplate(
                sessionId: sessionId,
                requestMode: requestMode,
                workflowInfo: new WorkflowInformParams
                {
                    FolderName = folderName,
                    WorkflowName = workflowName
                },
                serviceInfo: _serviceInfo, param: param,
                taskInstancePath: taskInstancePath));
        }
        /// <summary>
        /// running workflow at Informatica repository on since session\task name
        /// </summary>
        /// <param name="sessionId">login sessionId</param>
        /// <param name="attribute">attribute used to start or schedule a workflow or task</param>
        /// <param name="key">Key to use to start a workflow or task</param>
        /// <param name="param">optional parameters  as sends to worklow</param>
        /// <param name="requestMode">request mode Normal or Recovery</param>
        /// <param name="workflowInfo">workflow data  need to run</param>
        /// <param name="parameterFileName">parameter file to use when running the workflow</param>
        /// <param name="taskInstancePath">session\task name</param>
        /// <param name="isAbort">Indicate whether to abort a task</param>
        /// <param name="osUser">system profile assigned to the workflow</param>
        /// <param name="reason">request reason</param>
        /// <returns></returns>
        public async Task StartWorkflowFromTask(string sessionId, List<TaskAttribute> attribute, List<TaskKey> key, List<TaskParam> param, RequestMode requestMode, WorkflowInformParams workflowInfo, string parameterFileName, string taskInstancePath, bool isAbort, string osUser, string reason)
        {
            var result = await _integrationExecutor.ExecuteRequest(InformaticaSOAPTemplates.GetStartWorkflowFromTaskTemplate(
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionId">login sessionId</param>
        /// <param name="folderName">folder containing workflow</param>
        /// <param name="workflowName">workflow name which is need</param>
        /// <returns></returns>
        public async Task StartWorkflowLogFetch(string sessionId, string folderName,string workflowName)
        {
            await _integrationExecutor.ExecuteRequest(InformaticaSOAPTemplates.GetStartWorkflowLogFetchTemplate(
                   sessionId: sessionId,
                 workflowInfo: new WorkflowInformParams
                 {
                     FolderName=folderName,
                     WorkflowName=workflowName
                 },
                 serviceInfo: _serviceInfo));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionId">login sessionId</param>
        /// <param name="folderName">folder containing workflow</param>
        /// <param name="workflowName">workflow name which is need</param>
        /// <param name="workflowRunId">ID workflow run instance </param>
        /// <returns></returns>
        public async Task StartWorkflowLogFetch(string sessionId, string folderName, string workflowName, int workflowRunId)
        {
            await _integrationExecutor.ExecuteRequest(InformaticaSOAPTemplates.GetStartWorkflowLogFetchTemplate(
                   sessionId: sessionId,
                 workflowInfo: new WorkflowInformParams
                 {
                     FolderName = folderName,
                     WorkflowName = workflowName,
                     WorkflowRunId=workflowRunId
                 },
                 serviceInfo: _serviceInfo));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionId">login sessionId</param>
        /// <param name="workflowInfo">workflow data  need to run</param>
        /// <returns></returns>
        public async Task StartWorkflowLogFetch(string sessionId, WorkflowInformParams workflowInfo)
        {
            await _integrationExecutor.ExecuteRequest(InformaticaSOAPTemplates.GetStartWorkflowLogFetchTemplate(
                   sessionId: sessionId,
                 workflowInfo: workflowInfo,
                 serviceInfo: _serviceInfo));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionId">login sessionId</param>
        /// <param name="folderName">folder containing workflow</param>
        /// <param name="workflowName">workflow name which is need</param>
        /// <returns></returns>
        public async Task RecoverWorkflow(string sessionId, string folderName, string workflowName)
        {
            await _integrationExecutor.ExecuteRequest(InformaticaSOAPTemplates.GetRecoverWorkflowTemplate(
                  sessionId: sessionId,
                workflowInfo: new WorkflowInformParams
                {
                    FolderName = folderName,
                    WorkflowName = workflowName
                },
                serviceInfo: _serviceInfo));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionId">login sessionId</param>
        /// <param name="folderName">folder containing workflow</param>
        /// <param name="workflowName">workflow name which is need</param>
        /// <param name="requestMode">request mode Normal or Recovery</param>
        /// <returns></returns>
        public async Task RecoverWorkflow(string sessionId, string folderName, string workflowName, RequestMode requestMode)
        {
            await _integrationExecutor.ExecuteRequest(InformaticaSOAPTemplates.GetRecoverWorkflowTemplate(
                  sessionId: sessionId,
                requestMode: requestMode,
                workflowInfo: new WorkflowInformParams
                {
                    FolderName = folderName,
                    WorkflowName = workflowName
                },
                serviceInfo: _serviceInfo));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionId">login sessionId</param>
        /// <param name="folderName">folder containing workflow</param>
        /// <param name="workflowName">workflow name which is need</param>
        /// <param name="requestMode">request mode Normal or Recovery</param>
        /// <param name="parameterFileName">parameter file to use when running the workflow</param>
        /// <returns></returns>
        public async Task RecoverWorkflow(string sessionId, string folderName, string workflowName, RequestMode requestMode, string parameterFileName)
        {
            await _integrationExecutor.ExecuteRequest(InformaticaSOAPTemplates.GetRecoverWorkflowTemplate(
                  sessionId: sessionId,
                requestMode: requestMode,
                workflowInfo: new WorkflowInformParams
                {
                    FolderName = folderName,
                    WorkflowName = workflowName
                },
                serviceInfo: _serviceInfo, parameterFileName: parameterFileName));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionId">login sessionId</param>
        /// <param name="folderName">folder containing workflow</param>
        /// <param name="workflowName">workflow name which is need</param>
        /// <param name="requestMode">request mode Normal or Recovery</param>
        /// <param name="param">optional parameters  as sends to worklow</param>
        /// <returns></returns>
        public async Task RecoverWorkflow(string sessionId, string folderName, string workflowName, RequestMode requestMode, List<TaskParam> param)
        {
            await _integrationExecutor.ExecuteRequest(InformaticaSOAPTemplates.GetRecoverWorkflowTemplate(
                  sessionId: sessionId,
                requestMode: requestMode,
                workflowInfo: new WorkflowInformParams
                {
                    FolderName = folderName,
                    WorkflowName = workflowName
                },
                serviceInfo: _serviceInfo, param: param));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionId">login sessionId</param>
        /// <param name="attribute">attribute used to start or schedule a workflow or task</param>
        /// <param name="key">Key to use to start a workflow or task</param>
        /// <param name="param">optional parameters  as sends to worklow</param>
        /// <param name="requestMode">request mode Normal or Recovery</param>
        /// <param name="workflowInfo">workflow data  need to run</param>
        /// <param name="parameterFileName">parameter file to use when running the workflow</param>
        /// <param name="taskInstancePath">Path specifying the location of the task</param>
        /// <param name="isAbort">Indicate whether to abort a task</param>
        /// <param name="osUser">system profile assigned to the workflow</param>
        /// <param name="reason">request reason</param>
        /// <returns></returns>
        public async Task RecoverWorkflow(string sessionId, List<TaskAttribute> attribute, List<TaskKey> key, List<TaskParam> param, RequestMode requestMode, WorkflowInformParams workflowInfo, string parameterFileName, string taskInstancePath, bool isAbort, string osUser, string reason)
        {
             await _integrationExecutor.ExecuteRequest(InformaticaSOAPTemplates.GetRecoverWorkflowTemplate(
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionId">login sessionId</param>
        /// <param name="folderName">folder containing workflow</param>
        /// <param name="workflowName">workflow name which is need</param>
        /// <returns></returns>
        public async Task ResumeWorkflow(string sessionId, string folderName, string workflowName)
        {
            await _integrationExecutor.ExecuteRequest(InformaticaSOAPTemplates.GetResumeWorkflowTemplate(
                 sessionId: sessionId, workflowInfo: new WorkflowInformParams
               {
                   FolderName = folderName,
                   WorkflowName = workflowName
               },
               serviceInfo: _serviceInfo));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionId">login sessionId</param>
        /// <param name="folderName">folder containing workflow</param>
        /// <param name="workflowName">workflow name which is need</param>
        /// <param name="requestMode">request mode Normal or Recovery</param>
        /// <returns></returns>
        public async Task ResumeWorkflow(string sessionId, string folderName, string workflowName, RequestMode requestMode)
        {
            await _integrationExecutor.ExecuteRequest(InformaticaSOAPTemplates.GetResumeWorkflowTemplate(
                 sessionId: sessionId,
               requestMode: requestMode, workflowInfo: new WorkflowInformParams
               {
                   FolderName = folderName,
                   WorkflowName = workflowName
               },
               serviceInfo: _serviceInfo));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionId">login sessionId</param>
        /// <param name="folderName">folder containing workflow</param>
        /// <param name="workflowName">workflow name which is need</param>
        /// <param name="requestMode">request mode Normal or Recovery</param>
        /// <param name="parameterFileName">parameter file to use when running the workflow</param>
        /// <returns></returns>
        public async Task ResumeWorkflow(string sessionId, string folderName, string workflowName, RequestMode requestMode, string parameterFileName)
        {
            await _integrationExecutor.ExecuteRequest(InformaticaSOAPTemplates.GetResumeWorkflowTemplate(
                 sessionId: sessionId,
               requestMode: requestMode, workflowInfo: new WorkflowInformParams
               {
                   FolderName = folderName,
                   WorkflowName = workflowName
               },
               serviceInfo: _serviceInfo, parameterFileName: parameterFileName));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionId">login sessionId</param>
        /// <param name="folderName">folder containing workflow</param>
        /// <param name="workflowName">workflow name which is need</param>
        /// <param name="requestMode">request mode Normal or Recovery</param>
        /// <param name="param">optional parameters  as sends to worklow</param>
        /// <returns></returns>
        public async Task ResumeWorkflow(string sessionId, string folderName, string workflowName, RequestMode requestMode, List<TaskParam> param)
        {
            await _integrationExecutor.ExecuteRequest(InformaticaSOAPTemplates.GetResumeWorkflowTemplate(
                 sessionId: sessionId,
               requestMode: requestMode, workflowInfo: new WorkflowInformParams
               {
                   FolderName = folderName,
                   WorkflowName = workflowName
               },
               serviceInfo: _serviceInfo, param: param));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionId">login sessionId</param>
        /// <param name="attribute">attribute used to start or schedule a workflow or task</param>
        /// <param name="key">Key to use to start a workflow or task</param>
        /// <param name="param">optional parameters  as sends to worklow</param>
        /// <param name="requestMode">request mode Normal or Recovery</param>
        /// <param name="workflowInfo">workflow data  need to run</param>
        /// <param name="parameterFileName">parameter file to use when running the workflow</param>
        /// <param name="taskInstancePath">Path specifying the location of the task</param>
        /// <param name="isAbort">Indicate whether to abort a task</param>
        /// <param name="osUser">system profile assigned to the workflow</param>
        /// <param name="reason">request reason</param>
        /// <returns></returns>
        public async Task ResumeWorkflow(string sessionId, List<TaskAttribute> attribute, List<TaskKey> key, List<TaskParam> param, RequestMode requestMode, WorkflowInformParams workflowInfo, string parameterFileName, string taskInstancePath, bool isAbort, string osUser, string reason)
        {
            await _integrationExecutor.ExecuteRequest(InformaticaSOAPTemplates.GetResumeWorkflowTemplate(
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionId">login sessionId</param>
        /// <param name="folderName">folder containing workflow</param>
        /// <param name="workflowName">workflow name which is need</param>
        /// <returns></returns>
        public async Task ScheduleWorkflow(string sessionId, string folderName, string workflowName)
        {
            await _integrationExecutor.ExecuteRequest(InformaticaSOAPTemplates.GetScheduleWorkflowTemplate(
                sessionId: sessionId,
              workflowInfo: new WorkflowInformParams
              {
                  FolderName = folderName,
                  WorkflowName = workflowName
              },
              serviceInfo: _serviceInfo
              ));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionId">login sessionId</param>
        /// <param name="folderName">folder containing workflow</param>
        /// <param name="workflowName">workflow name which is need</param>
        /// <param name="requestMode">request mode Normal or Recovery</param>
        /// <returns></returns>
        public async Task ScheduleWorkflow(string sessionId, string folderName, string workflowName, RequestMode requestMode)
        {
            await _integrationExecutor.ExecuteRequest(InformaticaSOAPTemplates.GetScheduleWorkflowTemplate(
                sessionId: sessionId,
              requestMode: requestMode,
              workflowInfo: new WorkflowInformParams
              {
                  FolderName = folderName,
                  WorkflowName = workflowName
              },
              serviceInfo: _serviceInfo
              ));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionId">login sessionId</param>
        /// <param name="folderName">folder containing workflow</param>
        /// <param name="workflowName">workflow name which is need</param>
        /// <param name="requestMode">request mode Normal or Recovery</param>
        /// <param name="parameterFileName">parameter file to use when running the workflow</param>
        /// <returns></returns>
        public async Task ScheduleWorkflow(string sessionId, string folderName, string workflowName, RequestMode requestMode, string parameterFileName)
        {
            await _integrationExecutor.ExecuteRequest(InformaticaSOAPTemplates.GetScheduleWorkflowTemplate(
                sessionId: sessionId,
              requestMode: requestMode,
              workflowInfo: new WorkflowInformParams
              {
                  FolderName = folderName,
                  WorkflowName = workflowName
              },
              serviceInfo: _serviceInfo, parameterFileName:parameterFileName
              ));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionId">login sessionId</param>
        /// <param name="folderName">folder containing workflow</param>
        /// <param name="workflowName">workflow name which is need</param>
        /// <param name="requestMode">request mode Normal or Recovery</param>
        /// <param name="param">optional parameters  as sends to worklow</param>
        /// <returns></returns>
        public async Task ScheduleWorkflow(string sessionId, string folderName, string workflowName, RequestMode requestMode, List<TaskParam> param)
        {
            await _integrationExecutor.ExecuteRequest(InformaticaSOAPTemplates.GetScheduleWorkflowTemplate(
                sessionId: sessionId,
              requestMode: requestMode,
              workflowInfo: new WorkflowInformParams
              {
                  FolderName = folderName,
                  WorkflowName = workflowName
              },
              serviceInfo: _serviceInfo, param: param
              ));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionId">login sessionId</param>
        /// <param name="attribute">attribute used to start or schedule a workflow or task</param>
        /// <param name="key">Key to use to start a workflow or task</param>
        /// <param name="param">optional parameters  as sends to worklow</param>
        /// <param name="requestMode">request mode Normal or Recovery</param>
        /// <param name="workflowInfo">workflow data  need to run</param>
        /// <param name="parameterFileName">parameter file to use when running the workflow</param>
        /// <param name="taskInstancePath">Path specifying the location of the task</param>
        /// <param name="isAbort">Indicate whether to abort a task</param>
        /// <param name="osUser">system profile assigned to the workflow</param>
        /// <param name="reason">request reason</param>
        /// <returns></returns>
        public async Task ScheduleWorkflow(string sessionId, List<TaskAttribute> attribute, List<TaskKey> key, List<TaskParam> param, RequestMode requestMode, WorkflowInformParams workflowInfo, string parameterFileName, string taskInstancePath, bool isAbort, string osUser, string reason)
        {
            await _integrationExecutor.ExecuteRequest(InformaticaSOAPTemplates.GetScheduleWorkflowTemplate(
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionId">login sessionId</param>
        /// <param name="folderName">folder containing workflow</param>
        /// <param name="workflowName">workflow name which is need</param>
        /// <returns></returns>
        public async Task StopWorkflow(string sessionId, string folderName, string workflowName)
        {
            await _integrationExecutor.ExecuteRequest(InformaticaSOAPTemplates.GetStopWorkflowTemplate(
                sessionId: sessionId,
              workflowInfo: new WorkflowInformParams
              {
                  FolderName = folderName,
                  WorkflowName = workflowName
              },
              serviceInfo: _serviceInfo
              ));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionId">login sessionId</param>
        /// <param name="folderName">folder containing workflow</param>
        /// <param name="workflowName">workflow name which is need</param>
        /// <param name="requestMode">request mode Normal or Recovery</param>
        /// <returns></returns>
        public async Task StopWorkflow(string sessionId, string folderName, string workflowName, RequestMode requestMode)
        {
            await _integrationExecutor.ExecuteRequest(InformaticaSOAPTemplates.GetStopWorkflowTemplate(
                 sessionId: sessionId,
               requestMode: requestMode,
               workflowInfo: new WorkflowInformParams
               {
                   FolderName = folderName,
                   WorkflowName = workflowName
               },
               serviceInfo: _serviceInfo
               ));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionId">login sessionId</param>
        /// <param name="folderName">folder containing workflow</param>
        /// <param name="workflowName">workflow name which is need</param>
        /// <param name="requestMode">request mode Normal or Recovery</param>
        /// <param name="parameterFileName">parameter file to use when running the workflow</param>
        /// <returns></returns>
        public async Task StopWorkflow(string sessionId, string folderName, string workflowName, RequestMode requestMode, string parameterFileName)
        {
            await _integrationExecutor.ExecuteRequest(InformaticaSOAPTemplates.GetStopWorkflowTemplate(
                sessionId: sessionId,
              requestMode: requestMode,
              workflowInfo: new WorkflowInformParams
              {
                  FolderName = folderName,
                  WorkflowName = workflowName
              },
              serviceInfo: _serviceInfo,
              parameterFileName: parameterFileName
              ));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionId">login sessionId</param>
        /// <param name="folderName">folder containing workflow</param>
        /// <param name="workflowName">workflow name which is need</param>
        /// <param name="requestMode">request mode Normal or Recovery</param>
        /// <param name="param">optional parameters  as sends to worklow</param>
        /// <returns></returns>
        public async Task StopWorkflow(string sessionId, string folderName, string workflowName, RequestMode requestMode, List<TaskParam> param)
        {
            await _integrationExecutor.ExecuteRequest(InformaticaSOAPTemplates.GetStopWorkflowTemplate(
                 sessionId: sessionId,
               requestMode: requestMode,
               workflowInfo: new WorkflowInformParams
               {
                   FolderName = folderName,
                   WorkflowName = workflowName
               },
               serviceInfo: _serviceInfo,
               param: param
               ));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionId">login sessionId</param>
        /// <param name="attribute">attribute used to start or schedule a workflow or task</param>
        /// <param name="key">Key to use to start a workflow or task</param>
        /// <param name="param">optional parameters  as sends to worklow</param>
        /// <param name="requestMode">request mode Normal or Recovery</param>
        /// <param name="workflowInfo">workflow data  need to run</param>
        /// <param name="parameterFileName">parameter file to use when running the workflow</param>
        /// <param name="taskInstancePath">Path specifying the location of the task</param>
        /// <param name="isAbort">Indicate whether to abort a task</param>
        /// <param name="osUser">system profile assigned to the workflow</param>
        /// <param name="reason">request reason</param>
        /// <returns></returns>
        public async Task StopWorkflow(string sessionId, List<TaskAttribute> attribute, List<TaskKey> key, List<TaskParam> param, RequestMode requestMode, WorkflowInformParams workflowInfo, string parameterFileName, string taskInstancePath, bool isAbort, string osUser, string reason)
        {
            await _integrationExecutor.ExecuteRequest(InformaticaSOAPTemplates.GetStopWorkflowTemplate(
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionId">login sessionId</param>
        /// <param name="folderName">folder containing workflow</param>
        /// <param name="workflowName">workflow name which is need</param>
        /// <returns></returns>
        public async Task UncheduleWorkflow(string sessionId, string folderName, string workflowName)
        {
            await _integrationExecutor.ExecuteRequest(InformaticaSOAPTemplates.GetUncheduleWorkflowTemplate(
                sessionId: sessionId,
              workflowInfo: new WorkflowInformParams
              {
                  FolderName = folderName,
                  WorkflowName = workflowName
              },
              serviceInfo: _serviceInfo
              ));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionId">login sessionId</param>
        /// <param name="folderName">folder containing workflow</param>
        /// <param name="workflowName">workflow name which is need</param>
        /// <param name="requestMode">request mode Normal or Recovery</param>
        /// <returns></returns>
        public async Task UncheduleWorkflow(string sessionId, string folderName, string workflowName, RequestMode requestMode)
        {
            await _integrationExecutor.ExecuteRequest(InformaticaSOAPTemplates.GetUncheduleWorkflowTemplate(
               sessionId: sessionId,
             requestMode: requestMode,
             workflowInfo: new WorkflowInformParams
             {
                 FolderName = folderName,
                 WorkflowName = workflowName
             },
             serviceInfo: _serviceInfo
             ));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionId">login sessionId</param>
        /// <param name="folderName">folder containing workflow</param>
        /// <param name="workflowName">workflow name which is need</param>
        /// <param name="requestMode">request mode Normal or Recovery</param>
        /// <param name="parameterFileName">parameter file to use when running the workflow</param>
        /// <returns></returns>
        public async Task UncheduleWorkflow(string sessionId, string folderName, string workflowName, RequestMode requestMode, string parameterFileName)
        {
            await _integrationExecutor.ExecuteRequest(InformaticaSOAPTemplates.GetUncheduleWorkflowTemplate(
               sessionId: sessionId,
             requestMode: requestMode,
             workflowInfo: new WorkflowInformParams
             {
                 FolderName = folderName,
                 WorkflowName = workflowName
             },
             serviceInfo: _serviceInfo
             ));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionId">login sessionId</param>
        /// <param name="folderName">folder containing workflow</param>
        /// <param name="workflowName">workflow name which is need</param>
        /// <param name="requestMode">request mode Normal or Recovery</param>
        /// <param name="param">optional parameters  as sends to worklow</param>
        /// <returns></returns>
        public async Task UncheduleWorkflow(string sessionId, string folderName, string workflowName, RequestMode requestMode, List<TaskParam> param)
        {
            await _integrationExecutor.ExecuteRequest(InformaticaSOAPTemplates.GetUncheduleWorkflowTemplate(
                sessionId: sessionId,
              requestMode: requestMode,
              workflowInfo: new WorkflowInformParams
              {
                  FolderName = folderName,
                  WorkflowName = workflowName
              },
              serviceInfo: _serviceInfo,
               param: param
              ));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionId">login sessionId</param>
        /// <param name="attribute">attribute used to start or schedule a workflow or task</param>
        /// <param name="key">Key to use to start a workflow or task</param>
        /// <param name="param">optional parameters  as sends to worklow</param>
        /// <param name="requestMode">request mode Normal or Recovery</param>
        /// <param name="workflowInfo">workflow data  need to run</param>
        /// <param name="parameterFileName">parameter file to use when running the workflow</param>
        /// <param name="taskInstancePath">Path specifying the location of the task</param>
        /// <param name="isAbort">Indicate whether to abort a task</param>
        /// <param name="osUser">system profile assigned to the workflow</param>
        /// <param name="reason">request reason</param>
        /// <returns></returns>
        public async Task UncheduleWorkflow(string sessionId, List<TaskAttribute> attribute, List<TaskKey> key, List<TaskParam> param, RequestMode requestMode, WorkflowInformParams workflowInfo, string parameterFileName, string taskInstancePath, bool isAbort, string osUser, string reason)
        {
            await _integrationExecutor.ExecuteRequest(InformaticaSOAPTemplates.GetUncheduleWorkflowTemplate(
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionId">login sessionId</param>
        /// <param name="folderName">folder containing workflow</param>
        /// <param name="workflowName">workflow name which is need</param>
        /// <returns></returns>
        public async Task WaitTillWorkflowComplete(string sessionId, string folderName, string workflowName)
        {
            await _integrationExecutor.ExecuteRequest(InformaticaSOAPTemplates.GetWaitTillWorkflowCompleteTemplate(
                sessionId: sessionId,
              workflowInfo: new WorkflowInformParams
              {
                  FolderName = folderName,
                  WorkflowName = workflowName
              },
              serviceInfo: _serviceInfo
              ));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionId">login sessionId</param>
        /// <param name="folderName">folder containing workflow</param>
        /// <param name="workflowName">workflow name which is need</param>
        /// <param name="requestMode">request mode Normal or Recovery</param>
        /// <returns></returns>
        public async Task WaitTillWorkflowComplete(string sessionId, string folderName, string workflowName, RequestMode requestMode)
        {
            await _integrationExecutor.ExecuteRequest(InformaticaSOAPTemplates.GetWaitTillWorkflowCompleteTemplate(
                sessionId: sessionId,
              requestMode: requestMode,
              workflowInfo: new WorkflowInformParams
              {
                  FolderName = folderName,
                  WorkflowName = workflowName
              },
              serviceInfo: _serviceInfo
              ));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionId">login sessionId</param>
        /// <param name="folderName">folder containing workflow</param>
        /// <param name="workflowName">workflow name which is need</param>
        /// <param name="requestMode">request mode Normal or Recovery</param>
        /// <param name="parameterFileName">parameter file to use when running the workflow</param>
        /// <returns></returns>
        public async Task WaitTillWorkflowComplete(string sessionId, string folderName, string workflowName, RequestMode requestMode, string parameterFileName)
        {
            await _integrationExecutor.ExecuteRequest(InformaticaSOAPTemplates.GetWaitTillWorkflowCompleteTemplate(
                sessionId: sessionId,
              requestMode: requestMode,
              workflowInfo: new WorkflowInformParams
              {
                  FolderName = folderName,
                  WorkflowName = workflowName
              },
              serviceInfo: _serviceInfo,
               parameterFileName: parameterFileName
              ));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionId">login sessionId</param>
        /// <param name="folderName">folder containing workflow</param>
        /// <param name="workflowName">workflow name which is need</param>
        /// <param name="requestMode">request mode Normal or Recovery</param>
        /// <param name="param">optional parameters  as sends to worklow</param>
        /// <returns></returns>
        public async Task WaitTillWorkflowComplete(string sessionId, string folderName, string workflowName, RequestMode requestMode, List<TaskParam> param)
        {
            await _integrationExecutor.ExecuteRequest(InformaticaSOAPTemplates.GetWaitTillWorkflowCompleteTemplate(
                 sessionId: sessionId,
               requestMode: requestMode,
               workflowInfo: new WorkflowInformParams{
                   FolderName=folderName,
                   WorkflowName=workflowName},
               serviceInfo: _serviceInfo,
                param: param
               ));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionId">login sessionId</param>
        /// <param name="attribute">attribute used to start or schedule a workflow or task</param>
        /// <param name="key">Key to use to start a workflow or task</param>
        /// <param name="param">optional parameters  as sends to worklow</param>
        /// <param name="requestMode">request mode Normal or Recovery</param>
        /// <param name="workflowInfo">workflow data  need to run</param>
        /// <param name="parameterFileName">parameter file to use when running the workflow</param>
        /// <param name="taskInstancePath">Path specifying the location of the task</param>
        /// <param name="isAbort">Indicate whether to abort a task</param>
        /// <param name="osUser">system profile assigned to the workflow</param>
        /// <param name="reason">request reason</param>
        /// <returns></returns>
        public async Task WaitTillWorkflowComplete(string sessionId, List<TaskAttribute> attribute, List<TaskKey> key, List<TaskParam> param, RequestMode requestMode, WorkflowInformParams workflowInfo, string parameterFileName, string taskInstancePath, bool isAbort, string osUser, string reason)
        {
            await _integrationExecutor.ExecuteRequest(InformaticaSOAPTemplates.GetWaitTillWorkflowCompleteTemplate(
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
       
        #endregion
#region TASK
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionId">login sessionId</param>
        /// <param name="folderName">folder containing workflow</param>
        /// <param name="workflowName">workflow name which is need</param>
        /// <param name="taskInstancePath">Path specifying the location of the task</param>
        /// <returns></returns>
        public async Task<string> TaskDetail(string sessionId, string folderName, string workflowName, string taskInstancePath)
        {
          var result=  await _integrationExecutor.ExecuteRequest(InformaticaSOAPTemplates.GetTaskDetailTemplate(
                sessionId: sessionId,
                workflowInfo: new WorkflowInformParams
                {
                    FolderName = folderName,
                    WorkflowName = workflowName
                },
                serviceInfo: _serviceInfo,
                taskInstancePath: taskInstancePath));
            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionId">login sessionId</param>
        /// <param name="folderName">folder containing workflow</param>
        /// <param name="workflowName">workflow name which is need</param>
        /// <param name="taskInstancePath">Path specifying the location of the task</param>
        /// <param name="requestMode">request mode Normal or Recovery</param>
        /// <returns></returns>
        public async Task<string> TaskDetail(string sessionId, string folderName, string workflowName, string taskInstancePath, RequestMode requestMode)
        {
            var result = await _integrationExecutor.ExecuteRequest(InformaticaSOAPTemplates.GetTaskDetailTemplate(
                sessionId: sessionId,
                requestMode: requestMode,
                workflowInfo: new WorkflowInformParams
                {
                    FolderName = folderName,
                    WorkflowName = workflowName
                },
                serviceInfo: _serviceInfo,
                taskInstancePath: taskInstancePath));
            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionId">login sessionId</param>
        /// <param name="folderName">folder containing workflow</param>
        /// <param name="workflowName">workflow name which is need</param>
        /// <param name="taskInstancePath">Path specifying the location of the task</param>
        /// <param name="requestMode">request mode Normal or Recovery</param>
        /// <param name="param">optional parameters  as sends to worklow</param>
        /// <returns></returns>
        public async Task<string> TaskDetail(string sessionId, string folderName, string workflowName, string taskInstancePath, RequestMode requestMode, List<TaskParam> param)
        {
            var result = await _integrationExecutor.ExecuteRequest(InformaticaSOAPTemplates.GetTaskDetailTemplate(
                sessionId: sessionId,
                requestMode: requestMode,
                workflowInfo: new WorkflowInformParams
                {
                    FolderName = folderName,
                    WorkflowName = workflowName
                },
                serviceInfo: _serviceInfo, param: param,
                taskInstancePath: taskInstancePath));
            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionId">login sessionId</param>
        /// <param name="param">optional parameters  as sends to worklow</param>
        /// <param name="requestMode">request mode Normal or Recovery</param>
        /// <param name="workflowInfo">workflow data  need to run</param>
        /// <param name="taskInstancePath">Path specifying the location of the task</param>
        /// <param name="isAbort">Indicate whether to abort a task</param>
        /// <returns></returns>
        public async Task<string> TaskDetail(string sessionId, List<TaskParam> param, RequestMode requestMode, WorkflowInformParams workflowInfo, string taskInstancePath, bool isAbort)
        {
            var result = await _integrationExecutor.ExecuteRequest(InformaticaSOAPTemplates.GetTaskDetailTemplate(
                sessionId: sessionId,
                requestMode: requestMode,
                workflowInfo: workflowInfo,
                serviceInfo: _serviceInfo,
                isAbort: isAbort, param: param,
                taskInstancePath: taskInstancePath));
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionId">login sessionId</param>
        /// <param name="folderName">folder containing workflow</param>
        /// <param name="workflowName">workflow name which is need</param>
        /// <param name="taskInstancePath">Path specifying the location of the task</param>
        /// <returns></returns>
        public async Task<string> TaskDetailEx(string sessionId, string folderName, string workflowName, string taskInstancePath)
        {
            var result = await _integrationExecutor.ExecuteRequest(InformaticaSOAPTemplates.GetTaskDetailExTemplate(
                   sessionId: sessionId,
                   workflowInfoEx: new WorkflowInforExParams
                   {
                       FolderName = folderName,
                       WorkflowName = workflowName
                   },
                   serviceInfo: _serviceInfo,
                   taskInstancePath: taskInstancePath));
            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionId">login sessionId</param>
        /// <param name="folderName">folder containing workflow</param>
        /// <param name="workflowName">workflow name which is need</param>
        /// <param name="taskInstancePath">Path specifying the location of the task</param>
        /// <param name="workflowRunInstanceName"></param>
        /// <returns></returns>
        public async Task<string> TaskDetailEx(string sessionId, string folderName, string workflowName, string taskInstancePath,string workflowRunInstanceName)
        {
            var result = await _integrationExecutor.ExecuteRequest(InformaticaSOAPTemplates.GetTaskDetailExTemplate(
                   sessionId: sessionId,
                   workflowInfoEx: new WorkflowInforExParams
                   {
                       FolderName = folderName,
                       WorkflowName = workflowName,
                       WorkflowInstanceName =workflowRunInstanceName
                   },
                   serviceInfo: _serviceInfo,
                   taskInstancePath: taskInstancePath));
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionId">login sessionId</param>
        /// <param name="folderName">folder containing workflow</param>
        /// <param name="workflowName">workflow name which is need</param>
        /// <param name="taskInstancePath">Path specifying the location of the task</param>
        /// <returns></returns>
        public async Task StartTask(string sessionId, string folderName, string workflowName, string taskInstancePath)
        {
             await _integrationExecutor.ExecuteRequest(InformaticaSOAPTemplates.GetStartTaskTemplate(
                  sessionId: sessionId,
                  workflowInfo: new WorkflowInformParams
                  {
                      FolderName = folderName,
                      WorkflowName = workflowName
                  },
                  serviceInfo: _serviceInfo,
                  taskInstancePath: taskInstancePath));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionId">login sessionId</param>
        /// <param name="folderName">folder containing workflow</param>
        /// <param name="workflowName">workflow name which is need</param>
        /// <param name="taskInstancePath">Path specifying the location of the task</param>
        /// <param name="requestMode">request mode Normal or Recovery</param>
        /// <returns></returns>
        public async Task StartTask(string sessionId, string folderName, string workflowName, string taskInstancePath, RequestMode requestMode)
        {
            await _integrationExecutor.ExecuteRequest(InformaticaSOAPTemplates.GetStartTaskTemplate(
                sessionId: sessionId,
                requestMode: requestMode,
                workflowInfo: new WorkflowInformParams
                {
                    FolderName = folderName,
                    WorkflowName = workflowName
                },
                serviceInfo: _serviceInfo,
                taskInstancePath: taskInstancePath));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionId">login sessionId</param>
        /// <param name="folderName">folder containing workflow</param>
        /// <param name="workflowName">workflow name which is need</param>
        /// <param name="taskInstancePath">Path specifying the location of the task</param>
        /// <param name="requestMode">request mode Normal or Recovery</param>
        /// <param name="param">optional parameters  as sends to worklow</param>
        /// <returns></returns>
        public async Task StartTask(string sessionId, string folderName, string workflowName, string taskInstancePath, RequestMode requestMode, List<TaskParam> param)
        {
             await _integrationExecutor.ExecuteRequest(InformaticaSOAPTemplates.GetStartTaskTemplate(
                sessionId: sessionId,
                requestMode: requestMode,
                workflowInfo: new WorkflowInformParams
                {
                    FolderName = folderName,
                    WorkflowName = workflowName
                },
                serviceInfo: _serviceInfo, param: param,
                taskInstancePath: taskInstancePath));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionId">login sessionId</param>
        /// <param name="param">optional parameters  as sends to worklow</param>
        /// <param name="requestMode">request mode Normal or Recovery</param>
        /// <param name="workflowInfo">workflow data  need to run</param>
        /// <param name="taskInstancePath">Path specifying the location of the task</param>
        /// <param name="isAbort">Indicate whether to abort a task</param>
        /// <returns></returns>
        public async Task StartTask(string sessionId, List<TaskParam> param, RequestMode requestMode, WorkflowInformParams workflowInfo,string taskInstancePath, bool isAbort)
        {
            await _integrationExecutor.ExecuteRequest(InformaticaSOAPTemplates.GetStartTaskTemplate(
                sessionId: sessionId,
                requestMode: requestMode,
                workflowInfo: workflowInfo,
                serviceInfo: _serviceInfo,
                isAbort: isAbort, param: param,
                taskInstancePath: taskInstancePath));
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionId">login sessionId</param>
        /// <param name="folderName">folder containing workflow</param>
        /// <param name="workflowName">workflow name which is need</param>
        /// <param name="taskInstancePath">Path specifying the location of the task</param>
        /// <returns></returns>
        public async Task StopTask(string sessionId, string folderName, string workflowName, string taskInstancePath)
        {
            await _integrationExecutor.ExecuteRequest(InformaticaSOAPTemplates.GetStopTaskTemplate(
                 sessionId: sessionId,
                 workflowInfo: new WorkflowInformParams
                 {
                     FolderName = folderName,
                     WorkflowName = workflowName
                 },
                 serviceInfo: _serviceInfo,
                 taskInstancePath: taskInstancePath));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionId">login sessionId</param>
        /// <param name="folderName">folder containing workflow</param>
        /// <param name="workflowName">workflow name which is need</param>
        /// <param name="taskInstancePath">Path specifying the location of the task</param>
        /// <param name="requestMode">request mode Normal or Recovery</param>
        /// <returns></returns>
        public async Task StopTask(string sessionId, string folderName, string workflowName, string taskInstancePath, RequestMode requestMode)
        {
            await _integrationExecutor.ExecuteRequest(InformaticaSOAPTemplates.GetStopTaskTemplate(
                sessionId: sessionId,
                requestMode: requestMode,
                workflowInfo: new WorkflowInformParams
                {
                    FolderName = folderName,
                    WorkflowName = workflowName
                },
                serviceInfo: _serviceInfo,
                taskInstancePath: taskInstancePath));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionId">login sessionId</param>
        /// <param name="folderName">folder containing workflow</param>
        /// <param name="workflowName">workflow name which is need</param>
        /// <param name="taskInstancePath">Path specifying the location of the task</param>
        /// <param name="requestMode">request mode Normal or Recovery</param>
        /// <param name="param">optional parameters  as sends to worklow</param>
        /// <returns></returns>
        public async Task StopTask(string sessionId, string folderName, string workflowName, string taskInstancePath, RequestMode requestMode, List<TaskParam> param)
        {
            await _integrationExecutor.ExecuteRequest(InformaticaSOAPTemplates.GetStopTaskTemplate(
               sessionId: sessionId,
               requestMode: requestMode,
               workflowInfo: new WorkflowInformParams
               {
                   FolderName = folderName,
                   WorkflowName = workflowName
               },
               serviceInfo: _serviceInfo, param: param,
               taskInstancePath: taskInstancePath));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionId">login sessionId</param>
        /// <param name="param">optional parameters  as sends to worklow</param>
        /// <param name="requestMode">request mode Normal or Recovery</param>
        /// <param name="workflowInfo">workflow data  need to run</param>
        /// <param name="taskInstancePath">Path specifying the location of the task</param>
        /// <param name="isAbort">Indicate whether to abort a task</param>
        /// <returns></returns>
        public async Task StopTask(string sessionId,  List<TaskParam> param, RequestMode requestMode, WorkflowInformParams workflowInfo, string taskInstancePath, bool isAbort)
        {
            await _integrationExecutor.ExecuteRequest(InformaticaSOAPTemplates.GetStopTaskTemplate(
                sessionId: sessionId,
                requestMode: requestMode,
                workflowInfo: workflowInfo,
                serviceInfo: _serviceInfo,
                isAbort: isAbort, param: param,
                taskInstancePath: taskInstancePath));

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionId">login sessionId</param>
        /// <param name="folderName">folder containing workflow</param>
        /// <param name="workflowName">workflow name which is need</param>
        /// <param name="taskInstancePath">Path specifying the location of the task</param>
        /// <returns></returns>
        public async Task WaitTillTaskComplete(string sessionId, string folderName, string workflowName, string taskInstancePath)
        {
            await _integrationExecutor.ExecuteRequest(InformaticaSOAPTemplates.GetWaitTillTaskCompleteTemplate(
                 sessionId: sessionId,
                 workflowInfo: new WorkflowInformParams
                 {
                     FolderName = folderName,
                     WorkflowName = workflowName
                 },
                 serviceInfo: _serviceInfo,
                 taskInstancePath: taskInstancePath));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionId">login sessionId</param>
        /// <param name="folderName">folder containing workflow</param>
        /// <param name="workflowName">workflow name which is need</param>
        /// <param name="taskInstancePath">Path specifying the location of the task</param>
        /// <param name="requestMode">request mode Normal or Recovery</param>
        /// <returns></returns>
        public async Task WaitTillTaskComplete(string sessionId, string folderName, string workflowName, string taskInstancePath, RequestMode requestMode)
        {
            await _integrationExecutor.ExecuteRequest(InformaticaSOAPTemplates.GetWaitTillTaskCompleteTemplate(
                sessionId: sessionId,
                requestMode: requestMode,
                workflowInfo: new WorkflowInformParams
                {
                    FolderName = folderName,
                    WorkflowName = workflowName
                },
                serviceInfo: _serviceInfo,
                taskInstancePath: taskInstancePath));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionId">login sessionId</param>
        /// <param name="folderName">folder containing workflow</param>
        /// <param name="workflowName">workflow name which is need</param>
        /// <param name="taskInstancePath">Path specifying the location of the task</param>
        /// <param name="requestMode">request mode Normal or Recovery</param>
        /// <param name="param">optional parameters  as sends to worklow</param>
        /// <returns></returns>
        public async Task WaitTillTaskComplete(string sessionId, string folderName, string workflowName, string taskInstancePath, RequestMode requestMode, List<TaskParam> param)
        {
            await _integrationExecutor.ExecuteRequest(InformaticaSOAPTemplates.GetWaitTillTaskCompleteTemplate(
               sessionId: sessionId,
               requestMode: requestMode,
               workflowInfo: new WorkflowInformParams
               {
                   FolderName = folderName,
                   WorkflowName = workflowName
               },
               serviceInfo: _serviceInfo, param: param,
               taskInstancePath: taskInstancePath));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionId">login sessionId</param>
        /// <param name="param">optional parameters  as sends to worklow</param>
        /// <param name="requestMode">request mode Normal or Recovery</param>
        /// <param name="workflowInfo">workflow data  need to run</param>
        /// <param name="taskInstancePath">Path specifying the location of the task</param>
        /// <param name="isAbort">Indicate whether to abort a task</param>
        /// <returns></returns>
        public async Task WaitTillTaskComplete(string sessionId, List<TaskParam> param, RequestMode requestMode, WorkflowInformParams workflowInfo,string taskInstancePath, bool isAbort)
        {
            await _integrationExecutor.ExecuteRequest(InformaticaSOAPTemplates.GetWaitTillTaskCompleteTemplate(
                sessionId: sessionId,
                requestMode: requestMode,
                workflowInfo: workflowInfo,
                serviceInfo: _serviceInfo,
                isAbort: isAbort, param: param,
                taskInstancePath: taskInstancePath));

        }

#endregion

        #region SESSION
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionId">login sessionId</param>
        /// <param name="folderName">folder containing workflow</param>
        /// <param name="workflowName">workflow name which is need</param>
        /// <param name="taskInstancePath">Path specifying the location of the task</param>
        /// <returns></returns>
        public async Task<string> SessionPerfomanceData(string sessionId, string folderName, string workflowName, string taskInstancePath)
        {
            var result=  await _integrationExecutor.ExecuteRequest(InformaticaSOAPTemplates.GetSessionPerfomanceDataTemplate(
               sessionId: sessionId,
             folderName:folderName,
             workflowName:workflowName,
             taskInstancePath:taskInstancePath,
             serviceInfo: _serviceInfo
             ));
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionId">login sessionId</param>
        /// <param name="folderName">folder containing workflow</param>
        /// <param name="workflowName">workflow name which is need</param>
        /// <param name="taskInstancePath">Path specifying the location of the task</param>
        /// <returns></returns>
        public async Task<string> SessionStatistic(string sessionId, string folderName, string workflowName, string taskInstancePath)
        {
            var result = await _integrationExecutor.ExecuteRequest(InformaticaSOAPTemplates.GetSessionStatisticTemplate(
                  sessionId: sessionId,
                folderName: folderName,
                workflowName: workflowName,
                taskInstancePath: taskInstancePath,
                serviceInfo: _serviceInfo
                ));
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionId">login sessionId</param>
        /// <param name="folderName">folder containing workflow</param>
        /// <param name="workflowName">workflow name which is need</param>
        /// <param name="taskInstancePath">Path specifying the location of the task</param>
        /// <returns></returns>
        public async Task StartSessLogFetch(string sessionId, string folderName, string workflowName, string taskInstancePath)
        {
            await _integrationExecutor.ExecuteRequest(InformaticaSOAPTemplates.GetStartSessLogFetchTemplate(
                    sessionId: sessionId,
                  folderName: folderName,
                  workflowName: workflowName,
                  taskInstancePath: taskInstancePath,
                  serviceInfo: _serviceInfo
                  ));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionId">login sessionId</param>
        /// <param name="folderName">folder containing workflow</param>
        /// <param name="workflowName">workflow name which is need</param>
        /// <param name="taskInstancePath">Path specifying the location of the task</param>
        /// <returns></returns>
        public async Task<List<LogMessage>> GetSessionLog(string sessionId, string folderName, string workflowName, string taskInstancePath)
        {
            var result = await _integrationExecutor.ExecuteRequest(InformaticaSOAPTemplates.GetSessionLogTemplate(sessionId, folderName, workflowName, taskInstancePath, _serviceInfo));
            return InfaLogParser.ParseWorkflowLogs(ValuesSoapXml.GetValueOnElement(result, "Buffer"));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionId">login sessionId</param>
        /// <param name="folderName">folder containing workflow</param>
        /// <param name="workflowName">workflow name which is need</param>
        /// <param name="taskInstancePath">Path specifying the location of the task</param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public async Task<List<LogMessage>> GetSessionLog(string sessionId, string folderName, string workflowName, string taskInstancePath,int timeout)
        {
            var result = await _integrationExecutor.ExecuteRequest(InformaticaSOAPTemplates.GetSessionLogTemplate(sessionId, folderName, workflowName, taskInstancePath, _serviceInfo, timeout));
            return InfaLogParser.ParseWorkflowLogs(ValuesSoapXml.GetValueOnElement(result, "Buffer"));
        }
        #endregion

        #region SERVER
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionId">login sessionId</param>
        /// <returns></returns>
        public async Task DeinitServerConnection(string sessionId)
        {
            await _integrationExecutor.ExecuteRequest(InformaticaSOAPTemplates.GetDeinitServerConnectionTemplate(sessionId));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionId">login sessionId</param>
        /// <returns></returns>
        public async Task<string> DIServerProp(string sessionId)
        {
            string result = await _integrationExecutor.ExecuteRequest(InformaticaSOAPTemplates.GetDIServerPropTemplate(sessionId, _serviceInfo));
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionId">login sessionId</param>
        /// <param name="logHandle"></param>
        /// <param name="timeOut"></param>
        /// <returns></returns>
        public async Task<string> NextLogSegment(string sessionId, int logHandle, int timeOut = 60)
        {
           string result = await _integrationExecutor.ExecuteRequest(InformaticaSOAPTemplates.GetNextLogSegmentTemplate(sessionId, logHandle, timeOut));
           return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionId">login sessionId</param>
        /// <returns></returns>
        public async Task InitServerConnection(string sessionId)
        {
            await _integrationExecutor.ExecuteRequest(InformaticaSOAPTemplates.GetInitServerConnectionTemplate(sessionId));
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionId">login sessionId</param>
        /// <param name="loginHandle"></param>
        /// <param name="serverName"></param>
        /// <param name="domainName"></param>
        /// <returns></returns>
        public async Task InitServerConnection(string sessionId, string loginHandle, string serverName, string domainName)
        {
            await _integrationExecutor.ExecuteRequest(InformaticaSOAPTemplates.GetInitServerConnectionTemplate(sessionId,loginHandle,serverName,domainName));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionId">login sessionId</param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public async Task<string> MonitorDIServer(string sessionId,MonitorMode mode)
        {
            var result = await _integrationExecutor.ExecuteRequest(InformaticaSOAPTemplates.GetMonitorDIServerTemplate(sessionId, mode,_serviceInfo));
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionId">login sessionId</param>
        /// <param name="timeOut"></param>
        /// <returns></returns>
        public async Task<bool> PingDIServer(string sessionId, int timeOut = 10)
        {
            string result = "";
            try
            {
                result = await _integrationExecutor.ExecuteRequest(InformaticaSOAPTemplates.GetPingDIServerTemplate(sessionId, _serviceInfo, timeOut));
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
