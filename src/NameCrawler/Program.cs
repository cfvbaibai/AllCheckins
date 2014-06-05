using AllCheckin.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace NameCrawler
{
    class Program
    {
        private static readonly string AssemblyDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private static readonly string FirstNameCharsFilePath = GetDeployedPath("FirstNameChars.txt");
        private static readonly string AppDataDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        private static readonly string ResultDir = Path.Combine(AppDataDir, @"Cfvbaibai\AllCheckin\NameResult");
        private static readonly string NamePattern = @"<div class=""name-list""><a href=""(?<Link>[^""]+)"">(?<Name>[^<]+)</a></div>";

        private static string GetDeployedPath(string relativePath)
        {
            return Path.Combine(AssemblyDir, relativePath);
        }
        static void Main(string[] args)
        {
            if (!Directory.Exists(ResultDir))
            {
                Directory.CreateDirectory(ResultDir);
            }

            var start = (args.Length == 0 ? "" : args[0]);
            var firstNameChars = File.ReadAllText(FirstNameCharsFilePath);

            int startIndex = 0;
            for (int i = 0; i < firstNameChars.Length; ++i)
            {
                if (firstNameChars[i].Equals(start))
                {
                    startIndex = i + 1;
                }
            }

            var resultFilePath = Path.Combine(ResultDir, "Names.csv");
            if (File.Exists(resultFilePath))
            {
                Console.WriteLine("Removing existing result file: {0}", resultFilePath);
                File.Delete(resultFilePath);
            }

            for (int i = startIndex; i < firstNameChars.Length; ++i)
            {
                using (var writer = new StreamWriter(resultFilePath, true, Encoding.UTF8))
                {
                    CrawlNames(writer, firstNameChars[i]);
                }
            }
        }

        private static void CrawlNames(StreamWriter writer, char firstNameChar)
        {
            WebRobot robot = new WebRobot();
            var secondNameUrl = "http://renlifang.msra.cn/namelist.aspx?f=" + HttpUtility.UrlEncode(firstNameChar.ToString());
            Console.WriteLine("[{0}] Loading from {1}...", firstNameChar, secondNameUrl);
            var response = robot.Get(secondNameUrl);
            // <div class="name-list"><a href="namelist.aspx?f=%e9%98%bf&s=%e9%98%bf">阿阿</a></div>
            var matches = Regex.Matches(response, NamePattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);

            foreach (Match match in matches)
            {
                var firstSecondName = match.Groups["Name"].Value;
                var relativeLink = match.Groups["Link"].Value;
                if (firstSecondName.Length != 2)
                {
                    Console.WriteLine("[{0}] Invalid FirstSecondName: {1}", firstSecondName + firstSecondName);
                    continue;
                }
                ExtractNames(writer, firstSecondName, relativeLink);
            }

            writer.Flush();
        }

        private static void ExtractNames(StreamWriter writer, string firstSecondName, string relativeLink)
        {
            var logPrefix = string.Format("[{0}][{1}]", firstSecondName[0], firstSecondName[1]);
            Console.WriteLine("=======================================================================", logPrefix);

            var namesUrl = "http://renlifang.msra.cn/" + relativeLink;
            Console.WriteLine("{0} Loading names from {1}...", logPrefix, namesUrl);
            WebRobot robot = new WebRobot();
            var nameRawHtml = robot.Get(namesUrl);

            var matches = Regex.Matches(nameRawHtml, NamePattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            foreach (Match match in matches)
            {
                var nameText = match.Groups["Name"].Value;
                Match nameMatch = Regex.Match(nameText, @"^(?<Name>[^(]+)\((?<Count>.+)\)$", RegexOptions.Compiled);
                if (nameMatch.Success)
                {
                    var name = nameMatch.Groups["Name"].Value;
                    var count = nameMatch.Groups["Count"].Value;
                    Console.WriteLine("{0} Name: {1}, Count: {2}", logPrefix, name, count);
                    writer.WriteLine(string.Format(
                        "{0},{1},{2},{3}", firstSecondName[0], firstSecondName[1], name, count));
                }
                else
                {
                    Console.WriteLine("{0} Invalid name: {1}", logPrefix, nameText);
                }
            }
        }
    }
}
