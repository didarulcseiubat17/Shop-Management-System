using ShopApplicationV.Logic;
using ShopApplicationV.Models;
using ShopApplicationV.ViewModel;
using ShopApplicationV.Views;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using MahApps.Metro.Controls.Dialogs;

namespace ShopApplicationV.ViewModels
{
    public class OrdersViewModel : ViewModelBase, IPageViewModel
    {

        public string PageName => "Orders";

        private Order currentOrder;
        private OrderItem currentOrderItem;


        private ObservableCollection<Order> orders;
        public ObservableCollection<Order> Orders
        {
            get { return orders; }
            set { Set(ref orders, value); }
        }

        public Order CurrentOrder
        {
            get => currentOrder;
            set { Set(ref currentOrder, value); SetActionsEnabledValue(); }
        }

        public OrderItem CurrentOrderItem
        {
            get => currentOrderItem;
            set { Set(ref currentOrderItem, value); }
        }

        private ObservableCollection<Manager> managers;
        public ObservableCollection<Manager> Managers
        {
            get { return managers; }
            set { Set(ref managers, value); }
        }

        private ObservableCollection<Product> product;
        public ObservableCollection<Product> Products
        {
            get { return product; }
            set { Set(ref product, value); }
        }


        private ObservableCollection<Customer> customer;
        public ObservableCollection<Customer> Customers
        {
            get { return customer; }
            set { Set(ref customer, value); }
        }

        private ObservableCollection<OrderItem> orderItems;
        public ObservableCollection<OrderItem> OrderItems
        {
            get { return orderItems; }
            set { Set(ref orderItems, value); }
        }




        public OrdersViewModel()
        {
            //if (DataContext.GetInstance().AuthUser?.isAdmin == true)
            //    OrderPredicate = null;
            //else OrderPredicate = delegate (Order ord) { return ord.Manager.isAdmin; };
            ////         api.Managers.Load();
            ////       api.Customers.Load();
            ////     api.Products.Load();
        }

        public void Init()
        {
            Task.Factory.StartNew(() =>
            {
                Products = new ObservableCollection<Product>(DataContext.GetInstance().Products.Local.ToBindingList());
                Customers = new ObservableCollection<Customer>(DataContext.GetInstance().Customers.Local.ToBindingList());
                Managers = new ObservableCollection<Manager>(DataContext.GetInstance().Managers.Local.ToBindingList());
                Orders = new ObservableCollection<Order>(DataContext.GetInstance().Orders.Local.ToBindingList());
                //            Products = new ObservableCollection<Product>(DataContext.GetInstance().Products).ToList());
                //Managers = new ObservableCollection<Manager>((from p in DataContext.GetInstance().Managers select p).ToList());
                //Customers = new ObservableCollection<Customer>((from p in DataContext.GetInstance().Customers select p).ToList());
                //Orders = new ObservableCollection<Order>((from p in DataContext.GetInstance().Orders select p).ToList());
                //         api = data;
                //Managers = new ObservableCollection<Manager>((from p in api.Managers select p).ToList());
                //Customers = new ObservableCollection<Customer>((from p in api.Customers select p).ToList());
                //Products = new ObservableCollection<Product>((from p in api.Products select p).ToList());
                //   Orders = new ObservableCollection<Order>((from p in api.Orders select p).ToList());
                //api.Managers.Load();
                //api.Customers.Load();
                //api.Products.Load();

            });
        }

