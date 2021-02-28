namespace MapReduceOrchestrator.Entities
{
    public class FunctionDTO<T>
    {
        public string OriginalFileDrop { get; set; }
        public T Input { get; set; }
        public string OrchestrationId { get; set; }
    }
}
