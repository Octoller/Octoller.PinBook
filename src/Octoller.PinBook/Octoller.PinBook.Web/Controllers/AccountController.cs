using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Octoller.PinBook.Web.Data.Model;
using Octoller.PinBook.Web.Kernel;
using Octoller.PinBook.Web.Kernel.Services;
using Octoller.PinBook.Web.ViewModels;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Octoller.PinBook.Web.Controllers
{
    public class AccountController : Controller
    {
        private UserManager<User> UserManager { get; }
        private SignInManager<User> SignInManager { get; }
        private VkontakteApiService VkApiService { get; }

        public AccountController(
            UserManager<User> userManager,
            VkontakteApiService vkontacteApiService,
            SignInManager<User> signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
            VkApiService = vkontacteApiService;
        }


        [HttpGet]
        public async Task<IActionResult> Login(string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction(returnUrl);
            }

            var providers = await SignInManager.GetExternalAuthenticationSchemesAsync();

            return View(new LoginViewModel { 
                ReturnUrl = returnUrl ?? Url.Action("Index", "Home"),
                Providers = providers
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel loginModel)
        {
            if (User.Identity.IsAuthenticated)
            {
                return Redirect(loginModel.ReturnUrl);
            }

            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(loginModel.Email);

                if (user is not null)
                {
                    var signInResult = await SignInManager.PasswordSignInAsync(
                       user: user, 
                       password: loginModel.Password, 
                       isPersistent: loginModel.IsPersistent, 
                       lockoutOnFailure: false);

                    if (signInResult.Succeeded)
                    {
                        return Redirect(loginModel.ReturnUrl);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Пользователь не найден");
                }
            }

            ModelState.AddModelError(string.Empty, "Неверно указан пароль или логин");

            loginModel.Password = string.Empty;

            loginModel.Providers = await SignInManager.GetExternalAuthenticationSchemesAsync();

            return View(loginModel);
        }

        [HttpGet]
        public async Task<IActionResult> Register(string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return Redirect(returnUrl);
            }

            var providers = await SignInManager.GetExternalAuthenticationSchemesAsync();

            return View(new RegisterViewModel
            {
                ReturnUrl = returnUrl ?? Url.Action("Index", "Home"),
                Providers = providers
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel registerModel)
        {
            if (User.Identity.IsAuthenticated)
            {
                return Redirect(registerModel.ReturnUrl);
            }

            if (ModelState.IsValid)
            {
                var user = new User
                {
                    UserName = registerModel.Email,
                    Email = registerModel.Email
                };

                var resultCreate = await UserManager.CreateAsync(user, registerModel.Password);

                if (resultCreate.Succeeded)
                {
                    await SignInManager.SignInAsync(user, false);

                    return Redirect(registerModel.ReturnUrl);
                } 
                else
                {
                    foreach (var error in resultCreate.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            return View(registerModel);
        }

        [HttpGet]
        public IActionResult ExternalLogin(string returnUrl, string providerName)
        {
            returnUrl ??= Url.Action("Index", "Home");
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account", new { returnUrl });
            var properties = SignInManager.ConfigureExternalAuthenticationProperties(providerName, redirectUrl);

            return Challenge(properties, providerName);
        }

        [HttpGet]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await SignInManager.GetExternalLoginInfoAsync();
            
            if (loginInfo is null)
            {
                return RedirectToAction("Login", "Account");
            }

            var signInResult = await SignInManager.ExternalLoginSignInAsync(
               loginProvider: loginInfo.LoginProvider, 
               providerKey: loginInfo.ProviderKey, 
               isPersistent: true, 
               bypassTwoFactor: false);

            if (signInResult.Succeeded)
            {
                return Redirect(returnUrl);
            }
            else
            {
                var userEmail = loginInfo.Principal.Claims
                    .Where(c => c.Type == ClaimTypes.Email)
                    .FirstOrDefault()
                    .Value;

                var user = await UserManager.FindByEmailAsync(userEmail);

                if (user is null)
                {
                    _ = await UserManager.CreateAsync(new User
                    {
                        Email = userEmail,
                        UserName = userEmail
                    });

                    user = await UserManager.FindByEmailAsync(userEmail);
                }

                var createAccountResult = await VkApiService.CreateVkAccountAsync(user.Id, userEmail, loginInfo);

                if (createAccountResult.Succeeded)
                {
                    if (!await UserManager.IsInRoleAsync(user, AppData.RolesData.UserRoleName))
                    {
                        _ = await UserManager.AddToRoleAsync(user, AppData.RolesData.UserRoleName);
                    }

                    var addLoginInfoResult = await UserManager.AddLoginAsync(user, loginInfo);

                    if (addLoginInfoResult.Succeeded)
                    {
                        await SignInManager.SignOutAsync();
                        await SignInManager.SignInAsync(user, true);

                        return Redirect(returnUrl);
                    }
                }
            }

            return RedirectToAction("Login", new LoginViewModel
            {
                ReturnUrl = returnUrl
            });
        }

        [HttpGet]
        public async Task<IActionResult> LogOut()
        {
            await SignInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
