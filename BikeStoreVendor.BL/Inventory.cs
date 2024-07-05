using BikeStoreVendor.API.Model;
using BikeStoreVendor.Data.Access;
using BikeStoreVendor.Data.Model;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Data.Common;

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

        public async Task<int> InsertProductOnCategory(ProductCategory productCategory)
        {
            try
            {
                // Fetch category_id using category name
                var categoryIdQuery = "SELECT category_id FROM production.categories WHERE category_name = @CategoryName";
                var categoryParams = new DynamicParameters();
                categoryParams.Add("@CategoryName", productCategory.CategoryName, DbType.String);
                var categoryId = _dapper.Get<int>(categoryIdQuery, categoryParams, CommandType.Text);

                if (categoryId == 0)
                {
                    // Category not found
                    return categoryId;
                }

                // Fetch store_id using store name
                var storeIdQuery = "SELECT store_id FROM sales.stores WHERE store_name = @StoreName";
                var storeParams = new DynamicParameters();
                storeParams.Add("@StoreName", productCategory.ProductInfo.StoreName, DbType.String);
                var storeId = _dapper.Get<int>(storeIdQuery, storeParams, CommandType.Text);

                if (storeId == 0)
                {
                    // Store not found
                    return 0;
                }

                var brandIdQuery = "SELECT brand_id FROM production.brands WHERE brand_name = @BrandName";
                var brandParams = new DynamicParameters();
                brandParams.Add("@BrandName", productCategory.ProductInfo.Brand, DbType.String);
                var brandId = _dapper.Get<int>(brandIdQuery, brandParams, CommandType.Text);

                if (brandId == 0)
                {
                    // Brand not found, abort operation
                    return brandId;
                }


                var insertProductQuery = @"
                    INSERT INTO production.products (product_name, brand_id, category_id, model_year, list_price)
                    VALUES (@ProductName, @BrandId, @CategoryId, @ModelYear, @ListPrice);
                    SELECT CAST(SCOPE_IDENTITY() as int)";

                var productParams = new DynamicParameters();
                productParams.Add("@ProductName", productCategory.ProductInfo.ProductName, DbType.String);
                productParams.Add("@BrandId", brandId, DbType.Int32);
                productParams.Add("@CategoryId", categoryId, DbType.Int32);
                productParams.Add("@ModelYear", productCategory.ProductInfo.ModelYear, DbType.Int16);
                productParams.Add("@ListPrice", productCategory.ProductInfo.ListPrice, DbType.Decimal);

                var productId = _dapper.Get<int>(insertProductQuery, productParams, CommandType.Text);

                // Update stocks quantity
                var updateStockQuery = @"
                    UPDATE production.stocks
                    SET quantity = quantity + @Quantity
                    WHERE store_id = @StoreId AND product_id = @ProductId";

                var stockParams = new DynamicParameters();
                stockParams.Add("@Quantity", productCategory.ProductInfo.TotalQuantity, DbType.Int32);
                stockParams.Add("@StoreId", storeId, DbType.Int32);
                stockParams.Add("@ProductId", productId, DbType.Int32);

                int record = _dapper.Get<int>(updateStockQuery, stockParams, CommandType.Text);

                
                
                return record;
            }
            catch
            {
                // Rollback transaction in case of error
                
                return 0;
            }

        }
    }
}
