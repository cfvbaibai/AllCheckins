using AllCheckin.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllCheckin.Core
{
    public abstract class RandomSequence<T> : ISequence<T>
    {
        public void Reset()
        {
            // Do nothing
        }

        public void Seek(T position)
        {
            // Do nothing
        }

        public abstract T Current { get; }

        public void MoveNext()
        {
            // Do nothing
        }

        public bool EndOfSequence
        {
            get { return false; }
        }
    }
}
