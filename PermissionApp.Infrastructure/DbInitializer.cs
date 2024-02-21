using System.Net.Mime;
using PermissionApp.Domain;

namespace PermissionApp.Infrastructure;

public static class DbInitializer
{
    public static void Initialize(AppDbContext context)
    {
        Employee?[] employees = new[]
        {
            new Employee() { Id = new Guid("32388767-97d2-48c0-7ab2-08dc2bd9c05d"), Name = "Jhon", Surname = "Doe"},
            new Employee() { Id = new Guid("572ac435-a418-46a8-7ab3-08dc2bd9c05d"), Name = "Maria", Surname = "Speer"}
        };
        
        context.Employees.AddRange(employees);
        Console.WriteLine();
        Console.WriteLine("--------------------------------------------------------------");
        foreach (var employee in employees)
        {
            Console.WriteLine($@"Employee {employee.Id} - {employee.Name} {employee.Surname}");    
        }

        var permissionTypes = new[]
        {
            new PermissionType("Account Information", new Guid("572ac435-a418-46a8-7ab3-08dc2bd9c05d")),
            new PermissionType("Finance Information", new Guid("b4236f80-4364-4e5c-81e7-70bda04acb06")),
            new PermissionType("User History Information", new Guid("106c5c61-70ef-42f8-8a8e-1c689a4d73f5"))
        };
        context.PermissionTypes.AddRange(permissionTypes);
        
        foreach (var permission in permissionTypes)
        {
            Console.WriteLine($@"Permission Type {permission.Id} - {permission.Name}");    
        }
        Console.WriteLine(@"--------------------------------------------------------------");
        Console.WriteLine();

        var permission1 = permissionTypes[0].RequestFor(employees[0]);
        context.Permissions.AddAsync(permission1);
        
        context.SaveChanges();
    }
}