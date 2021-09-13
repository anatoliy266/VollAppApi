using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication2.Models
{
    public class MyUserProperties
    {
        /// <summary>
        /// информация о юзере
        /// </summary>
        public User User { get; set; }
        /// <summary>
        /// список событий на которые подписан юзер
        /// </summary>
        public List<VolEventToUserRef> EventToUserRef { get; set; }
        /// <summary>
        /// список организаций в которых юзер - участник
        /// </summary>
        public List<VolOrgToUserRef> VolOrgToUserRef { get; set; }
    }
}