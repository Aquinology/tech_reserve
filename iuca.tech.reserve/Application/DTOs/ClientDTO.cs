namespace Application.DTOs;

public class ClientDTO
{
    public string? ApplicationUserId { get; set; }

    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? OtherInfo { get; set; }

    public string FullName
    {
        get
        {
            return $"{LastName} {FirstName}";
        }
    }    
}

