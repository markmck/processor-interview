namespace ProcessorAPI.Models.DTOs
{
    public class UploadResponseDTO
    {
        public string Message { get; set; } = string.Empty;
        public int Count { get; set; }
        public DateTime ProcessedAt { get; set; }
    }
}
