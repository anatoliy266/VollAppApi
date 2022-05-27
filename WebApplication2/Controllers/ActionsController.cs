using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Xml.Linq;

using MailKit.Net.Smtp;

using Microsoft.AspNetCore.Cors;

using MimeKit;

using Newtonsoft.Json;

using NLog;

using WebApplication2.Models;

namespace WebApplication2.Api
{

    public class ActionsController : ApiController
    {
        #region Event
        /// <summary>
        /// Получает все события
        /// </summary>
        /// <returns></returns>
        [Route("api/v2/actions/getvolevents")]
        public IHttpActionResult GetEvents()
        {
            var logger = LogManager.GetCurrentClassLogger();
            try
            {
                var ctx = new u1436679_VolEventsBaseEntities();
                return Json(ctx.VolEvent.ToList());
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                return StatusCode(HttpStatusCode.BadRequest);
            }

        }

        /// <summary>
        /// Получает событие по идентификатору
        /// </summary>
        /// <param name="id">идентификатор события</param>
        /// <returns></returns>
        [Route("api/v2/actions/getvolevent/{id}")]
        public IHttpActionResult GetEvent(int id)
        {
            var logger = LogManager.GetCurrentClassLogger();
            try
            {
                var ctx = new u1436679_VolEventsBaseEntities();
                return Json(ctx.VolEvent.Where(x => x.Id == id).ToList());
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                return StatusCode(HttpStatusCode.BadRequest);
            }
        }

        /// <summary>
        /// Добавляет новое событие
        /// </summary>
        [Route("api/v2/actions/addvolevent")]
        [HttpPost]
        public async Task<IHttpActionResult> addevent()
        {
            var logger = LogManager.GetCurrentClassLogger();
            try
            {
                var ctx = new u1436679_VolEventsBaseEntities();
                //получаем json из потока запроса и парсим его в класс
                var json = await Request.Content.ReadAsStringAsync();
                var myData = JsonConvert.DeserializeObject<MyRouteData<VolEvent>>(json);
                //получаем юзера по переданному гуиду, если юзер найден 
                //upd добавить проверку на признак возможности создания ивентов 1!!!!!
                if (ctx.User.Where(x => x.Guid == myData.Guid).Count() > 0)
                {
                    ///если проверка пройдена - добавляем новый ивент
                    var e = ctx.VolEvent.Add(myData.RouteObject);
                    ctx.SaveChanges();
                    return Json(new MyRouteData<VolEvent>()
                    {
                        Guid = myData.Guid,
                        RouteObject = e,
                    });
                }
                else
                {
                    //если нет - ошибка
                    return StatusCode(HttpStatusCode.BadRequest);
                }
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                return StatusCode(HttpStatusCode.BadRequest);
            }
        }

        /// <summary>
        /// Возвращает список пользователей, подписанных на событие
        /// </summary>
        /// <param name="id">id события</param>
        /// <returns></returns>
        [Route("api/v2/actions/geteventmembers/{id}")]
        public IHttpActionResult GetEventMembers(int id)
        {
            var logger = LogManager.GetCurrentClassLogger();
            try
            {
                var ctx = new u1436679_VolEventsBaseEntities();
                return Json(ctx.User.Where(x => ctx.VolEventToUserRef.Where(y => y.VolEventId == id).Select(y => y.UserId).Contains(x.Id)).ToList());
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                return StatusCode(HttpStatusCode.BadRequest);
            }
        }


        /// <summary>
        /// Подписать юзера на событие
        /// </summary>
        /// <returns></returns>
        [Route("api/v2/actions/assignusertoevent")]
        [HttpPost]
        public async Task<IHttpActionResult> AssignUserToEvent()
        {
            var logger = LogManager.GetCurrentClassLogger();
            try
            {
                var ctx = new u1436679_VolEventsBaseEntities();
                ///получаем тело запроса в json и парсим в класс таблицы
                var json = await Request.Content.ReadAsStringAsync();
                var myData = JsonConvert.DeserializeObject<MyRouteData<VolEventToUserRef>>(json);

                ///если юзер по гуиду определен - подписываем на событие
                ///если нет - ошибка
                if (ctx.User.Where(x => x.Guid == myData.Guid).Count() > 0)
                {
                    ///записываем отношение пользователя и события в таблицу
                    ctx.VolEventToUserRef.Add(myData.RouteObject);
                    ctx.SaveChanges();
                    ///увеличиваем на 1 количество участников
                    var evnt = ctx.VolEvent.Where(x => x.Id == myData.RouteObject.VolEventId).FirstOrDefault();
                    evnt.NumUsers = ctx.VolEventToUserRef.Where(x => x.VolEventId == evnt.Id).Count();
                    ctx.SaveChanges();

                    return Json(new MyRouteData<VolEvent>() { Guid = myData.Guid, RouteObject = evnt });
                }
                else
                {
                    return StatusCode(HttpStatusCode.BadRequest);
                }
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                return StatusCode(HttpStatusCode.BadRequest);
            }
        }

