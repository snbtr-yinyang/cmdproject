namespace CMDProject.Domain.Entities;

public class AuthUsers
{
    public int AuthUsers_Id { get; set; }
    public int Users_Id { get; set; }
    public string Auth_UserName { get; set; } = string.Empty;
    public string Auth_Password { get; set; } = string.Empty;
    public DateTime Created_Date { get; set; }
    public string Created_By { get; set; }
    public DateTime Modified_Date { get; set; }
    public string Modified_By { get; set; }
}