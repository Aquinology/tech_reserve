namespace Domain.Exceptions
{
    public class ErrorException
    {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
