namespace Octoller.PinBook.Web.ViewModels.Profiles
{
    public class ProfileViewModel
    {
        /// <summary>
        /// Имя пользователя
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Сайт пользователя
        /// </summary>
        public string Site { get; set; }

        /// <summary>
        /// Местоположение пользователя
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// Дополнительная информация о пользователе
        /// </summary>
        public string About { get; set; }
    }
}
