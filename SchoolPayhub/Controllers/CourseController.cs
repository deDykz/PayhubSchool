using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SchoolPayhub.Models;
using SchoolPayhub.DAL;
using PagedList;

namespace SchoolPayhub.Controllers
{
    public class CourseController : Controller
    {
        private SchoolContext db = new SchoolContext();

        //
        // GET: /Course/

        public ActionResult Index(string sortOrder, string searchString, string currentFilter, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.TitleSortParm = String.IsNullOrEmpty(sortOrder) ? "Title_desc" : "";
            ViewBag.NumSortParm = sortOrder == "Num" ? "Num_desc" : "Num";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var courses = from s in db.Courses
                          select s;

            if (!String.IsNullOrEmpty(searchString))
            {
                courses = courses.Where(s => s.Title.ToUpper().Contains(searchString.ToUpper()));
            }

            switch (sortOrder)
            {
                case "Title_desc":
                    courses = courses.OrderByDescending(s => s.Title);
                    break;

                case "Num":
                    courses = courses.OrderBy(s => s.Credits);
                    break;
                case "Num_desc":
                    courses = courses.OrderByDescending(s => s.Credits);
                    break;

                default:
                    courses = courses.OrderBy(s => s.Title);
                    break;
            }

            int pageSize = 5;
            int pageNumber = (page ?? 1);

            return View(courses.ToPagedList(pageNumber, pageSize));
        }

        //
        // GET: /Course/Details/5

        public ActionResult Details(int id = 0)
        {
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }

        //
        // GET: /Course/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Course/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(
            [Bind(Include = "Title, Credit")]
            Course course)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Courses.Add(course);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (DataException)
            {

                ModelState.AddModelError("", "Unable to add new course. Try again and call the Boss myself is the problem persist");
            }


            return View(course);
        }

        //
        // GET: /Course/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }

        //
        // POST: /Course/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(
            [Bind(Include = "CourseID, Title, Credits")]
            Course course)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(course).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (DataException)
            {

                ModelState.AddModelError("", "Unable to edit course details so contact me the big man to kill it for you");

            }

            return View(course);
        }

        //
        // GET: /Course/Delete/5

        public ActionResult Delete(bool? saveChangesError, int id = 0)
        {
            if (saveChangesError.GetValueOrDefault())
            {
                ViewBag.ErrorMessage = "Delete has failed. Try again and if the problem persist call me the Boss Man himself.";
            }
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }

        //
        // POST: /Course/Delete/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            try
            {
                Course course = db.Courses.Find(id);
                db.Courses.Remove(course);
                /* This gives a better performance if in a high volume application
                Course courseToDelete = new Course() { CourseID = id };
                db.Entry(courseToDelete).State = EntityState.Deleted;
                */
                db.SaveChanges();
            }
            catch (DataException)
            {
                ModelState.AddModelError("", "Unable to delete course. If you can't then I am the boss and access is denied you.");
            }

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}