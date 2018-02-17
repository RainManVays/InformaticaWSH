namespace InformaticaWSH
{
    public class WorkflowInforExParams
    {
        public string FolderName { get; set; }
        public string WorkflowName { get; set; }
        private string WorkflowInstanceName;


        public void ReorderWorkflowInstanceName(string value)
        {
            this.WorkflowInstanceName = value;
        }

        public string GetWorkflowInstaceName()
        {
            if (string.IsNullOrEmpty(this.WorkflowInstanceName))
                return this.WorkflowName;
            return this.WorkflowInstanceName;
        }
    }
}
