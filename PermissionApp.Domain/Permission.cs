namespace PermissionApp.Domain;

public class Permission
{
    private Status _status;
    public int Id { get; set; }
    public Employee Employee { get; set; }
    public PermissionType PermissionType { get; set; }
    public Status Status => _status;
    
    protected Permission()
    {
        _status = Status.Requested;
    }

    public Permission(Employee employee, PermissionType permissionType)
    {
        Employee = employee;
        PermissionType = permissionType;
        _status = Status.Requested;
    }

    public void Grant()
    {
        if(Status is Status.Denied)
            throw new InvalidStateException();

        _status = Status.Granted;
    }

    public void Deny()
    {
        _status = Status.Denied;
    }
}

public enum Status
{
    Requested,
    Granted,
    Denied
}