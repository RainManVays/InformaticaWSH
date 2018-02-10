# InformaticaWSH
InformaticaWSH is framework for work with Informatica SOAP Requests


Mini Example:
```C#
          string _url = @"http://localhost:7333/wsh/services/BatchServices/DataIntegration?WSDL";
          var controller = new InformaticaWebRequestController(_url, new DIServiceInfo { 
                                                                     DomainName = "Domain_domain", 
                                                                     ServiceName = "infa_rep_is" });

            
            string sessionId = await controller.Login( domain:"Domain_melchior",
                                                        repository: "infa_rep_primera", 
                                                        login:"sys",
                                                        password:"sys");

            List<LogMessage> workflowLog = await controller.GetSessionLog(sessionId:sessionId, 
                                                                            folderName:"OUT",
                                                                            workflowName:"WF_M_INF_CTL_OUT_TEXT_DATA", 
                                                                            taskInstancePath:"S_M_INF_CTL_OUT_TEXT_DATA");
            log.ItemsSource = workflowLog;
            log.Items.Refresh();
            controller.Dispose();
```
