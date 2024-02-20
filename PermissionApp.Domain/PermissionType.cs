using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("PermissionApp.Tests")]
namespace PermissionApp.Domain;

public class PermissionType
{
    private readonly string _name;
    private readonly Guid _id;
    public Guid Id => _id;
    public string Name => _name;

    internal PermissionType(string name)    
    {
        _id = Guid.NewGuid();
        _name = name;
    }

    public PermissionType(string name, Guid id)
    {
        _id = id;
        _name = name;
    }

    public Permission RequestFor(Employee employee)
    {
        return new Permission(employee, this);
    }
}