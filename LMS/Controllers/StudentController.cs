using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LMS.Models.LMSModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentController : CommonController
    {

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Catalog()
        {
            return View();
        }

        public IActionResult Class(string subject, string num, string season, string year)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            return View();
        }

        public IActionResult Assignment(string subject, string num, string season, string year, string cat, string aname)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
            ViewData["aname"] = aname;
            return View();
        }


        public IActionResult ClassListings(string subject, string num)
        {
            System.Diagnostics.Debug.WriteLine(subject + num);
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            return View();
        }


        /*******Begin code to modify********/

        /// <summary>
        /// Returns a JSON array of the classes the given student is enrolled in.
        /// Each object in the array should have the following fields:
        /// "subject" - The subject abbreviation of the class (such as "CS")
        /// "number" - The course number (such as 5530)
        /// "name" - The course name
        /// "season" - The season part of the semester
        /// "year" - The year part of the semester
        /// "grade" - The grade earned in the class, or "--" if one hasn't been assigned
        /// </summary>
        /// <param name="uid">The uid of the student</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetMyClasses(string uid)
        {
            var query =
                from co in db.Course
                join cl in db.Class on co.CourseId equals cl.CourseId
                join e in db.Enroll on cl.ClassId equals e.ClassId
                join s in db.Student on e.StudentId equals s.UId
                where s.UId == uid
                select new
                {
                    subject = co.DepartmentAbbr,
                    number = co.Number,
                    name = co.Name,
                    season = cl.Season,
                    year = cl.Year,
                    grade = e.Grade == null ? "--" : e.Grade
                };

            return Json(query.ToArray());
        }

        /// <summary>
        /// Returns a JSON array of all the assignments in the given class that the given student is enrolled in.
        /// Each object in the array should have the following fields:
        /// "aname" - The assignment name
        /// "cname" - The category name that the assignment belongs to
        /// "due" - The due Date/Time
        /// "score" - The score earned by the student, or null if the student has not submitted to this assignment.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="uid"></param>
        /// <returns>The JSON array</returns>
        public IActionResult GetAssignmentsInClass(string subject, int num, string season, int year, string uid)
        {
            var classID = getClassID(subject, num, season, year);

            var query =
                from ac in db.AssignmentCategory
                join a in db.Assignment on ac.CategoryId equals a.CategoryId
                where classID == ac.ClassId
                select a;

            var query1 =
                from q in query
                join s in db.Submission 
                on new {A=q.AssignmentId, B=uid} equals new {A=s.AssignmentId, B=s.StudentId}
                into join1
                from j in join1.DefaultIfEmpty()
                select new
                {
                    aname = q.Name,
                    cname = q.Category.Name,
                    due = q.DueTime,
                    score = j==null ? null : (int?)j.Score
                };

            return Json(query1.ToArray());
        }



        /// <summary>
        /// Adds a submission to the given assignment for the given student
        /// The submission should use the current time as its DateTime
        /// You can get the current time with DateTime.Now
        /// The score of the submission should start as 0 until a Professor grades it
        /// If a Student submits to an assignment again, it should replace the submission contents
        /// and the submission time (the score should remain the same).
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The new assignment name</param>
        /// <param name="uid">The student submitting the assignment</param>
        /// <param name="contents">The text contents of the student's submission</param>
        /// <returns>A JSON object containing {success = true/false}</returns>
        public IActionResult SubmitAssignmentText(string subject, int num, string season, int year,
          string category, string asgname, string uid, string contents)
        {
            var classID = getClassID(subject, num, season, year);

            // To get the AssignmentID
            var query =
                from ac in db.AssignmentCategory 
                join a in db.Assignment on ac.CategoryId equals a.CategoryId
                where classID == ac.ClassId && category == ac.Name && asgname == a.Name
                select a;
            int assignmentID = query.ToArray()[0].AssignmentId;

            // To see if the student has already submitted for the assignment
            var query1 =
                from s in db.Submission
                where s.AssignmentId == assignmentID && uid == s.StudentId
                select s;
            // if he has, update the row
            if (query1.Count() == 1)
            {
                query1.ToArray()[0].Time = DateTime.Now;
                query1.ToArray()[0].Content = contents;
                query1.ToArray()[0].Score = 0;
            }
            else
            // if he hasn't, create a new row
            {
                Submission s = new Submission();
                s.StudentId = uid;
                s.AssignmentId = assignmentID;
                s.Time = DateTime.Now;
                s.Score = 0;
                s.Content = contents;
                db.Submission.Add(s);
            }

            db.SaveChanges();

            return Json(new { success = true });
        }


        /// <summary>
        /// Enrolls a student in a class.
        /// </summary>
        /// <param name="subject">The department subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester</param>
        /// <param name="year">The year part of the semester</param>
        /// <param name="uid">The uid of the student</param>
        /// <returns>A JSON object containing {success = {true/false}. 
        /// false if the student is already enrolled in the class, true otherwise.</returns>
        public IActionResult Enroll(string subject, int num, string season, int year, string uid)
        {
            var classID = getClassID(subject, num, season, year);

            // To see if the student has already enrolled in that class
            var query1 =
                from e in db.Enroll
                where e.ClassId == classID && e.StudentId == uid
                select e;
            // if he has already enrolled in the class, return false
            if (query1.Count() > 0)
                return Json(new { success = false });
            else
            // otherwise, create a new row
            {
                Enroll e = new Enroll();
                e.StudentId = uid;
                e.ClassId = classID;
                e.Grade = "--";
                db.Enroll.Add(e);
                db.SaveChanges();
                return Json(new { success = true });
            }
        }



        /// <summary>
        /// Calculates a student's GPA
        /// A student's GPA is determined by the grade-point representation of the average grade in all their classes.
        /// Assume all classes are 4 credit hours.
        /// If a student does not have a grade in a class ("--"), that class is not counted in the average.
        /// If a student is not enrolled in any classes, they have a GPA of 0.0.
        /// Otherwise, the point-value of a letter grade is determined by the table on this page:
        /// https://advising.utah.edu/academic-standards/gpa-calculator-new.php
        /// </summary>
        /// <param name="uid">The uid of the student</param>
        /// <returns>A JSON object containing a single field called "gpa" with the number value</returns>
        public IActionResult GetGPA(string uid)
        {
            var map = new Dictionary<string, double>();
            map.Add("A", 4.0);
            map.Add("A-", 3.7);
            map.Add("B+", 3.3);
            map.Add("B", 3.0);
            map.Add("B-", 2.7);
            map.Add("C+", 2.3);
            map.Add("C", 2.0);
            map.Add("C-", 1.7);
            map.Add("D+", 1.3);
            map.Add("D", 1.0);
            map.Add("D-", 0.7);
            map.Add("E", 0.0);

            var allGrades = (from e in db.Enroll where uid == e.StudentId select e.Grade).ToArray();

            double totalGPA = 0.0;
            int count = 0;
            foreach (var grade in allGrades)
            {
                if (grade != "--")
                {
                    totalGPA += map[grade];
                    count++;
                }
            }

            // count == 0 means the student hasn't enrolled in any class
            //            or he hasn't received grades from any class he enrolled in
            if (count == 0)
                return Json(new { gpa = 0.0 });

            return Json(new { gpa = totalGPA/count });
        }

        /*******End code to modify********/

    }
}