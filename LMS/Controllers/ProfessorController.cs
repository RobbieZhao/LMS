using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LMS.Models.LMSModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Controllers
{
    [Authorize(Roles = "Professor")]
    public class ProfessorController : CommonController
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Students(string subject, string num, string season, string year)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
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

        public IActionResult Categories(string subject, string num, string season, string year)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            return View();
        }

        public IActionResult CatAssignments(string subject, string num, string season, string year, string cat)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
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

        public IActionResult Submissions(string subject, string num, string season, string year, string cat, string aname)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
            ViewData["aname"] = aname;
            return View();
        }

        public IActionResult Grade(string subject, string num, string season, string year, string cat, string aname, string uid)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
            ViewData["aname"] = aname;
            ViewData["uid"] = uid;
            return View();
        }

        /*******Begin code to modify********/


        /// <summary>
        /// Returns a JSON array of all the students in a class.
        /// Each object in the array should have the following fields:
        /// "fname" - first name
        /// "lname" - last name
        /// "uid" - user ID
        /// "dob" - date of birth
        /// "grade" - the student's grade in this class
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetStudentsInClass(string subject, int num, string season, int year)
        {
            var classID = getClassID(subject, num, season, year);

            var query = 
                from e in db.Enroll
                join s in db.Student on e.StudentId equals s.UId
                where classID == e.ClassId
                select new
                {
                    fname = s.FirstName,
                    lname = s.LastName,
                    uid = s.UId,
                    dob = s.DoB,
                    grade = e.Grade
                };

            return Json(query.ToArray());
        }



        /// <summary>
        /// Returns a JSON array with all the assignments in an assignment category for a class.
        /// If the "category" parameter is null, return all assignments in the class.
        /// Each object in the array should have the following fields:
        /// "aname" - The assignment name
        /// "cname" - The assignment category name.
        /// "due" - The due DateTime
        /// "submissions" - The number of submissions to the assignment
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class, 
        /// or null to return assignments from all categories</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetAssignmentsInCategory(string subject, int num, string season, int year, string category)
        {
            var classID = getClassID(subject, num, season, year);

            var query =
                from ac in db.AssignmentCategory
                join a in db.Assignment on ac.CategoryId equals a.CategoryId
                where classID == ac.ClassId
                select new { ac, a };

            if (category == null)
            {
                var query1 =
                    from q in query
                    select new
                    {
                        aname = q.a.Name,
                        cname = q.ac.Name,
                        due = q.a.DueTime,
                        submissions = (from s in db.Submission where s.AssignmentId == q.a.AssignmentId select s).Count()
                    };
                return Json(query1.ToArray());
            }
            else
            {
                var query1 =
                    from q in query
                    where q.ac.Name == category
                    select new
                    {
                        aname = q.a.Name,
                        cname = q.ac.Name,
                        due = q.a.DueTime,
                        submissions = (from s in db.Submission where s.AssignmentId == q.a.AssignmentId select s).Count()
                    };
                return Json(query1.ToArray());
            }
        }


        /// <summary>
        /// Returns a JSON array of the assignment categories for a certain class.
        /// Each object in the array should have the folling fields:
        /// "name" - The category name
        /// "weight" - The category weight
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetAssignmentCategories(string subject, int num, string season, int year)
        {
            var classID = getClassID(subject, num, season, year);

            var query = 
                from ac in db.AssignmentCategory
                where classID == ac.ClassId
                select new { name = ac.Name, weight = ac.Weight };

            return Json(query.ToArray());
        }

        /// <summary>
        /// Creates a new assignment category for the specified class.
        /// If a category of the given class with the given name already exists, return success = false.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The new category name</param>
        /// <param name="catweight">The new category weight</param>
        /// <returns>A JSON object containing {success = true/false} </returns>
        public IActionResult CreateAssignmentCategory(string subject, int num, string season, int year, string category, int catweight)
        {
            var classID = getClassID(subject, num, season, year);

            // To see if the category of the given class already exists
            var query1 =
                from ac in db.AssignmentCategory
                where ac.ClassId == classID && ac.Name == category
                select ac;
            if (query1.Count() > 0)
                return Json(new { success = false });

            AssignmentCategory a = new AssignmentCategory();
            a.ClassId = classID;
            a.Weight = catweight;
            a.Name = category;
            db.AssignmentCategory.Add(a);
            db.SaveChanges();

            return Json(new { success = true });
        }

        /// <summary>
        /// Creates a new assignment for the given class and category.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The new assignment name</param>
        /// <param name="asgpoints">The max point value for the new assignment</param>
        /// <param name="asgdue">The due DateTime for the new assignment</param>
        /// <param name="asgcontents">The contents of the new assignment</param>
        /// <returns>A JSON object containing success = true/false</returns>
        public IActionResult CreateAssignment(string subject, int num, string season, int year, string category, string asgname, int asgpoints, DateTime asgdue, string asgcontents)
        {
            var classID = getClassID(subject, num, season, year);

            // To get the categoryID and create a new assignment
            var categoryID = (from ac in db.AssignmentCategory
                              where classID == ac.ClassId && category == ac.Name
                              select ac.CategoryId).ToArray()[0];

            // If the assignment name already exists, then return false
            var count = (from a in db.Assignment
                         where categoryID == a.CategoryId && asgname == a.Name
                         select a).Count();
            if (count > 0)
                return Json(new { success = false });

            Assignment new_asg = new Assignment();
            new_asg.CategoryId = categoryID;
            new_asg.Name = asgname;
            new_asg.MaxPoint = asgpoints;
            new_asg.DueTime = asgdue;
            new_asg.Content = asgcontents;
            db.Assignment.Add(new_asg);
            db.SaveChanges();

            // Get all the studentsID in this class
            var all_uids = (from e in db.Enroll where classID == e.ClassId select e.StudentId).ToArray();

            foreach (var uid in all_uids)
                updateGrade(uid, classID);

            return Json(new { success = true });
        }

        public void updateGrade(string uid, int classID)
        {
            // Filter the categories of the class that has assignments in it
            var query =
                from ac in db.AssignmentCategory
                join a in db.Assignment on ac.CategoryId equals a.CategoryId
                where classID == ac.ClassId
                group ac by ac.CategoryId into group1
                select new { categoryID = group1.First().CategoryId, weight = (int)group1.First().Weight };

            int totalWeight = query.Sum(p => p.weight);

            System.Diagnostics.Debug.WriteLine("weight " + totalWeight);


            double totalGrade = 0.0;
            foreach (var q in query)
            {
                var assignments = (from a in db.Assignment
                                    where a.CategoryId == q.categoryID
                                    select a).ToArray();

                int totalMaxPoint = 0;
                int totalScore = 0;
                foreach(var a in assignments)
                {
                    totalMaxPoint += (int)a.MaxPoint;

                    var query1 =
                        from s in db.Submission
                        where s.AssignmentId == a.AssignmentId && s.StudentId == uid
                        select s;
                    // The student hasn't submitted the assignment
                    if (query1.Count() == 0)
                    {
                        totalScore += 0;
                    } else
                    {
                        totalScore += (int)query1.ToArray()[0].Score;
                    }
                    
                }

                System.Diagnostics.Debug.WriteLine(q.weight.ToString() + " "
                     + totalScore.ToString() + " " + totalMaxPoint.ToString());

                totalGrade += q.weight * (double)totalScore / totalMaxPoint;
                System.Diagnostics.Debug.WriteLine("totalgrade " + totalGrade);

            }

            System.Diagnostics.Debug.WriteLine("totalgrade " + totalGrade);


            double normalizedGrade = totalGrade * 100 / totalWeight;



            var query2 = from e in db.Enroll where classID == e.ClassId && uid == e.StudentId select e;

            query2.ToArray()[0].Grade = ScoreToGrade(normalizedGrade);

            db.SaveChanges();
        }


        /// <summary>
        /// Gets a JSON array of all the submissions to a certain assignment.
        /// Each object in the array should have the following fields:
        /// "fname" - first name
        /// "lname" - last name
        /// "uid" - user ID
        /// "time" - DateTime of the submission
        /// "score" - The score given to the submission
        /// 
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The name of the assignment</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetSubmissionsToAssignment(string subject, int num, string season, int year, string category, string asgname)
        {
            // To get the AssignmentID
            var classID = getClassID(subject, num, season, year);

            var query =
                from ac in db.AssignmentCategory
                join a in db.Assignment on ac.CategoryId equals a.CategoryId
                join s in db.Submission on a.AssignmentId equals s.AssignmentId
                join st in db.Student on s.StudentId equals st.UId
                where category == ac.Name && asgname == a.Name
                select new
                {
                    fname = st.FirstName,
                    lname = st.LastName,
                    uid = st.UId,
                    time = s.Time,
                    score = s.Score
                };

            return Json(query.ToArray());
        }


        /// <summary>
        /// Set the score of an assignment submission
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The name of the assignment</param>
        /// <param name="uid">The uid of the student who's submission is being graded</param>
        /// <param name="score">The new score for the submission</param>
        /// <returns>A JSON object containing success = true/false</returns>
        public IActionResult GradeSubmission(string subject, int num, string season, int year, string category, string asgname, string uid, int score)
        {
            var classID = getClassID(subject, num, season, year);

            // First we update the student's score for the assignment
            var query =
                from ac in db.AssignmentCategory
                join a in db.Assignment on ac.CategoryId equals a.CategoryId
                join s in db.Submission on a.AssignmentId equals s.AssignmentId
                where ac.Name == category && a.Name == asgname && s.StudentId == uid
                select s;
            query.ToArray()[0].Score = score;
            db.SaveChanges();

            updateGrade(uid, classID);

            return Json(new { success = true });
        }

        private string ScoreToGrade(double score)
        {
            if (score >= 93)
                return "A";
            else if (score >= 90)
                return "A-";
            else if (score >= 87)
                return "B+";
            else if (score >= 83)
                return "B";
            else if (score >= 80)
                return "B-";
            else if (score >= 77)
                return "C+";
            else if (score >= 73)
                return "C";
            else if (score >= 70)
                return "C-";
            else if (score >= 67)
                return "D+";
            else if (score >= 63)
                return "D";
            else if (score >= 60)
                return "D-";
            else
                return "E";
        }


        /// <summary>
        /// Returns a JSON array of the classes taught by the specified professor
        /// Each object in the array should have the following fields:
        /// "subject" - The subject abbreviation of the class (such as "CS")
        /// "number" - The course number (such as 5530)
        /// "name" - The course name
        /// "season" - The season part of the semester in which the class is taught
        /// "year" - The year part of the semester in which the class is taught
        /// </summary>
        /// <param name="uid">The professor's uid</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetMyClasses(string uid)
        {
            var query =
                from co in db.Course
                join cl in db.Class on co.CourseId equals cl.CourseId
                join p in db.Professor on cl.ProfessorId equals p.UId
                where uid == p.UId
                select new
                {
                    subject = co.DepartmentAbbr,
                    number = co.Number,
                    name = co.Name,
                    season = cl.Season,
                    year = cl.Year
                };

            return Json(query.ToArray());
        }


        /*******End code to modify********/

    }
}