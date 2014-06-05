using AllCheckin.Contract;
using AllCheckin.DB;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AllCheckin.Core
{
    public class CrawlProgress
    {
        public long Total { get; set; }
        public long Current { get; set; }
        public string CurrentKeyword { get; set; }
        public string Message { get; set; }
    }
    public class Crawler
    {
        private ISequence<string> keywordSequence;

        public int PauseInterval { get; set; }

        public Crawler(ISequence<string> keywordSequence)
        {
            this.keywordSequence = keywordSequence;
        }

        public void Crawl(long maxStep, QueryType queryType, Action<CrawlProgress> onCrawled)
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
                        var entries = checkinEntryProvider.GetEntries(queryType, keyword).ToList();
                        foreach (var entry in entries)
                        {
                            storageProvider.SaveEntry(entry);
                        }
                        if (onCrawled != null)
                        {
                            onCrawled.Invoke(new CrawlProgress
                            {
                                Current = currentStep,
                                Total = maxStep,
                                CurrentKeyword = keyword,
                                Message = entries.Count + " entry(s) updated."
                            });
                        }
                    }
                    catch (Exception e)
                    {
                        Trace.TraceError("[keyword = {0}] {1}", keywordSequence.Current, e);
                        if (onCrawled != null)
                        {
                            onCrawled.Invoke(new CrawlProgress
                            {
                                Current = currentStep,
                                Total = maxStep,
                                CurrentKeyword = keyword,
                                Message = "Error!"
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
