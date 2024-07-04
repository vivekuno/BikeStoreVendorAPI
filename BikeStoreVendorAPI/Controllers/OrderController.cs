using BikeStoreVendor.API.Model;
using BikeStoreVendor.Data.Access;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BikeStoreVendor.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IDapper _dapper;
        public OrderController(IDapper dapper)
        {
            _dapper = dapper;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] OrderModel orderModel)
        {
            if (orderModel == null || orderModel.OrderItems == null || !orderModel.OrderItems.Any())
            {
                return BadRequest("Invalid order data.");
            }

            BL.Order order = new BL.Order(_dapper);

            var result = order.CreateOrderAsync(orderModel).Result;

            if (result)
            {
                return Ok("Order created successfully.");
            }

            return BadRequest("Failed to create order due to insufficient stock.");
        }

        [HttpGet]
        public async Task<IActionResult> GetOrderDetails(int orderId)
        {
            BL.Order order = new BL.Order(_dapper);

            var orderDetails = order.GetOrderDetailsAsync(orderId);

            if (orderDetails == null)
            {
                return NotFound();
            }

            return Ok(orderDetails);
        }
    }
}
