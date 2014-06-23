using AllCheckin.Contract;
using AllCheckin.DB;
using Cfvbaibai.CommonUtils;
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

        public RandomNameSequence(int givenNameLength)
        {
            using (var storageProvider = new AllCheckinSqlStorageProvider())
            {
                givenNames = storageProvider.GetGivenNames(givenNameLength);
                surNames = storageProvider.GetSurNames();
            }
        }

        public override string GenerateItem()
        {
            var currentSurName = surNames.WeightGet(surName => surName.Weight);
            var currentGivenName = givenNames.WeightGet(givenName => 1);
            var current = currentSurName.Chinese + currentGivenName;
            return current;
        }
    }
}
