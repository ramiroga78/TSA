using AutoMapper;
using TSA.Infrastructure.DTOs;
using TSA.Infrastructure.Entities;
using TSALibrary.Models;

namespace TSA.Infrastructure.Mappings
{
    public class AutoMapperProfile : AutoMapper.Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, UserDTO>().ReverseMap();
            CreateMap<ProfileType, ProfileTypeDTO>().ReverseMap();
            CreateMap<ProfileType, ProfileTypesHistory>().ReverseMap();
            CreateMap<ProfileValue, ProfileValueDTO>().ReverseMap();
            CreateMap<ProfileValue, ProfileValueHistory>().ReverseMap();
            CreateMap<ExternalUser, ExternalUserDTO>().ReverseMap();

            CreateMap<IpAddress, IpAddressDTO>().ReverseMap();
            
            CreateMap<IpAddress, IpAddressesHistory>().ReverseMap();
         

            CreateMap<ExternalUserHistory, ExternalUser>().ReverseMap();
            CreateMap<UserHistory, User>().ReverseMap();
            CreateMap<Role, RoleDTO>().ReverseMap();
           
            CreateMap<NTPServer, NTPServerDTO>().ReverseMap();
            //CreateMap<NTPServerDTO, NTPServer>().ReverseMap();
            //CreateMap<NTPServerHistory, NTPServerDTO>().ReverseMap();
            //CreateMap<NTPServerHistoryDTO, NTPServer>().ReverseMap();
            CreateMap<NTPServerHistory, NTPServer>().ReverseMap();
            
            CreateMap<RoleUser, RoleUserHistory>().ReverseMap();
            CreateMap<RolesHistory, Role>().ReverseMap();
            CreateMap<CertificateOrganizationDTO, CertificateOrganization>().ReverseMap();
            CreateMap<Certificate, CertificateDTO>().ReverseMap();

            CreateMap<RoleFunction, RoleFunctionDTO>().ReverseMap();
            CreateMap<RoleFunction, RoleFunctionHistory>().ReverseMap();
            CreateMap<PoliciesType, PolicyTypeDTO>().ReverseMap();
            CreateMap<Certificate, CertificatesHistory>().ReverseMap();
            CreateMap<Alert, AlertDTO>().ReverseMap();

            CreateMap<Delta, DeltaDTO>().ReverseMap();


            //Yo can separate both instead of using .ReverseMap() and edit them separately, for example
            //CreateMap<User, UserDTO>()
            //CreateMap<UserDTO, User>()
            //    .ForMember(dest => dest.Id, opt => opt.Ignore())

        }
    }
}