        #endregion

        #region Organization

        /// <summary>
        /// получает список всех организаций
        /// </summary>
        /// <returns></returns>
        [Route("api/v2/actions/getorganizations")]
        public IHttpActionResult GetOrganizations()
        {
            var logger = LogManager.GetCurrentClassLogger();
            try
            {
                var ctx = new u1436679_VolEventsBaseEntities();
                var result = ctx.VolOrganization.ToList();
                return Json(result);
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                return StatusCode(HttpStatusCode.BadRequest);
            }

        }

        /// <summary>
        /// Получает организацию с заданным идентификатором
        /// </summary>
        /// <param name="id">идентификатор организации</param>
        /// <returns></returns>
        [Route("api/v2/actions/getorganization/{id}")]
        public IHttpActionResult GetOrganizations(int id)
        {
            var logger = LogManager.GetCurrentClassLogger();
            try
            {
                var ctx = new u1436679_VolEventsBaseEntities();
                var result = ctx.VolOrganization.Where(x => x.Id == id).ToList();
                return Json(result);
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                return StatusCode(HttpStatusCode.BadRequest);
            }

        }

        /// <summary>
        /// Добавляет новую организацию
        /// </summary>
        /// <returns></returns>
        [Route("api/v2/actions/addorganization")]
        [HttpPost]
        public async Task<IHttpActionResult> AddOrganization()
        {
            var logger = LogManager.GetCurrentClassLogger();
            try
            {
                var ctx = new u1436679_VolEventsBaseEntities();
                ///ПАРСИМ json из запроса
                var json = await Request.Content.ReadAsStringAsync();
                var myData = JsonConvert.DeserializeObject<MyRouteData<VolOrganization>>(json);

                ///проверка на юзера по гуиду
                if (ctx.User.Where(x => x.Guid == myData.Guid).Count() > 0)
                {
                    ///если существует - добавляем организация
                    ctx.VolOrganization.Add(myData.RouteObject);
                    ctx.SaveChanges();
                    return Ok();
                }
                else
                {
                    ///если нет - ошибка
                    return StatusCode(HttpStatusCode.BadRequest);
                }
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                return StatusCode(HttpStatusCode.BadRequest);
            }
        }



        /// <summary>
        /// Подписать юзера в сообщество организации
        /// </summary>
        /// <returns></returns>
        [Route("api/v2/actions/assignusertoorg")]
        [HttpPost]
        public async Task<IHttpActionResult> AssignUserToOrg()
        {
            var logger = LogManager.GetCurrentClassLogger();
            try
            {
                var ctx = new u1436679_VolEventsBaseEntities();
                ///парсим json из запроса
                var json = await Request.Content.ReadAsStringAsync();
                var myData = JsonConvert.DeserializeObject<MyRouteData<VolOrgToUserRef>>(json);
                ///прроверка юзера по гуиду
                if (ctx.User.Where(x => x.Guid == myData.Guid).Count() > 0)
                {
                    ///подписываем юзера на событие если проверка прошла
                    ctx.VolOrgToUserRef.Add(myData.RouteObject);
                    ctx.SaveChanges();
                    return Ok();
                }
                else
                {
                    ///если проверка не прошла - ошибка
                    return StatusCode(HttpStatusCode.BadRequest);
                }
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                return StatusCode(HttpStatusCode.BadRequest);
            }
        }
        #endregion

        #region Images

        /// <summary>
        /// Запрос списка изображений, относящихся к ивенту с заданным id
        /// </summary>
        /// <param name="id">id ивента</param>
        /// <returns>список изображений ивента</returns>
        [Route("api/v2/actions/getimages/{id}")]
        public IHttpActionResult GetImages(int id)
        {
            var logger = LogManager.GetCurrentClassLogger();
            try
            {
                var ctx = new u1436679_VolEventsBaseEntities();
                var imageslist = ctx.VolImageToEventRef.Where(x => x.VolEventId == id).ToList();
                return Json(imageslist);
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                return StatusCode(HttpStatusCode.BadRequest);
            }
        }

