using Assg.Models.StudentModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Assg.Service;
using Assg.Entities;


namespace Assg.Controllers.TutorControllers;

public class CourseController : Controller
{
    private readonly AppDbContext _context;
    private readonly IHubContext<NotificationHub> _notificationHub;

    public CourseController(AppDbContext _context, IHubContext<NotificationHub> notificationHub)
    {
        this._context = _context;
        _notificationHub = notificationHub;
    }

    [Route("Course/Details/{courseId}")]
    public IActionResult Details(string courseId)
    {
        var course = _context.Course.FirstOrDefault(c => c.Id == courseId);
        
        if (course == null)
        {
            return NotFound();
        }

        var chapters = _context.Chapter.Where(c => c.CourseId == courseId).ToList();

        var content = _context.ChapterContent.ToList();

        foreach (var cont in content)
        {
            if (cont.Type == "Quiz")
            {
                var question = _context.QuizQuestion.Where(c => c.ChapterContentId == cont.Id).ToList();
                cont.QuizQuestions = question;
            }
        }

        var chapterContent = _context.ChapterContent
             .Select(c => c.Id)
             .ToList();

        var chatQuestion = _context.ChatQuestion
            .Where(c => chapterContent.Contains(c.ChapterContentId))
            .ToList();


        List<ChatAnswer> chatAnswers = new List<ChatAnswer>();

        foreach (var q in chatQuestion)
        {
            var chatAnswersForQuestion = _context.ChatAnswer.Where(c => c.QuestionId == q.Id).ToList();
            Console.WriteLine($"Question ID: {q.Id} - Number of Answers: {chatAnswersForQuestion.Count}");
            foreach (var chatAnswer in chatAnswersForQuestion)
            {
                var tutorChatAnswer = new ChatAnswer
                {
                    Id = chatAnswer.Id,
                    Content = chatAnswer.Content,
                    QuestionId = chatAnswer.QuestionId
                };

                chatAnswers.Add(tutorChatAnswer);
            }
        }


        var tutorCourse = new Assg.Models.StudentModels.Course
        {
            Id = course.Id,
            Name = course.Name,
            Price = (decimal)course.Price,
            categoryId = course.categoryId,
            subtitle = course.subtitle,
            description = course.description,
            Participant = course.Participant,
            createdDate = course.createdDate,
        };

        Console.WriteLine($"tutorCourse id : {tutorCourse.Id}");
        Console.WriteLine($"tutorCourse name : {tutorCourse.Name}");
        Console.WriteLine($"tutorCourse price : {tutorCourse.Price}");
        Console.WriteLine($"tutorCourse categoryId : {tutorCourse.categoryId}");
        Console.WriteLine($"tutorCourse subtitle : {tutorCourse.subtitle}");
        Console.WriteLine($"tutorCourse description : {tutorCourse.description}");
        Console.WriteLine($"tutorCourse participant : {tutorCourse.Participant}");
        Console.WriteLine($"tutorCourse created date : {tutorCourse.createdDate}");

        var viewModel = new Assg.Models.TutorModels.CourseDetailsViewModel
        {
            Course = tutorCourse,
            Chapter = chapters,
            chapterContents = content,
            chatQuestions = chatQuestion,
            chatAnswers = chatAnswers
        };

        var categories = _context.Category
        .Select(category => new
        {
            categoryId = category.CategoryId,
            categoryName = category.CategoryName
        })
        .Distinct()
        .ToList();

        var subcategories = _context.Category
        .Where(c => c.CategoryId == course.categoryId)
        .Select(c => new
        {
            subcategoryName = c.SubcategoryName
        })
        .Distinct()
        .ToList();


        ViewBag.Categories = categories;
        ViewBag.Subcategories = subcategories;

        return View("~/Views/TutorViews/Course/Details.cshtml", viewModel);
    }

