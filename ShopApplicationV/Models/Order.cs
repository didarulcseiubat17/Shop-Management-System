using ShopApplicationV.Validators;
using ShopApplicationV.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.ObjectModel;

using ShopApplicationV.Logic;

namespace ShopApplicationV.Models
{
    public class Order : ValidViewModelBase<Order>, IEquatable<Order>
    {
        public Order(): base(OrderValidator.Singleton.Value) { OrderItems = new ObservableCollection<OrderItem>(); }

        private int orderid;
        public int OrderID { get => orderid; set { Set(ref orderid, value); } }

        private int managerid;
        public int ManagerID { get => managerid; set { Set(ref managerid, value); } }

        private int customerid;
        public int CustomerID { get => customerid; set { Set(ref customerid, value); } }

        private Customer customer;
        public virtual Customer Customer { get => customer; set { Set(ref customer, value); if(value!=null)
                    CustomerID = value.CustomerID; } }

        private Manager manager;
        public virtual Manager Manager { get => manager; set { Set(ref manager, value); if (value!=null)
                    ManagerID = value.ManagerID; } }

        private ObservableCollection<OrderItem> orderItems;
        public virtual ObservableCollection<OrderItem> OrderItems
        {
            get => orderItems; set { Set(ref orderItems, value); }
        }


        public Order(Order PrevObj) : this()
        {
            OrderID = PrevObj.OrderID;
            ManagerID = PrevObj.ManagerID;
            CustomerID = PrevObj.CustomerID;
            ShipDate = PrevObj.ShipDate;
            CreationDate = PrevObj.CreationDate;
            Total = PrevObj.Total;
            Customer = PrevObj.Customer;
            Manager = PrevObj.Manager;
            IsDone = PrevObj.IsDone;
        }

        public void CopyFields(Order Copy)
        {
            OrderID = Copy.OrderID;
            ManagerID = Copy.ManagerID;
            CustomerID = Copy.CustomerID;
//            Customer = Copy.Customer;
  //          Manager = Copy.Manager;
            ShipDate = Copy.ShipDate;
            CreationDate = Copy.CreationDate;
            Total = Copy.Total;
            IsDone = Copy.IsDone;
        }


        private DateTime shipdate;
        private DateTime creationdate;
        private Decimal total;

        public DateTime ShipDate { get => shipdate; set { Set(ref shipdate, value); } }
        public DateTime CreationDate { get => creationdate; set { Set(ref creationdate, value); } }
        public Decimal Total { get => total; set { Set(ref total, value); } }

        public void CalculateTotal()
        {
            Total = OrderItems.Sum(x => x.UnitsTotal);
            if ((Customer.BirthDate.Day == DateTime.Now.Date.Day && Customer.BirthDate.Month == DateTime.Now.Date.Month) | (Customer.BonusCard != null && Customer.BonusCard == true))
            {
                var discont = DataContext.GetInstance().discontRegularCustomer;
                if (discont > 0)
                    Total -= (Total * DataContext.GetInstance().discontRegularCustomer / 100);
            }
        }

        private bool isdone;
        public bool IsDone { get => isdone; set { Set(ref isdone, value); } }


        public bool Equals(Order other)
        {
            return OrderID == other.OrderID;
        }

        public override int GetHashCode()
        {
            return OrderID.GetHashCode();
        }
    }
    }
