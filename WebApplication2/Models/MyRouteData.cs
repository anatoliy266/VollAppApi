using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication2.Models
{
    /// <summary>
    /// Данные для передачи клиент<--->сервер
    /// </summary>
    /// <typeparam name="T">Класс передаваемого обьекта</typeparam>
    public class MyRouteData<T> where T: new()
    {
        /// <summary>
        /// Логин юзера
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// Уникальный идентификатор юзера
        /// </summary>
        public string Guid { get; set; }
        /// <summary>
        /// передаваемые данные
        /// </summary>
        public T RouteObject { get; set; }
    }
}