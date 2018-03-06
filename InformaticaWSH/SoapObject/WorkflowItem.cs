namespace InformaticaWSH
{
    /// <summary>
    /// workflow item
    /// </summary>
    public class WorkflowItem
    {
        /// <summary>
        /// workflow name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// true or false if workflow valid or invalid
        /// </summary>
        public bool IsValid { get; set; }
        /// <summary>
        /// folder contains current workflow
        /// </summary>
        public string FolderName { get; set; }
    }
}
