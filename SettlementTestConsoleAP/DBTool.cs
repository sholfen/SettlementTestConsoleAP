using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SettlementTestConsoleAP
{
    public class DBTool
    {
        public static void SqlBulkCopy(SqlConnection sqlConnection, DataTable dt, string tableName)
        {
            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(sqlConnection))
            {
                bulkCopy.DestinationTableName = tableName;

                bulkCopy.WriteToServer(dt);
            }
        }

        public static DataTable CreateDataTable<T>(List<T> list)
        {
            DataTable dt = new DataTable();
            List<PropertyInfo> properties = new List<PropertyInfo>();

            int ordinal = 0;
            foreach (PropertyInfo propertyInfo in typeof(T).GetProperties())
            {
                DataColumn dc = new DataColumn();
                dc.ColumnName = propertyInfo.Name;
                dc.DataType = propertyInfo.PropertyType;
                
                //dt.Columns.Add(propertyInfo.Name, propertyInfo.PropertyType);
                dt.Columns.Add(dc);
                properties.Add(propertyInfo);
                dc.SetOrdinal(ordinal);

                ordinal++;
            }

            foreach (T item in list)
            {
                DataRow dr = dt.NewRow();
                foreach (PropertyInfo property in properties)
                {
                    dr[property.Name] = property.GetValue(item, null);
                }
                dt.Rows.Add(dr);
            }

            return dt;
        }

        public static DataTable CreateDataTable2(List<TempStockData> stocks) 
        {
            DataTable dt = new DataTable();

            foreach (TempStockData item in stocks)
            {
                DataRow dr = dt.NewRow();

                dr[0] = item.Id;
                dr[1] = item.Name;
                dr[2] = item.Amount;
                dr[3] = item.Winloss;
                dt.Rows.Add(dr);

                //dt.Rows.Add(new object[] { item.Id, item.Name, item.Amount, item.Winloss });
            }

            return dt;
        }

        public static List<Member> GetAllMembers(SqlConnection sqlConnection)
        {
            sqlConnection.Open();
            List<Member> list = sqlConnection.Query<Member>("select * from Member;").AsList();
            sqlConnection.Close();

            return list;
        }
    }
}
