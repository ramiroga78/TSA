using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using TSA.Infrastructure.DTOs;

namespace TSA.Infrastructure.Interfaces
{
    public interface IDeltaService
    {
        //DELTA
        public DeltaDTO CreateDeltaDTO();
        public Task<IEnumerable<DeltaDTO>> GetAllDeltas();
        public Task<DeltaDTO> GetAllDeltaByIdAndModelToDto(int id);
        public Task AddAndSave(HttpContext httpContext, DeltaDTO delta);
        public Task UpdateAndSave(HttpContext httpContext, DeltaDTO delta);
        //DELTATYPE
        public Task<DeltaTypeDTO> GetDeltaType(int typeId);
        //DELTAUSER
        public DeltaUserDTO CreateDeltaUserDTO();
        public Task<IEnumerable<DeltaUserDTO>> GetAllUsersByDeltaIdAndModelToDto(int deltaId);
        public Task<IEnumerable<DeltaUserDTO>> GetAllUsersAndModelToDto(int alertId);
        public Task<DeltaUserDTO> GetUserAndModelToDeltaUserDtoById(int id);
        public Task CreateDeltaUser(HttpContext httpContext, DeltaUserDTO deltaUserDTO);
        public Task UpdateAndSaveDeltaUser(HttpContext httpContext, DeltaUserDTO deltaUserDTO);
        public Task DeleteDeltaUser(HttpContext httpContext, int id);
      
    }
}
