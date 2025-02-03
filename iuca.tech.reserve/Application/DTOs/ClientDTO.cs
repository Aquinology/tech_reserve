using System.ComponentModel.DataAnnotations;

namespace Application.DTOs;

public class ClientDTO
{
    public string? ApplicationUserId { get; set; }

    [Required]
    [MaxLength(128)]
    public string Email { get; set; } = null!;

    [MaxLength(128)]
    public string? FirstName { get; set; }

    [MaxLength(128)]
    public string? LastName { get; set; }

    [MaxLength(128)]
    public string? PhoneNumber { get; set; }

    [MaxLength(256)]
    public string? OtherInfo { get; set; }

    public string FullName
    {
        get
        {
            return $"{LastName} {FirstName}";
        }
    }

    public IList<RequestDTO> Requests { get; set; } = new List<RequestDTO>();
}

