using AllCheckin.Contract;
using AllCheckin.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace AllCheckinCrawler
{
    class Program
    {
        static void Main(string[] args)
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

                ISequence <string> sequence = null;
                QueryType queryType = QueryType.Unknown;

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

                var crawler = new Crawler(sequence);
                crawler.PauseInterval = pause;
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
                crawler.AfterCrawl += (sender, e) =>
                {
                    var progress = e.Progress;
                    if (e.Error == null)
                    {
                        File.AppendAllText(recordFilePath, progress.CurrentKeyword + Environment.NewLine);
                    }
                    if (e.Error != null)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                    }
                    Console.WriteLine(
                        "[{0}/{1} keyword={2}] {3}",
                        progress.Current, progress.Total, progress.CurrentKeyword, progress.Message);
                    Console.ResetColor();
                };
                crawler.BeforeCrawl += (sender, e) =>
                {
                    var keyword = e.CurrentKeyword;
                    if (processedKeywords.ContainsKey(keyword))
                    {
                        e.Cancel = true;
                    }
                    else
                    {
                        processedKeywords[keyword] = true;
                    }
                };
                crawler.Crawl(max, queryType);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Trace.TraceError(e.ToString());
            }
        }
    }
}
