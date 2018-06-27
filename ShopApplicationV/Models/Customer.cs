using ShopApplicationV.Validators;
using ShopApplicationV.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShopApplicationV.Logic;

namespace ShopApplicationV.Models
{
    public class Customer : ValidViewModelBase<Customer>, IEquatable<Customer>
    {
        public Customer(): base(CustomerValidator.Singleton.Value) { orders = new ObservableCollection<Order>(); }


        public Customer(Customer PrevObj) : this()
        {
            CustomerID = PrevObj.CustomerID;
            Name = PrevObj.Name;
            SurName = PrevObj.SurName;
            Email = PrevObj.Email;
            BirthDate = PrevObj.BirthDate;
            Phone = PrevObj.Phone;
            BonusCard = PrevObj.BonusCard;
        }

        public void CopyFields(Customer Copy)
        {

            Name = Copy.Name;
            SurName = Copy.SurName;
            Email = Copy.Email;
            BirthDate = Copy.BirthDate;
            Phone = Copy.Phone;
            BonusCard = Copy.BonusCard;
        }


        private int customerid;
        public int CustomerID { get => customerid; set {Set(ref customerid,value); } }

        public string DisplayName => Name + " " + SurName;



        private string name;
        public string Name
        {
            get { return name; }
            set { Set(ref name, value); OnPropertyChanged(nameof(DisplayName)); }
        }

        private string surname;
        public string SurName
        {
            get { return surname; }
            set { Set(ref surname, value); OnPropertyChanged(nameof(DisplayName)); }
        }



        private DateTime birthdate;
        public DateTime BirthDate { get => birthdate; set { Set(ref birthdate, value); } }

        private string phone;
        public string Phone { get => phone; set { Set(ref phone, value); } }


        private string email;
        public string Email { get => email; set { Set(ref email, value); } }

        private ObservableCollection<Order> orders;
        public virtual ObservableCollection<Order> Orders
        {
            get => orders; set { Set(ref orders, value); }
        }

        public void CalcCustomerTotal()
        {
            if (bonusCard == null | bonusCard == false)
            {
                Decimal total = Orders.Sum(x => x.Total);
                if (total > DataContext.GetInstance().requiredCustomer)
                    BonusCard = true;
            }
        }

        private bool? bonusCard;
        public bool? BonusCard { get => bonusCard; set { Set(ref bonusCard, value); } }

        private bool? isbanned;
        public bool? isBanned { get => isbanned; set { Set(ref isbanned, value); } }

        public bool Equals(Customer other)
        {
            return CustomerID == other.CustomerID;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Customer))
                return false;
            return (obj as Customer).CustomerID == CustomerID;
        }

        public override int GetHashCode()
        {
            return CustomerID.GetHashCode();
        }
    }
}
