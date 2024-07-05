using BikeStoreVendor.API.Model;
using BikeStoreVendor.Data.Access;
using Dapper;
using System.Data;
using System.Data.Common;

namespace BikeStoreVendor.BL
{
    public class Order
    {
        private readonly IDapper _dapper;

        public Order(IDapper dapper)
        {
            _dapper = dapper;
        }
        public async Task<bool> CreateOrderAsync(OrderModel orderModel)
        {
            //using (var conn = _dapper.GetDbconnection())
            {
                
                //using (var transaction = _dapper.GetDbconnection().BeginTransaction())
                {
                    try
                    {
                        DynamicParameters dapperDyna = null;
                        // Check stock availability
                        foreach (var item in orderModel.OrderItems)
                        {
                            dapperDyna = new Dapper.DynamicParameters();
                            dapperDyna.Add("@StoreId", orderModel.StoreId);
                            dapperDyna.Add("@ProductId", item.ProductId);

                            var stock = _dapper.Get<int>("SELECT quantity FROM production.stocks WHERE store_id = @StoreId AND product_id = @ProductId", dapperDyna, System.Data.CommandType.Text);//, transaction);

                            if (stock < item.Quantity)
                            {
                                return false; // Insufficient stock
                            }

                            dapperDyna = new Dapper.DynamicParameters();
                            dapperDyna.Add("@Quantity", item.Quantity);
                            dapperDyna.Add("@StoreId", orderModel.StoreId);
                            dapperDyna.Add("@ProductId", item.ProductId);

                            // Reserve stock
                            var updateResult = _dapper.Get<int>("UPDATE production.stocks SET quantity = quantity - @Quantity WHERE store_id = @StoreId AND product_id = @ProductId",
                                dapperDyna, System.Data.CommandType.Text);//, transaction);

                        }

                        dapperDyna = new Dapper.DynamicParameters();
                        dapperDyna.Add("@CustomerId", orderModel.CustomerId);
                        dapperDyna.Add("@OrderDate", DateTime.UtcNow);
                        dapperDyna.Add("@RequiredDate", orderModel.RequiredDate);
                        dapperDyna.Add("@StoreId", orderModel.StoreId);
                        dapperDyna.Add("@StaffId", orderModel.StaffId);

                        // Create order
                        var orderId = _dapper.Get<int>(@"INSERT INTO sales.orders (customer_id, order_status, order_date, required_date, store_id, staff_id) 
                      OUTPUT INSERTED.order_id 
                      VALUES (@CustomerId, 1, @OrderDate, @RequiredDate, @StoreId, @StaffId)", dapperDyna, System.Data.CommandType.Text);//, transaction);

                        // Create order items
                        foreach (var item in orderModel.OrderItems)
                        {
                            dapperDyna = new Dapper.DynamicParameters();
                            dapperDyna.Add("@OrderId", orderId);
                            dapperDyna.Add("@ItemId", new Random().Next(1,99));
                            dapperDyna.Add("@ProductId", item.ProductId);
                            dapperDyna.Add("@Quantity", item.Quantity);


                            _dapper.Get<int>(@"INSERT INTO sales.order_items (order_id, item_id, product_id, quantity, list_price) 
                          VALUES (@OrderId, @ItemId, @ProductId, @Quantity, (
                             Select 
                                  case when list_price > (select TRY_CAST(discount_cat_value AS DECIMAL(10, 2)) from sales.discount where discount_cat='amount')
	                                   then FLOOR(list_price - (list_price * (select [percentage] from sales.discount where discount_cat='amount') / 100.0))
	                               	 else list_price 
	                        	 end as list_price 
                                 from production.products  where product_id=@ProductId
                           ))", dapperDyna, CommandType.Text);//, transaction);

                        }

                        dapperDyna = new Dapper.DynamicParameters();
                        dapperDyna.Add("@OrderId", orderId);
                       
                        //Enter credit 
                        var credited = _dapper.Get<int>(@"INSERT INTO sales.customer_credit (order_id, customer_id, amount)
                                                         SELECT 
                                                             o.order_id,
                                                             o.customer_id,
                                                             FLOOR(SUM(oi.list_price - oi.discount) / 1000) * 10 AS credit_amount
                                                         FROM 
                                                             sales.order_items oi join sales.orders o on oi.order_id=o.order_id 
                                                         WHERE
                                                             o.order_id = 1619
                                                         GROUP BY
                                                             o.order_id, o.customer_id",dapperDyna, CommandType.Text);

                        //transaction.Commit();
                        return true;
                    }
                    catch (Exception)
                    {
                        //transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public async Task<OrderDetailModel> GetOrderDetailsAsync(int orderId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@OrderId", orderId, DbType.Int32);

            var orderQuery = @"SELECT order_id AS OrderId, customer_id AS CustomerId, order_status AS OrderStatus, 
                           order_date AS OrderDate, required_date AS RequiredDate, shipped_date AS ShippedDate, 
                           store_id AS StoreId, staff_id AS StaffId
                           FROM sales.orders
                           WHERE order_id = @OrderId";

            var order = _dapper.Get<OrderDetailModel>(orderQuery, parameters, CommandType.Text);

            if (order != null)
            {
                var orderItemsQuery = @"SELECT product_id AS ProductId, quantity AS Quantity
                                    FROM sales.order_items
                                    WHERE order_id = @OrderId";

                var orderItems = _dapper.GetAll<OrderItemDetailModel>(orderItemsQuery, parameters, CommandType.Text);
                order.OrderItems = orderItems.ToList();
            }

            return order;
        }

        //Return Order

    }
}
