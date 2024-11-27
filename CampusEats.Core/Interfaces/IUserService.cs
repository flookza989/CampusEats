using CampusEats.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusEats.Core.Interfaces
{
    public interface IUserService
    {
        Task DeleteUserAsync(int userId);
        Task<bool> CanDeleteUserAsync(int userId);
        Task ResetPasswordAsync(int userId, string newPassword);
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task ToggleUserStatusAsync(int userId);
        Task<User> RegisterUserAsync(User user, string password);
        Task<User> AuthenticateAsync(string username, string password);
        Task<User> GetUserProfileAsync(int userId);
        Task UpdateUserProfileAsync(User user);
        Task ChangePasswordAsync(int userId, string currentPassword, string newPassword);
        Task<IEnumerable<Order>> GetUserOrderHistoryAsync(int userId);
        Task<bool> IsEmailUniqueAsync(string email);
        Task<bool> IsUsernameUniqueAsync(string username);
    }
}
