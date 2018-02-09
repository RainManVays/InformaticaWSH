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

        public async Task<List<LogMessage>> GetWorkflowLog(string sessionId, string folderName, string workflowName, int workflowRunId, int timeout = 60)
        {
            var result = await executor.ExecuteRequest(InformaticaWebRequestsTemplates.GetWorkflowLogTemplate(sessionId, folderName, workflowName, workflowRunId, _serviceInfo, timeout));
            return InfaLogParser.ParseWorkflowLogs(ValuesSoapXml.GetValueOnElement(result, "Buffer"));
        }
        public async Task<List<LogMessage>> GetSessionLog(string sessionId, string folderName, string workflowName, string taskInstancePath, int timeout = 60)
        {
            var result = await executor.ExecuteRequest(InformaticaWebRequestsTemplates.GetSessionLogTemplate(sessionId, folderName, workflowName,taskInstancePath, _serviceInfo, timeout));
            return InfaLogParser.ParseWorkflowLogs(ValuesSoapXml.GetValueOnElement(result, "Buffer"));
        }

        public async Task Logout(string sessionId)
        {
            await executor.ExecuteRequest(InformaticaWebRequestsTemplates.GetLogoutTemplate(sessionId));
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
