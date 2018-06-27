using ShopApplicationV.Validators;
using ShopApplicationV.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopApplicationV.Models
{
    public class Product : ValidViewModelBase<Product>, IEquatable<Product>
    {
        public Product(): base(ProductValidator.Singleton.Value) { }


        public Product(Product PrevObj) : this()
        {
            ProductID = PrevObj.ProductID;
            Name = PrevObj.Name;
            Price = PrevObj.Price;
            isAvailable = PrevObj.isAvailable;
            AvailableQuantity = PrevObj.AvailableQuantity;
        }

        public void CopyFields(Product Copy)
        {
            ProductID = Copy.ProductID;
            Name = Copy.Name;
            Price = Copy.Price;
            AvailableQuantity = Copy.AvailableQuantity;
            isAvailable = Copy.isAvailable;
        }


        private int productid;
        public int ProductID { get => productid; set { Set(ref productid, value); } }

        private string name;
        public string Name
        {
            get { return name; }
            set { Set(ref name, value); }
        }

        private Decimal price;
        public Decimal Price { get => price; set { Set(ref price, value); } }

        private bool? isavailable;
        public bool? isAvailable { get => isavailable; set { Set(ref isavailable, value); } }

        public bool Equals(Product other)
        {
            return ProductID == other.ProductID;
        }

        private int availableQuantity;
        public int AvailableQuantity { get => availableQuantity; set { Set(ref availableQuantity, value); } }


        public override int GetHashCode()
        {
            return ProductID.GetHashCode();
        }
    }
}
