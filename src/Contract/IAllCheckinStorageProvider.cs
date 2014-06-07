using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllCheckin.Contract
{
    public interface IAllCheckinStorageProvider : IDisposable
    {
        void SaveEntry(CheckinEntry entry);

        IList<string> GetNames();

        IList<string> GetNamesOrderByCount();

        IList<string> GetGivenNames();
        IList<SurName> GetSurNames();
        bool IsNameQueried(string name);
    }
}
