using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using ShopApplicationV.Models;
using System.Configuration;

namespace ShopApplicationV.Logic
{

    public class DataContext : DbContext 
    {
        protected DataContext() : base("DefaultConnection") {
            Database.SetInitializer<DataContext>(null);
        }


        public static DataContext explicitCallCtor() => instance = new DataContext();

        private static DataContext instance;
        public static DataContext GetInstance() {
            if (instance == null)
            {

                instance = new DataContext();
                instance.ReadConfigValues();
            }
            return instance;
        }

        public void ReadConfigValues()
        {
            try
            {
                instance.requiredCustomer = Decimal.Parse(ConfigurationManager.AppSettings["requiredCustomer"]);
                instance.discontRegularCustomer = Decimal.Parse(ConfigurationManager.AppSettings["discontRegularCustomer"]);
            }
            catch
            {
                instance.requiredCustomer = instance.discontRegularCustomer = 0;
            }
        }

        public void SetConfigValues(Decimal required, Decimal discont)
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var settings = config.AppSettings.Settings;
            settings["requiredCustomer"].Value = required.ToString();
            settings["discontRegularCustomer"].Value = discont.ToString();
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection(config.AppSettings.SectionInformation.Name);
            requiredCustomer = required;
            discontRegularCustomer = discont;
        }


        public Decimal requiredCustomer;
        public Decimal discontRegularCustomer;

        public Manager AuthUser;
        public DbSet<Manager> Managers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        #region Manager


        public void AddManager(Manager M)
        {
            Managers.Add(M);
            SaveChanges();
        }

        public void DeleteManager(Manager M)
        {
            Managers.Remove(M);
            SaveChanges();
        }

        public Manager FindManager(int id)
        {
            return Managers.Find(id);
        }

        public IEnumerable<Manager> GetAllManagers()
        {
            return Managers;
        }

        #endregion

        #region Customer
        public void AddCustomer(Customer M)
        {
            Customers.Add(M);
            SaveChanges();
        }

        public void DeleteCustomer(Customer M)
        {
            Customers.Remove(M);
            SaveChanges();
        }

        public Customer FindCustomer(int id)
        {
            return Customers.Find(id);
        }

        public IEnumerable<Customer> GetAllCustomers()
        {
            return Customers.Select(p => p);
        }
        #endregion

        #region Order
        public void AddOrder(Order M)
        {
            Orders.Add(M);
            SaveChanges();
        }

        public void DeleteOrder(Order M)
        {
            Orders.Remove(M);
            SaveChanges();
        }

        public Order FindOrder(int id)
        {
            return Orders.Find(id);
        }

        public IEnumerable<Order> GetAllOrders()
        {
            return Orders.Select(p => p);
        }
        #endregion


        #region Product
        public void AddProduct(Product M)
        {
            Products.Add(M);
            SaveChanges();
        }

        public void DeleteProduct(Product M)
        {
            Products.Remove(M);
            SaveChanges();
        }

        public Product FindProduct(int id)
        {
            return Products.Find(id);
        }

        public IEnumerable<Product> GetAllProducts()
        {
            return Products.Select(p =>p);
        }
        #endregion

        #region OrderItems
        public void AddOrderItem(OrderItem M)
        {
            OrderItems.Add(M);
            SaveChanges();
        }

        public void DeleteOrderItem(OrderItem M)
        {
            OrderItems.Remove(M);
            SaveChanges();
        }

        public IEnumerable<OrderItem> GetAllOrderItems()
        {
            return OrderItems;
        }
        #endregion







    }

}
