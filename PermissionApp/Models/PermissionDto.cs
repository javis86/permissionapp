using System.Runtime.InteropServices.ComTypes;
using PermissionApp.Domain;

namespace PermissionApp.Models;

public class PermissionsDto
{
    public EmployeeDto Employee { get; set; }
    public List<PermissionDto> Permissions { get; set; }
}

public class EmployeeDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
}

public class PermissionDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Status { get; set; }
}