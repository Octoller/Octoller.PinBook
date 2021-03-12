using Microsoft.AspNetCore.Identity;
using Octoller.PinBook.Web.Data.Model;
using Octoller.PinBook.Web.Data.Stores;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using VkNet;
using VkNet.Abstractions;
using VkNet.Enums.Filters;
using VkNet.Model;

namespace Octoller.PinBook.Web.Kernel.Services
{
    public class VkontakteApiService
    {
        private readonly IVkApi vkApi;
        private readonly AccountStore accountStore;
        private readonly UserManager<Data.Model.User> userManager;

        public VkontakteApiService(
            UserManager<Data.Model.User> manager,
            AccountStore store)
        {
            vkApi = new VkApi();
            accountStore = store;
            userManager = manager;
        }

        public async Task<Account> FindAccounByUserIdAsync(string userId)
        {
            var vkAccount = await accountStore.GetByUserIdAsync(userId);

            if (vkAccount is null)
            {
                return default(Account);
            }

            return vkAccount;
        }

        public async Task<IdentityResult> CreateVkAccountAsync(string userId, string email, ExternalLoginInfo loginInfo)
        {
            if ((await FindAccounByUserIdAsync(userId))?.Id != null)
            {
                return IdentityResult.Success;
            }

            var token = loginInfo.AuthenticationTokens.
                    Where(at => at.Name.Equals("access_token"))
                    ?.FirstOrDefault();

            var vkId = long.Parse(loginInfo.ProviderKey);

            try
            {
                await vkApi.AuthorizeAsync(new ApiAuthParams
                {
                    AccessToken = token.Value,
                    UserId = vkId
                });

            } 
            catch (Exception ex)
            {
                return FailedResult(ex.Message);
            }

            var vkUser = (await vkApi.Users.GetAsync(
                    userIds: new[] { vkId },
                    fields: ProfileFields.Photo100))
                .FirstOrDefault();

            var account = new Account
            {
                VkId = vkId.ToString(),
                Name = vkUser.FirstName + " " + vkUser.LastName ?? " ",
                AccessToken = token.Value,
                Photo = await GetFileByteArray(vkUser.Photo100.AbsoluteUri),
                UserId = userId
            };

            var initiatorName = GetUserName(userId);

            var resultCreateAccount = await accountStore.CreateAsync(account, initiatorName);

            if (resultCreateAccount)
            {
                return IdentityResult.Success;
            }

            return FailedResult("Ошибка записи");
        }

        private static IdentityResult FailedResult(string description)
        {
            return IdentityResult.Failed(new[]
            {
                new IdentityError
                {
                    Code = "",
                    Description = description
                }
            });
        }

        private static async Task<byte[]> GetFileByteArray(string url)
        {
            using HttpContent content = (await new HttpClient().GetAsync(url)).Content;
            return await content.ReadAsByteArrayAsync();
        }

        private string GetUserName(string id) => userManager.FindByIdAsync(id).Result.UserName;

    }
}
