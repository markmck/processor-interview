namespace ProcessorAPI.Models.Results
{
    public class ProcessingResult
    {
        public bool Success { get; set; }
        public int ProcessedCount { get; set; }
        public string? ErrorMessage { get; set; }
    }
}