    [HttpPost]
    public IActionResult AddChapter(string courseId, string chapterTitle)
    {
        if (ModelState.IsValid)
        {
            var newChapter = new Chapter
            {
                Title = chapterTitle,
                CourseId = courseId
            };
            _context.Chapter.Add(newChapter);
            _context.SaveChanges();

            return RedirectToAction("Details", new { courseId });
        }
        return RedirectToAction("Details", new { courseId });
    }

    [HttpPost]
    public IActionResult AddContent(string courseId, int chapterId, ChapterContent model, List<IFormFile> files, IFormCollection form)
    {
        if (model.Type.Equals("Material"))
        {
            if (files.Any())
            {
                var chapterContents = new List<ChapterContent>();
                foreach (var file in files)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        file.CopyTo(memoryStream);

                        var content = new ChapterContent
                        {
                            Title = model.Title,
                            Description = model.Description,
                            Type = "Material",
                            FileData = memoryStream.ToArray(),
                            FileName = file.FileName,
                            FileType = file.ContentType,
                            ChapterId = chapterId,
                            courseId = courseId
                        };

                        chapterContents.Add(content);
                    }
                }

                _context.ChapterContent.AddRange(chapterContents);
            }
            else
            {
                var content = new ChapterContent
                {
                    Title = model.Title,
                    Description = model.Description,
                    Type = "Material",
                    ChapterId = chapterId,
                    courseId = courseId
                };

                _context.ChapterContent.Add(content);
            }
        }
        else if (model.Type.Equals("Quiz"))
        {
            var questions = new List<Questions>();
            var processedQuestionIds = new HashSet<string>();
            var course = _context.Course.Where(c => c.Id == courseId).FirstOrDefault();

            var maxQuiz = _context.QuizQuestion
                .OrderByDescending(c => c.Id)
                .FirstOrDefault();

            int idCounter = maxQuiz == null ? 1000 : int.Parse(maxQuiz.Id.Substring(1));

            idCounter += 1;
            string newId = "Q" + idCounter.ToString();

            foreach (var key in form.Keys)
            {
                if (key.StartsWith("questions["))
                {
                    var questionId = key.Split('[')[1].Split(']')[0];

                    if (processedQuestionIds.Contains(questionId))
                        continue;

                    processedQuestionIds.Add(questionId);

                    var questionText = form[$"questions[{questionId}][questionText]"];
                    var file = Request.Form.Files.FirstOrDefault(f => f.Name == $"questions[{questionId}][file]");

                    var optionsKey = $"questions[{questionId}][Options][]";
                    var options = form.ContainsKey(optionsKey) ? form[optionsKey].ToList() : new List<string>();

                    var correctAnswerKey = $"questions[{questionId}][CorrectAnswers]";
                    var correctAnswer = form.ContainsKey(correctAnswerKey) ? form[correctAnswerKey].ToString() : null;

                    var question = new Questions
                    {
                        quizId = newId,
                        Question = questionText,
                        CorrectAnswer = correctAnswer,
                        Options = options,
                    };


                    questions.Add(question);
                }
            }

            var quizQuestion = new QuizQuestion
            {
                Id = newId,
                name = course.Name,
                courseId = course.Id,
                description = course.description,
                createdDate = DateTime.Now,
                questions = questions
            };

            var content = new ChapterContent
            {
                Title = model.Title,
                Description = model.Description,
                Type = "Quiz",
                ChapterId = chapterId,
                QuizQuestions = new List<QuizQuestion> { quizQuestion },
                courseId = courseId
            };

            _context.ChapterContent.Add(content);
            _context.SaveChanges();

            foreach (var question in questions)
            {
                question.quizQuestion = quizQuestion;
                _context.Questions.Update(question);
            }

            _context.SaveChanges();
        }


        _context.SaveChanges();
        return RedirectToAction("Details", new { id = courseId });
    }

    [HttpGet]
    public IActionResult Download(int id)
    {
        var content = _context.ChapterContent.Find(id);
        if (content == null || content.FileData == null)
        {
            return NotFound();
        }

        return File(content.FileData, content.FileType, content.FileName);
    }


    [HttpPost]
    public IActionResult EditCourse(string courseId, string courseName, string categoryId, string subcategoryId, string subtitle, decimal price, string description)
    {
        if (ModelState.IsValid)
        {
            var existingCourse = _context.Course
                .FirstOrDefault(c => c.Id == courseId);

            if (existingCourse != null)
            {
                existingCourse.Name = courseName;
                existingCourse.categoryId = categoryId;
                existingCourse.subtitle = subtitle;
                existingCourse.Price = price;
                existingCourse.description = description;

                _context.Course.Update(existingCourse);
                _context.SaveChanges();

                return RedirectToAction("Details", new { courseId });
            }

            ModelState.AddModelError("", "Course not found.");
        }
        return RedirectToAction("Details", new { courseId });
    }

    public IActionResult DeleteChapter(int id)
    {
        var content = _context.ChapterContent.Where(c => c.ChapterId == id).ToList();

        foreach (var c in content)
        {
            DeleteContent(c.Id);
        }

        var chapter = _context.Chapter.Find(id);
        if (content != null)
        {
            _context.Chapter.Remove(chapter);
            _context.SaveChanges();
        }

        return RedirectToAction("Details", "Course", new { id = chapter.CourseId });
    }

    public IActionResult DeleteContent(int id)
    {
        var content = _context.ChapterContent.Find(id);
        if (content != null)
        {
            if (content.Type == "Quiz")
            {
                var questions = _context.QuizQuestion.Where(c => c.ChapterContentId == id).ToList();
                foreach (var q in questions)
                {
                    _context.QuizQuestion.Remove(q);
                }
            }
            _context.ChapterContent.Remove(content);
            _context.SaveChanges();
        }

        return RedirectToAction("Details", "Course", new { id = content.courseId });
    }


    [HttpGet]
    public IActionResult DeleteCourse(string courseId)
    {
        var courseToDelete = _context.Course.FirstOrDefault(c => c.Id == courseId);

        if (courseToDelete != null)
        {
            _context.Course.Remove(courseToDelete);
            _context.SaveChanges();

            return RedirectToAction("Index", "MyCourse");
        }

        return RedirectToAction("Details", new { courseId });
    }

    public IActionResult GetQuestion(string courseId)
    {
                var course = _context.Course
            .Include(c => c.Chapters)
                .ThenInclude(ch => ch.ChapterContents)
                    .ThenInclude(c => c.ChatQuestion)
                        .ThenInclude(q => q.Answers)
            .FirstOrDefault(c => c.Id == courseId);

        if (course == null)
        {
            return NotFound();
        }

        var chapterContents = course.Chapters?
        .SelectMany(ch => ch.ChapterContents ?? new List<ChapterContent>())
        .ToList() ?? new List<ChapterContent>();


        ViewData["ChapterContentId"] = chapterContents.FirstOrDefault()?.Id;

        var tutorCourse = new Assg.Models.StudentModels.Course
        {
            Id = course.Id,
            Name = course.Name,
            Price = (decimal)course.Price,
            categoryId = course.categoryId,
            subtitle = course.subtitle,
            description = course.description,
            Participant = course.Participant,
            createdDate = course.createdDate,
            Chapters = course.Chapters?.Select(chapter => new Chapter
            {
                Id = chapter.Id,
                Title = chapter.Title,
            }).ToList()
        };

        var viewModel = new Assg.Models.TutorModels.CourseDetailsViewModel
        {
            Course = tutorCourse,
            chapterContents = chapterContents,
            chatQuestions = chapterContents.SelectMany(cc => cc.ChatQuestion).ToList(),
            chatAnswers = chapterContents.SelectMany(cc => cc.ChatQuestion).SelectMany(q => q.Answers).ToList()
        };

        return View("Details", viewModel); 
    }

    //[HttpPost]
    //public async Task<IActionResult> PostQuestion(int chapterContentId, string Title, string Content)
    //{
    //    if (ModelState.IsValid)
    //    {
    //        var question = new ChatQuestion
    //        {
    //            QuestionTitle = Title,
    //            QuestionContent = Content,
    //            ChapterContentId = chapterContentId,
    //            CreatedAt = DateTime.UtcNow
    //        };

    //        _context.ChatQuestion.Add(question);
    //        await _context.SaveChangesAsync();

    //        await _notificationHub.Clients.All.SendAsync("ReceiveNotification", $"New question: {Title}", chapterContentId);

    //        return RedirectToAction("Details", new { id = chapterContentId });
    //    }

    //    return RedirectToAction("Details", new { id = chapterContentId });
    //}

    //[HttpPost]
    //public async Task<IActionResult> PostAnswer(string courseId, int QuestionId, string Content)
    //{
    //    try
    //    {
    //        if (!ModelState.IsValid || string.IsNullOrEmpty(Content))
    //        {
    //            return RedirectToAction("Details", new { courseId });
    //        }

    //        var currentUserName = User.Identity.Name;
    //        if (string.IsNullOrEmpty(currentUserName))
    //        {
    //            return RedirectToAction("Login", "Account");
    //        }

    //        var currentUser = await _context.UserAccount
    //                                .FirstOrDefaultAsync(u => u.Username == currentUserName);

    //        if (currentUser == null)
    //        {
    //            return RedirectToAction("Login", "Account");
    //        }

    //        // Verify the question exists
    //        var question = await _context.ChatQuestion.FindAsync(QuestionId);
    //        if (question == null)
    //        {
    //            return RedirectToAction("Details", new { courseId });
    //        }

    //        var answer = new ChatAnswer
    //        {
    //            QuestionId = QuestionId,
    //            Content = Content,
    //            CreatedAt = DateTime.UtcNow,
    //            UserAccount = currentUser
    //        };

    //        _context.ChatAnswer.Add(answer);
    //        await _context.SaveChangesAsync();

    //        // Send notification
    //        await _notificationHub.Clients.All.SendAsync("ReceiveNotification", "A new reply was posted!", courseId);

    //        return RedirectToAction("Details", new { courseId });
    //    }
    //    catch (Exception ex)
    //    {
    //        // Log the error
    //        Console.WriteLine($"Error posting answer: {ex.Message}");
    //        return RedirectToAction("Details", new { courseId });
    //    }
    //}

    [HttpPost]
    public async Task<IActionResult> PostAnswer(int courseId, int questionId, string content)
    {
        if (!ModelState.IsValid)
        {
            return RedirectToAction(nameof(Details), new { id = courseId });
        }

        var answer = new ChatAnswer
        {
            QuestionId = questionId,
            Content = content,
            CreatedAt = DateTime.UtcNow,
        };

        _context.ChatAnswer.Add(answer);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Details), new { id = courseId });
    }


    [HttpPost]
    public async Task<IActionResult> DeleteQuestion(string courseId, int QuestionId)
    {
        var questionToDelete = await _context.ChatQuestion.FirstOrDefaultAsync(c => c.Id == QuestionId);

        if (questionToDelete != null)
        {
            _context.ChatQuestion.Remove(questionToDelete);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { courseId });
        }

        return RedirectToAction("Details", new { courseId });
    }

    [HttpPost]
    public async Task<IActionResult> DeleteAnswer(string courseId, int AnswerId)
    {
        var answerToDelete = await _context.ChatAnswer.FirstOrDefaultAsync(c => c.Id == AnswerId);

        if (answerToDelete != null)
        {
            _context.ChatAnswer.Remove(answerToDelete);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { courseId });
        }

        return RedirectToAction("Details", new { courseId });
    }



}
