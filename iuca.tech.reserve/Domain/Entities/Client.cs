namespace Domain.Entities;

public class Client
{
    public string ApplicationUserId { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? OtherInfo { get; set; }

    public IList<Request> Requests { get; private set; } = new List<Request>();
}
