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
            builder.Services.AddSingleton<UserService>();
            builder.Services.AddSingleton<HtmlService>();

            var app = builder.Build();

            app.UseSession();
            app.CheckUserSession();
            app.UseStaticFiles();

            app.MapGet("/", async context =>
            {
                context.Response.Redirect($"/register");
            });
            app.MapGet("/login", async context =>
            {   
                var error = context.Request.Query["error"];
                var templateService = context.RequestServices.GetRequiredService<HtmlService>();
                context.Response.ContentType = "text/html";
                await context.Response.WriteAsync(
                    templateService.RenderRegAuht("login",
                    new Dictionary<string, string>
                    {
                        {"title", "Login page" },
                        { "error", string.IsNullOrWhiteSpace(error) ? "" : error }
                    })
                    );
            });
            app.MapPost("/login", async context =>
            {
                var form = await context.Request.ReadFormAsync();
                var username = form["username"];
                var password = form["password"];

                var userService = context.RequestServices.GetRequiredService<UserService>();
                var user = userService.FindByLogin(username);
                if (user == null)
                {
                    context.Response.Redirect("/login?error=User not found");
                    return;
                }
                if (!userService.VerifyPassword(username, password))
                {
                    context.Response.Redirect("/login?error=Wrong password");
                    return;
                }
                context.Session.SetString("user", JsonSerializer.Serialize(user));
                context.Response.Redirect("/success");
            });
            app.MapGet("/register", async context =>
            {
                var error = context.Request.Query["error"];
                var templateService = context.RequestServices.GetRequiredService<HtmlService>();
                context.Response.ContentType = "text/html";
                await context.Response.WriteAsync(
                    templateService.RenderRegAuht("register",
                    new Dictionary<string, string>
                    {
                        {"title", "Registration form" },
                        { "error", string.IsNullOrWhiteSpace(error) ? "" : error }
                    })
                    );
            });
            app.MapPost("/register", async context =>
            {
                var form = await context.Request.ReadFormAsync();
                var name = form["name"].ToString();
                var username = form["username"].ToString();
                var password = form["password"].ToString();
                var userService = context.RequestServices.GetRequiredService<UserService>();
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
            var templateService = context.RequestServices.GetRequiredService<HtmlService>();
            context.Response.ContentType = "text/html";
            await context.Response.WriteAsync(
                templateService.Render("success",
                new Dictionary<string, string>{
                    {"title", "Home" }
                })
                );
            });
            app.MapGet("/table", async context =>
            {
                var templateService = context.RequestServices.GetRequiredService<HtmlService>();
                context.Response.ContentType = "text/html";
                await context.Response.WriteAsync(
                    templateService.Render("table",
                    new Dictionary<string, string>{
                        {"title", "Knowledge" }
                    })
                    );
            });
            app.Run();
        }
    }
}
