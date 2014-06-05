using AllCheckin.Contract;
using AllCheckin.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                int pause = int.Parse(args[3]);

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
                else
                {
                    throw new ArgumentException("Invalid sequence type: " + sequenceType);
                }
                var crawler = new Crawler(sequence);
                crawler.PauseInterval = pause;
                crawler.Crawl(max, queryType, progress =>
                {
                    Console.WriteLine(
                        "[{0}/{1} keyword={2}] {3}",
                        progress.Current, progress.Total, progress.CurrentKeyword, progress.Message);
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Trace.TraceError(e.ToString());
            }
        }
    }
}
