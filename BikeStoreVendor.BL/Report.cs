using BikeStoreVendor.API.Model;
using BikeStoreVendor.Data.Access;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeStoreVendor.BL
{
    public class Report
    {
        private readonly IDapper _dapper;

        public Report(IDapper dapper)
        {
            _dapper = dapper;
        }

        public async Task<ProductSalesModel> GetHighestSellingProductAsync(DateTime startDate, DateTime endDate)
        {
            var query = @"
        SELECT TOP 1
            p.product_id AS ProductId,
            p.product_name AS ProductName,
            SUM(oi.quantity) AS TotalQuantitySold
        FROM
            sales.orders o
        INNER JOIN
            sales.order_items oi ON o.order_id = oi.order_id
        INNER JOIN
            production.products p ON oi.product_id = p.product_id
        WHERE
            o.order_date BETWEEN @StartDate AND @EndDate
        GROUP BY
            p.product_id, p.product_name
        ORDER BY
            TotalQuantitySold DESC;";

            var parameters = new DynamicParameters();
            parameters.Add("@StartDate", startDate, DbType.DateTime);
            parameters.Add("@EndDate", endDate, DbType.DateTime);

            return _dapper.Get<ProductSalesModel>(query, parameters, CommandType.Text);
        }
    }
}
