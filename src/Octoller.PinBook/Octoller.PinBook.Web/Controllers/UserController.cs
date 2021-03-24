using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Octoller.PinBook.Web.Data.Model;
using Octoller.PinBook.Web.Kernel.Services;
using Octoller.PinBook.Web.ViewModels.Profiles;
using System.Linq;
using System.Threading.Tasks;

namespace Octoller.PinBook.Web.Controllers
{
    public class UserController : Controller
    {
        private ProfileManager ProfileManager { get; }
        private UserManager<User> UserManager { get; }
        private SignInManager<User> SignInManager { get; }
        private AccountManager AccountManager { get; }

        public UserController(
            ProfileManager profileManager,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            AccountManager accountManager)
        {
            ProfileManager = profileManager;
            UserManager = userManager;
            SignInManager = signInManager;
            AccountManager = accountManager;
        }

        [Authorize(Policy = "Users")]
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var user = await UserManager.FindByNameAsync(User.Identity.Name);

            if (user is not null)
            {
                var profile = await ProfileManager.FindProfileByUserAsync(user);
                if (profile is not null)
                {
                    return View(new ProfileViewModel
                    {
                        Name = profile.Name,
                        About = profile.About,
                        Location = profile.Location,
                        Site = profile.Site
                    });
                }
            }

            return View(new ProfileViewModel());
        }

        [HttpPost]
        [Authorize(Policy = "Users")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(ProfileViewModel profileModel)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(User.Identity.Name);

                if (user is not null)
                {
                    var profile = new Profile
                    {
                        Name = profileModel.Name,
                        Site = profileModel.Site,
                        Location = profileModel.Location,
                        About = profileModel.About
                    };

                    var updateResult = await ProfileManager.UpdateProfileAsync(user, profile);
                    if (updateResult.Succeeded)
                    {
                        return View(profileModel);
                    }
                    else
                    {
                        foreach(var e in updateResult.Errors)
                        {
                            ModelState.AddModelError("", e.Description);
                        }

                        return View(profile);
                    }
                }
            }

            return View(profileModel);
        }

        [HttpGet]
        [Authorize(Policy = "Users")]
        public async Task<IActionResult> Account()
        {
            var user = await UserManager.FindByNameAsync(User.Identity.Name);
            if (user is not null)
            {
                var profile = await ProfileManager.FindProfileByUserAsync(user); 
                if (profile is not null)
                {
                    var vk = await IsExternalAuthSchem("VKontakte");

                    return View(new AccountViewModel
                    {
                        Name = profile.Name,
                        Email = user.Email,
                        VkAccount = vk
                    });
                }
            }

            return View(new AccountViewModel());
        }

        [HttpPost]
        [Authorize(Policy = "Users")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Account(AccountViewModel accountModel)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(User.Identity.Name);
                if (user is not null)
                {
                    if (await UserManager.CheckPasswordAsync(user, accountModel.CurrentPassword))
                    {
                        var profile = await ProfileManager.FindProfileByUserAsync(user);
                        if (profile is not null)
                        {
                            var vk = await IsExternalAuthSchem("VKontakte");

                            var updateResult = await AccountManager.UpdateAccount(user.Id, accountModel.Email, accountModel.Password);
                            if (updateResult.Succeeded)
                            {
                                var newUser = await UserManager.FindByNameAsync(User.Identity.Name);

                                return View(new AccountViewModel
                                {
                                    Name = profile.Name,
                                    Email = newUser.Email,
                                    VkAccount = vk
                                });
                            } else
                            {
                                foreach (var e in updateResult.Errors)
                                {
                                    ModelState.AddModelError("", e.Description);
                                }

                                return View(new AccountViewModel
                                {
                                    Name = profile.Name,
                                    Email = user.Email,
                                    VkAccount = vk
                                });
                            }
                        }
                    } 
                    else
                    {
                        ModelState.AddModelError("", "Неверно указан текущий пароль.");
                    }
                } 
                else
                {
                    ModelState.AddModelError("", "Пользователь не найден.");
                }
            }

            return View(accountModel);
        }

        private async Task<bool> IsExternalAuthSchem(string schemeName) =>
            (await SignInManager.GetExternalAuthenticationSchemesAsync())
                        .Any(s => s.Name == schemeName);
    }
}