        /// <summary>
        /// Добавляет изображение к событию
        /// </summary>
        /// <returns></returns>
        /// 

        [Route("api/v2/actions/addimage")]
        [HttpPost]
        public async Task<IHttpActionResult> AddImange()
        {
            var ctx = new u1436679_VolEventsBaseEntities();
            ///получаем json из запроса
            var json = await Request.Content.ReadAsStringAsync();
            try
            {
                ///парсим json
                var item = JsonConvert.DeserializeObject<MyRouteData<VolImageToEventRef>>(json);
                ///записываем в таблицу полученный обьект
                ctx.VolImageToEventRef.Add(item.RouteObject);
                ctx.SaveChanges();
                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(HttpStatusCode.BadRequest);
            }
        }
        #endregion

        #region Library

        /// <summary>
        /// возвращает список со статьями из базы
        /// </summary>
        /// <returns></returns>
        [Route("api/v2/actions/getinfopages")]
        public IHttpActionResult GetInfoPages()
        {
            var logger = LogManager.GetCurrentClassLogger();
            try
            {
                var ctx = new u1436679_VolEventsBaseEntities();
                var records = ctx.Library.ToList();
                return Json(records);
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                return StatusCode(HttpStatusCode.BadRequest);
            }
        }


        /// <summary>
        /// Возвращает статью с заданным id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("api/v2/actions/getinfopage/{id}")]
        public IHttpActionResult GetInfoPages(int id)
        {
            var logger = LogManager.GetCurrentClassLogger();
            try
            {
                var ctx = new u1436679_VolEventsBaseEntities();
                var records = ctx.Library.Where(x => x.Id == id).ToList();
                return Json(records);
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                return StatusCode(HttpStatusCode.BadRequest);
            }
        }
        #endregion

