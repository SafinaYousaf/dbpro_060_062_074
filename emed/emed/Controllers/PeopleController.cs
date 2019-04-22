﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using emed.Models;

namespace emed.Controllers
{
    public class PeopleController : Controller
    {
        private DB53Entities db = new DB53Entities();

        // GET: People
        public ActionResult Index()
        {
            var people = db.People.Include(p => p.Login).Include(p => p.Staff);
            return View(people.ToList());
        }

        // GET: People/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Person person = db.People.Find(id);
            if (person == null)
            {
                return HttpNotFound();
            }
            return View(person);
        }

        // GET: People/Create
        public ActionResult Create()
        {
            ViewBag.Id = new SelectList(db.Logins, "Login_Id", "Email");
            ViewBag.Id = new SelectList(db.Staffs, "Staff_Id", "Designation");
            return View();
        }

        // POST: People/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,FirstName,LastName,Contact,Email,Address,Country,DateOfBirth,Gender,Discriminator")] Person person)
        {
            string pass = "123";
            Login newuser = new Login();
            newuser.Login_Id = person.Id;
            newuser.Password = pass;
            newuser.Email = person.Email;
            newuser.Discriminator = "Admin";
            if (ModelState.IsValid)
            {
                db.People.Add(person);
                db.Logins.Add(newuser);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Id = new SelectList(db.Logins, "Login_Id", "Email", person.Id);
            ViewBag.Id = new SelectList(db.Staffs, "Staff_Id", "Designation", person.Id);
            return View(person);
        }

        // GET: People/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Person person = db.People.Find(id);
            if (person == null)
            {
                return HttpNotFound();
            }
            ViewBag.Id = new SelectList(db.Logins, "Login_Id", "Email", person.Id);
            ViewBag.Id = new SelectList(db.Staffs, "Staff_Id", "Designation", person.Id);
            return View(person);
        }

        // POST: People/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,FirstName,LastName,Contact,Email,Address,Country,DateOfBirth,Gender,Discriminator")] Person person)
        {
            if (ModelState.IsValid)
            {
                db.Entry(person).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Id = new SelectList(db.Logins, "Login_Id", "Email", person.Id);
            ViewBag.Id = new SelectList(db.Staffs, "Staff_Id", "Designation", person.Id);
            return View(person);
        }

        // GET: People/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Person person = db.People.Find(id);
            if (person == null)
            {
                return HttpNotFound();
            }
            return View(person);
        }

        // POST: People/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Person person = db.People.Find(id);
            db.People.Remove(person);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        //MINE
        public ActionResult SignUp()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignUp([Bind(Include = "FirstName,LastName,Contact,Email,Address,Country,DateOfBirth,Gender,Discriminator,Password,ConfirmPassword ")] User userrr)
        {
            using (DB53Entities db = new DB53Entities())
            {
                Person pers = new Person();
                pers.Address = userrr.Address;
                pers.Contact = userrr.Contact;
                pers.Country = userrr.Country;
                pers.DateOfBirth = userrr.DateOfBirth;
                pers.Email = userrr.Email;
                pers.FirstName = userrr.FirstName;
                pers.LastName = userrr.LastName;
                pers.Gender = userrr.Gender;
                db.People.Add(pers);
                db.SaveChanges();
                Person user = db.People.FirstOrDefault(newuser => newuser.Email == (userrr.Email));

                Login newlog = new Login();
                newlog.Login_Id = user.Id;
                newlog.Email = userrr.Email;
                newlog.Password = userrr.Password;
                newlog.Discriminator = userrr.Discriminator;
                db.Logins.Add(newlog);
                db.SaveChanges();
                return RedirectToAction("Index", "People");
            }
        }
            //Login
            public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(Login entity)
        {
            using (DB53Entities db = new DB53Entities())
            {
                //fetche the data of user on the basis of inserted email
                Login user = db.Logins.FirstOrDefault(u => u.Email == (entity.Email));
                
                //if entered email is not bound to any user then object will be null
                if (user == null)
                {
                    TempData["ErrorMSG"] = "object not found";
                    return View(entity);

                }
                //if we are here then it mean we sucsessfuly retrived the data
                //now we compare password
                int a = entity.Password.Count();
                if (user.Password.Substring(0, a) != entity.Password)
                {
                    TempData["ErrorMSG"] = "Password not matched";
                    return View(entity);

                }

                if (user != null)
                {


                    if (user.Password.Substring(0, a) == entity.Password)
                    {
                        return RedirectToAction("Index", "People");

                        
                    }
                    return View();
                }
                return View();
            }
        }
    }
}
