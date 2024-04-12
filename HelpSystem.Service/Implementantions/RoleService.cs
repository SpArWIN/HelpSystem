using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HelpSystem.DAL.Implementantions;
using HelpSystem.DAL.Interfasces;
using HelpSystem.Domain.Entity;
using HelpSystem.Domain.Enum;
using HelpSystem.Domain.Extension;
using HelpSystem.Domain.Response;
using HelpSystem.Domain.ViewModel.Product.Role;
using HelpSystem.Service.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HelpSystem.Service.Implementantions
{
    public class RoleService : IUserRoleService
    {
        private readonly IBaseRepository<Role> _roleRepository;
        public RoleService(IBaseRepository<Role> roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task<BaseResponse<IEnumerable<RoleViewModel>>> GetAllRoles()
        {
            try
            {
                var roles = await _roleRepository.GetAll().ToListAsync();

                var Response = roles.Select(x => new RoleViewModel
                {
                    Id = x.Id,
                    RoleType = x.RoleType,
                    RoleName = x.RoleType.GetDisplayName()
                }).ToList();
                if (Response.Any())
                {
                    return new BaseResponse<IEnumerable<RoleViewModel>>()
                    {
                        Data = Response,
                        StatusCode = StatusCode.Ok
                    };
                }

                return new BaseResponse<IEnumerable<RoleViewModel>>()
                {
                    Description = $"Роли не найдены",
                    StatusCode = StatusCode.NotFind
                };

            }
            catch (Exception ex)
            {
                return new BaseResponse<IEnumerable<RoleViewModel>>()
                {
                    Description = $"{ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }
    }
}
