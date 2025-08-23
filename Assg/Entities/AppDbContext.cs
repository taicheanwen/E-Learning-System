using Assg.Models.StudentModels;
using Microsoft.EntityFrameworkCore;

namespace Assg.Entities
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<UserAccount> UserAccount { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<Cart>Cart { get; set; }
        public DbSet<Payment> Payment { get; set; }
        public DbSet<Review> Review { get; set; }
        public DbSet<Course> Course { get; set; }
        public DbSet<Chapter> Chapter { get; set; }
        public DbSet<ChapterContent> ChapterContent { get; set; }
        public DbSet<QuizQuestion> QuizQuestion { get; set; }
        public DbSet<Questions> Questions { get; set; }
        public DbSet<QuizAnswer> QuizAnswer { get; set; }
        public DbSet<ChatQuestion> ChatQuestion { get; set; }
        public DbSet<ChatAnswer> ChatAnswer { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Course>().HasKey(c => c.Id);
            modelBuilder.Entity<QuizQuestion>().HasKey(c => c.Id);
            // Please edit this to adhere to your category db
            modelBuilder.Entity<Course>()
            .HasOne(c => c.Category)
            .WithMany(cat => cat.Courses)
            .HasForeignKey(c => c.categoryId);

            modelBuilder.Entity<Chapter>().HasKey(c => c.Id);
            modelBuilder.Entity<ChapterContent>().HasKey(cc => cc.Id);

            modelBuilder.Entity<Chapter>()
            .HasOne(c => c.Course)
            .WithMany(course => course.Chapters)
            .HasForeignKey(c => c.CourseId)
            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ChapterContent>()
                .HasOne(cc => cc.Chapter)
                .WithMany(chapter => chapter.ChapterContents)
                .HasForeignKey(cc => cc.ChapterId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<QuizQuestion>()
                .HasOne(q => q.course)
                .WithMany(cc => cc.QuizQuestions)
                .HasForeignKey(q => q.courseId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<QuizAnswer>()
                .HasOne(q => q.question)
                .WithMany(cc => cc.QuizAnswers)
                .HasForeignKey(q => q.quizId)
                .OnDelete(DeleteBehavior.Cascade);

            //modelBuilder.Entity<ChatQuestion>()
            //.HasOne(c => c.Course)
            //.WithMany(course => course.ChatQuestions)
            //.HasForeignKey(c => c.CourseId)
            //.OnDelete(DeleteBehavior.Cascade);

            //modelBuilder.Entity<ChatQuestion>()
            //.HasOne(cq => cq.ChapterContent)
            //.WithMany()
            //.HasForeignKey(cq => cq.ChapterContentId)
            //.OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Review>()
            .HasOne(r => r.ChapterContent)
            .WithMany(c => c.Reviews)
            .HasForeignKey(r => r.ChapterContentId)
            .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<ChatAnswer>()
            .HasOne(c => c.Question)
            .WithMany(course => course.Answers)
            .HasForeignKey(c => c.QuestionId)
            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Questions>()
            .HasOne(c => c.quizQuestion)
            .WithMany(quiz => quiz.questions)
            .HasForeignKey(c => c.quizId)
            .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
