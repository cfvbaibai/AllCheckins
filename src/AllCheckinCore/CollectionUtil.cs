using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllCheckin.Core
{
    public static class CollectionUtil
    {
        public static T WeightGet<T> (this IList <T> list, Func <T, int> weighter)
        {
            return list.WeightGet(weighter, totalWeight => R.Next(0, totalWeight));
        }

        public static T WeightGet<T> (this IList <T> list, Func <T, int> weighter, Func<int, int> tester)
        {
            if (list.Count == 0)
            {
                return default(T);
            }
            if (list.Count == 1)
            {
                return list[0];
            }
            int totalWeight = list.Sum(weighter);
            int test = tester(totalWeight);
            int w0 = weighter(list[0]);
            if (test < w0)
            {
                return list[0];
            }
            for (int n = 1, k = w0; n < list.Count; ++n)
            {
                int wN = weighter(list[n]);
                if (test >= k && test < k + wN)
                {
                    return list[n];
                }
                k += wN;
            }
            throw new ApplicationException("Should not reach here! Please verify the tester is valid.");
        }
    }
}
