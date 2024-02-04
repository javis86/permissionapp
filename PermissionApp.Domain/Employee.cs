namespace PermissionApp.Domain;

public class Employee
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public List<Permission> Permissions { get; set; }
}