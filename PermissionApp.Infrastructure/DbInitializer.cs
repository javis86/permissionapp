using System.Net.Mime;
using PermissionApp.Domain;

namespace PermissionApp.Infrastructure;

public static class DbInitializer
{
    public static void Initialize(AppDbContext context)
    {
        Employee?[] employees = new[]
        {
            new Employee() { Name = "Jhon", Surname = "Doe"},
            new Employee() { Name = "Maria", Surname = "Speer"}
        };
        
        context.Employees.AddRange(employees);
        Console.WriteLine();
        Console.WriteLine("--------------------------------------------------------------");
        foreach (var employee in employees)
        {
            Console.WriteLine($@"Employee {employee.Id} - {employee.Name} {employee.Name}");    
        }

        var permissionTypes = new[]
        {
            new PermissionType("Account Information"),
            new PermissionType("Finance Information"),
            new PermissionType("User History Information")
        };
        context.PermissionTypes.AddRange(permissionTypes);
        
        foreach (var permission in permissionTypes)
        {
            Console.WriteLine($@"Permission Type {permission.Id} - {permission.Name}");    
        }
        Console.WriteLine(@"--------------------------------------------------------------");
        Console.WriteLine();
        
        context.SaveChanges();
    }
}