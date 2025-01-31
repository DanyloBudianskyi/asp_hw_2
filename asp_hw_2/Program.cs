using asp_hw_2.Services;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace asp_hw_2
{
    public class Program
    {
        public static string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Users.txt");
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddSession(options => { });
            builder.Services.AddDistributedMemoryCache();
            var app = builder.Build();

            app.UseStaticFiles();
            app.UseSession();
            app.CheckUserSession();
            

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

                var userService = new UserService();
                var user = userService.FindByLogin(username);
                if(user == null)
                {
                    context.Response.Redirect("/login?error=User not found");
                    return;
                }
                if(!userService.VerifyPassword(username, password))
                {
                    context.Response.Redirect("/login?error=Wrong password");
                    return;
                }
                context.Session.SetString("user", JsonSerializer.Serialize(user));
                context.Response.Redirect("/success");
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
                var name = form["name"].ToString();
                var username = form["username"].ToString();
                var password = form["password"].ToString();
                var userService = new UserService();
                var user = userService.FindByLogin(username);
                if (user != null)
                {
                    context.Response.Redirect("/register?error=User already exist");
                    return;
                }
                user = new User
                {
                    Name = name,
                    Username = username,
                    Password = password
                };
                userService.AddUser(user);
                context.Session.SetString("user", JsonSerializer.Serialize(user));
                context.Response.Redirect("/success");

            });
            app.MapGet("/logout", async context =>
            {
                context.Session.Clear();
                context.Response.Redirect("/login");
            });
            app.MapGet("/success", async context =>
            {
                var user = context.Items["MyUser"] as User;
                var service = context.RequestServices.GetService<IWebHostEnvironment>();
                var wwwRootPath = service.WebRootPath;
                var filePath = Path.Combine(wwwRootPath, "success.html");
                var html = File.ReadAllText(filePath);

                context.Response.ContentType = "text/html";
                var replacements = new Dictionary<string, string>
                {
                    {"{name}" , user.Name},
                    {"{username}" , user.Username},
                };
                html = HtmlService.ReadHtml(filePath, replacements);
                await context.Response.WriteAsync(html);
            });

            app.Run();
        }
    }
}
