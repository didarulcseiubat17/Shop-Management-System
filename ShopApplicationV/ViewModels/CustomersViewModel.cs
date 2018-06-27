using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShopApplicationV.Logic;
using ShopApplicationV.Models;
using ShopApplicationV.ViewModel;
using ShopApplicationV.Views;
using System.Collections.ObjectModel;
using MahApps.Metro.Controls.Dialogs;

namespace ShopApplicationV.ViewModels
{
    public class CustomersViewModel : ViewModelBase, IPageViewModel
    {
        public string PageName => "Customers";
        private Customer currentCustomer;

        private ObservableCollection<Customer> customers;

        public ObservableCollection<Customer> Customers
        {
            get { return customers; }
            set { Set(ref customers, value); }
        }

        public Customer CurrentCustomer
        {
            get => currentCustomer;
            set { Set(ref currentCustomer, value); }
        }

        public CustomersViewModel()
        {}

        public  void Init()
        {
            Task.Factory.StartNew(() =>
            {
                Customers = new ObservableCollection<Customer>(DataContext.GetInstance().Customers.Local.Where(p => p.isBanned == null || p.isBanned == false));
            });
        }

        private RelayCommand editCustomer;
        public RelayCommand EditCustomer
        {
            get
            {
                return editCustomer ??
                    (editCustomer = new RelayCommand((o) =>
                    {
                        if (o == null)
                            return;
                        var wind = new EditCustomerWindow();
                        Customer entity = o as Customer;
                        Customer copy = new Customer(entity);
                        EditEntityViewModel<Customer> context = new EditEntityViewModel<Customer>(copy, wind);
                        wind.DataContext = context;
                        if (wind.ShowDialog() == true)
                        {
                            entity = DataContext.GetInstance().FindCustomer(entity.CustomerID);
                            if (entity != null)
                            {
                                entity.CopyFields(copy);
                                DataContext.GetInstance().Entry(entity).State = EntityState.Modified;
                                DataContext.GetInstance().SaveChanges();
                            }
                        }

                    }));
            }
        }

        private string search;
        public string Search
        {
            get => search;
            set
            {
                Set(ref search, value);
                CurrentCustomer = Customers.FirstOrDefault(s => s.SurName.StartsWith(search));
            }
        }



        private RelayCommand addCustomer;
        public RelayCommand AddCustomer
        {
            get
            {
                return addCustomer ??
                    (addCustomer = new RelayCommand(async (o) =>
                    {
                        var wind = new EditCustomerWindow();
                        Customer copy = new Customer() { BirthDate = DateTime.Now };
                        EditEntityViewModel<Customer> context = new EditEntityViewModel<Customer>(copy, wind);
                        wind.DataContext = context;
                        if (wind.ShowDialog() == true)
                        {
                            if (copy != null)
                            {
                                var obj = DataContext.GetInstance().Customers.FirstOrDefault(x => (x.Phone == copy.Phone || x.Email == copy.Email) && x.CustomerID != copy.CustomerID);
                                if (obj != null)
                                {
                                    MessageDialogResult result = await((MahApps.Metro.Controls.MetroWindow)App.Current.Windows[0]).ShowMessageAsync("Duplicate", "Duplicate found. Changes will not be applied", MessageDialogStyle.Affirmative);
                                    if (result == MessageDialogResult.Affirmative)
                                        return;
                                }
                                DataContext.GetInstance().AddCustomer(copy);

                                Customers.Add(copy);
                            }
                        }
                    }));
            }
        }

        private RelayCommand deleteCustomer;
        public RelayCommand DeleteCustomer
        {
            get
            {
                return deleteCustomer ??
                    (deleteCustomer = new RelayCommand((o) =>
                    {
                        if (o != null)
                        {
                            Customer Cust = o as Customer;
                            Cust.isBanned = true;
                            DataContext.GetInstance().Entry(Cust).State = EntityState.Modified;
                            DataContext.GetInstance().SaveChanges();
                            Customers.Remove(Cust);
                            CurrentCustomer = Customers.FirstOrDefault();
                        }
                    }));
            }
        }
    }
}
