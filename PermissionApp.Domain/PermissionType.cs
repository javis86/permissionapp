namespace PermissionApp.Domain;

public class PermissionType
{
    private readonly string _name;
    private readonly Guid _id;

    public PermissionType(string name)
    {
        _id = Guid.NewGuid();
        _name = name;
    }

    public Guid Id => _id;
    public string Name => _name;

    public Permission RequestFor(Employee employee)
    {
        return new Permission(employee, this);
    }
}