namespace Assg.Models.StudentModels;

public class Chapter
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string CourseId { get; set; }
    public Course Course { get; set; }
    public ICollection<ChapterContent> ChapterContents { get; set; }
}
