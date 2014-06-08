using AllCheckin.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllCheckin.Core
{
    public class IdCardNumber
    {
        private static readonly int[] Weights = new int[] {
            7, 9, 10, 5, 8, 4, 2, 1, 6, 3, 7, 9, 10, 5, 8, 4, 2,
        };
        private static readonly int[] ChecksumMapping = new int[] {
            1, 0, 10, 9, 8, 7, 6, 5, 4, 3, 2,
        };

        public int RegionNumber;
        public DateTime Birthdate;
        public int OrderId;
        public int Checksum
        {
            get
            {
                var effectiveNumber = EffectiveNumber;
                int sum = 0;
                for (int i = 0; i < 17; ++i)
                {
                    int a = int.Parse(effectiveNumber[i] == 'X' ? "11" : effectiveNumber[i].ToString());
                    int w = Weights[i];
                    sum += a * w;
                }
                return ChecksumMapping[sum % 11];
            }
        }

        private string EffectiveNumber
        {
            get
            {
                return
                    RegionNumber.ToString() +
                    Birthdate.ToString("yyyyMMdd") +
                    OrderId.ToString().PadLeft(3, '0');
            }
        }

        public string FullNumber
        {
            get
            {
                return EffectiveNumber + (Checksum == 10 ? "X" : Checksum.ToString());
            }
        }
    }

    public class RandomIdCardNumberSequence : RandomSequence<string>
    {
        private static Random random = new Random();

        public override string Current
        {
            get
            {
                int regionCodeIndex = random.Next(RegionCode.FullList.Count);
                int regionCode = RegionCode.FullList[regionCodeIndex].Code;
                DateTime birthdate = GetRandomTime(new DateTime(1960, 1, 1), new DateTime(1997, 1, 1));
                int orderId = GetRandomOrderId();
                IdCardNumber number = new IdCardNumber { Birthdate = birthdate, RegionNumber = regionCode, OrderId = orderId };
                return number.FullNumber;
            }
        }

        private static int GetRandomOrderId()
        {
            // y = a + k / (x - b)
            // a = -332.33333
            // k = -444.44444
            // b = -1.33333
            const double orderIdA = -332.0 + 1.0 / 3.0;
            const double orderIdK = -444.0 + 4.0 / 9.0;
            const double orderIdB = -4.0 / 3.0;
            double orderIdY = orderIdA + orderIdK / (random.NextDouble() + orderIdB);
            int orderId = Convert.ToInt32(orderIdY);
            if (orderId > 999) { orderId = 999; }
            return orderId;
        }

        private static DateTime GetRandomTime(DateTime time1, DateTime time2)
        {
            DateTime minTime = new DateTime();
            DateTime maxTime = new DateTime();

            System.TimeSpan ts = new System.TimeSpan(time1.Ticks - time2.Ticks);

            // 获取两个时间相隔的秒数
            double dTotalSecontds = ts.TotalSeconds;
            int iTotalSecontds = 0;

            if (dTotalSecontds > System.Int32.MaxValue)
            {
                iTotalSecontds = System.Int32.MaxValue;
            }
            else if (dTotalSecontds < System.Int32.MinValue)
            {
                iTotalSecontds = System.Int32.MinValue;
            }
            else
            {
                iTotalSecontds = (int)dTotalSecontds;
            }

            if (iTotalSecontds > 0)
            {
                minTime = time2;
                maxTime = time1;
            }
            else if (iTotalSecontds < 0)
            {
                minTime = time1;
                maxTime = time2;
            }
            else
            {
                return time1;
            }

            int maxValue = iTotalSecontds;

            if (iTotalSecontds <= System.Int32.MinValue)
                maxValue = System.Int32.MinValue + 1;

            int i = random.Next(System.Math.Abs(maxValue));

            return minTime.AddSeconds(i);
        }
    }
}
