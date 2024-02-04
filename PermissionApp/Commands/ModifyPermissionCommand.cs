using MediatR;
using PermissionApp.Domain;

namespace PermissionApp.Commands;

public class ModifyPermissionCommand : IRequest<bool>
{
    public Guid PermissionTypeId { get; set; }
    public Guid EmployeeId { get; set; }
    public PermissionCommandType PermissionCommandType { get; set; }
}

public enum PermissionCommandType
{
    Grant,
    Deny
}