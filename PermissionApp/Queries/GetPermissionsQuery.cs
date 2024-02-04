using MediatR;
using PermissionApp.Domain;

public class GetPermissionsQuery : IRequest<Employee?>
{
    public Guid EmployeeId { get; set; }
}