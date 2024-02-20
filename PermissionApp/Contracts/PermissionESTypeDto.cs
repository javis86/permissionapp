using PermissionApp.Domain;

namespace PermissionApp.Contracts;

public record PermissionESTypeDto(int Id, Guid EmployeeID, Guid PermissionTypeId, Status Status)
{
    public override string ToString()
    {
        return $"{{ Id = {Id}, EmployeeID = {EmployeeID}, PermissionTypeId = {PermissionTypeId}, Status = {Status} }}";
    }
}