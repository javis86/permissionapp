using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using PermissionApp.Commands;
using PermissionApp.Models;

namespace PermissionApp.Tests.IntegrationTests;

[TestClass]
public class IntegrationTests
{
    [TestMethod]
    public async Task GetPermissions_ReturnListOfPermissions()
    {
        
        await using var application = new WebApplicationFactory<Program>();
        
        var client = application.CreateClient();
    
        var result = await client.PostAsJsonAsync("/GetPermissions", new GetPermissionsQuery
        {
            EmployeeId = new Guid("32388767-97d2-48c0-7ab2-08dc2bd9c05d")
        });
        
        var content = await result.Content.ReadAsStringAsync();
        var dto = JsonSerializer.Deserialize<PermissionsDto>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        dto.Permissions.Count.Should().Be(1);
    }
    
    [TestMethod]
    public async Task RequestPermission_ReturnsTrue()
    {
        
        await using var application = new WebApplicationFactory<Program>();
        
        var client = application.CreateClient();
    
        var result = await client.PostAsJsonAsync("/RequestPermission", new RequestPermissionCommand()
        {
            EmployeeId = new Guid("32388767-97d2-48c0-7ab2-08dc2bd9c05d"),
            PermissionTypeId = new Guid("106c5c61-70ef-42f8-8a8e-1c689a4d73f5")
        });
        
        var content = await result.Content.ReadAsStringAsync();
        var response = JsonSerializer.Deserialize<bool>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Should().Be(true);
    }
    
    [TestMethod]
    public async Task ModifyPermission_ReturnsTrue()
    {
        await using var application = new WebApplicationFactory<Program>();
        
        var client = application.CreateClient();
        
        await client.PostAsJsonAsync("/RequestPermission", new RequestPermissionCommand()
        {
            EmployeeId = new Guid("32388767-97d2-48c0-7ab2-08dc2bd9c05d"),
            PermissionTypeId = new Guid("106c5c61-70ef-42f8-8a8e-1c689a4d73f5")
        });
    
        var result = await client.PostAsJsonAsync("/ModifyPermission", new ModifyPermissionCommand()
        {
            EmployeeId = new Guid("32388767-97d2-48c0-7ab2-08dc2bd9c05d"),
            PermissionTypeId = new Guid("106c5c61-70ef-42f8-8a8e-1c689a4d73f5"),
            PermissionCommandType = PermissionCommandType.Grant
        });
        
        var content = await result.Content.ReadAsStringAsync();
        var response = JsonSerializer.Deserialize<bool>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Should().Be(true);
    }
}