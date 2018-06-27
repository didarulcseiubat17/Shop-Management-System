using ShopApplicationV.Logic;
using ShopApplicationV.Models;
using ShopApplicationV.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using ShopApplicationV.Views;
using System.Collections.ObjectModel;
using MahApps.Metro.Controls.Dialogs;

namespace ShopApplicationV.ViewModels
{
    public class ProductsViewModel : ViewModelBase, IPageViewModel
    {
        public string PageName => "Products";
        private Product currentproduct;

        private ObservableCollection<Product> products;

        public ObservableCollection<Product> Products
        {
            get { return products; }
            set { Set(ref products, value); }
        }



        public Product CurrentProduct
        {
            get => currentproduct;
            set { Set(ref currentproduct, value); }
        }

        public ProductsViewModel()
        {     }

        public void Init()
        {

            Task.Factory.StartNew(() =>
            {
                Products = new ObservableCollection<Product>(DataContext.GetInstance().Products.Local.Where(p => p.isAvailable == null || p.isAvailable == true));
            });
        }

        private RelayCommand editproduct;
        public RelayCommand EditProduct
        {
            get
            {
                return editproduct ??
                    (editproduct = new RelayCommand((o) =>
                    {
                        if (o == null)
                            return;
                        var wind = new EditProductView();
                        Product entity = o as Product;
                        Product copy = new Product(entity);
                        EditEntityViewModel<Product> context = new EditEntityViewModel<Product>(copy, wind);
                        wind.DataContext = context;
                        if (wind.ShowDialog() == true)
                        {
                            entity = DataContext.GetInstance().FindProduct(entity.ProductID);
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
                CurrentProduct = products.FirstOrDefault(s => s.ProductID.ToString().StartsWith(search));
            }
        }



        private RelayCommand addproduct;
        public RelayCommand AddProduct
        {
            get
            {
                return addproduct ??
                    (addproduct = new RelayCommand(async (o) =>
                    {
                        var wind = new EditProductView();
                        Product copy = new Product();
                        EditEntityViewModel<Product> context = new EditEntityViewModel<Product>(copy, wind);
                        wind.DataContext = context;
                        if (wind.ShowDialog() == true)
                        {
                            if (copy != null)
                            {
                                var obj = DataContext.GetInstance().Products.FirstOrDefault(x => x.Name == copy.Name && x.ProductID != copy.ProductID);
                                if (obj != null)
                                {
                                    MessageDialogResult result = await ((MahApps.Metro.Controls.MetroWindow)App.Current.Windows[0]).ShowMessageAsync("Duplicate", "Duplicate found. Changes will not be applied", MessageDialogStyle.Affirmative);
                                    if (result == MessageDialogResult.Affirmative)
                                        return;
                                }
                                DataContext.GetInstance().AddProduct(copy);
                                Products.Add(copy);
                            }
                        }

                    }));
            }
        }

        private RelayCommand deleteproduct;
        public RelayCommand DeleteProduct
        {
            get
            {
                return deleteproduct ??
                    (deleteproduct = new RelayCommand((o) =>
                    {
                        if (o != null)
                        {
                            var prod = o as Product;
                            prod.AvailableQuantity = 0;
                            prod.isAvailable = false;
                            DataContext.GetInstance().Entry(prod).State = EntityState.Modified;
                            DataContext.GetInstance().SaveChanges();
                            Products.Remove(prod);
                            CurrentProduct = products.FirstOrDefault();
                        }
                    }));
            }
        }
    }
}
