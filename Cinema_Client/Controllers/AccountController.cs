using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Cinema_Client.Controllers
{
    public class AccountController : Controller
    {
        //GET:Account
        public ActionResult Index()
        {
            using (CinemaEntities db = new CinemaEntities())
            {
                return View(db.USERS.ToList());
            }
        }
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(USERS account)
        {
            if (ModelState.IsValid)
            {
                using (CinemaEntities db = new CinemaEntities())
                {

                    byte[] client_password = Encoding.Default.GetBytes(account.PASSWORD); //pobrana wartosc z pola login_textBox w byte

                    //utworzenie skrotu od pobranego hasla (SHA_1)
                    using (var sha1 = SHA1.Create())
                    {
                        byte[] client_password_sha1 = sha1.ComputeHash(client_password);  //Convert the input byte to a byte array and compute the hash.
                        string s_client_password_sha1 = Encoding.Default.GetString(client_password_sha1);

                        var number_of_clients = db.USERS.Where(a => a.USER_LOGIN == account.USER_LOGIN);

                        //if (number_of_admins.Count() != 0)

                        if (number_of_clients.Count() != 0)
                        {
                            ModelState.AddModelError("", "Podany login jest zajęty!");
                        }
                        else
                        {
                            account.PASSWORD = s_client_password_sha1;

                            db.USERS.Add(account);
                            db.SaveChanges(); //zapis do bazy danych
                            ModelState.Clear();
                            ViewBag.Message = "Pomyślnie utworzono konto dla użytkownika: " + account.USER_LOGIN;
                        }
                    }
                } 

            }
            return View();
        }

        //Login
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(USERS user)
        {
            using (CinemaEntities db = new CinemaEntities())
            {

                byte[] client_password = Encoding.Default.GetBytes(user.PASSWORD);

                //utworzenie skrotu od pobranego hasla (SHA_1)
                using (var sha1 = SHA1.Create())
                {
                    byte[] client_password_sha1 = sha1.ComputeHash(client_password);
                    string s_client_password_sha1 = Encoding.Default.GetString(client_password_sha1);
                    user.PASSWORD = s_client_password_sha1;
                }

                var usr = db.USERS.Where(u => u.USER_LOGIN == user.USER_LOGIN &&
                  u.PASSWORD == user.PASSWORD).FirstOrDefault();


                if (usr != null)
                {
                    Session["USER_LOGIN"] = usr.USER_LOGIN.ToString();
                    Session["NAME"] = usr.NAME.ToString();
                    return RedirectToAction("LoggedIn");
                }
                else
                {
                    ModelState.AddModelError("", "Dane logowania są niepoprawne!");
                }
            }
            return View();
        }
        public ActionResult LoggedIn()
        {
            if (Session["USER_LOGIN"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            Session["USER_LOGIN"] = null;
            Session["NAME"] = null;
            return RedirectToAction("Index", "Home");
        }

    }
}