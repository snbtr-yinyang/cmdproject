namespace CMDProject.Domain.Entities;

public class MsUsers
{
    public int Users_Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string MobilePhone { get; set; }
    public string Gender_Id { get; set; }
    public string Religion_Id { get; set; }
    public string Is_Active { get; set; }
    public DateTime Created_Date { get; set; }
    public string Created_By { get; set; }
    public DateTime? Modified_Date { get; set; }
    public string? Modified_By { get; set; }
    
    
}
