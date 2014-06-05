using AllCheckin.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllCheckin.Core
{
    public class NaturalNumberSequence : ISequence<long>
    {
        private long current;
        private bool endOfSequence;

        public NaturalNumberSequence()
        {
            Reset();
        }

        public void Reset()
        {
            current = 1;
            endOfSequence = false;
        }

        public void Seek(long position)
        {
            if (position <= 0) { throw new ArgumentException("Invalid position: " + position); }
            current = position;
            endOfSequence = false;
        }

        public long Current
        {
            get { return current; }
        }

        public void MoveNext()
        {
            if (endOfSequence)
            {
                throw new InvalidOperationException("No more elements!");
            }
            if (current == Int64.MaxValue)
            {
                endOfSequence = true;
            }
            ++current;
        }

        public bool EndOfSequence
        {
            get { return endOfSequence; }
        }
    }

    public class TextSequence <T> : ISequence <string>
    {
        private ISequence<T> innerSequence;

        public TextSequence(ISequence<T> innerSequence)
        {
            this.innerSequence = innerSequence;
        }
        public void Reset()
        {
            innerSequence.Reset();
        }

        public void Seek(string position)
        {
            throw new NotSupportedException();
        }

        public string Current
        {
            get { return innerSequence.Current.ToString(); }
        }

        public void MoveNext()
        {
            innerSequence.MoveNext();
        }

        public bool EndOfSequence
        {
            get { return innerSequence.EndOfSequence; }
        }
    }
}