        #region Authorization
        /// <summary>
        /// Метод авторизации
        /// </summary>
        /// <returns>Guid пользователя</returns>
        /// 
        [Route("api/v2/actions/auth")]
        [HttpPost]
        public async Task<IHttpActionResult> Auth()
        {
            var logger = LogManager.GetCurrentClassLogger();

            try
            {
                var ctx = new u1436679_VolEventsBaseEntities();
                ///получаем из запроса json с данными авторизации и парсим его в обьект
                var json = await Request.Content.ReadAsStringAsync();
                var myData = JsonConvert.DeserializeObject<MyRouteData<MyCredentials>>(json);

                ///в полученном обьекте по признаку Reg определяем авторизация или регистрация
                if (myData.RouteObject.Reg == true)
                {
                    ///если регистрация
                    var userData = myData.RouteObject;
                    ///проверка на уникальность по емейлу если не проходит - ошибка
                    if (ctx.User.Where(x => x.Email == userData.Email).Count() > 0)
                    {
                        return StatusCode(HttpStatusCode.NotFound);
                    }
                    ///создаем для юзера ггуид и проверочный код
                    var guid = Guid.NewGuid().ToString();
                    userData.Guid = guid;
                    var checkCode = GenerateCheckCode();
                    ///добавляем нового пользователя в таблицу и получаем обьект пользователя с присвоенным id
                    ctx.User.Add(new User() { Email = userData.Email, Guid = guid.ToString(), DateReg = DateTime.Now, Family = userData.Mac, Name = userData.Username });
                    ctx.SaveChanges();
                    var userid = ctx.User.Where(x => x.Guid == guid.ToString()).FirstOrDefault();

                    ////записываем проверочный код в таблицу для дальнейшей валидации юзера
                    ctx.UserCheckCodeRef.Add(new UserCheckCodeRef() { UserId = userid.Id, CheckCode = checkCode, TimeSend = DateTime.Now, TimeValid = DateTime.Now.AddSeconds(3600) });
                    ctx.SaveChanges();
                    ////отправляем письмо с проверочным кодом на указанный при регистрации email
                    SendMessage(myData.RouteObject.Email, $"Для регистрации в приложение СберВолонтёр, Вам был выслан следующий код подтверждения: {checkCode}", MailType.Register);
                    ///возвращаем guid юзера
                    return Json(new MyRouteData<MyCredentials>() { Guid = guid, RouteObject = new MyCredentials() { Guid = guid, UserData = userid } });
                }
                else
                {
                    ///если авторизация
                    ///проверяем по таблице юзеров на наличие по емайлу, если нет - ошибка
                    if (ctx.User.Where(x => x.Email == myData.RouteObject.Email).Count() == 0) return StatusCode(HttpStatusCode.Unauthorized);
                    else
                    {
                        ///если юзер найден
                        var user = ctx.User.Where(x => x.Email == myData.RouteObject.Email).ToList().FirstOrDefault();
                        ///пересоздаем ему гуид, записываем в базу измененный обьект
                        var newGuid = Guid.NewGuid();
                        user.Guid = newGuid.ToString();
                        ///создабем новый проверочный код
                        var checkCode = GenerateCheckCode();
                        ctx.SaveChanges();
                        ///получаем id юзера
                        var userid = ctx.User.Where(x => x.Guid == newGuid.ToString()).FirstOrDefault();
                        ///записываем в таблицу id юзера и чек код
                        ctx.UserCheckCodeRef.Add(new UserCheckCodeRef() { UserId = userid.Id, CheckCode = checkCode, TimeSend = DateTime.Now, TimeValid = DateTime.Now.AddSeconds(3600) });
                        ctx.SaveChanges();
                        ///отправляем на емейл юзера проверочный код
                        SendMessage(myData.RouteObject.Email, $"Для входа в приложение СберВолонтёр, Вам был выслан следующий код подтверждения: {checkCode}", MailType.Login);
                        ///новый гуид отправляем на клиент
                        return Json(new MyRouteData<MyCredentials>() { Guid = newGuid.ToString(), RouteObject = new MyCredentials() { Guid = newGuid.ToString(), UserData = user } });
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                return StatusCode(HttpStatusCode.BadRequest);
                //return Json(e.Message + e.StackTrace);
            }
        }

        /// <summary> 
        /// подтверждение регистрации
        /// </summary>
        /// <returns></returns>
        /// 
        [Route("api/v2/actions/authconfirm")]
        [HttpPost]
        public async Task<IHttpActionResult> AuthConfirm()
        {
            var logger = LogManager.GetCurrentClassLogger();

            try
            {
                var ctx = new u1436679_VolEventsBaseEntities();
                ///парсим json из запроса в обьект
                var json = await Request.Content.ReadAsStringAsync();
                var myData = JsonConvert.DeserializeObject<MyRouteData<MyCredentials>>(json);
                /// получаем юзера по гуиду из таблицы
                var user = ctx.User.Where(x => x.Guid == myData.Guid.Replace("\"", "")).FirstOrDefault();
                ///если юзер найден - проверяем чек код
                if (ctx.UserCheckCodeRef.Where(x => x.UserId == user.Id).Count() > 0)
                {
                    ///заменить когда автоматически чек код из базы удаляться будет!!!
                    foreach (var item in ctx.UserCheckCodeRef.Where(x => x.UserId == user.Id).ToList())
                    {
                        ///фильтруем таблицу чек кодов по id юзера
                        ///для каждой найденной записи проверяем на соответствие присланный и записанный чек-кода
                        if (item.CheckCode == myData.RouteObject.ConfirmCode)
                        {
                            ///если чек коды совпадают - ставим признак пройденной валидации пользователем
                            user.Verifyed = true;
                            ctx.SaveChanges();
                            ///возвращаем на клиент обьект юзера
                            return Json(new MyRouteData<MyCredentials>()
                            {
                                Guid = user.Guid,
                                RouteObject = new MyCredentials
                                {
                                    Guid = user.Guid,
                                    UserData = user,
                                }
                            });
                        }
                    }
                }
                ///если юзер не найден - ошибка
                return StatusCode(HttpStatusCode.BadRequest);
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                return StatusCode(HttpStatusCode.BadRequest);
            }
        }



        /// <summary>
        /// Изменение анкеты пользователя
        /// </summary>
        /// <returns></returns>
        [Route("api/v2/actions/changeuserproperties")]
        [HttpPost]
        public async Task<IHttpActionResult> ChangeUserProperties()
        {
            var ctx = new u1436679_VolEventsBaseEntities();
            ///получаем json из запроса и парсим его в обьект
            var json = await Request.Content.ReadAsStringAsync();
            var myUser = JsonConvert.DeserializeObject<MyRouteData<User>>(json);

            try
            {
                ///ищем в базе юзера по id и перезаписываем присланным обьектом
                var user = ctx.User.Where(x => x.Id == myUser.RouteObject.Id).FirstOrDefault();
                user = myUser.RouteObject;
                ctx.SaveChanges();
                return Ok();
            }
            catch
            {
                return StatusCode(HttpStatusCode.BadRequest);
            }
        }

        /// <summary>
        /// Возвращает обьект юзера по гуиду
        /// </summary>
        /// <returns></returns>
        [Route("api/v2/actions/getuser")]
        [HttpPost]
        public async Task<IHttpActionResult> GetUser()
        {
            var ctx = new u1436679_VolEventsBaseEntities();
            ///парсим json из запроса в обьект
            var json = await Request.Content.ReadAsStringAsync();
            var mycred = JsonConvert.DeserializeObject<MyRouteData<MyCredentials>>(json);

            try
            {
                ///если юзер по гуиду найден - возвращаем его клиенту, если нет - возвращаем пустой обьект
                if (ctx.User.Where(x => x.Guid == mycred.RouteObject.Guid).Count() > 0)
                {
                    var user = ctx.User.Where(x => x.Guid == mycred.RouteObject.Guid).FirstOrDefault();
                    return Json(new MyRouteData<User>() { Guid = user.Guid, RouteObject = user });
                }
                else return Json(new MyRouteData<User>());

            }
            catch
            {
                return StatusCode(HttpStatusCode.BadRequest);
            }
        }


        /// <summary>
        /// типы сообщений для отправки
        /// </summary>
        /// 
        public enum MailType
        {
            Register,
            Login
        }


        /// <summary>
        /// отправляет сообщение на указанный емайл с содержимым в зависимости от типа сообщения
        /// </summary>
        /// <param name="address"></param>
        /// <param name="content"></param>
        /// <param name="mailtype"></param>
        private void SendMessage(string address, string content, MailType mailtype)
        {
            var logger = LogManager.GetCurrentClassLogger();

            ///создаем сообщение для почтового сервера
            MimeMessage message = new MimeMessage();
            ///определяем отправителя
            MailboxAddress from = new MailboxAddress("СберВолонтер", ConfigurationManager.AppSettings.Get("smtpMail"));
            message.From.Add(from);
            ///определяем получателя
            MailboxAddress to = new MailboxAddress("User", address);
            message.To.Add(to);
            ///Создаем тело сообщения
            BodyBuilder bodyBuilder = new BodyBuilder();
            ///в зависимости от типа сообщения будет меняться тема сообщения
            switch (mailtype)
            {
                case MailType.Register:
                    {
                        message.Subject = "Подтверждение регистрации.";
                        bodyBuilder.HtmlBody = content;
                        message.Body = bodyBuilder.ToMessageBody();
                        break;
                    }
                case MailType.Login:
                    {
                        message.Subject = "Подтверждение входа.";
                        bodyBuilder.HtmlBody = content;
                        message.Body = bodyBuilder.ToMessageBody();
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
            ///создаем smtp клиент
            SmtpClient client = new SmtpClient();
            try
            {
                ///соединяемся с smtp сервером
                client.Connect(ConfigurationManager.AppSettings.Get("smtpServer"), int.Parse(ConfigurationManager.AppSettings.Get("smtpPort")), true);
                ///авторизуемся
                client.Authenticate(ConfigurationManager.AppSettings.Get("smtpMail"), ConfigurationManager.AppSettings.Get("smtpPwd"));
                ///и отправляем подготовленное сообщение
                client.Send(message);
                ///отключаемся от сервера и зануляем обьект smtp клиента
                client.Disconnect(true);
                client.Dispose();
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                client.Dispose();
            }
        }


        /// <summary>
        /// генерирует случайный 6значный код
        /// </summary>
        /// <returns>6значный код</returns>
        private string GenerateCheckCode()
        {
            var random = new Random();
            //const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, 6).Select(s => s[random.Next(s.Length)]).ToArray());
        }
        #endregion

        #region User

        /// <summary>
        /// отдает данные по подпискам пользователя и личной информации
        /// </summary>
        /// <returns></returns>
        [Route("api/v2/actions/getuserproperties")]
        [HttpPost]
        public async Task<IHttpActionResult> GetUserProperties()
        {
            var logger = LogManager.GetCurrentClassLogger();

            try
            {
                var ctx = new u1436679_VolEventsBaseEntities();
                ///парсим json из запроса в строку
                var json = await Request.Content.ReadAsStringAsync();
                var myUserGuid = JsonConvert.DeserializeObject<string>(json);
                ///если пользователь найден - забираем инфу по подпискам на ивенты, организации и сам обьект юзера
                if (ctx.User.Where(x => x.Guid == myUserGuid).Count() > 0)
                {
                    var user = ctx.User.Where(x => x.Guid == myUserGuid).ToList().FirstOrDefault();
                    var events = ctx.VolEventToUserRef.Where(x => x.UserId == user.Id).ToList();
                    var orgs = ctx.VolOrgToUserRef.Where(x => x.UserId == user.Id).ToList();
                    ///возвращаем обьект с данными на клиент
                    var userProp = new MyUserProperties()
                    {
                        User = user,
                        EventToUserRef = events,
                        VolOrgToUserRef = orgs
                    };
                    return Json<MyUserProperties>(userProp);
                }
                ///если не найден юзер по гуиду - ошибка
                else
                {
                    return StatusCode(HttpStatusCode.BadRequest);
                }
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                return StatusCode(HttpStatusCode.BadRequest);
            }
        }

        /// <summary>
        /// Возвращает список событий на которые подписан юзер с заданным id 
        /// </summary>
        /// <param name="id">id юзера</param>
        /// <returns></returns>
        [Route("api/v2/actions/getuservolevents/{id}")]
        public IHttpActionResult GetUserEvents(int id)
        {
            var logger = LogManager.GetCurrentClassLogger();
            try
            {
                var ctx = new u1436679_VolEventsBaseEntities();
                var events = ctx.VolEvent.Where(x => ctx.VolEventToUserRef.Where(y => y.UserId == id).Select(y => y.VolEventId).Contains(x.Id)).ToList();
                return Json(events);
            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
                return StatusCode(HttpStatusCode.BadRequest);
            }
        }

        /// <summary>
        /// Возвращает количество подписок юзера
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("api/v2/actions/getuservoleventscount/{id}")]
        public IHttpActionResult GetUserEventsCount(int id)
        {
            var logger = LogManager.GetCurrentClassLogger();
            try
            {
                var ctx = new u1436679_VolEventsBaseEntities();
                var events = ctx.VolEvent.Where(x => ctx.VolEventToUserRef.Where(y => y.UserId == id).Select(y => y.VolEventId).Contains(x.Id)).ToList().Count;
                return Json(events);
            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
                return StatusCode(HttpStatusCode.BadRequest);
            }
        }

        /// <summary>
        /// Обновляет аватар юзера
        /// </summary>
        /// <returns></returns>
        [Route("api/v2/actions/updateuserimage")]
        [HttpPost]
        public async Task<IHttpActionResult> UpdateUserImage()
        {
            var logger = LogManager.GetCurrentClassLogger();

            try
            {
                var ctx = new u1436679_VolEventsBaseEntities();
                ///парсим json из запроса в обьект юзера
                var json = await Request.Content.ReadAsStringAsync();
                var mydata = JsonConvert.DeserializeObject<MyRouteData<User>>(json);
                ///проверяем наличие юзера с указанным гуидом
                if (ctx.User.Where(x => x.Guid == mydata.Guid).Count() > 0)
                {
                    ///если юзер найден - обновляем изображение
                    var user = ctx.User.Where(x => x.Guid == mydata.Guid).ToList().FirstOrDefault();
                    user.Image = mydata.RouteObject.Image;
                    ctx.SaveChanges();
                    /// возвращаем обьект юзера на клиент
                    return Json(new MyRouteData<User>() { Guid = mydata.Guid, RouteObject = user, });
                }
                else
                {
                    ///если юзер не найден - ошибка
                    return StatusCode(HttpStatusCode.BadRequest);
                }
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                return StatusCode(HttpStatusCode.BadRequest);
            }
        }
        #endregion

        #region City

        /// <summary>
        /// Метод добавляет город в базу
        /// </summary>
        /// <returns></returns>
        [Route("api/v2/actions/addcity")]
        [HttpPost]
        public async Task<IHttpActionResult> AddCity()
        {
            var ctx = new u1436679_VolEventsBaseEntities();
            var json = await Request.Content.ReadAsStringAsync();
            try
            {
                ///парсим json в обьект города и записываем в базу
                var item = JsonConvert.DeserializeObject<MyRouteData<City>>(json);
                ctx.City.Add(item.RouteObject);
                ctx.SaveChanges();
                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(HttpStatusCode.BadRequest);
            }
        }
        #endregion
    }
}

