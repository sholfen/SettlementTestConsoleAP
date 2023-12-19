using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SettlementTestConsoleAP
{
    public class SettlementManager
    {
        public static SettlementManager Instance = new SettlementManager();

        private SettlementLib _settlementLib;

        private SettlementManager() 
        {
            _settlementLib = new SettlementLib();
            _sqlConnection = new SqlConnection(SQLConnectionString);
        }

        private SqlConnection _sqlConnection = null;
        public static string SQLConnectionString;

        public async Task StartFlow()
        {
            _sqlConnection.ConnectionString = SQLConnectionString;
            _sqlConnection.Open();
            var list = await _settlementLib.PullDatas(_sqlConnection);
            await _settlementLib.HandleStockList(list);
            await _settlementLib.UpdateWinloss();
            Task.WaitAll();
            await _settlementLib.UpdateDatas(_sqlConnection);
            _sqlConnection.Close();
        }
    }
}
