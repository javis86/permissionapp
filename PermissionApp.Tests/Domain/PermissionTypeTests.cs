using FluentAssertions;
using PermissionApp.Domain;

namespace PermissionApp.Tests.Domain;

[TestClass]
public class PermissionTypeTests
{
    [TestMethod]
    public void Request_NewPermissionWithRequestedStatus()
    {
        // Arrange
        var permissionType = new PermissionType("Account Permission");

        var employee = new Employee();
        
        // Act
        Permission permission = permissionType.RequestFor(employee);

        // Assert
        permission.Employee.Should().Be(employee);
        permission.PermissionType.Should().Be(permissionType);
        permission.Status.Should().Be(Status.Requested);
    }
}