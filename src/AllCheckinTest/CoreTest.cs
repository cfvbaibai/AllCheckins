using AllCheckin.Contract;
using AllCheckin.Core;
using AllCheckin.DB;
using Cfvbaibai.CommonUtils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;

namespace AllCheckinTest
{
    [TestClass]
    public class CoreTest
    {
        [TestMethod]
        public void TestGetEntry()
        {
            using (ICheckinEntryProvider provider = new CheckinEntryProvider())
            {
                provider.Connect();
                var entries = provider.GetEntries(QueryType.IdCardNumber, "1");
                Console.WriteLine("{0} entry(s) found!", entries.Count());
                foreach (var entry in entries)
                {
                    Console.WriteLine("entry found: ");
                    Console.WriteLine(ObjectDumper.ShallowDumpProperties(entry, 4));
                }
            }
        }

        [TestMethod]
        public void TestRobot()
        {
            WebRobot robot = new WebRobot();
            robot.IsAjax = true;
            var response = robot.Post("http://zhaokaifang.com/q.php", "application/x-www-form-urlencoded", "name=&idcard=1");
            Console.WriteLine(response);
        }

        [TestMethod]
        public void TestStorage()
        {
            using (IAllCheckinStorageProvider storageProvider = new AllCheckinSqlStorageProvider())
            {
                CheckinEntry entry = new CheckinEntry();
                entry.Id = "111222";
                entry.Name = "xxxxx";
                entry.TelephoneNumber = "3214323";
                storageProvider.SaveEntry(entry);

                storageProvider.SaveEntry(entry);
                entry.Id = "2222222";
                storageProvider.SaveEntry(entry);
            }
            
        }

        [TestMethod]
        public void TestCrawler()
        {
            var innerSequence = new NaturalNumberSequence();
            innerSequence.Seek(9);
            var sequence = new TextSequence<long>(innerSequence);
            var crawler = new Crawler(sequence);
            crawler.AfterCrawl += (sender, e) =>
            {
                var progress = e.Progress;
                Console.WriteLine(
                    "[{0}/{1} keyword={2}] {3}",
                    progress.Current, progress.Total, progress.CurrentKeyword, progress.Message);
            };

            crawler.Crawl(3, QueryType.IdCardNumber);
        }

        [TestMethod]
        public void TestCrawlerWithIdCardNumberSequence()
        {
            var sequence = new RandomIdCardNumberSequence();
            var crawler = new Crawler(sequence);
            crawler.AfterCrawl += (sender, e) =>
            {
                var progress = e.Progress;
                Console.WriteLine(
                    "[{0}/{1} keyword={2}] {3}",
                    progress.Current, progress.Total, progress.CurrentKeyword, progress.Message);
            };
            crawler.Crawl(100, QueryType.IdCardNumber);
        }

        [TestMethod]
        public void DateTimeParseTest()
        {
            var birthdateText = "19990921";
            var dateTime = DateTime.ParseExact(
                birthdateText, "yyyyMMdd", CultureInfo.InvariantCulture,
                DateTimeStyles.None);
            Console.WriteLine(dateTime.ToString("yyyy-MM-dd HH:mm:ss"));
        }

        [TestMethod]
        public void TestMaxIdCard()
        {
            long id = 310109198310211017L;
            long maxLong = long.MaxValue;
            Console.WriteLine(id);
            Console.WriteLine(maxLong);
            Console.WriteLine(maxLong - id);
        }

        [TestMethod]
        public void TestIdCardNumber()
        {
            IdCardNumber idCardNumber = new IdCardNumber
            {
                OrderId = 101,
                RegionNumber = 310109,
                Birthdate = new DateTime(1983, 10, 21),
            };
            Assert.AreEqual("310109198310211017", idCardNumber.FullNumber);
        }

        [TestMethod]
        public void TestRandomIdCardNumberSequence()
        {
            var sequence = new RandomIdCardNumberSequence();
            for (int i = 0; i < 100; ++i)
            {
                Console.WriteLine(sequence.Current);
            }
        }

