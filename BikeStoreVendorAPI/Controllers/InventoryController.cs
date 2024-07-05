using BikeStoreVendor.API.Model;
using BikeStoreVendor.Data.Access;
using BikeStoreVendor.Data.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BikeStoreVendor.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly IDapper _dapper;
        public InventoryController(IDapper dapper)
        {
            _dapper = dapper;
        }
        [HttpGet]
        //[Route("ListCategories")]
        public Task<IEnumerable<Categories>> GetCategories()
        {
            BL.Inventory inventoryBL = new BL.Inventory(_dapper);
            return inventoryBL.GetAllCategories();           

        }
        //need to avoid plural names for tables.
        [HttpPost]
        public string CreateCategory(string category)
        {
            BL.Inventory inventoryBL = new BL.Inventory(_dapper);
            int result = inventoryBL.CreateCategory(category).Result;
            if(result == 1)
            {
                return category + " created.";
            }
            else
            {
                return "Duplicate or invalid category-" + category;
            }

        }

        [HttpPost]
        public string InsertProductOnCategory(ProductCategory productCategory)
        {
            BL.Inventory inventory = new BL.Inventory(_dapper);
            int insert = inventory.InsertProductOnCategory(productCategory).Result;
            if (insert == 1)
            {
                return "Product inserted " + insert;
            }
            else
            {
                return "No product inserted, incorrect product info";
            }
        }

        [HttpPut]
        public string UpdateCategory(Categories category)
        {
            BL.Inventory inventoryBL = new BL.Inventory(_dapper);
            int result = inventoryBL.UpdateCategory(category).Result;
            if (result == 1)
            {
                return category.category_name + " updated";
            }
            else
            {
                return "Invalid category id details";
            }
        }

        [HttpGet]
        //[Route("ListStockByCategory/{categoryName}", Name = "ListStockByCategory")]
        public Task<List<ProductQuantity>> GetStockByCategory(string categoryName)
        {
            BL.Inventory inventoryBL = new BL.Inventory(_dapper);
            return inventoryBL.GetStockByCategory(categoryName);
        }

    }
}
