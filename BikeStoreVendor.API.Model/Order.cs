using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeStoreVendor.API.Model
{
    public class OrderModel
    {
        public int CustomerId { get; set; }
        public List<OrderItemModel> OrderItems { get; set; }
        public DateTime RequiredDate { get; set; }
        public int StoreId { get; set; }
        public int StaffId { get; set; }
    }

    public class OrderItemModel
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }

    public class OrderDetailModel
    {
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public byte OrderStatus { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime RequiredDate { get; set; }
        public DateTime? ShippedDate { get; set; }
        public int StoreId { get; set; }
        public int StaffId { get; set; }
        public List<OrderItemDetailModel> OrderItems { get; set; }
    }

    public class OrderItemDetailModel
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }

}
