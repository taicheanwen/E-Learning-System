using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System.Net.Mail;
using System.Net;
using System.Text.RegularExpressions;

namespace Assg
{
    public class Helper
    {
        private readonly IWebHostEnvironment en;
        private readonly IHttpContextAccessor ct;
        private readonly IConfiguration cf;

        public Helper(IWebHostEnvironment en, IHttpContextAccessor ct, IConfiguration cf)
        {
            this.en = en;
            this.ct = ct;
            this.cf = cf;
        }

        public string ValidatePhoto(IFormFile f)
        {
            var reType = new Regex(@"^image\/(jpeg|png)$", RegexOptions.IgnoreCase);
            var reName = new Regex(@"^.+\.(jpeg|jpg|png)$", RegexOptions.IgnoreCase);

            if (!reType.IsMatch(f.ContentType) || !reName.IsMatch(f.FileName))
            {
                return "Only JPG and PNG photo is allowed.";
            }
            else if (f.Length > 1 * 1024 * 1024)
            {
                return "Photo size cannot more than 1MB.";
            }

            return "";
        }

        public string SavePhoto(IFormFile f, string folder)
        {
            var file = Guid.NewGuid().ToString("n") + ".jpg";
            var path = Path.Combine(en.WebRootPath, folder, file);
            var options = new ResizeOptions
            {
                Size = new(200, 200),
                Mode = ResizeMode.Crop,
            };
            using var stream = f.OpenReadStream();
            using var img = Image.Load(stream);
            img.Mutate(x => x.Resize(options));
            img.Save(path);

            return file;
        }

        public void DeletePhoto(string file, string folder)
        {
            file = Path.GetFileName(file);
            var path = Path.Combine(en.WebRootPath, folder, file);
            File.Delete(path);
        }

        public void SendEmail(MailMessage mail)
        {
            Console.WriteLine("Activate the helper cs send email");
            string user = cf["Smtp:User"] ?? "";
            string pass = cf["Smtp:Pass"] ?? "";
            string name = cf["Smtp:Name"] ?? "";
            string host = cf["Smtp:Host"] ?? "";
            int port = cf.GetValue<int>("Smtp:Port");

            mail.From = new MailAddress(user, name);

            using var smtp = new SmtpClient
            {
                Host = host,
                Port = port,
                EnableSsl = true,
                Credentials = new NetworkCredential(user, pass)
            };
            smtp.Send(mail);
            Console.WriteLine($"{user} {pass} {name} {host} {port}");
        }
    }
}
