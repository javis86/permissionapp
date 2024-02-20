using Moq;
using Elastic.Clients.Elasticsearch;
using FluentAssertions;
using MassTransit;
using PermissionApp.Commands;
using PermissionApp.Contracts;
using PermissionApp.Domain;
using PermissionApp.Handlers;

namespace PermissionApp.Tests.UnitTests.Handlers;

[TestClass]
public class RequestPermissionHandlerTests
{
    [TestMethod]
    public async Task Handle_NewPermissionRequested_AddsPermissionAndReturnsTrue()
    {
        // Arrange
        var kafkaProducerMock = new Mock<ITopicProducer<KafkaMessage>>();
        var elasticsearchClientMock = new Mock<ElasticsearchClient>();
        var unitOfWork = new Mock<IUnitOfWork>();
        var permissionRepository = new Mock<IPermissionRepository>();
        var permissionTypeRepository = new Mock<IPermissionTypeRepository>();
        var employeeRepository = new Mock<IEmployeeRepository>();
        var employee = new Employee();
        var request = new RequestPermissionCommand
        {
            EmployeeId = Guid.NewGuid(),
            PermissionTypeId = Guid.NewGuid()
        };
        
        var permissionType = new PermissionType("Demo history permission", request.PermissionTypeId);
        
        permissionTypeRepository.Setup(x => x.GetAsync(request.PermissionTypeId, It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(permissionType));
        
        employeeRepository.Setup(x => x.GetAsync(request.EmployeeId, It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(employee));

        Permission savedPermission = null;
        permissionRepository.Setup(m => m.AddAsync(It.IsAny<Permission>(), It.IsAny<CancellationToken>()))
            .Callback((Permission permissiontoSave, CancellationToken arg2) => savedPermission = permissiontoSave)
            .Returns(Task.CompletedTask);

        // Act
        var handler = new RequestPermissionHandler(kafkaProducerMock.Object,
            elasticsearchClientMock.Object,
            unitOfWork.Object,
            permissionRepository.Object, 
            permissionTypeRepository.Object, 
            employeeRepository.Object);
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
        savedPermission.Should().NotBeNull();
        savedPermission.Status.Should().Be(Status.Requested);
        savedPermission.Employee.Should().Be(employee);
        savedPermission.PermissionType.Should().Be(permissionType);
        permissionRepository.Verify(m => m.AddAsync(It.IsAny<Permission>(), It.IsAny<CancellationToken>()), Times.Once);
        unitOfWork.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        kafkaProducerMock.Verify(m => m.Produce(It.IsAny<KafkaMessage>(), It.IsAny<CancellationToken>()), Times.Once);
        elasticsearchClientMock.Verify(m => m.IndexAsync(It.IsAny<PermissionESTypeDto>(), "permission-index", It.IsAny<CancellationToken>()), Times.Once);
    }
}