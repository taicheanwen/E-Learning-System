namespace Assg.Models.TutorModels;
using Assg.Models.StudentModels;
public class CourseDetailsViewModel
{
    public Course Course { get; set; }
    public IEnumerable<Course> Courses { get; set; }
    public List<Chapter> Chapter { get; set; }
    public List<ChapterContent> chapterContents { get; set; }
    public List<ChatQuestion> chatQuestions { get; set; }
    public List<ChatAnswer> chatAnswers { get; set; }
    public List<UserAccount> UserAccount { get; set; }
}
