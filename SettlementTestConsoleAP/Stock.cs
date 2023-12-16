using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SettlementTestConsoleAP
{
    public class Stock
    {
        public string Id { get; set; } = string.Empty;
        //public string UserId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public decimal Winloss { get; set; }
        public float Odd { get; set; }
        public bool IsWin { get; set; } = false;
        public bool Settled { get; set; } = false;
    }
}
