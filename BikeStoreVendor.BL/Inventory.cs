using BikeStoreVendor.API.Model;
using BikeStoreVendor.Data.Access;
using BikeStoreVendor.Data.Model;
using Dapper;
using Microsoft.Data.SqlClient;

namespace BikeStoreVendor.BL
{
    public class Inventory
    {
        private readonly IDapper _dapper;

        public Inventory( IDapper dapper)
        {
            _dapper = dapper;
        }

        public async Task<IEnumerable<Categories>> GetAllCategories()
        {
           return _dapper.GetAll<Categories>("SELECT [category_id],[category_name]  FROM [BikeStores].[production].[categories]", null);
        }

        public async Task<List<ProductQuantity>> GetStockByCategory(string categoryName)
        {
            DynamicParameters dynamicParameters = new DynamicParameters();
            dynamicParameters.Add("@catname", categoryName);
            var result = _dapper.GetAll<ProductQuantity>("  SELECT      " +
                " p.product_name ProductName ,sum(s.[quantity]) as TotalQuantity " +
                " FROM [BikeStores].[production].[stocks] s join [production].products p on s.product_id = p.product_id  " +
                "join production.categories c on p.category_id = c.category_id " +
                " where c.category_name = @catname " +
                "group by p.product_name", dynamicParameters);
            return result;
        }

        public async Task<int> CreateCategory(string categoryName)
        {
            DynamicParameters dynamicParameters = new DynamicParameters();
            dynamicParameters.Add("@catname", categoryName);
            var result = _dapper.Insert<int>("INSERT INTO production.categories (category_name) " +
                "SELECT @catname " +
                "WHERE NOT EXISTS (" +
                "    SELECT 1" +
                "    FROM production.categories" +
                "    WHERE category_name = @catname);", dynamicParameters);
            return result;
        }

        public async Task<int> UpdateCategory(Categories category)
        {
            DynamicParameters dynamicParameters = new DynamicParameters();
            dynamicParameters.Add("@catid", category.category_id);
            dynamicParameters.Add("@catname", category.category_id);
            var result = _dapper.Update<int>("UPDATE production.categories " +
                "SET category_name = @catname " +
                "WHERE category_id = @catid;", dynamicParameters);
            return result;

        }
    }
}
