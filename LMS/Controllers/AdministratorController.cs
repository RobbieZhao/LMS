using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LMS.Models.LMSModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdministratorController : CommonController
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Department(string subject)
        {
            ViewData["subject"] = subject;
            return View();
        }

        public IActionResult Course(string subject, string num)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            return View();
        }

        /*******Begin code to modify********/

        /// <summary>
        /// Returns a JSON array of all the courses in the given department.
        /// Each object in the array should have the following fields:
        /// "number" - The course number (as in 5530)
        /// "name" - The course name (as in "Database Systems")
        /// </summary>
        /// <param name="subject">The department subject abbreviation (as in "CS")</param>
        /// <returns>The JSON result</returns>
        public IActionResult GetCourses(string subject)
        {
            var query =
                from c in db.Course
                where c.DepartmentAbbr == subject
                select new
                {
                    number = c.Number,
                    name = c.Name
                };

            return Json(query.ToArray());
        }

        /// <summary>
        /// Returns a JSON array of all the professors working in a given department.
        /// Each object in the array should have the following fields:
        /// "lname" - The professor's last name
        /// "fname" - The professor's first name
        /// "uid" - The professor's uid
        /// </summary>
        /// <param name="subject">The department subject abbreviation</param>
        /// <returns>The JSON result</returns>
        public IActionResult GetProfessors(string subject)
        {
            var query =
                from p in db.Professor
                where p.DepartmentAbbr == subject
                select new
                {
                    lname = p.FirstName,
                    fname = p.LastName,
                    uid = p.UId
                };

            return Json(query.ToArray());
        }



        /// <summary>
        /// Creates a course.
        /// A course is uniquely identified by its number + the subject to which it belongs
        /// </summary>
        /// <param name="subject">The subject abbreviation for the department in which the course will be added</param>
        /// <param name="number">The course number</param>
        /// <param name="name">The course name</param>
        /// <returns>A JSON object containing {success = true/false}.
        /// false if the course already exists, true otherwise.</returns>
        public IActionResult CreateCourse(string subject, int number, string name)
        {
            // To see if the course already exists
            var query =
                from co in db.Course
                where subject == co.DepartmentAbbr && number.ToString() == co.Number && name == co.Name
                select co;
            // if it exists, return false
            if (query.Count() > 0)
                return Json(new { success = false });

            // if it doesn't exist, create a new course
            Course c = new Course();
            c.DepartmentAbbr = subject;
            c.Name = name;
            c.Number = number.ToString();
            db.Course.Add(c);
            db.SaveChanges();

            return Json(new { success = true });
        }



        /// <summary>
        /// Creates a class offering of a given course.
        /// </summary>
        /// <param name="subject">The department subject abbreviation</param>
        /// <param name="number">The course number</param>
        /// <param name="season">The season part of the semester</param>
        /// <param name="year">The year part of the semester</param>
        /// <param name="start">The start time</param>
        /// <param name="end">The end time</param>
        /// <param name="location">The location</param>
        /// <param name="instructor">The uid of the professor</param>
        /// <returns>A JSON object containing {success = true/false}. 
        /// false if another class occupies the same location during any time 
        /// within the start-end range in the same semester, or if there is already
        /// a Class offering of the same Course in the same Semester,
        /// true otherwise.</returns>
        public IActionResult CreateClass(string subject, int number, string season, int year, DateTime start, DateTime end, string location, string instructor)
        {
            // To get the CourseID
            var query =
                from co in db.Course
                where subject == co.DepartmentAbbr && number.ToString() == co.Number
                select co;
            int courseID = query.ToArray()[0].CourseId;

            // To see if there is already a class offering of the same course in the same semester
            var query1 =
                from cl in db.Class
                where cl.CourseId == courseID && cl.Year == year && cl.Season == season
                select cl;
            if (query1.Count() > 0)
                return Json(new { success = false });

            // To see if the time clashes with another class
            var query2 =
                from cl in db.Class
                where cl.Year == year && cl.Season == season && cl.Location == location
                select new { start = cl.StartTime, end = cl.EndTime };
            var intervals = query2.ToArray();
            foreach (var interval in intervals)
            {
                if (interval.start < end.TimeOfDay && interval.end > start.TimeOfDay)
                {
                    return Json(new { success = false });
                }
            }

            Class c = new Class();
            c.Year = (uint)year;
            c.Season = season;
            c.Location = location;
            c.StartTime = start.TimeOfDay;
            c.EndTime = end.TimeOfDay;
            c.ProfessorId = instructor;
            c.CourseId = courseID;
            db.Class.Add(c);
            db.SaveChanges();

            return Json(new { success = true });
            /*******End code to modify********/
        }
    }
}