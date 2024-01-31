namespace PermissionApp.Domain;

public class Permission
{
    public int Id { get; set; }
    public Employee Employee { get; set; }
    public PermissionType PermissionType { get; set; }
    public Status Status { get; set; }
    
    public Permission()
    {
        Status = Status.Requested;
    }
}

public enum Status
{
    Requested,
    Granted,
    Denied
}