using MahApps.Metro.Controls.Dialogs;
using ShopApplicationV.Logic;
using ShopApplicationV.Models;
using ShopApplicationV.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopApplicationV.ViewModels
{
    public class EditOrderItemViewModel : EditEntityViewModel<OrderItem>
    {
        public EditOrderItemViewModel(OrderItem ent, MahApps.Metro.Controls.MetroWindow wind, DataContext api,
            IEnumerable<Product> prod) : base(ent, wind)
        {
            Products = prod;
        }

        public IEnumerable<Product> Products { get; private set; }


    }
}
