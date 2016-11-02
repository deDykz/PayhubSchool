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
    public class EnrollmentController : Controller
    {
        private SchoolContext db = new SchoolContext();

        //
        // GET: /Enrollment/

        public ActionResult Index(string sortOrder, string searchString, string currentFilter, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParam = String.IsNullOrEmpty(sortOrder) ? "Course_desc" : "";
            ViewBag.CourseSortParam = sortOrder == "Name" ? "Name_desc" : "Name";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var enrollments = db.Enrollments.Include(e => e.Course).Include(e => e.Student);

            if (!String.IsNullOrEmpty(searchString))
            {
                enrollments = enrollments.Where(s => s.Student.LastName.ToUpper().Contains(searchString.ToUpper())
                    || s.Course.Title.ToUpper().Contains(searchString.ToUpper()));
            }

            switch (sortOrder)
            {
                case "Name_desc":
                    enrollments = enrollments.OrderByDescending(s => s.Student.LastName);
                    break;
                case "Course_desc":
                    enrollments = enrollments.OrderByDescending(s => s.Course.Title);
                    break;
                case "Name":
                    enrollments = enrollments.OrderBy(s => s.Student.LastName);
                    break;
                default:
                    enrollments = enrollments.OrderBy(s => s.Course.Title);
                    break;
            }

            int pageSize = 5;
            int pageNumber = (page ?? 1);

            return View(enrollments.ToPagedList(pageNumber, pageSize));
        }

        //
        // GET: /Enrollment/Details/5

        public ActionResult Details(int id = 0)
        {
            Enrollment enrollment = db.Enrollments.Find(id);
            if (enrollment == null)
            {
                return HttpNotFound();
            }
            return View(enrollment);
        }

        //
        // GET: /Enrollment/Create

        public ActionResult Create()
        {
            ViewBag.CourseID = new SelectList(db.Courses, "CourseID", "Title");
            ViewBag.StudentID = new SelectList(db.Students, "StudentID", "LastName");
            return View();
        }

        //
        // POST: /Enrollment/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Enrollment enrollment)
        {
            if (ModelState.IsValid)
            {
                db.Enrollments.Add(enrollment);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CourseID = new SelectList(db.Courses, "CourseID", "Title", enrollment.CourseID);
            ViewBag.StudentID = new SelectList(db.Students, "StudentID", "LastName", enrollment.StudentID);
            return View(enrollment);
        }

        //
        // GET: /Enrollment/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Enrollment enrollment = db.Enrollments.Find(id);
            if (enrollment == null)
            {
                return HttpNotFound();
            }
            ViewBag.CourseID = new SelectList(db.Courses, "CourseID", "Title", enrollment.CourseID);
            ViewBag.StudentID = new SelectList(db.Students, "StudentID", "LastName", enrollment.StudentID);
            ViewBag.Grade = new SelectList(db.Enrollments, "Grade", "Grade", enrollment.Grade);
            return View(enrollment);
        }

        //
        // POST: /Enrollment/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Enrollment enrollment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(enrollment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CourseID = new SelectList(db.Courses, "CourseID", "Title", enrollment.CourseID);
            ViewBag.StudentID = new SelectList(db.Students, "StudentID", "LastName", enrollment.StudentID);
            ViewBag.Grade = new SelectList(db.Enrollments, "Grade", "Grade", enrollment.Grade);
            return View(enrollment);
        }

        //
        // GET: /Enrollment/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Enrollment enrollment = db.Enrollments.Find(id);
            if (enrollment == null)
            {
                return HttpNotFound();
            }
            return View(enrollment);
        }

        //
        // POST: /Enrollment/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Enrollment enrollment = db.Enrollments.Find(id);
            db.Enrollments.Remove(enrollment);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}