using AllCheckin.Contract;
using AllCheckin.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllCheckin.Core
{
    public class RandomNameSequence : RandomSequence <string>
    {
        private IList<SurName> surNames;
        private IList<string> givenNames;

        public RandomNameSequence()
        {
            using (var storageProvider = new AllCheckinSqlStorageProvider())
            {
                givenNames = storageProvider.GetGivenNames();
                surNames = storageProvider.GetSurNames();
            }
            Reset();
        }

        public override string Current
        {
            get
            {
                while (true)
                {
                    var currentSurName = surNames.WeightGet(surName => surName.Weight);
                    var currentGivenName = givenNames.WeightGet(givenName => 1);
                    var current = currentSurName.Chinese + currentGivenName;
                    return current;
                }
            }
        }
    }
}
