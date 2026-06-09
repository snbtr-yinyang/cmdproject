namespace CMDProject.Domain.Entities;

public class AuthUsers
{
    public int AuthUserId { get; set; }
    public int UserId { get; set; }
    public string AuthUserName { get; set; } = string.Empty;
    public string AuthPassword { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public string CreatedBy { get; set; }
    public DateTime ModifiedDate { get; set; }
    public string ModifiedBy { get; set; }
}