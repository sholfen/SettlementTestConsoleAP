// See https://aka.ms/new-console-template for more information

using Microsoft.Data.SqlClient;
using SettlementTestConsoleAP;
using System.Collections.Concurrent;
using System.Data;
using System.Diagnostics;

string connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=BulkCopyDemo;Integrated Security=false;Trusted_Connection=true;";

//insert members
//InsertMembers();

//insert stocks
//InsertStocks();
SettlementManager settlementManager = SettlementManager.Instance;
SettlementManager.SQLConnectionString = connectionString;
await settlementManager.StartFlow();
Task.WaitAll();

//TestConcurrentDictionaryLimit();

void InsertMembers()
{
    SqlConnection sqlConnection=new SqlConnection(connectionString);
    sqlConnection.Open();

    List<Member> members = new List<Member>();
    int max = 10000;
    for (int i = 0; i < max; i++)
    {
        string id = Guid.NewGuid().ToString();
        Member member = new Member()
        {
            Id = id,
            Name = id,
            Credit = i,
        };
        members.Add(member);
    }

    DataTable dt = DBTool.CreateDataTable<Member>(members);
    DBTool.SqlBulkCopy(sqlConnection, dt, "Member");

    sqlConnection.Close();
}

void InsertStocks()
{
    SqlConnection sqlConnection = new SqlConnection(connectionString);
    var list = DBTool.GetAllMembers(sqlConnection);
    List<Stock> stocks = new List<Stock>();
    foreach (Member member in list)
    {
        for (int i = 0; i < 5; ++i)
        {
            string id = member.Id;
            Stock stock = new Stock
            {
                Amount = 10,
                Id = id,
                Name = member.Name,
                IsWin = true,
                Odd = 1.7f,
                Settled = false,
                Winloss = 0,
            };
            stocks.Add(stock);
        }
    }

    sqlConnection = new SqlConnection(connectionString);
    sqlConnection.Open();
    DataTable dt = DBTool.CreateDataTable<Stock>(stocks);
    DBTool.SqlBulkCopy(sqlConnection, dt, "Stock");
    sqlConnection.Close();
}

void TestConcurrentDictionaryLimit()
{
    List<Stock> stockList1 = new List<Stock>
    {
        new Stock{ Id="Peter", Name="Peter", Amount=-7 },
    };

    List<Stock> stockList2 = new List<Stock>
    {
        new Stock{ Id="Peter", Name="Peter", Amount=-7 },
    };

    List<Stock> stockList3 = new List<Stock>
    {
        new Stock{ Id="Peter", Name="Peter", Amount=-7 },
    };

    int max = 10_000_000;
    ConcurrentDictionary<string, int> dic = new ConcurrentDictionary<string, int>();
    Stopwatch stopwatch = Stopwatch.StartNew();
    for (int i = 0; i < max; i++)
    {
        string guid = Guid.NewGuid().ToString();
        dic.TryAdd(guid, i);
    }
    stopwatch.Stop();
    Console.WriteLine($"Dictionary total Time: {stopwatch.ElapsedMilliseconds}ms");

    stopwatch.Reset();
    stopwatch.Start();
    foreach (string guid in dic.Keys)
    {
        Random rnd = new Random();
        dic[guid] += rnd.Next();
    }
    stopwatch.Stop();
    Console.WriteLine($"Dictionary searching total Time: {stopwatch.ElapsedMilliseconds}ms");
}