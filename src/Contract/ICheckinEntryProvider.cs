using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllCheckin.Contract
{
    public interface ICheckinEntryProvider : IDisposable
    {
        void Connect();

        IEnumerable<CheckinEntry> GetEntries(QueryType queryType, string id);
    }
}
