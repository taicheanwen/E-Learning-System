//using Microsoft.EntityFrameworkCore;
//using Assg.Models.StudentModels;
//namespace Assg.Models.TutorModels;

//public class CourseContext : DbContext
//{
//    public CourseContext(DbContextOptions options) : base(options)
//    {
//    }

//    public DbSet<Category> Category { get; set; }
//    public DbSet<Course> Course { get; set; }
//    public DbSet<Chapter> Chapter { get; set; }
//    public DbSet<ChapterContent> ChapterContent { get; set; }
//    public DbSet<QuizQuestion> QuizQuestion { get; set; }
//    public DbSet<Questions> Questions { get; set; }
//    public DbSet<QuizAnswer> QuizAnswer { get; set; }
//    public DbSet<ChatQuestion> ChatQuestion { get; set; }
//    public DbSet<ChatAnswer> ChatAnswer { get; set; }
//    public DbSet<UserAccount> UserAccounts { get; set; }


//    protected override void OnModelCreating(ModelBuilder modelBuilder)
//    {
//        modelBuilder.Entity<Course>().HasKey(c => c.Id);
//        modelBuilder.Entity<QuizQuestion>().HasKey(c => c.Id);
//        modelBuilder.Entity<Course>()
//        .HasOne(c => c.Category)
//        .WithMany(cat => cat.Courses)
//        .HasForeignKey(c => c.categoryId);

//        modelBuilder.Entity<Chapter>().HasKey(c => c.Id);
//        modelBuilder.Entity<ChapterContent>().HasKey(cc => cc.Id);


//        modelBuilder.Entity<Chapter>()
//        .HasOne(c => c.Course)
//        .WithMany(course => course.Chapters)
//        .HasForeignKey(c => c.CourseId)
//        .OnDelete(DeleteBehavior.Cascade);

//        modelBuilder.Entity<ChapterContent>()
//                .HasOne(cc => cc.Chapter)
//                .WithMany(chapter => chapter.ChapterContents)
//                .HasForeignKey(cc => cc.ChapterId)
//                .OnDelete(DeleteBehavior.Cascade);

//        modelBuilder.Entity<QuizQuestion>()
//            .HasOne(q => q.course)
//            .WithMany(cc => cc.QuizQuestions)
//            .HasForeignKey(q => q.courseId)
//            .OnDelete(DeleteBehavior.Cascade);

//        modelBuilder.Entity<QuizAnswer>()
//            .HasOne(q => q.question)
//            .WithMany(q => q.QuizAnswers)
//            .HasForeignKey(q => q.quizId)
//            .OnDelete(DeleteBehavior.Cascade);


//         modelBuilder.Entity<ChatAnswer>()
//        .HasOne(c => c.Question)
//        .WithMany(course => course.Answers)
//        .HasForeignKey(c => c.QuestionId)
//        .OnDelete(DeleteBehavior.Cascade);

//        modelBuilder.Entity<Questions>()
//        .HasOne(c => c.quizQuestion)
//        .WithMany(quiz => quiz.questions)
//        .HasForeignKey(c => c.quizId)
//        .OnDelete(DeleteBehavior.Cascade);
//    }

//}
