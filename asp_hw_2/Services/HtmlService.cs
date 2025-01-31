namespace asp_hw_2.Services
{
    public class HtmlService
    {
        public static string ReadHtml(string filename, Dictionary<string, string> replacements)
        {
            if (!File.Exists(filename))
            {
                return "";
            }
            string html = File.ReadAllText(filename);
            foreach (var item in replacements)
            {
                html = html.Replace(item.Key, item.Value);
            }
            return html;
        }
    }
}
