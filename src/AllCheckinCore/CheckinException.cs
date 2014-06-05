using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllCheckin.Core
{
    [Serializable]

    public class CheckinException : ApplicationException
    {

        public CheckinException() { }
        public CheckinException(string message) : base(message) { }
        public CheckinException(string message, Exception inner) : base(message, inner) { }
    }
}
