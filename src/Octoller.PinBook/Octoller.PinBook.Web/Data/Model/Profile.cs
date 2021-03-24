using Octoller.PinBook.Web.Data.Model.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Octoller.PinBook.Web.Data.Model
{
    public class Profile : IIdEntity, IAuditable, IEquatable<Profile>
    {
        #region SystemProperties

        ///<inheritdoc />
        public string Id { get; set; }

        ///<inheritdoc />
        public DateTime CreatedAt { get; set; }

        ///<inheritdoc />
        public string CreatedBy { get; set; }

        ///<inheritdoc />
        public DateTime UpdatedAt { get; set; }

        ///<inheritdoc />
        public string UpdatedBy { get; set; }

        #endregion

        /// <summary>
        /// Id пользователя
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Пользователь
        /// </summary>
        public User User { get; set; }

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

        /// <summary>
        /// Списки
        /// </summary>
        public IEnumerable<BookList> BookLists { get; set; }

        #region Override object methods

        ///<inheritdoc/>
        public override bool Equals(object obj) =>
            this.Equals(obj as Profile);

        ///<inheritdoc/>
        public bool Equals(Profile profile) =>
            profile != null
            && this.Id == profile.Id
            && this.UserId == profile.UserId;

        ///<inheritdoc/>
        public override string ToString() =>
            "\nProfile: " +
            $"\n\tId: {Id}" +
            $"\n\tUser: {UserId}" +
            $"\n\tName: {Name}" +
            $"\n\tSite: {Site}" +
            $"\n\tLocation: {Location}" +
            $"\n\tNumber of book list: {BookLists?.Count()}";

        ///<inheritdoc/>
        public override int GetHashCode() =>
            HashCode.Combine(this.Id, this.UserId); 

        #endregion
    }
}
