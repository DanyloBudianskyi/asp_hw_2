namespace asp_hw_2
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var app = builder.Build();
            app.UseStaticFiles();
            app.MapGet("/", async context =>
            {
                context.Response.Redirect("/login.html");
            });
            app.MapGet("/login", async context =>
            {
                var service = context.RequestServices.GetService<IWebHostEnvironment>();
                var wwwRootPath = service.WebRootPath;
                var html = File.ReadAllText(Path.Combine(wwwRootPath, "login.html"));
                context.Response.ContentType = "text/html";
                await context.Response.WriteAsync(html);
            });
            app.MapPost("/login", async context =>
            {
                var username = context.Request.Form["username"];
                var password = context.Request.Form["password"];
                if (!CheckLogin(username))
                {
                    context.Response.Redirect("/ErrorPage.html?errorMessage=invalid login");
                    
                }
                else if (!CheckPassword(password))
                {
                    context.Response.Redirect("/ErrorPage.html?errorMessage=invalid password");
                }
                else
                {
                    context.Response.Redirect("/success.html");
                }
                
            });
            app.MapGet("/ErrorPage.html", async context =>
            {
                var service = context.RequestServices.GetService<IWebHostEnvironment>();
                var wwwRootPath = service.WebRootPath;
                var errorMessage = context.Request.Query["errorMessage"];

                var html = File.ReadAllText(Path.Combine(wwwRootPath, "ErrorPage.html"));
                html = html.Replace("{errorMessage}", errorMessage);

                context.Response.ContentType = "text/html";
                await context.Response.WriteAsync(html);
            });

            app.Run();
        }
        static private bool CheckLogin(string login)
        {
            if(login == "admin")
            {
                return true;
            }
            return false;
        }
        static private bool CheckPassword(string password)
        {
            if (password == "1234")
            {
                return true;
            }
            return false;
        }
    }
}
