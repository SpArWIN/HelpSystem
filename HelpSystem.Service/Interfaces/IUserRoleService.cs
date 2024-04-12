using HelpSystem.Domain.Response;
using HelpSystem.Domain.ViewModel.Product.Role;

namespace HelpSystem.Service.Interfaces
{
    public interface IUserRoleService
    {
        Task<BaseResponse<IEnumerable<RoleViewModel>>> GetAllRoles();
    }
}
