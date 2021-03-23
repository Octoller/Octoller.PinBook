using Microsoft.AspNetCore.Identity;
using Octoller.PinBook.Web.Data.Model;
using System.Linq;
using System.Threading.Tasks;

namespace Octoller.PinBook.Web.Kernel.Services
{
    public class AccountManager
    {
        private IUserValidator<User>  UserValidator { get; }
        private IPasswordValidator<User> PasswordValidator { get; }
        private IPasswordHasher<User> PasswordHasher { get; }
        private UserManager<User> UserManager { get; }

        public AccountManager(
            IUserValidator<User> userValidator,
            IPasswordValidator<User> passwordValidator,
            IPasswordHasher<User> passwordHasher,
            UserManager<User> userManager)
        {
            UserValidator = userValidator;
            PasswordValidator = passwordValidator;
            PasswordHasher = passwordHasher;
            UserManager = userManager;
        }

        public async Task<IdentityResult> CreateAccount(string email, string password)
        {
            var user = new User
            {
                UserName = email,
                Email = email
            };

            var resultCreate = await UserManager.CreateAsync(user, password);
            if (resultCreate.Succeeded)
            {
                user = await UserManager.FindByEmailAsync(user.Email);
                if (!await UserManager.IsInRoleAsync(user, AppData.RolesData.UserRoleName))
                {
                    _ = await UserManager.AddToRoleAsync(user, AppData.RolesData.UserRoleName);
                }

                return IdentityResult.Success;
            }

            return IdentityResult.Failed(resultCreate.Errors.ToArray());
        }

        public async Task<IdentityResult> CreateAccount(string email)
        {
            var user = new User
            {
                UserName = email,
                Email = email
            };

            var resultCreate = await UserManager.CreateAsync(user);
            if (resultCreate.Succeeded)
            {
                user = await UserManager.FindByEmailAsync(user.Email);
                if (!await UserManager.IsInRoleAsync(user, AppData.RolesData.UserRoleName))
                {
                    _ = await UserManager.AddToRoleAsync(user, AppData.RolesData.UserRoleName);
                }

                return IdentityResult.Success;
            }

            return IdentityResult.Failed(resultCreate.Errors.ToArray());
        }
    }
}
