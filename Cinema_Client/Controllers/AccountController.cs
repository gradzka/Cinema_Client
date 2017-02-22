using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Cinema_Client.Controllers
{
    public class AccountController : Controller
    {
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

        public ActionResult ChangePass()
        {
            if (Session["USER_LOGIN"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("", "Home");
            }
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePass(string[] pass)
        {
            if (Session["USER_LOGIN"] != null)
            {
                using (CinemaEntities db = new CinemaEntities())
                {
                    byte[] old_password = Encoding.Default.GetBytes(pass[0]);
                    //utworzenie skrotu od pobranego hasla (SHA_1)
                    using (var sha1 = SHA1.Create())
                    {
                        byte[] old_password_sha1 = sha1.ComputeHash(old_password);
                        string s_old_password_sha1 = Encoding.Default.GetString(old_password_sha1);
                        string user_login = Session["USER_LOGIN"].ToString();
                        if (s_old_password_sha1 == db.USERS.Where(x=>x.USER_LOGIN== user_login).Select(x=>x.PASSWORD).FirstOrDefault().ToString())
                        {
                            if (pass[1] == pass[2])
                            {
                                //nowe hasla sa poprawne
                                byte[] new_pass1 = Encoding.Default.GetBytes(pass[1]);
                                byte[] new_pass1_sha1 = sha1.ComputeHash(new_pass1); //skrot sha_1 w byte
                                string s_new_pass1_sha1 = Encoding.Default.GetString(new_pass1_sha1);

                                var findUser = db.USERS.Where(u => u.USER_LOGIN == user_login).FirstOrDefault();
                                findUser.PASSWORD = s_new_pass1_sha1;
                                db.Entry(findUser).State = EntityState.Modified;
                                db.SaveChanges();
                                ModelState.AddModelError("", "Hasło zmieniono poprawnie!");
                            }
                            else
                            {
                                ModelState.AddModelError("", "Podane hasła nie zgadzają się!");
                            }
   
                        } //podano niepoprawne stare haslo
                        else
                        {
                            ModelState.AddModelError("", "Podane stare hasło jest nieprawidłowe!");
                        }
                    }

                    return View();
                }
            }
            else
            {
                return RedirectToAction("", "Home");
            }
        }
        //Login
        public ActionResult Login()
        {
            if (Session["USER_LOGIN"] == null)
            {
               
                    return View();
            }
            else
            {
                return RedirectToAction("", "Home");
            }
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
                    Session["USER_NAME"] = usr.NAME.ToString();
                    usr.LAST_LOGIN =DateTime.Now;
                    db.Entry(usr).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Dane logowania są niepoprawne!");
                }
            }
            return View();
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            if (Session["USER_LOGIN"] != null)
            {
                Session["USER_LOGIN"] = null;
                Session["USER_NAME"] = null;
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return RedirectToAction("", "Home");
            }

        }

        // GET: temp/Edit/5
        public ActionResult Edit()
        {
            if (Session["USER_LOGIN"] != null)
            {
                using (CinemaEntities db = new CinemaEntities())
                {
                    USERS uSERS = db.USERS.Find(Session["USER_LOGIN"]);
                    if (uSERS == null)
                    {
                        return HttpNotFound();
                    }
                    return View(uSERS);
                }
            }
            else
            {
                return RedirectToAction("", "Home");
            }
        }

        // POST: temp/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "USER_LOGIN,PASSWORD,NAME,SURNAME,E_MAIL,TELEPHONE,LAST_LOGIN")] USERS uSERS)
        {
            if (uSERS.NAME != "" && uSERS.SURNAME != "" && uSERS.E_MAIL != "" && uSERS.TELEPHONE != "")
            {
                using (CinemaEntities db = new CinemaEntities())
                {
                    string login = Session["USER_LOGIN"].ToString();
                    var findUser = db.USERS.Where(u => u.USER_LOGIN == login).FirstOrDefault();
                    findUser.NAME = uSERS.NAME;
                    findUser.SURNAME = uSERS.SURNAME;
                    findUser.E_MAIL = uSERS.E_MAIL;
                    findUser.TELEPHONE = uSERS.TELEPHONE;
                    db.Entry(findUser).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index", "Home");
                }
            }
            return View(uSERS);
        }

        // GET: temp/Delete/5
        public ActionResult Delete()
        {
            if (Session["USER_LOGIN"] != null)
            {
                using (CinemaEntities db = new CinemaEntities())
                {
                return View();
                }
            }
            else
            {
                return RedirectToAction("", "Home");
            }
        }

        // POST: temp/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            if (Session["USER_LOGIN"] != null)
            {
                using (CinemaEntities db = new CinemaEntities())
                {
                    USERS uSERS = db.USERS.Find(Session["USER_LOGIN"]);
                    if (uSERS == null)
                    {
                        return HttpNotFound();
                    }

                    db.USERS.Remove(uSERS);
                    db.SaveChanges();
                }
            }
            return RedirectToAction("", "Home");
        }

    }
}