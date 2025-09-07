using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SocialMedia.Data;
using SocialMedia.Helpers;
using SocialMedia.Models.DTOs.Common;
using SocialMedia.Models.DTOs.Requists;
using SocialMedia.Models.DTOs.UserrManagment;
using SocialMedia.Models.Entities;

namespace SocialMedia.Services.Auth
{
    public interface IAuthServices
    {
        Task<LoginResponseDTO?> Login(LoginDTO form);
        Task<ApiResponse<LoginResponseDTO>> Register(ProfileCreateDTO form);
        Task<ApiResponse<LoginResponseDTO>> UpdarePassword(UpdatePasswordDTO form);
    }
    public class AuthServices : IAuthServices
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly AppData _context;

        private readonly IConfiguration _configuration;

        public AuthServices(UserManager<User> u,
            SignInManager<User> s, IConfiguration configuration,
            RoleManager<Role> role,
            AppData context)
        {
            _userManager = u;
            _signInManager = s;
            _configuration = configuration;
            _roleManager = role;
            _context = context;
        }

        public async Task<LoginResponseDTO?> Login(LoginDTO form)
        {
            var user = await _userManager.Users
                .FirstOrDefaultAsync(u => u.UserName == form.Username);

            if (user == null) return null;

            var result = await _signInManager.CheckPasswordSignInAsync(user, form.Password, false);
            if (!result.Succeeded)
                return new LoginResponseDTO { Token = "INVALID_PASSWORD" };

            var roles = await _userManager
                .GetRolesAsync(user);
            string userRole = roles.FirstOrDefault();

            var secretKey = _configuration["Jwt:SecretKey"];
            var token = JwtToken.GenToken(user.Id, userRole, "moys.iq", 30, secretKey);
            var roleEntity = await _context.Roles
        .FirstOrDefaultAsync(r => r.Name == userRole);

            var profile = await _context.Profiles
                .FirstOrDefaultAsync(p => p.UserId == user.Id);

            return new LoginResponseDTO
            {
                Token = token,
                Id = user.Id,
                Username = user.UserName,
                Role = userRole,
                FullName = profile.FullName,
                profileId = profile.Id

            };
        }

        public async Task<ApiResponse<LoginResponseDTO>> Register(ProfileCreateDTO form)
        {
            var existingUser = await _userManager.FindByNameAsync(form.Username);
            if (existingUser != null)
                return ApiResponse<LoginResponseDTO>.Fail("Username already exists", 400);
            var user = new User
            {
                UserName = form.Username,
            };
            var createUserResult = await _userManager.CreateAsync(user, form.Password);
            if (!createUserResult.Succeeded)
            {
                var errors = string.Join(", ", createUserResult.Errors.Select(e => e.Description));
                return ApiResponse<LoginResponseDTO>.Fail($"User creation failed: {errors}", 400);
            }
            var roleExists = await _roleManager.RoleExistsAsync("User");
            if (!roleExists)
            {
                var roleResult = await _roleManager.CreateAsync(new Role { Name = "User" });
                if (!roleResult.Succeeded)
                {
                    var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                    return ApiResponse<LoginResponseDTO>.Fail($"Role creation failed: {errors}", 500);
                }
            }
            await _userManager.AddToRoleAsync(user, "User");
            var profile = new Profile
            {
                FullName = form.Name,
                Bio = form.Bio,
                UserId = user.Id
            };
            _context.Profiles.Add(profile);
            await _context.SaveChangesAsync();
            var loginResponse = await Login(new LoginDTO
            {
                Username = form.Username,
                Password = form.Password
            });
            if (loginResponse == null || loginResponse.Token == "INVALID_PASSWORD")
                return ApiResponse<LoginResponseDTO>.Fail("Registration succeeded but login failed.", 500);
            return ApiResponse<LoginResponseDTO>.Success(loginResponse, "Registration and login successful", 201);

        }

        public async Task<ApiResponse<LoginResponseDTO>> UpdarePassword(UpdatePasswordDTO form)
        {
            var userId = _signInManager.Context.User?.FindFirst("id")?.Value;
            if (userId == null)
                return ApiResponse<LoginResponseDTO>.Fail("User not authenticated", 401);
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return ApiResponse<LoginResponseDTO>.Fail("User not found", 404);
            var passwordCheck = await _signInManager.CheckPasswordSignInAsync(user, form.CurrentPassword, false);
            if (!passwordCheck.Succeeded)
                return ApiResponse<LoginResponseDTO>.Fail("Current password is incorrect", 400);
            var passwordChangeResult = await _userManager.ChangePasswordAsync(user, form.CurrentPassword, form.NewPassword);
            if (!passwordChangeResult.Succeeded)
            {
                var errors = string.Join(", ", passwordChangeResult.Errors.Select(e => e.Description));
                return ApiResponse<LoginResponseDTO>.Fail($"Password change failed: {errors}", 400);
            }
            // Re-authenticate and generate new token
            var roles = await _userManager.GetRolesAsync(user);
            string userRole = roles.FirstOrDefault();
            var secretKey = _configuration["Jwt:SecretKey"];
            var token = JwtToken.GenToken(user.Id, userRole, "moys.iq", 30, secretKey);
            var profile = await _context.Profiles.FirstOrDefaultAsync(p => p.UserId == user.Id);
            var loginResponse = new LoginResponseDTO
            {
                Token = token,
                Id = user.Id,
                Username = user.UserName,
                Role = userRole,
                FullName = profile.FullName,
                profileId = profile.Id
            };
            return ApiResponse<LoginResponseDTO>.Success(loginResponse, "Password updated successfully", 200);
        }
    }
}
