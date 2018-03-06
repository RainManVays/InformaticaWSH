namespace InformaticaWSH
{
    /// <summary>
    /// workflow ex parameter from send in some methods in InformaticaWebRequestController
    /// </summary>
    public class WorkflowInforExParams
    {
        /// <summary>
        /// folder contains current workflow
        /// </summary>
        public string FolderName { get; set; }
        /// <summary>
        /// workflow name
        /// </summary>
        public string WorkflowName { get; set; }
        /// <summary>
        /// I don`t fully understand what is it
        /// </summary>
        public string WorkflowInstanceName { get; set; }
    }
}
