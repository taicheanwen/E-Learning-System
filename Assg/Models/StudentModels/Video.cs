namespace Assg.Models.StudentModels
{
    public class Video
    {
        public int VideoId { get; set; }
        public string VideoName { get; set; }
        public string VideoPath { get; set; }
        public double Progress { get; set; }
        public Course Course { get; set; }
        public Payment Payment { get; set; }

    }
}
