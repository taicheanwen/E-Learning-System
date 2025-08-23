namespace Assg.Models.TutorModels;
using Assg.Models.StudentModels;
public class QuizAnswer
{
    public int id { get; set; }
    public string name { get; set; }
    public string quizId { get; set; }
    //public int UserAccountId { get; set; }
    public int? score { get; set; }
    public DateTime createdDate { get; set; }
    public QuizQuestion question { get; set; }

}
