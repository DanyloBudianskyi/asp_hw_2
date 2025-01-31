using System.Text.Json;

namespace asp_hw_2
{
    public static class CheckUserExtension
    {
        public static IApplicationBuilder CheckUserSession(this IApplicationBuilder builder)
        {
            builder.Use(async (context, next) =>
            {
                var pagesForAuthorized = new List<string>() { "/success", "/logout" };

                if (!pagesForAuthorized.Contains(context.Request.Path))
                {
                    await next.Invoke();
                    return;
                }

                User myUser = null;
                try
                {
                    myUser = JsonSerializer.Deserialize<User>(context.Session.GetString("user"));
                    context.Items["MyUser"] = myUser;
                    await next.Invoke();
                }
                catch (Exception ex)
                {
                    context.Response.Redirect("/login");
                    return;
                }
            });
            return builder;
        }
    }
}
