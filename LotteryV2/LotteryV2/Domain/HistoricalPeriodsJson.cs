using System.Collections.Generic;
using System;

namespace LotteryV2.Domain
{
    public class HistoricalPeriodsJson
    {
        public DateTime DrawingDate { get; set; }
        public int[] Numbers { get; set; }
        public string KeyString { get; set; }
        public KeyValuePair<string,FingerPrint>[] JsonHistoricalFingerPrints { get; set; }

        public HistoricalPeriodsJson()
        {

        }
    }
}
