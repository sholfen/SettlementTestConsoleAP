using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SettlementTestConsoleAP
{
    public class TempStockData
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public decimal Amount { get; set; } = 0;   
        public decimal Winloss { get; set; } = 0;
    }
}
