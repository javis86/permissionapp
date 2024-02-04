using MediatR;

namespace PermissionApp.Commands;

public class RequestPermissionCommand : IRequest<bool>
{
    public Guid PermissionTypeId { get; set; }
    public Guid EmployeeId { get; set; }
}