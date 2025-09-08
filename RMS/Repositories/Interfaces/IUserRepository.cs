using RMS.Models.DTOs.Users;  

namespace RMS.Repositories.Interfaces
{
    public interface IUserRepository : IGenericRepository<RMS.Models.Entities.User>
    {
        Task<UserDto?> GetUserWithRolesAsync(string userId);
    }
}