        [TestMethod]
        public void TestNameSequence()
        {
            var sequence = new NameSequence();
            sequence.Seek("聂");
            for (int i = 0; i < 100 && !sequence.EndOfSequence; ++i, sequence.MoveNext())
            {
                Console.WriteLine(sequence.Current);
            }
        }

        [TestMethod]
        public void TestNameQueried()
        {
            using (var storageProvider = new AllCheckinSqlStorageProvider())
            {
                Assert.IsFalse(storageProvider.IsNameQueried("王冉"));
                Assert.IsFalse(storageProvider.IsNameQueried("张三"));
                Assert.IsTrue(storageProvider.IsNameQueried("张帅"));
            }
        }

        [TestMethod]
        public void TestGetGivenNames()
        {
            using (var storageProvider = new AllCheckinSqlStorageProvider())
            {
                Stopwatch watch = new Stopwatch();
                watch.Start();
                try
                {
                    var givenNames = storageProvider.GetGivenNames();
                    Console.WriteLine("Total {0} given names.", givenNames.Count);
                }
                finally
                {
                    watch.Stop();
                    Console.WriteLine("Time Cost: {0} ms", watch.Elapsed.TotalMilliseconds);
                }
            }
        }

        [TestMethod]
        public void TestGetSurNames()
        {
            using (var storageProvider = new AllCheckinSqlStorageProvider())
            {
                var surNames = storageProvider.GetSurNames();
                var totalWeight = surNames.Sum(surName => surName.Weight);
                Console.WriteLine("Total Weight: {0}", totalWeight);
                Console.WriteLine("Max Int: {0}", Int32.MaxValue);
                Console.WriteLine("================================");
                foreach (var surName in surNames)
                {
                    Console.WriteLine("{0,-16}, {1,-16}, {2}", surName.Id, surName.Chinese, surName.Weight);
                }
            }
        }

        [TestMethod]
        public void TestWeightGet()
        {
            List<SurName> list = new List<SurName>();
            list.Add(new SurName { Id = "0-9", Weight = 10, });
            list.Add(new SurName { Id = "10-14", Weight = 5, });
            list.Add(new SurName { Id = "15-24", Weight = 10, });
            list.Add(new SurName { Id = "25-25", Weight = 1, });
            list.Add(new SurName { Id = "26-26", Weight = 1, });
            Assert.AreEqual("0-9", list.WeightGet(surName => surName.Weight, totalWeight => 0).Id);
            Assert.AreEqual("0-9", list.WeightGet(surName => surName.Weight, totalWeight => 1).Id);
            Assert.AreEqual("0-9", list.WeightGet(surName => surName.Weight, totalWeight => 3).Id);
            Assert.AreEqual("0-9", list.WeightGet(surName => surName.Weight, totalWeight => 9).Id);
            Assert.AreEqual("10-14", list.WeightGet(surName => surName.Weight, totalWeight => 10).Id);
            Assert.AreEqual("10-14", list.WeightGet(surName => surName.Weight, totalWeight => 12).Id);
            Assert.AreEqual("10-14", list.WeightGet(surName => surName.Weight, totalWeight => 14).Id);
            Assert.AreEqual("15-24", list.WeightGet(surName => surName.Weight, totalWeight => 15).Id);
            Assert.AreEqual("15-24", list.WeightGet(surName => surName.Weight, totalWeight => 20).Id);
            Assert.AreEqual("15-24", list.WeightGet(surName => surName.Weight, totalWeight => 24).Id);
            Assert.AreEqual("25-25", list.WeightGet(surName => surName.Weight, totalWeight => 25).Id);
            Assert.AreEqual("26-26", list.WeightGet(surName => surName.Weight, totalWeight => 26).Id);
        }

        [TestMethod]
        public void TestRandomNameSequence()
        {
            var sequence = new RandomNameSequence();
            for (int i = 0; i < 100; ++i)
            {
                Console.WriteLine(sequence.Current);
            }
        }
    }
}
