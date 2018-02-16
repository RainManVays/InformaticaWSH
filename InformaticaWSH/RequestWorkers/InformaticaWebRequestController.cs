using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace InformaticaWSH
{
    public class InformaticaWebRequestController:IDisposable
    {
        WebRequestsExecutor executor;
        DIServiceInfo _serviceInfo;
        private string _sessionId;
        public InformaticaWebRequestController(string url,DIServiceInfo serviceInfo)
        {
            executor = new WebRequestsExecutor(url);
            _serviceInfo = serviceInfo;
        }
        public async Task<string> Login(string domain, string repository, string login, string password)
        {
            Dispose();
            string result=await executor.ExecuteRequest(InformaticaWebRequestsTemplates.GetLoginTemplate(domain, repository, login, password));
            string sessionId = ValuesSoapXml.GetValueOnElement(result, "SessionId");
            _sessionId = sessionId;
            return sessionId;
        }
        public async Task Logout(string sessionId)
        {
            await executor.ExecuteRequest(InformaticaWebRequestsTemplates.GetLogoutTemplate(sessionId));
        }

#region WORKFLOW
        public async Task AllWorkflows(string sessionId, string folderName)
        {
        }

        public async Task WorkflowDetail(string sessionId, string folderName)
        {
        }

        public async Task WorkflowDetailEx(string sessionId, string folderName)
        {
        }
        public async Task<List<LogMessage>> GetWorkflowLog(string sessionId, string folderName, string workflowName, int workflowRunId, int timeout = 60)
        {
            var result = await executor.ExecuteRequest(InformaticaWebRequestsTemplates.GetWorkflowLogTemplate(sessionId, folderName, workflowName, workflowRunId, _serviceInfo, timeout));
            return InfaLogParser.ParseWorkflowLogs(ValuesSoapXml.GetValueOnElement(result, "Buffer"));
        }
        public async Task StartWorkflow(string sessionId, string folderName)
        {
        }

        public async Task StartWorkflowEx(string sessionId, string folderName)
        {
        }

        public async Task StartWorkflowFromTask(string sessionId, string folderName)
        {
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
            var result = await executor.ExecuteRequest(InformaticaWebRequestsTemplates.GetSessionLogTemplate(sessionId, folderName, workflowName, taskInstancePath, _serviceInfo, timeout));
            return InfaLogParser.ParseWorkflowLogs(ValuesSoapXml.GetValueOnElement(result, "Buffer"));
        }
#endregion


        public async Task AllFolders(string sessionId)
        {
        }

        public async Task AllRepositories(string sessionId)
        {
        }

        public async Task DeinitServerConnection(string sessionId, string folderName)
        {
        }

        public async Task DIServerProp(string sessionId, DIServiceInfo serviceInfo)
        {
        }

        public async Task NextLogSegment(string sessionId, int logHandle, int timeOut = 60)
        {
        }




        public async Task InitServerConnection(string sessionId, string folderName)
        {
        }

        public async Task MonitorDIServer(string sessionId, string folderName)
        {
        }

        public async Task PingDIServer(string sessionId, int timeOut = 60)
        {
        }

        public async void Dispose()
        {
            if (!string.IsNullOrEmpty(_sessionId))
            {
                await Logout(_sessionId);
                _sessionId = null;
            }
        }
    }
}
