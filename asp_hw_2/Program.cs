using static System.Runtime.InteropServices.JavaScript.JSType;

namespace asp_hw_2
{
    public class User
    {
        public string Name { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
    public class Program
    {
        public static string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Users.txt");
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddSession(options => { });
            builder.Services.AddDistributedMemoryCache();
            var app = builder.Build();

            app.UseSession();
            app.UseStaticFiles();

            app.MapGet("/", async context =>
            {
                context.Response.Redirect($"/register");
            });
            app.MapGet("/login", async context =>
            {
                var service = context.RequestServices.GetService<IWebHostEnvironment>();
                var wwwRootPath = service.WebRootPath;
                var error = context.Request.Query["error"];
                var html = File.ReadAllText(Path.Combine(wwwRootPath, "login.html"));
                html = html.Replace("{error}", string.IsNullOrWhiteSpace(error) ? "" : error);
                context.Response.ContentType = "text/html";
                await context.Response.WriteAsync(html);
            });
            app.MapPost("/login", async context =>
            {
                var form = await context.Request.ReadFormAsync();
                var username = form["username"];
                var password = form["password"];
                if (!CheckLogin(username))
                {
                    context.Response.Redirect("/login?error=Wrong login");
                }
                else if (!CheckPassword(password))
                {
                    context.Response.Redirect("/login?error=Wrong password");
                }
                else {
                    var user = FindUser(username);
                    context.Session.SetString("username", user.Username);
                    context.Session.SetString("name", user.Name);
                    context.Response.Redirect("/success");
                }
            });
            app.MapGet("/register", async context =>
            {
                var service = context.RequestServices.GetService<IWebHostEnvironment>();
                var wwwRootPath = service.WebRootPath;
                var error = context.Request.Query["error"];
                var html = File.ReadAllText(Path.Combine(wwwRootPath, "register.html"));
                html = html.Replace("{error}", string.IsNullOrWhiteSpace(error) ? "" : error);
                context.Response.ContentType = "text/html";
                await context.Response.WriteAsync(html);
            });
            app.MapPost("/register", async context =>
            {
                var form = await context.Request.ReadFormAsync();
                var name = form["name"];
                var username = form["username"];
                var password = form["password"];
                var user = FindUser(username);
                if (user != null)
                {
                    context.Response.Redirect("/register?error=User already exist");
                }
                else
                {
                    SaveUser($"{name} {username} {password}\n");
                    context.Session.SetString("username", username);
                    context.Session.SetString("name", name);
                    context.Response.Redirect("/success");
                }
            });
            app.MapGet("/logout", async context =>
            {
                context.Session.Clear();
                context.Response.Redirect("/login");
            });
            app.MapGet("/success", async context =>
            {
                var service = context.RequestServices.GetService<IWebHostEnvironment>();
                var wwwRootPath = service.WebRootPath;
                var filePath = Path.Combine(wwwRootPath, "success.html");
                var html = File.ReadAllText(filePath);
                context.Response.ContentType = "text/html";

                var name = context.Session.GetString("name");
                var username = context.Session.GetString("username");

                html = html.Replace("{name}", name);
                html = html.Replace("{username}", username);
                await context.Response.WriteAsync(html);
            });

            app.Run();
        }
        
        static private bool CheckLogin(string username)
        {
            var users = LoadUsers();
            return users.Exists(user => user.Username == username);
        }
        static private bool CheckPassword(string password)
        {
            var users = LoadUsers();
            return users.Exists(user => user.Password == password);
        }
        static private void SaveUser(string userInfo)
        {
            if (!File.Exists(filePath))
            {
                using (File.Create(filePath)) { }
            }
            File.AppendAllText(filePath, userInfo);
        }
        static private List<User> LoadUsers()
        {
            if (!File.Exists(filePath)) return new List<User>();

            var users = File.ReadAllLines(filePath)
                .Select(line => line.Split(" "))
                .Select(parts => new User { Name = parts[0], Username = parts[1], Password = parts[2] }).ToList();
            return users;
        }
        static private User FindUser(string username)
        {
            var users = LoadUsers();
            return users.FirstOrDefault(user => user.Username == username);
        }
    }
}
