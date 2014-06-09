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
        private T current;

        public RandomSequence()
        {
            Reset();
        }
        public void Reset()
        {
            this.current = GenerateItem();
        }

        public void Seek(T position)
        {
            // Do nothing
        }

        public T Current { get { return current; } }

        public abstract T GenerateItem();

        public void MoveNext()
        {
            GenerateItem();
        }

        public bool EndOfSequence
        {
            get { return false; }
        }
    }
}
