using CampusEats.Core.Entities;
using CampusEats.Core.Enums;
using CampusEats.Core.Exceptions;
using CampusEats.Core.Interfaces;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CampusEats.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> CanDeleteUserAsync(int userId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
                throw new UserNotFoundException(userId.ToString());

            if (user.UserType == UserType.Staff)
                return false;

            if (user.UserType == UserType.RestaurantOwner)
            {
                var restaurant = await _unitOfWork.Restaurants.GetByOwnerIdAsync(userId);
                if (restaurant != null)
                {
                    var hasOrders = (await _unitOfWork.Orders.GetOrdersByUserAsync(userId)).Any();
                    if (hasOrders)
                        return false;
                }
            }

            if (user.UserType == UserType.Student)
            {
                var pendingOrders = (await _unitOfWork.Orders.GetOrdersByUserAsync(userId))
                    .Any(o => o.Status != OrderStatus.Completed && o.Status != OrderStatus.Cancelled);
                if (pendingOrders)
                    return false;
            }

            return true;
        }

        public async Task DeleteUserAsync(int userId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
                throw new UserNotFoundException(userId.ToString());

            if (!await CanDeleteUserAsync(userId))
                throw new ValidationException("User", userId, "Cannot delete this user due to existing dependencies or role restrictions.");

            try
            {
                var notifications = await _unitOfWork.Notifications.GetUnreadNotificationsAsync(userId);
                foreach (var notification in notifications)
                {
                    await _unitOfWork.Notifications.DeleteAsync(notification.NotificationId);
                }

                await _unitOfWork.Users.DeleteAsync(userId);
                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Delete", "User", ex);
            }
        }
        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            try
            {
                return await _unitOfWork.Users.GetAllAsync();
            }
            catch (Exception ex)
            {
                throw new DataAccessException("GetAll", "User", ex);
            }
        }

        public async Task ToggleUserStatusAsync(int userId)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null)
                    throw new UserNotFoundException(userId.ToString());

                if (user.UserType == UserType.Staff && user.IsActive)
                {
                    var activeAdminCount = (await GetAllUsersAsync())
                        .Count(u => u.UserType == UserType.Staff && u.IsActive);

                    if (activeAdminCount <= 1)
                    {
                        throw new ValidationException(
                            nameof(user.IsActive),
                            false,
                            "Cannot deactivate the last active admin user.");
                    }
                }

                user.IsActive = !user.IsActive;
                user.UpdatedAt = DateTime.UtcNow;

                await _unitOfWork.Users.UpdateAsync(user);
                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception ex) when (!(ex is UserNotFoundException || ex is ValidationException))
            {
                throw new DataAccessException("Update", "User", ex);
            }
        }

        public async Task<User> RegisterUserAsync(User user, string password)
        {
            if (await _unitOfWork.Users.EmailExistsAsync(user.Email))
                throw new DuplicateUserException("email", user.Email);

            if (await _unitOfWork.Users.UsernameExistsAsync(user.Username))
                throw new DuplicateUserException("username", user.Username);

            user.PasswordHash = HashPassword(password);
            user.CreatedAt = DateTime.UtcNow;

            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return user;
        }

        public async Task<User> AuthenticateAsync(string username, string password)
        {
            var user = await _unitOfWork.Users.GetByUsernameAsync(username);
            if (user == null)
                throw new AuthenticationException("Invalid username or password");

            if (!VerifyPassword(password, user.PasswordHash))
                throw new AuthenticationException("Invalid username or password");

            return user;
        }

        public async Task<User> GetUserProfileAsync(int userId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
                throw new UserNotFoundException(userId.ToString());

            return user;
        }

        public async Task UpdateUserProfileAsync(User user)
        {
            var existingUser = await _unitOfWork.Users.GetByIdAsync(user.UserId);
            if (existingUser == null)
                throw new UserNotFoundException(user.UserId.ToString());

            // Check if email is being changed and if it's already in use
            if (existingUser.Email != user.Email && await _unitOfWork.Users.EmailExistsAsync(user.Email))
                throw new DuplicateUserException("email", user.Email);

            existingUser.FullName = user.FullName;
            existingUser.Email = user.Email;
            existingUser.Phone = user.Phone;
            existingUser.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Users.UpdateAsync(existingUser);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task ResetPasswordAsync(int userId, string newPassword)
        {
            if (string.IsNullOrEmpty(newPassword))
                throw new ValidationException("Password", newPassword, "New password is required.");

            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
                throw new UserNotFoundException(userId.ToString());

            user.PasswordHash = HashPassword(newPassword);
            user.UpdatedAt = DateTime.UtcNow;

            try
            {
                await _unitOfWork.Users.UpdateAsync(user);
                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Update", "User", ex);
            }
        }

        public async Task ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            if (string.IsNullOrEmpty(currentPassword))
                throw new AuthenticationException("Current password is required.");

            if (string.IsNullOrEmpty(newPassword))
                throw new AuthenticationException("New password is required.");

            if (newPassword.Length < 6)
                throw new AuthenticationException("New password must be at least 6 characters long.");

            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
                throw new UserNotFoundException(userId.ToString());

            if (!VerifyPassword(currentPassword, user.PasswordHash))
                throw new AuthenticationException("Current password is incorrect.");

            user.PasswordHash = HashPassword(newPassword);
            user.UpdatedAt = DateTime.UtcNow;

            try
            {
                await _unitOfWork.Users.UpdateAsync(user);
                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Update", "User", ex);
            }
        }

        public async Task<IEnumerable<Order>> GetUserOrderHistoryAsync(int userId)
        {
            if (!await UserExistsAsync(userId))
                throw new UserNotFoundException(userId.ToString());

            return await _unitOfWork.Users.GetUserOrdersAsync(userId);
        }

        public async Task<bool> IsEmailUniqueAsync(string email)
        {
            return !await _unitOfWork.Users.EmailExistsAsync(email);
        }

        public async Task<bool> IsUsernameUniqueAsync(string username)
        {
            return !await _unitOfWork.Users.UsernameExistsAsync(username);
        }

        private async Task<bool> UserExistsAsync(int userId)
        {
            return await _unitOfWork.Users.GetByIdAsync(userId) != null;
        }

        private string HashPassword(string password)
        {
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            return $"{Convert.ToBase64String(salt)}.{hashed}";
        }

        private bool VerifyPassword(string password, string storedHash)
        {
            var parts = storedHash.Split('.');
            if (parts.Length != 2)
                return false;

            var salt = Convert.FromBase64String(parts[0]);
            var hash = parts[1];

            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            return hash == hashed;
        }
    }
}
