using AllCheckin.Contract;
using AllCheckin.DB;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace AllCheckin.Core
{
    public class CrawlProgress
    {
        public long Total { get; set; }
        public long Current { get; set; }
        public string CurrentKeyword { get; set; }
        public string Message { get; set; }
    }

    public class BeforeCrawlEventArgs : EventArgs
    {
        public string CurrentKeyword { get; set; }
        public bool Cancel { get; set; }
    }

    public class AfterCrawlEventArgs : EventArgs
    {
        public Exception Error { get; set; }
        public CrawlProgress Progress { get; set; }
    }

    public class Crawler
    {
        private ISequence<string> keywordSequence;

        public event EventHandler<BeforeCrawlEventArgs> BeforeCrawl;
        public event EventHandler<AfterCrawlEventArgs> AfterCrawl;

        public int PauseInterval { get; set; }

        public Crawler(ISequence<string> keywordSequence)
        {
            this.keywordSequence = keywordSequence;
        }

        public void Crawl(long maxStep, QueryType queryType)
        {
            long currentStep = 0;
            using (ICheckinEntryProvider checkinEntryProvider = new CheckinEntryProvider())
            using (IAllCheckinStorageProvider storageProvider = new AllCheckinSqlStorageProvider())
            {
                checkinEntryProvider.Connect();
                while (currentStep < maxStep && !keywordSequence.EndOfSequence)
                {
                    var keyword = keywordSequence.Current;
                    try
                    {
                        if (BeforeCrawl != null)
                        {
                            var args = new BeforeCrawlEventArgs
                            {
                                Cancel = false,
                                CurrentKeyword = keyword,
                            };
                            BeforeCrawl.Invoke(this, args);
                            if (args.Cancel)
                            {
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.WriteLine("{0} already processed. Skip!", args.CurrentKeyword);
                                Console.ResetColor();
                                continue;
                            }
                        }
                        var entries = checkinEntryProvider.GetEntries(queryType, keyword).ToList();
                        foreach (var entry in entries)
                        {
                            storageProvider.SaveEntry(entry);
                        }
                        if (AfterCrawl != null)
                        {
                            AfterCrawl.Invoke(this, new AfterCrawlEventArgs
                            {
                                Progress = new CrawlProgress
                                {
                                    Current = currentStep,
                                    Total = maxStep,
                                    CurrentKeyword = keyword,
                                    Message = entries.Count + " entry(s) updated."
                                }
                            });
                        }
                    }
                    catch (Exception e)
                    {
                        Trace.TraceError("[keyword = {0}] {1}", keywordSequence.Current, e);
                        if (AfterCrawl != null)
                        {
                            AfterCrawl.Invoke(this, new AfterCrawlEventArgs {
                                Error = e,
                                Progress = new CrawlProgress
                                {
                                    Current = currentStep,
                                    Total = maxStep,
                                    CurrentKeyword = keyword,
                                    Message = "Error!"
                                }
                            });
                        }
                    }

                    ++currentStep;
                    keywordSequence.MoveNext();
                    Thread.Sleep(PauseInterval);
                }
            }
        }
    }
}
