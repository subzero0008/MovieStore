using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MovieStoreMvc.Models.Domain;
using MovieStoreMvc.Models.DTO;
using MovieStoreMvc.Repositories.Abstract;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MovieStoreMvc.Repositories.Implementation
{
    public class UserAuthenticationService : IUserAuthenticationService
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly SignInManager<ApplicationUser> signInManager;

        public UserAuthenticationService(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.signInManager = signInManager;
        }

        public async Task<Status> RegisterAsync(RegistrationModel model)
        {
            var status = new Status();
            var userExists = await userManager.FindByNameAsync(model.Username);
            if (userExists != null)
            {
                status.StatusCode = 0;
                status.Message = "User already exists";
                return status;
            }

            ApplicationUser user = new ApplicationUser()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username,
                Name = string.IsNullOrEmpty(model.Name) ? "Default Name" : model.Name, // Стойност за Name
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
            };



            var result = await userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                status.StatusCode = 0;
                status.Message = "User creation failed";
                return status;
            }

            if (!await roleManager.RoleExistsAsync(model.Role))
                await roleManager.CreateAsync(new IdentityRole(model.Role));

            if (await roleManager.RoleExistsAsync(model.Role))
            {
                await userManager.AddToRoleAsync(user, model.Role);
            }

            status.StatusCode = 1;
            status.Message = "You have registered successfully";
            return status;
        }

        public async Task<Status> LoginAsync(LoginModel model)
        {
            var status = new Status();
            var user = await userManager.FindByNameAsync(model.Username);
            if (user == null)
            {
                status.StatusCode = 0;
                status.Message = "Invalid username";
                return status;
            }

            if (!await userManager.CheckPasswordAsync(user, model.Password))
            {
                status.StatusCode = 0;
                status.Message = "Invalid password";
                return status;
            }

            var signInResult = await signInManager.PasswordSignInAsync(user, model.Password, true, true);
            if (signInResult.Succeeded)
            {
                var userRoles = await userManager.GetRolesAsync(user);
                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                };

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                status.StatusCode = 1;
                status.Message = "Logged in successfully";
            }
            else if (signInResult.IsLockedOut)
            {
                status.StatusCode = 0;
                status.Message = "User is locked out";
            }
            else
            {
                status.StatusCode = 0;
                status.Message = "Error on logging in";
            }

            return status;
        }

        public async Task LogoutAsync()
        {
            await signInManager.SignOutAsync();
        }

        public async Task<Status> UpdateUserRoleAsync(string userId, string newRole)
        {
            var status = new Status();
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                status.StatusCode = 0;
                status.Message = "User not found";
                return status;
            }

            var currentRoles = await userManager.GetRolesAsync(user);

            // Пропускаме премахването на съществуващите роли, ако няма нужда.
            if (!await roleManager.RoleExistsAsync(newRole))
            {
                await roleManager.CreateAsync(new IdentityRole(newRole));
            }

            var result = await userManager.AddToRoleAsync(user, newRole);
            if (!result.Succeeded)
            {
                status.StatusCode = 0;
                status.Message = "Error adding new role.";
                return status;
            }

            status.StatusCode = 1;
            status.Message = "User role updated successfully";
            return status;
        }


        public async Task<Status> DeleteUserAsync(string userId)
        {
            var status = new Status();
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                status.StatusCode = 0;
                status.Message = "User not found";
                return status;
            }

            var result = await userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                status.StatusCode = 1;
                status.Message = "User deleted successfully";
            }
            else
            {
                status.StatusCode = 0;
                status.Message = "Failed to delete user";
            }

            return status;
        }
    }
}
