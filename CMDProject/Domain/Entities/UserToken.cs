namespace CMDProject.Domain.Entities;

public class UserToken
{
    public int IdToken { get; set; }
    public int UserId { get; set; }
    public string TokenName { get; set; } = string.Empty;
    public bool ActiveStatus { get; set; } = true;
    public DateTime ExpiredDate { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? RevokedDate { get; set; }
}
