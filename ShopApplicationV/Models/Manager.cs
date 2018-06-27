using ShopApplicationV.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShopApplicationV.Validators;
using FluentValidation.Validators;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopApplicationV.Models
{
    public class Manager : ValidViewModelBase<Manager>, IEquatable<Manager>
    {

        public Manager() : base(ManagerValidator.Singleton.Value) { orders = new ObservableCollection<Order>(); }

        public Manager(Manager PrevObj) : this()
        {
            ManagerID = PrevObj.ManagerID;
            Name = PrevObj.Name;
            SurName = PrevObj.SurName;
            Salary = PrevObj.Salary;
            BirthDate = PrevObj.BirthDate;
            isAdmin = PrevObj.isAdmin;
            Email = PrevObj.Email;
            PasswordHash = PrevObj.PasswordHash;
            PasswordSalt = PrevObj.PasswordSalt;
        }

        public void CopyFields(Manager Copy)
        {
            ManagerID = Copy.ManagerID;
            Name = Copy.Name;
            SurName = Copy.SurName;
            Salary = Copy.Salary;
            BirthDate = Copy.BirthDate;
            isAdmin = Copy.isAdmin;
            Email = Copy.Email;
            PasswordHash = Copy.PasswordHash;
            PasswordSalt = Copy.PasswordSalt;
        }

        private int managerid;
        public int ManagerID { get => managerid; set { Set(ref managerid, value); } }

        private string name;
        public string Name
        {
            get { return name; }
            set { Set(ref name, value);
                OnPropertyChanged(nameof(DisplayName)); }
        }


        private string surname;
        public string SurName
        {
            get { return surname; }
            set { Set(ref surname, value);
                OnPropertyChanged(nameof(DisplayName)); }
        }


        private Decimal salary;
        public Decimal Salary
        {
            get { return salary; }
            set { Set(ref salary, value); }
        }

        private string email;
        public string Email
        {
            get { return email; }
            set { Set(ref email, value); }
        }


        private bool isadmin;
        public bool isAdmin
        {
            get { return isadmin; }
            set { Set(ref isadmin, value); }
        }

        private byte[] passwordhash;
        public byte[] PasswordHash
        {
            get { return passwordhash; }
            set
            {
                passwordhash = value;
            }
        }

        private byte[] passwordsalt;
        public byte[] PasswordSalt
        {
            get { return passwordsalt; }
            set
            {
                passwordsalt = value;
            }
        }


        private DateTime birthdate;
        public DateTime BirthDate
        {
            get { return birthdate; }
            set { Set(ref birthdate, value); }
        }

        private bool? isfired;
        public bool? isFired { get => isfired; set { Set(ref isfired, value); } }

        //private bool? isadmin;
        //public bool? isAdmin { get => isadmin; set { Set(ref isadmin, value); } }



        public bool Equals(Manager other)
        {
            return ManagerID == other.ManagerID;
        }

        public override int GetHashCode()
        {
            return ManagerID.GetHashCode();
        }

        private ObservableCollection<Order> orders;
        public virtual ObservableCollection<Order> Orders
        {
            get => orders; set { Set(ref orders, value); }
        }

        public string DisplayName => Name + " " + SurName;

    }
}
