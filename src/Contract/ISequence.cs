using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllCheckin.Contract
{
    public interface ISequence <T>
    {
        void Reset();

        void Seek(T position);

        T Current { get; }

        void MoveNext();

        bool EndOfSequence { get; }

        void Initialize();
    }
}
