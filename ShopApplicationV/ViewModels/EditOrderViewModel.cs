using ShopApplicationV.Logic;
using ShopApplicationV.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Collections.ObjectModel;
using ShopApplicationV.ViewModel;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.Controls;

namespace ShopApplicationV.ViewModels
{
    public class EditOrderViewModel : EditEntityViewModel<Order>
    {

        public IEnumerable<Product> Products { get; private set; }
        public IEnumerable<Manager> Managers { get; private set; }
        public IEnumerable<Customer> Customers { get; private set; }

        public EditOrderViewModel(Order ent, MahApps.Metro.Controls.MetroWindow wind, DataContext api, bool isnew,
            IEnumerable<Product> prod, IEnumerable<Manager> man, IEnumerable<Customer> cust) : base(ent, wind)
        {
            Products = prod;
            Managers = man;
            Customers = cust;
        }

        private RelayCommand closeOrder;
        public RelayCommand CloseOrder
        {
            get
            {
                return closeOrder ??
                    (closeOrder = new RelayCommand(async (o) =>
                    {
                        if (Entity.IsValid == false)
                        {
                            await Wind.ShowMessageAsync("Error", "Order has erros. Fix errors and then close Order.", MessageDialogStyle.AffirmativeAndNegative);
                            return;
                        }
                        string Title = "Confirm";
                        string Message = "Are you sure want to close this order?";
                        MessageDialogResult result = await Wind.ShowMessageAsync(Title, Message, MessageDialogStyle.AffirmativeAndNegative);
                        if (result == MessageDialogResult.Affirmative)
                        {
                            Entity.IsDone = true;
                            Entity.Customer.CalcCustomerTotal();
                            Close.Execute(null);
                        }
                        else return;
                    }));
            }
        }

    }
}
