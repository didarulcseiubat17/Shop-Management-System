using ShopApplicationV.Logic;
using ShopApplicationV.Validators;
using ShopApplicationV.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopApplicationV.Models
{
    public class OrderItem : ValidViewModelBase<OrderItem>, IEquatable<OrderItem>
    {

        public OrderItem() : base(OrderItemValidator.Singleton.Value) { }

        private int orderitemid;
        public int OrderItemID { get => orderitemid; set { Set(ref orderitemid, value); } }
        private int? orderid;
        public int? OrderID { get => orderid; set { Set(ref orderid, value); } }

        

        private int productid;
        public int ProductID { get => productid; set { Set(ref productid, value); } }


        private Order order;
        public virtual Order Order { get => order; set { Set(ref order, value); if (value != null) OrderID = value.OrderID; } }

        private Product product;
        public virtual Product Product { get => product; set { Set(ref product, value); if (value != null)
                {
                    ProductID = value.ProductID;
                }
            } }

        public void SetPrice()
        {
            UnitPrice = Product.Price;
            Product.AvailableQuantity -= quantity;
            Order?.CalculateTotal();
        }

        public void RestoreProductQuantity()
        {
            Product.AvailableQuantity += quantity;
            Order?.CalculateTotal();
            quantity = 0;
        }

        private int quantity;
        public int Quantity { get => quantity; set {Set(ref quantity, value); OnPropertyChanged(nameof(UnitsTotal)); } }

        private Decimal unitPrice;
        public Decimal UnitPrice { get => unitPrice; set { Set(ref unitPrice, value); OnPropertyChanged(nameof(UnitsTotal)); } }

        public Decimal UnitsTotal => unitPrice * Quantity;


        public bool Equals(OrderItem other)
        {
            return OrderItemID == other.OrderItemID;
        }

        public override int GetHashCode()
        {
            return OrderItemID.GetHashCode();
        }
    }
}
