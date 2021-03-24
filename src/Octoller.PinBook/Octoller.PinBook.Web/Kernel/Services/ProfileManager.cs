using Microsoft.AspNetCore.Identity;
using Octoller.PinBook.Web.Data.Model;
using Octoller.PinBook.Web.Data.Stores;
using System.Linq;
using System.Threading.Tasks;

namespace Octoller.PinBook.Web.Kernel.Services
{
    public class ProfileManager
    {
        private ProfileStore ProfileStore { get; }

        public ProfileManager(ProfileStore profileStore)
        {
            ProfileStore = profileStore;
        }

        /// <summary>
        /// Создает профиль информации о пользователе.
        /// </summary>
        /// <param name="targetUser">Пользователь, для которого создается профиль.</param>
        /// <param name="newProfile">Профиль.</param>
        /// <returns><see cref="IdentityResult"/>.</returns>
        public async Task<IdentityResult> CreateProfileAsync(User targetUser, Profile newProfile)
        {
            if (targetUser is null)
            {
                return FailedResult("Пользователь не указан.");
            }

            if (newProfile is null)
            {
                return FailedResult("Профиль не указан.");
            }

            var profile = await ProfileStore.GetByUserIdAsync(targetUser.Id);

            if (profile is null)
            {
                newProfile.Name = string.IsNullOrEmpty(newProfile.Name) 
                    ? targetUser.UserName 
                    : newProfile.Name;

                newProfile.UserId = targetUser.Id;

                var result = await ProfileStore.CreateAsync(newProfile, targetUser.UserName);

                if (!result)
                {
                    return FailedResult("Ошибка записи");
                }

                return IdentityResult.Success;
            }

            return FailedResult("Профиль уже создан.");
        }

        /// <summary>
        /// Создает профиль информации о пользователе.
        /// </summary>
        /// <param name="targetUser">Пользователь, для которого создается профиль.</param>
        /// <returns><see cref="IdentityResult"/>.</returns>
        public async Task<IdentityResult> CreateProfileAsync(User targetUser)
        {
            if (targetUser is null)
            {
                return FailedResult("Пользователь не указан.");
            }

            return await CreateProfileAsync(targetUser, new Profile());
        }

        public async Task<IdentityResult> UpdateProfileAsync(User targetUser, Profile modifiedProfile)
        {
            if (targetUser is null)
            {
                return FailedResult("Пользователь не указан.");
            }

            if (modifiedProfile is null)
            {
                return FailedResult("Профиль не указан.");
            }

            var currentProfile = await ProfileStore.GetByUserIdAsync(targetUser.Id);

            if (currentProfile is not null)
            {
                var validateResult = await ValidateProfile(modifiedProfile);

                if (validateResult.Succeeded)
                {
                    currentProfile.Name = modifiedProfile.Name;
                    currentProfile.About = modifiedProfile.About;
                    currentProfile.Location = modifiedProfile.Location;
                    currentProfile.Site = modifiedProfile.Site;

                    var updateResult = await ProfileStore.UpdateAsync(currentProfile);

                    if (updateResult)
                    {
                        return IdentityResult.Success;
                    } 
                    else
                    {
                        FailedResult("Внутренняя ошибка.");
                    }
                }

                return IdentityResult.Failed(validateResult.Errors.ToArray());
            }

            return FailedResult("Профиль не найден.");
        }

        /// <summary>
        /// Возвращает объект с информацией профиля пользователя по Id.
        /// </summary>
        /// <param name="id">Id профиля.</param>
        /// <returns>Объект <see cref="Profile"/>.</returns>
        public async Task<Profile> FindProfileByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return default;
            }

            return await ProfileStore.GetByIdAsync(id);
        }

        /// <summary>
        /// Возвращает объект с информацией профиля пользователя.
        /// </summary>
        /// <param name="user">Данные пользователя, профиль которого требуется вернуть.</param>
        /// <returns>Объект <see cref="Profile"/>.</returns>
        public async Task<Profile> FindProfileByUserAsync(User user)
        {
            if (user is null)
            {
                return default;
            }

            return await ProfileStore.GetByUserIdAsync(user.Id);
        }

        private async Task<IdentityResult> ValidateProfile(Profile profile)
        {
            //TODO: метод для валидации входных данных профиля

            return await Task.FromResult(IdentityResult.Success);
        }

        private IdentityResult FailedResult(string description)
        {
            return IdentityResult.Failed(new IdentityError
            {
                Code = "",
                Description = description
            });
        }
    }
}
