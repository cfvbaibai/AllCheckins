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
            Initialize();
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

        public virtual void Initialize()
        {
            // Do nothing
        }

        public void MoveNext()
        {
            this.current = GenerateItem();
        }

        public bool EndOfSequence
        {
            get { return false; }
        }
    }
}
