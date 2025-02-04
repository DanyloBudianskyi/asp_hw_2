namespace asp_hw_2.Services
{
    public class HtmlService(IWebHostEnvironment environment)
    {
        public string Layout { get; set; } = "layout";
        public string RenderPartial(string filename, Dictionary<string, string> replacements)
        {
            var wwwRootPath = environment.ContentRootPath;
            var filePath = Path.Combine(wwwRootPath, "Templates", filename + ".html");

            string html = File.ReadAllText(filePath);
            foreach (var item in replacements)
            {
                html = html.Replace($"{{{item.Key}}}", item.Value);
            }
            return html;
        }
        public string Render(string template, Dictionary<string, string> replacements)
        {
            var dict = new Dictionary<string, string>(replacements);
            dict.Add("content", RenderPartial(template, replacements));
            return RenderPartial(Layout, dict);
        }
        public string RenderRegAuht(string template, Dictionary<string, string> replacements)
        {
            var dict = new Dictionary<string, string>(replacements);
            dict.Add("content", RenderPartial(template, replacements));
            return RenderPartial("regAuthLayout", dict);
        }
    }
}
