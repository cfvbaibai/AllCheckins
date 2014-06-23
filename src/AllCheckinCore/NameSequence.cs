using AllCheckin.Contract;
using AllCheckin.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllCheckin.Core
{
    public class NameSequence : ISequence <string>
    {
        private IList<string> names;
        private int current;

        public NameSequence()
        {
            using (var storageProvider = new AllCheckinSqlStorageProvider())
            {
                names = storageProvider.GetNames();
            }
        }

        public void Initialize()
        {
            Reset();
        }

        public void Reset()
        {
            current = 0;
        }

        public void Seek(string position)
        {
            int iPosition = names.IndexOf(position);
            if (iPosition >= 0)
            {
                current = iPosition;
                return;
            }
        }

        public string Current
        {
            get
            {
                if (EndOfSequence)
                {
                    throw new InvalidOperationException("No more elements!");
                }
                return names[current];
            }
        }

        public void MoveNext()
        {
            if (EndOfSequence)
            {
                throw new InvalidOperationException("No more elements!");
            }
            ++current;
        }

        public bool EndOfSequence
        {
            get { return current >= names.Count; }
        }
    }
}
