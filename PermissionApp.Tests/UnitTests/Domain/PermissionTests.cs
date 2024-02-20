using FluentAssertions;
using PermissionApp.Domain;

namespace PermissionApp.Tests.Domain;

[TestClass]
public class PermissionTests
{
    [TestMethod]
    public void Grant_WhenRequestedFirst_Granted()
    {
        // Arrange
        var permission = new PermissionType("Account").RequestFor(new Employee());

        // Act
        permission.Grant();

        // Assert
        permission.Status.Should().Be(Status.Granted);
    }
    
    [TestMethod]
    public void Grant_GrantWhenAlreadyGranted_Granted()
    {
        // Arrange
        var permission = new PermissionType("Account").RequestFor(new Employee());
        permission.Grant();

        // Act
        permission.Grant();

        // Assert
        permission.Status.Should().Be(Status.Granted);
    }
    
    [TestMethod]
    public void Grant_GrantWhenAlreadyDenied_Granted()
    {
        // Arrange
        var permission = new PermissionType("Account").RequestFor(new Employee());
        permission.Deny();

        // Act
        Action action = () => permission.Grant();

        action.Should().ThrowExactly<InvalidStateException>();
    }
    
    [TestMethod]
    public void Denied_Denied()
    {
        // Arrange
        var permission = new PermissionType("Account").RequestFor(new Employee());

        // Act
        permission.Deny();

        permission.Status.Should().Be(Status.Denied);
    }
}