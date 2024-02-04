using AutoMapper;
using PermissionApp.Domain;
using PermissionApp.Models;

namespace PermissionApp;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        CreateMap<Permission, PermissionDto>()
            .ForMember(x => x.Id, y => y.MapFrom(z => z.PermissionType.Id))
            .ForMember(x => x.Name, y => y.MapFrom(z => z.PermissionType.Name))
            .ForMember(x => x.Status, y => y.MapFrom(z => z.Status));


        CreateMap<Employee, PermissionsDto>()
            .ForMember(x => x.Employee, y => y.MapFrom(z => z))
            .ForMember(x => x.Permissions, y => y.MapFrom(z => z.Permissions));
        
        CreateMap<Employee, EmployeeDto>();

        CreateMap<string, Status>();
    }
}