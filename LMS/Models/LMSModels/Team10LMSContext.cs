using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace LMS.Models.LMSModels
{
    public partial class Team10LMSContext : DbContext
    {
        public Team10LMSContext()
        {
        }

        public Team10LMSContext(DbContextOptions<Team10LMSContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Administrator> Administrator { get; set; }
        public virtual DbSet<Assignment> Assignment { get; set; }
        public virtual DbSet<AssignmentCategory> AssignmentCategory { get; set; }
        public virtual DbSet<Class> Class { get; set; }
        public virtual DbSet<Course> Course { get; set; }
        public virtual DbSet<Department> Department { get; set; }
        public virtual DbSet<Enroll> Enroll { get; set; }
        public virtual DbSet<Professor> Professor { get; set; }
        public virtual DbSet<Student> Student { get; set; }
        public virtual DbSet<Submission> Submission { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseMySql("Server=atr.eng.utah.edu;User Id=u1266639;Password=ZZX@hk131;Database=Team10LMS");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Administrator>(entity =>
            {
                entity.HasKey(e => e.UId)
                    .HasName("PRIMARY");

                entity.Property(e => e.UId)
                    .HasColumnName("uID")
                    .HasColumnType("char(7)");

                entity.Property(e => e.DoB).HasColumnType("date");

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasColumnType("varchar(100)");
            });

            modelBuilder.Entity<Assignment>(entity =>
            {
                entity.HasIndex(e => e.CategoryId)
                    .HasName("categoryID");

                entity.Property(e => e.AssignmentId)
                    .HasColumnName("assignmentID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.CategoryId)
                    .HasColumnName("categoryID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Content).HasColumnType("varchar(8192)");

                entity.Property(e => e.DueTime).HasColumnType("datetime");

                entity.Property(e => e.MaxPoint).HasColumnType("int(11)");

                entity.Property(e => e.Name).HasColumnType("varchar(100)");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Assignment)
                    .HasForeignKey(d => d.CategoryId)
                    .HasConstraintName("Assignment_ibfk_1");
            });

            modelBuilder.Entity<AssignmentCategory>(entity =>
            {
                entity.HasKey(e => e.CategoryId)
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.ClassId)
                    .HasName("classID");

                entity.HasIndex(e => new { e.Name, e.ClassId })
                    .HasName("Name")
                    .IsUnique();

                entity.Property(e => e.CategoryId)
                    .HasColumnName("categoryID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ClassId)
                    .HasColumnName("classID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.Weight).HasColumnType("int(11)");

                entity.HasOne(d => d.Class)
                    .WithMany(p => p.AssignmentCategory)
                    .HasForeignKey(d => d.ClassId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("AssignmentCategory_ibfk_1");
            });

            modelBuilder.Entity<Class>(entity =>
            {
                entity.HasIndex(e => e.CourseId)
                    .HasName("CourseID");

                entity.HasIndex(e => e.ProfessorId)
                    .HasName("ProfessorID");

                entity.HasIndex(e => new { e.Year, e.Season, e.CourseId })
                    .HasName("Year")
                    .IsUnique();

                entity.Property(e => e.ClassId)
                    .HasColumnName("classID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.CourseId)
                    .HasColumnName("CourseID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.EndTime).HasColumnType("time");

                entity.Property(e => e.Location)
                    .IsRequired()
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.ProfessorId)
                    .IsRequired()
                    .HasColumnName("ProfessorID")
                    .HasColumnType("char(7)");

                entity.Property(e => e.Season)
                    .IsRequired()
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.StartTime).HasColumnType("time");

                entity.HasOne(d => d.Course)
                    .WithMany(p => p.Class)
                    .HasForeignKey(d => d.CourseId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Class_ibfk_2");

                entity.HasOne(d => d.Professor)
                    .WithMany(p => p.Class)
                    .HasForeignKey(d => d.ProfessorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Class_ibfk_1");
            });

            modelBuilder.Entity<Course>(entity =>
            {
                entity.HasIndex(e => e.DepartmentAbbr)
                    .HasName("DepartmentAbbr");

                entity.Property(e => e.CourseId)
                    .HasColumnName("courseID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.DepartmentAbbr)
                    .IsRequired()
                    .HasColumnType("varchar(4)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.Number)
                    .IsRequired()
                    .HasColumnType("char(4)");

                entity.HasOne(d => d.DepartmentAbbrNavigation)
                    .WithMany(p => p.Course)
                    .HasForeignKey(d => d.DepartmentAbbr)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Course_ibfk_1");
            });

            modelBuilder.Entity<Department>(entity =>
            {
                entity.HasKey(e => e.Abbreviation)
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.Name)
                    .HasName("Name")
                    .IsUnique();

                entity.Property(e => e.Abbreviation).HasColumnType("varchar(4)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(100)");
            });

            modelBuilder.Entity<Enroll>(entity =>
            {
                entity.HasKey(e => new { e.StudentId, e.ClassId })
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.ClassId)
                    .HasName("classID");

                entity.Property(e => e.StudentId)
                    .HasColumnName("studentID")
                    .HasColumnType("char(7)");

                entity.Property(e => e.ClassId)
                    .HasColumnName("classID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Grade).HasColumnType("varchar(2)");

                entity.HasOne(d => d.Class)
                    .WithMany(p => p.Enroll)
                    .HasForeignKey(d => d.ClassId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Enroll_ibfk_2");

                entity.HasOne(d => d.Student)
                    .WithMany(p => p.Enroll)
                    .HasForeignKey(d => d.StudentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Enroll_ibfk_1");
            });

            modelBuilder.Entity<Professor>(entity =>
            {
                entity.HasKey(e => e.UId)
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.DepartmentAbbr)
                    .HasName("DepartmentAbbr");

                entity.Property(e => e.UId)
                    .HasColumnName("uID")
                    .HasColumnType("char(7)");

                entity.Property(e => e.DepartmentAbbr)
                    .IsRequired()
                    .HasColumnType("varchar(4)");

                entity.Property(e => e.DoB).HasColumnType("date");

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasColumnType("varchar(100)");

                entity.HasOne(d => d.DepartmentAbbrNavigation)
                    .WithMany(p => p.Professor)
                    .HasForeignKey(d => d.DepartmentAbbr)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Professor_ibfk_1");
            });

            modelBuilder.Entity<Student>(entity =>
            {
                entity.HasKey(e => e.UId)
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.DepartmentAbbr)
                    .HasName("DepartmentAbbr");

                entity.Property(e => e.UId)
                    .HasColumnName("uID")
                    .HasColumnType("char(7)");

                entity.Property(e => e.DepartmentAbbr)
                    .IsRequired()
                    .HasColumnType("varchar(4)");

                entity.Property(e => e.DoB).HasColumnType("date");

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasColumnType("varchar(100)");

                entity.HasOne(d => d.DepartmentAbbrNavigation)
                    .WithMany(p => p.Student)
                    .HasForeignKey(d => d.DepartmentAbbr)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Student_ibfk_1");
            });

            modelBuilder.Entity<Submission>(entity =>
            {
                entity.HasKey(e => new { e.StudentId, e.AssignmentId })
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.AssignmentId)
                    .HasName("assignmentID");

                entity.Property(e => e.StudentId)
                    .HasColumnName("studentID")
                    .HasColumnType("char(7)");

                entity.Property(e => e.AssignmentId)
                    .HasColumnName("assignmentID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Content)
                    .IsRequired()
                    .HasColumnType("varchar(8192)");

                entity.Property(e => e.Score).HasColumnType("int(11)");

                entity.Property(e => e.Time).HasColumnType("datetime");

                entity.HasOne(d => d.Assignment)
                    .WithMany(p => p.Submission)
                    .HasForeignKey(d => d.AssignmentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Submission_ibfk_2");

                entity.HasOne(d => d.Student)
                    .WithMany(p => p.Submission)
                    .HasForeignKey(d => d.StudentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Submission_ibfk_1");
            });
        }
    }
}
