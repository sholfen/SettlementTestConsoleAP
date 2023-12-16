using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Timers;
namespace SettlementTestConsoleAP
{
    public class SettlementLib
    {
        private ConcurrentDictionary<string, decimal> _memberWinloss;
        private ConcurrentQueue<Stock> _queue;
        private bool _continue = false;
        private System.Timers.Timer _timer;

        public SettlementLib() 
        {
            _memberWinloss = new ConcurrentDictionary<string, decimal>();
            _queue = new ConcurrentQueue<Stock>();
            _timer = new System.Timers.Timer(1000);
            _timer.Elapsed += _timer_Elapsed;
            //_timer.AutoReset = true;
            //_timer.Enabled = true;

            //timer.Elapsed += async ( sender, e ) => await HandleTimer();
            //https://earn.microsoft.com/zh-tw/dotnet/api/system.timers.timer?view=net-7.0
        }

        private void _timer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            Console.WriteLine("_timer_Elapsed");
        }

        public async Task UpdateDatas(SqlConnection sqlConnection)
        {
            string tableName = "StockTemp";
            List<TempStockData> data = new List<TempStockData>();
            foreach (var item in _memberWinloss)
            {
                TempStockData tempStockData = new TempStockData
                { 
                    Id = item.Key,
                    Name = item.Key,
                    Amount = 0,
                    Winloss = item.Value
                };
                data.Add(tempStockData);
            }
            var table = DBTool.CreateDataTable(data);
            //var table = DBTool.CreateDataTable2(data);
            DBTool.SqlBulkCopy(sqlConnection, table, tableName);

            //exec db SP
            var result = await sqlConnection.QueryAsync("dbo.UpdateWinlossDatasToUsers", commandType: CommandType.StoredProcedure);
            //var results = _sqlConnection.Query("dbo.H5NewUserLogin2022", new { P1 = userName, P2 = password, P3 = userIP, P4 = hwType, P5 = swVer, P6 = token, P7 = companyID },
            //        commandType: CommandType.StoredProcedure);
        }

        public async Task UpdateWinloss()
        {
            //_continue = true;
            bool flag = true;
            while (flag)
            {
                if (_queue.TryDequeue(out Stock stock))
                {
                    if(!_memberWinloss.ContainsKey(stock.Id))
                    {
                        _memberWinloss.TryAdd(stock.Id, 0);
                    }
                    _memberWinloss[stock.Id] += stock.Winloss;
                }
                else 
                {
                    flag = false; 
                }
            }
            await Task.Delay(100);
        }

        public async Task HandleStockList(List<Stock> stocks)
        {
            //foreach (Stock stock in stocks) 
            //{
            //    if(stock.IsWin)
            //    {
            //        stock.Winloss = (decimal)stock.Odd * stock.Amount;
            //    }
                
            //    _queue.Enqueue(stock);
            //    await Task.Delay(100);
            //}
            Parallel.ForEach(stocks, stock =>
            {
                if (stock.IsWin)
                {
                    stock.Winloss = (decimal)stock.Odd * stock.Amount;
                }

                _queue.Enqueue(stock);
            });
            await Task.Delay(100);
            _continue = false;
        }

        public async Task<List<Stock>> PullDatas(SqlConnection sqlConnection)
        {
            var data = new List<Stock>();
            //exec db SP
            var result = await sqlConnection.QueryAsync<Stock>("dbo.PullUnsettleStocks", commandType: CommandType.StoredProcedure);
            foreach (Stock stock in result)
            {
                data.Add(stock);
            }
            return data;
        }
    }
}