        private RelayCommand editOrder;
        public RelayCommand EditOrder
        {
            get
            {
                return editOrder ??
                    (editOrder = new RelayCommand((o) =>
                    {
                        if (o == null || (o as Order).IsDone == true)
                            return;
                        var wind = new OrderWindow();
                        Order entity = o as Order;
                        Order copy = new Order(entity);
                        EditOrderViewModel context = new EditOrderViewModel(copy, wind, DataContext.GetInstance(), false, Products, 
                            Managers.Where(p => p.ManagerID == entity.ManagerID | p.isFired == null || p.isFired == false) ,
                            Customers.Where(p => p.CustomerID == entity.CustomerID | p.isBanned == null || p.isBanned == false));
                        wind.DataContext = context;
                        if (wind.ShowDialog() == true)
                        {
                            entity = DataContext.GetInstance().FindOrder(entity.OrderID);
                            if (entity != null)
                            {
                                entity.CopyFields(copy);
                                entity.CalculateTotal();
                                DataContext.GetInstance().Entry(entity).State = EntityState.Modified;
                                
                                DataContext.GetInstance().SaveChanges();
                                Orders = new ObservableCollection<Order>((from p in DataContext.GetInstance().Orders select p).ToList());

        //                        OnPropertyChanged(nameof(Orders));
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
                CurrentOrder = Orders.FirstOrDefault(s => s.OrderID.ToString().StartsWith(search));
            }
        }

        //private bool isenabled;
        //public bool IsEnabled
        //{
        //    get { return !CurrentOrder?.IsDone ?? true; }
        //    set 
        //} 
        public void SetActionsEnabledValue()
        {
            try
            {
                IsActionsEnabled = !CurrentOrder.IsDone;
            }
            catch { }
        }

        private bool isActionsEnabled;
        public bool IsActionsEnabled
        {
            get { return isActionsEnabled; }
            set { Set(ref isActionsEnabled, value); }
        }



        private RelayCommand addOrder;
        public RelayCommand AddOrder
        {
            get
            {
                return addOrder ??
                    (addOrder = new RelayCommand((o) =>
                    {
                        var wind = new OrderWindow();
                        Order copy = new Order() { CreationDate = DateTime.Now, ShipDate = DateTime.Now};
                        try
                        {
                            copy.OrderID = Orders.Last().OrderID + 1;
                        }
                        catch
                        {
                            copy.OrderID = 1;
                        }
                        
                        EditOrderViewModel context = new EditOrderViewModel(copy, wind, DataContext.GetInstance(), true, Products,
                           Managers.Where(p => p.isFired == null || p.isFired == false),
                          Customers.Where(p => p.isBanned == null || p.isBanned == false));
                        wind.DataContext = context;
                        wind.CloseOrder.Visibility = System.Windows.Visibility.Collapsed;
                        if (wind.ShowDialog() == true)
                        {
                            if (copy != null)
                            {
                                DataContext.GetInstance().Entry(copy.Customer).State = EntityState.Unchanged;
                                DataContext.GetInstance().Entry(copy.Manager).State = EntityState.Unchanged;
                                DataContext.GetInstance().AddOrder(copy);
                                Orders.Add(copy);
                            }
                        }

                    }));
            }
        }

        private RelayCommand deleteOrder;
        public RelayCommand DeleteOrder
        {
            get
            {
                return deleteOrder ??
                    (deleteOrder = new RelayCommand(async (o) =>
                    {

                        if (o != null)
                        {
                            Order order = o as Order;
                            if (order.IsDone == true)
                            {
                                await ((MahApps.Metro.Controls.MetroWindow)App.Current.Windows[0]).ShowMessageAsync("Delete", "Yout can't delete closed order.", MessageDialogStyle.Affirmative);
                                return;
                            }
                            MessageDialogResult result = await((MahApps.Metro.Controls.MetroWindow)App.Current.Windows[0]).ShowMessageAsync("Delete", "Are you sure want to delete order??", MessageDialogStyle.AffirmativeAndNegative);
                            if (result != MessageDialogResult.Affirmative)
                                return;
                            foreach (OrderItem item in order.OrderItems)
                            {
                                item.RestoreProductQuantity();
                            }
                            DataContext.GetInstance().Entry(order).State = EntityState.Deleted;
                            //   api.OrderItems.RemoveRange(order.OrderItems);
                            DataContext.GetInstance().DeleteOrder(order);
                            Orders.Remove(order);
                            CurrentOrder = Orders.FirstOrDefault();
                        }
                    }));
            }
        }



        private RelayCommand addOrderItem;
        public RelayCommand AddOrderItem
        {
            get
            {
                return addOrderItem ??
                    (addOrderItem = new RelayCommand((o) =>
                    {
                        if (CurrentOrder == null || CurrentOrder.IsDone == true)
                            return;
                        var wind = new EditOrderItem();
                        OrderItem copy = new OrderItem() { OrderID = CurrentOrder.OrderID };
                        EditOrderItemViewModel context = new EditOrderItemViewModel(copy, wind, DataContext.GetInstance(), Products.Where(p => p.AvailableQuantity > 0));
                        wind.DataContext = context;
                        if (wind.ShowDialog() == true)
                        {
                            if (copy != null)
                            {
                                //         api.Entry(copy.Order).State = EntityState.Unchanged;
                                copy.SetPrice();
                             //   copy.Order.CalculateTotal();
                                DataContext.GetInstance().AddOrderItem(copy);
                                CurrentOrder.CalculateTotal();
                                //                         api.Entry(CurrentOrder).State = EntityState.Modified;
                                DataContext.GetInstance().SaveChanges();
//                                CurrentOrder.OrderItems.Add(copy);
                            }
                            CurrentOrder = null;
                            CurrentOrder = copy.Order;
                        }

                    }));
            }
        }

        private RelayCommand deleteOrderItem;
        public RelayCommand DeleteOrderItem
        {
            get
            {
                return deleteOrderItem ??
                    (deleteOrderItem = new RelayCommand((o) =>
                    {
                        if (o != null)
                        {
                            OrderItem ord = o as OrderItem;
                            if (ord.Order.IsDone == true)
                                return;
                            ord.RestoreProductQuantity();
                            //           api.Entry(ord.Product).State = EntityState.Modified;
                            DataContext.GetInstance().Entry(ord).State = EntityState.Deleted;
                            DataContext.GetInstance().DeleteOrderItem(ord);
                            CurrentOrder.CalculateTotal();
                            //                           api.Entry(CurrentOrder).State = EntityState.Modified;
                            DataContext.GetInstance().SaveChanges();
                        }
                    }));
            }
        }

        //private RelayCommand editOrderItem;
        //public RelayCommand EditOrderItem
        //{
        //    get
        //    {
        //        return editOrderItem ??
        //            (editOrderItem = new RelayCommand((o) =>
        //            {
        //                if (o == null)
        //                    return;
        //                var wind = new OrderWindow();
        //                Order entity = o as Order;
        //                Order copy = new Order(entity);
        //                EditOrderViewModel context = new EditOrderViewModel(copy, wind, api, false, Products, Managers, Customers);
        //                wind.DataContext = context;
        //                if (wind.ShowDialog() == true)
        //                {
        //                    entity = api.FindOrder(entity.OrderID);
        //                    if (entity != null)
        //                    {
        //                        entity.CopyFields(copy);
        //                        api.Entry(entity).State = EntityState.Modified;
        //                        api.SaveChanges();
        //                        using (var db = new DataContext())
        //                        {
        //                            orders = new ObservableCollection<Order>((from p in api.Orders select p).ToList());
        //                            OnPropertyChanged(nameof(Orders));
        //                        }
        //                    }
        //                }

        //            }));
        //    }
        //}



    }
}
