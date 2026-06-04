namespace CMDProject.Domain.Entities;

public class UserToken
{
    public int IdToken { get; set; }
    public int Users_Id { get; internal set; }
    public string TokenName { get; set; } = string.Empty;
    public bool ActiveStatus { get; set; } = true;
    public DateTime Expired_Date { get; set; }
    public DateTime Created_Date { get; set; }
    public DateTime Revoked_Date { get; set; }
    public int UserId { get; internal set; }
}
