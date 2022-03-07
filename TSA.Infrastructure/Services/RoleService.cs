using AutoMapper;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Mvc;
using TSA.Infrastructure.DTOs;
using TSA.Infrastructure.Interfaces;
using TSALibrary.Models;
using Microsoft.AspNetCore.Http;

namespace TSA.Infrastructure.Services
{
    public class RoleService : BaseService, IRoleService
    {
        public RoleService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        public async Task AddAndSave(HttpContext httpContext, RoleDTO roleDTO)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var role = _mapper.Map<Role>(roleDTO);
                role.AddUserId = Convert.ToInt32(httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
                role.IsActive = true;
                role.AddDate = DateTime.Now;
                await _unitOfWork.RoleRepository.Insert(role);

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();
            }
            catch (System.Exception)
            {
                await _unitOfWork.RollBackAsync();
                throw;
            }
        }
        public RoleDTO CreateRoleDto()
        {
            try
            {
                RoleDTO roleDTO = new RoleDTO();
                return roleDTO;
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<RoleDTO>> GetAllRoles()
        {
            try
            {
                var roles = await _unitOfWork.RoleRepository.GetAllAsync();
                var rolesDto = _mapper.Map<IEnumerable<RoleDTO>>(roles);
                return rolesDto;
            }
            catch (System.Exception)
            {
                throw;
            }
        }


        public async Task<RoleDTO> GetRoleByIdAndModelToDto(int id)
        {
            try
            {
                var role = await _unitOfWork.RoleRepository.GetRoleById(id);
                var roleDto = _mapper.Map<RoleDTO>(role);

                return roleDto;
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public Task<bool> RoleExists(RoleDTO user)
        {
            throw new NotImplementedException();
        }

        public async Task SoftDeleteAndSave(HttpContext httpContext, int id)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var role = await _unitOfWork.RoleRepository.GetRoleById(id);
                //Se guardan cambios en Histórico
                RolesHistory roleHistory = new RolesHistory();
                roleHistory.IdHistory = role.Id;
                roleHistory.Name = role.Name;
                roleHistory.AddUserId = role.AddUserId;
                roleHistory.EditUserId = role.EditUserId;
                roleHistory.DeleteUserId = role.DeleteUserId;
                roleHistory.AddDate = role.AddDate;
                roleHistory.EditDate = role.EditDate;
                roleHistory.DeleteDate = role.DeleteDate;
                roleHistory.IsActive = (bool)role.IsActive;
                await _unitOfWork.RoleHistoryRepository.Insert(roleHistory);
                //Se guardan cambios en Role
                role.IsActive = false;
                role.DeleteDate = System.DateTime.Now;
                role.DeleteUserId = Convert.ToInt32(httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
                _unitOfWork.RoleRepository.Update(role);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();
            }
            catch (System.Exception)
            {
                await _unitOfWork.RollBackAsync();
                throw;
            }
        }

        public async Task UpdateAndSave(HttpContext httpContext, RoleDTO roleDto)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var role = await _unitOfWork.RoleRepository.GetRoleById(roleDto.Id);
                //Se guardan cambios en Histórico
                RolesHistory roleHistory = new RolesHistory();
                roleHistory.IdHistory = role.Id;
                roleHistory.Name = role.Name;
                roleHistory.AddUserId = role.AddUserId;
                roleHistory.EditUserId = role.EditUserId;
                roleHistory.DeleteUserId = role.DeleteUserId;
                roleHistory.AddDate = role.AddDate;
                roleHistory.EditDate = role.EditDate;
                roleHistory.DeleteDate = role.DeleteDate;
                roleHistory.IsActive = (bool)role.IsActive;
                await _unitOfWork.RoleHistoryRepository.Insert(roleHistory);
                //Se guardan cambios en Role
                role.Name = roleDto.Name;
                role.EditUserId = Convert.ToInt32(httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
                role.EditDate = DateTime.Now;
                _unitOfWork.RoleRepository.Update(role);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();
            }
            catch (System.Exception)
            {
                await _unitOfWork.RollBackAsync();
                throw;
            }
        }
        public IEnumerable<SelectListItem> GetRolesList()
        {
            try
            {
                return _unitOfWork.RoleRepository.GetRolesList();
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public List<RoleFunctionDTO> GetRoleFunctionsByIdAndModelToDto(int id)
        {
            try
            {
                var rolesFunctionsDto = new List<RoleFunctionDTO>();
                var functions = _unitOfWork.FunctionRepository.GetAllFunctions();
                var rolesFunctions = _unitOfWork.RoleFunctionRepository.GetRoleFunctionsByRoleId(id);

                foreach (var function in functions)
                {
                    var roleFunctionDto = new RoleFunctionDTO();

                    roleFunctionDto.FunctionId = function.Id;
                    roleFunctionDto.FunctionName = function.Name;
                    roleFunctionDto.RoleId = id;

                    foreach (var roleFunction in rolesFunctions)
                    {
                        if (roleFunction.FunctionId == function.Id)
                        {
                            roleFunctionDto.Create = roleFunction.Create;
                            roleFunctionDto.Read = roleFunction.Read;
                            roleFunctionDto.Update = roleFunction.Update;
                            roleFunctionDto.Delete = roleFunction.Delete;
                        }
                    }
                    rolesFunctionsDto.Add(roleFunctionDto);
                }
                return rolesFunctionsDto;
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public async Task ModifyRolesFunctionsAndSave(HttpContext httpContext, List<RoleFunctionDTO> roleFunctionsDto)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                foreach (var roleFunctionDto in roleFunctionsDto)
                {
                    var roleFunction = _unitOfWork.RoleFunctionRepository.
                                                        GetRoleFunctionById(roleFunctionDto.RoleId, roleFunctionDto.FunctionId);
                    if (roleFunction != null)
                    {
                        if (roleFunctionDto.Create != roleFunction.Create ||
                            roleFunctionDto.Read != roleFunction.Read ||
                            roleFunctionDto.Update != roleFunction.Update ||
                            roleFunctionDto.Delete != roleFunction.Delete)
                        {
                            await InsertAndSaveRoleFunctionHistory(roleFunction);
                            roleFunction.Create = roleFunctionDto.Create;
                            roleFunction.Read = roleFunctionDto.Read;
                            roleFunction.Update = roleFunctionDto.Update;
                            roleFunction.Delete = roleFunctionDto.Delete;
                            roleFunction.EditDate = DateTime.Now;
                            _unitOfWork.RoleFunctionRepository.Update(roleFunction);
                        }
                    }
                    else
                    {
                        var roleFunctionEntity = new RoleFunction();
                        roleFunctionEntity.RoleId = roleFunctionDto.RoleId;
                        roleFunctionEntity.FunctionId = roleFunctionDto.FunctionId;
                        roleFunctionEntity.Create = roleFunctionDto.Create;
                        roleFunctionEntity.Read = roleFunctionDto.Read;
                        roleFunctionEntity.Update = roleFunctionDto.Update;
                        roleFunctionEntity.Delete = roleFunctionDto.Delete;
                        roleFunctionEntity.AddUserId = Convert.ToInt32(httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
                        roleFunctionEntity.AddDate = DateTime.Now;
                        await _unitOfWork.RoleFunctionRepository.Insert(roleFunctionEntity);
                    }
                }
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();
            }
            catch (System.Exception)
            {
                await _unitOfWork.RollBackAsync();
                throw;
            }
        }

        public async Task InsertAndSaveRoleFunctionHistory(RoleFunction roleFunction)
        {
            try
            {
                RoleFunctionHistory roleFunctionHistory = new RoleFunctionHistory();
                roleFunctionHistory.IdHistory = roleFunction.Id;
                roleFunctionHistory.RoleId = roleFunction.RoleId;
                roleFunctionHistory.FunctionId = roleFunction.FunctionId;
                roleFunctionHistory.AddUserId = roleFunction.AddUserId;
                roleFunctionHistory.Create = roleFunction.Create;
                roleFunctionHistory.Update = roleFunction.Update;
                roleFunctionHistory.Read = roleFunction.Read;
                roleFunctionHistory.Delete = roleFunction.Delete;
                roleFunctionHistory.AddDate = roleFunction.AddDate;
                roleFunctionHistory.EditDate = roleFunction.EditDate;
                await _unitOfWork.RoleFunctionHistoryRepository.Insert(roleFunctionHistory);
            }
            catch (System.Exception)
            {
                throw;
            }
        }
    }
}
