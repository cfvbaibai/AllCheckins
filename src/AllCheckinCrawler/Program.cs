using AllCheckin.Contract;
using AllCheckin.Core;
using Cfvbaibai.CommonUtils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace AllCheckin.CrawlerCli
{
    public class Program
    {
        private class CrawlContext
        {
            public ISequence<string> Sequence { get; set; }
            public QueryType QueryType { get; set; }
            public int Pause { get; set; }
        }

        public static void Main(string[] args)
        {
            try
            {
                string sequenceType = args[0];
                string start = args[1];
                long max = long.Parse(args[2]);
                int pause = int.Parse(args[3]); // Pause interval

                if (args.Length < 4)
                {
                    throw new ArgumentException("Invalid number of arguments!");
                }

                CrawlContext context = GetCrawlContext(sequenceType, start, pause);
                Crawler crawler = CreateCrawler(context);

                crawler.Crawl(max, context.QueryType);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Trace.TraceError(e.ToString());
            }
        }

        private static Crawler CreateCrawler(CrawlContext context)
        {
            var crawler = new Crawler(context.Sequence);
            crawler.PauseInterval = context.Pause;

            if (context.QueryType == QueryType.Name)
            {
                ExtraSetupForNameBasedCrawler(crawler);
            }

            crawler.AfterCrawl += (sender, e) =>
            {
                var progress = e.Progress;
                var message = string.Format("[{0}/{1} keyword={2}] {3}",
                    progress.Current, progress.Total, progress.CurrentKeyword, progress.Message);
                if (e.Error == null)
                {
                    PowerConsole.Info(message);
                }
                else
                {
                    PowerConsole.Error(message);
                }
            };

            return crawler;
        }

        private static void ExtraSetupForNameBasedCrawler(Crawler crawler)
        {
            string recordFilePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "AllCheckinRandomNameRecord.txt");
            IDictionary<string, bool> processedKeywords = new Dictionary<string, bool>();

            if (File.Exists(recordFilePath))
            {
                using (StreamReader reader = new StreamReader(recordFilePath))
                {
                    string line = null;
                    while ((line = reader.ReadLine()) != null)
                    {
                        processedKeywords[line] = true;
                    }
                }
            }
            Console.WriteLine("{0} processed keywords found!", processedKeywords.Count);

            crawler.BeforeCrawl += (sender, e) =>
            {
                var keyword = e.Progress.CurrentKeyword;
                if (processedKeywords.ContainsKey(keyword))
                {
                    e.Cancel = true;
                    var progress = e.Progress;
                    PowerConsole.WarningF(
                        "[{0}/{1} keyword={2}] {3}",
                        progress.Current, progress.Total, progress.CurrentKeyword, progress.Message);
                }
                else
                {
                    processedKeywords[keyword] = true;
                }
            };

            crawler.AfterCrawl += (sender, e) =>
            {
                if (e.Error == null)
                {
                    File.AppendAllText(recordFilePath, e.Progress.CurrentKeyword + Environment.NewLine);
                }
            };
        }

        private static CrawlContext GetCrawlContext(string sequenceType, string start, int pause)
        {
            ISequence<string> sequence;
            QueryType queryType;
            if (sequenceType.Equals("NN"))
            {
                var innerSequence = new NaturalNumberSequence();
                innerSequence.Seek(long.Parse(start));
                sequence = new TextSequence<long>(innerSequence);
                queryType = QueryType.IdCardNumber;
            }
            else if (sequenceType.Equals("N"))
            {
                sequence = new NameSequence();
                sequence.Seek(start);
                queryType = QueryType.Name;
            }
            else if (sequenceType.Equals("RICN"))
            {
                sequence = new RandomIdCardNumberSequence();
                queryType = QueryType.IdCardNumber;
            }
            else if (sequenceType.Equals("RN"))
            {
                sequence = new RandomNameSequence();
                queryType = QueryType.Name;
            }
            else
            {
                throw new ArgumentException("Invalid sequence type: " + sequenceType);
            }
            return new CrawlContext
            {
                Sequence = sequence,
                QueryType = queryType,
                Pause = pause,
            };
        }
    }
}
