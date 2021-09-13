using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication2.Models
{
    /// <summary>
    /// Данные для авторизации/регистрации
    /// </summary>
    public class MyCredentials
    {
        /// <summary>
        /// Имя пользователя
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// Почта пользователя
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// признак регистрации
        /// </summary>
        public bool Reg { get; set; }
        /// <summary>
        /// Уникальный идентификатор
        /// </summary>
        public string Guid { get; set; }
        /// <summary>
        /// Фамилия пользователя (должен был быть мак мобилы но не срослось)
        /// </summary>
        public string Mac { get; set; }
        /// <summary>
        /// Данные по пользователю
        /// </summary>
        public User UserData { get; set; }
        /// <summary>
        /// Код подтверждения
        /// </summary>
        public string ConfirmCode { get; set; }
    }


}