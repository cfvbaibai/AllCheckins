using AllCheckin.Contract;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace AllCheckin.Core
{
    public class CheckinEntryProvider : ICheckinEntryProvider
    {
        private WebRobot webRobot;
        private const string BaseServerUrl = "http://zhaokaifang.com";
        private const string QueryPageUrl = BaseServerUrl + "/q.php";
        private const string EntryLinePattern =
            @"<tr>\s*" +
                @"<td>\s*(?<Name>.*)\s*</td>\s*" +
                @"<td>\s*(?<Id>.*)\s*</td>\s*" +
                @"<td>\s*(?<Gendre>.*)\s*</td>\s*" +
                @"<td>\s*(?<Birthdate>.*)\s*</td>\s*" +
                @"<td>\s*(?<Address>.*)\s*</td>\s*" +
                @"<td>\s*(?<CellPhone>.*)\s*</td>\s*" +
                @"<td>\s*(?<TelePhone>.*)\s*</td>\s*" +
                @"<td>\s*(?<Mailbox>.*)\s*</td>\s*" +
                @"<td>\s*(?<CheckinTime>.*)\s*</td>\s*" +
            @"</tr>\s*";

        public CheckinEntryProvider()
        {
            this.webRobot = new WebRobot();
        }

        public void Connect()
        {
            try
            {
                var response = this.webRobot.Get(BaseServerUrl);
            }
            catch (Exception e)
            {
                throw new CheckinException("Cannot connect to " + BaseServerUrl + "!", e);
            }
        }

        public IEnumerable<CheckinEntry> GetEntries(QueryType queryType, string id)
        {
            string queryString = "";
            if (queryType == QueryType.Name)
            {
                queryString = "name=" + id + "&idcard=";
            }
            else if (queryType == QueryType.IdCardNumber)
            {
                queryString = "name=&idcard=" + id;
            }
            else
            {
                throw new ArgumentException("Invalid query type: " + queryType);
            }
            var response = webRobot.PostAjax(QueryPageUrl, queryString);
            /* response:
             * <table cellspacing="0" cellpadding="0" class="tb2">
             * <tr><th>姓名</th><th>证件</th><th>性别</th><th>生日</th><th>地址</th><th>手机</th><th>电话</th><th>邮箱</th><th>开房时间</th></tr>
             * <tr><td>周蓉琦</td><td>3</td><td>女</td><td></td><td>南大街14弄12号204-206室</td><td></td><td></td><td></td><td>2012-11-23 2:31:07</td></tr>
             * </table>
             */
            var result = new List<CheckinEntry>();
            int iFirstTr = response.IndexOf("<tr>");
            if (iFirstTr < 0) { return result; }
            int iTrBegin = response.IndexOf("<tr>", iFirstTr + 4);
            var lines = new List<string>();
            while (iTrBegin >= 0)
            {
                int iTrEnd = response.IndexOf("</tr>", iTrBegin);
                if (iTrEnd < 0)
                {
                    lines.Add((response + "</tr>").Substring(iTrBegin));
                    iTrEnd = response.Length;
                }
                else
                {
                    lines.Add(response.Substring(iTrBegin, iTrEnd + 5 - iTrBegin));
                }
                iTrBegin = response.IndexOf("<tr>", iTrEnd + 5);
            }
            for (int i = 0; i < lines.Count; ++i)
            {
                var line = lines[i].ToString();
                var match = Regex.Match(line, EntryLinePattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    CheckinEntry entry = ParseCheckinEntryFromRegexMatch(match);
                    result.Add(entry);
                }
                else
                {
                    Trace.TraceError("[id = {0}] Cannot parse entry line: {1}", id, line);
                }
            }
            return result;
        }

        private CheckinEntry ParseCheckinEntryFromRegexMatch(Match match)
        {
            var entry = new CheckinEntry();
            entry.Name = match.Groups["Name"].Value;
            entry.Id = match.Groups["Id"].Value;
            var gendreText = match.Groups["Gendre"].Value;
            if (string.Equals("男", gendreText))
            {
                entry.Gendre = Gendre.Male;
            }
            else if (string.Equals("女", gendreText))
            {
                entry.Gendre = Gendre.Female;
            }
            else
            {
                entry.Gendre = Gendre.Other;
            }
            var birthdateText = match.Groups["Birthdate"].Value;
            DateTime birthdate;
            if (DateTime.TryParseExact(
                birthdateText, "yyyyMMdd", CultureInfo.InvariantCulture,
                DateTimeStyles.None, out birthdate))
            {
                entry.Birthdate = birthdate;
            }
            entry.Address = match.Groups["Address"].Value;
            entry.CellPhoneNumber = match.Groups["CellPhone"].Value;
            entry.TelephoneNumber = match.Groups["TelePhone"].Value;
            entry.Mailbox = match.Groups["Mailbox"].Value;
            DateTime checkinTime;
            if (DateTime.TryParse(match.Groups["CheckinTime"].Value, out checkinTime))
            {
                entry.CheckinTime = checkinTime;
            }
            return entry;
        }

        public void Dispose()
        {
            // Nothing...
        }
    }
}
