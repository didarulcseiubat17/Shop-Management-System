using ShopApplicationV.ViewModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShopApplicationV.Logic;
using System.Data.Entity;
using ShopApplicationV.Views;
using ShopApplicationV.Utils;
using ShopApplicationV.Models;
using MahApps.Metro.Controls.Dialogs;
using System.Windows;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Data.SQLite;

namespace ShopApplicationV.ViewModels
{
    class ApplicationViewModel : ViewModelBase
    {
        private IPageViewModel currentPageViewModel;
        private List<IPageViewModel> pageViewModels;

        public async void InitCollections()
        { 
              await Task.Run(() =>
             {
                 DataContext.GetInstance().Managers.Load();
                 DataContext.GetInstance().Customers.Load();
                 DataContext.GetInstance().Orders.Load();
                 DataContext.GetInstance().Products.Load();
                 DataContext.GetInstance().OrderItems.Load();
                 DataContext.GetInstance().Notifications.Load();
             });
            
        }

        MainWindow mainwind;



        
        private bool collectionEnabled;
        public bool CollectionEnabled
        {
            get { return collectionEnabled; }
            set { Set(ref collectionEnabled, value); }
        }


        private ObservableCollection<Notification> notifications;
        public ObservableCollection<Notification> Notifications
        {
            get { return notifications; }
            set { Set(ref notifications, value); }
        }

        public void isCollectionEnabled()
        {
            if (Notifications.Count > 0)
                CollectionEnabled = true;
            else CollectionEnabled = false;
        }

        public ApplicationViewModel(MainWindow mainwind)
        {
            InitCollections();
            var wind = new LoginWindow();
            var Loginservice = new LoginService();
            LoginViewModel login = new LoginViewModel(wind, Loginservice);
            wind.DataContext = login;
            if (wind.ShowDialog() == false)
            {
                return;
            }
            DataContext.GetInstance().AuthUser = Loginservice.AuthUser;
            if (DataContext.GetInstance().AuthUser.isAdmin == true)
                PageViewModels.Add(new ManagersViewModel());
            else mainwind.SettingsButton.Opacity = 0;
            PageViewModels.Add(new CustomersViewModel());
            PageViewModels[0].Init();
            CurrentPageViewModel = PageViewModels[0];
            PageViewModels.Add(new ProductsViewModel());
            PageViewModels.Add(new OrdersViewModel());
            this.mainwind = mainwind;
            Notifications = new ObservableCollection<Notification>(DataContext.GetInstance().
                Notifications.Local.
                Where(p => p.Seen == false && p.ReceiverID == DataContext.GetInstance().AuthUser.ManagerID)
                .OrderByDescending(p => p.CreationDate));
            isCollectionEnabled();
            
            //PageViewModels[3].Init();
        }


        private RelayCommand openNotifications;
        public RelayCommand OpenNotifications
        {
            get
            {
                return openNotifications ??
                  (openNotifications = new RelayCommand((obj =>
                  {
                      var windLocal = new NotificationsView();
                      windLocal.DataContext = new NotificationsViewModel(Notifications, windLocal);
                      windLocal.ShowDialog();
                      isCollectionEnabled();
                  })));
            }
        }


        private RelayCommand openSettings;
        public RelayCommand OpenSettings
        {
            get
            {
                return openSettings ??
                  (openSettings = new RelayCommand((async obj =>
                  {

                      //var wind = new SettingsView();
                      //SettingsPageViewModel context = new SettingsPageViewModel();
                      //wind.DataContext = context;
                      //if (entity.isAdmin == false)
                      //    wind.SalaryEdit.IsEnabled = false;
                      //if (wind.ShowDialog() == true)
                      //{
                      //    entity.CopyFields(copy);
                      //    entity.PasswordHash = copy.PasswordHash;
                      //    entity.PasswordSalt = copy.PasswordSalt;
                      //    DataContext.GetInstance().Entry(entity).State = EntityState.Modified;
                      //    DataContext.GetInstance().SaveChanges();
                      //}


                  })));
            }
        }

        private RelayCommand resetdb;
        public RelayCommand ResetDb
        {
            get
            {
                return resetdb ??
                  (resetdb = new RelayCommand((async obj =>
                  {

                      MessageDialogResult result = await mainwind.ShowMessageAsync("Warning", "Danger area. Are you sure want to edit DataBase?", MessageDialogStyle.AffirmativeAndNegative);
                      if (result == MessageDialogResult.Affirmative)
                      {
                          if (DataContext.GetInstance().AuthUser.isAdmin == true)
                          {
                              string choose = await mainwind.ShowInputAsync("Select Menu", "Options:\n 1. Clear Database\n 2. Select another Database\n 3. Wide settings\n 4. Exit\n");
                              if (choose == "1")
                              {
                                  MessageDialogResult endres = await mainwind.ShowMessageAsync("WARNING", "You cant discard changes. Everything will be lost.", MessageDialogStyle.AffirmativeAndNegative);
                                  if (endres == MessageDialogResult.Affirmative)
                                  {
                                      DataContext.GetInstance().Database.ExecuteSqlCommand("DELETE FROM ORDERITEMS");
                                      DataContext.GetInstance().Database.ExecuteSqlCommand("DELETE FROM ORDERS");
                                      DataContext.GetInstance().Database.ExecuteSqlCommand("DELETE FROM CUSTOMERS");
                                      DataContext.GetInstance().Database.ExecuteSqlCommand("DELETE FROM PRODUCTS");
                                      DataContext.GetInstance().Database.ExecuteSqlCommand("DELETE FROM MANAGERS");
                                      //          DataContext.GetInstance().Database.ExecuteSqlCommand("UPDATE sqlite_sequence SET seq=0 WHERE name = 'ORDERS'");
                                      var admin = new Manager();
                                      admin.Name = admin.SurName = admin.Email = "admin";
                                      admin.BirthDate = new System.DateTime(2000, 1, 1);
                                      admin.isAdmin = true;
                                      var loginservice = new LoginService();
                                      loginservice.UpdatePass(admin, "admin");
                                      DataContext.GetInstance().Managers.Add(admin);
                                      DataContext.GetInstance().SaveChanges();
                                      await mainwind.ShowMessageAsync("Done", "Done. Default admin user created. (login:admin; pass: admin)", MessageDialogStyle.Affirmative);
                                      Process.Start(Application.ResourceAssembly.Location);
                                      Application.Current.Shutdown();
                                  }
                              }
                              else if (choose == "2")
                              {
                                  var ischanged = false;
                                 await Task.Run(() =>
                                 {
                                     var dialog = new Microsoft.Win32.OpenFileDialog() { CheckFileExists = true, Filter = "DataBase Files|*.db;" };
                                     bool? res = dialog.ShowDialog();
                                     if ((res.HasValue) && (res.Value))
                                     {
                                         var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                                         var connectionStringsSection = (ConnectionStringsSection)config.GetSection("connectionStrings");
                                         var oldconfg = connectionStringsSection.ConnectionStrings["DefaultConnection"].ConnectionString;
                                         //                      connectionStringsSection.ConnectionStrings["DefaultConnection"].ConnectionString = @"Data Source=" + dialog.FileName;
                                         //                     config.Save(ConfigurationSaveMode.Modified);
                                         List<string> OldTables;
                                         ConfigurationManager.RefreshSection("connectionStrings");
                                         try
                                         {
                                             using (var con = new SQLiteConnection(oldconfg))
                                             {
                                                 con.Open();
                                                 using (SQLiteCommand comm = con.CreateCommand())
                                                 {
                                                     OldTables = new List<string>();
                                                     comm.CommandText = "SELECT NAME from sqlite_master";
                                                     SQLiteDataReader r = comm.ExecuteReader();
                                                     while (r.Read())
                                                     {
                                                         OldTables.Add(System.Convert.ToString(r["NAME"]));
                                                     }
                                                 }
                                             }
                                             var newconfg = @"Data Source=" + dialog.FileName; 
                                             List<string> NewTables = new List<string>();
                                             using (var con = new SQLiteConnection(newconfg))
                                             {
                                                 con.Open();
                                                 using (SQLiteCommand comm = con.CreateCommand())
                                                 {
                                                     NewTables = new List<string>();
                                                     comm.CommandText = "SELECT NAME from sqlite_master";
                                                     SQLiteDataReader r = comm.ExecuteReader();
                                                     while (r.Read())
                                                     {
                                                         NewTables.Add(System.Convert.ToString(r["NAME"]));
                                                     }
                                                 }
                                             }
                                             if (NewTables.SequenceEqual(OldTables) == false)
                                                 throw new System.Exception();
                                             else
                                             {
                                                 connectionStringsSection.ConnectionStrings["DefaultConnection"].ConnectionString = newconfg;
                                                 config.Save(ConfigurationSaveMode.Modified);
                                                 ischanged = true;
                                             }
                                         }
                                         catch
                                         {
                                             ischanged = false;
                                         }
                                     }

                                 });
                                  if (ischanged == false)
                                  {
                                      await mainwind.ShowMessageAsync("Error", "Database doesnt suit. Database wasnt changed", MessageDialogStyle.Affirmative);
                                      return;
                                  }
                                  else
                                  {
                                      await mainwind.ShowMessageAsync("Done", "Database changed. App will be reloaded.", MessageDialogStyle.Affirmative);
                                      Process.Start(Application.ResourceAssembly.Location);
                                      Application.Current.Shutdown();
                                  }
                                  
                              }
                              else if (choose == "3")
                              {
                                  var wind =  new SettingsView();
                                  wind.DataContext  = new SettingsPageViewModel(wind);
                                  wind.Show();
                              }
                              else return;
                          }
                          else
                          {
                              await mainwind.ShowMessageAsync("Error", "Hah you can't do this.", MessageDialogStyle.Affirmative);
                          }
                      }


                  })));
            }
        }


        private RelayCommand editProfile;
        public RelayCommand EditProfile
        {
            get
            {
                return editProfile ??
                  (editProfile = new RelayCommand(( obj =>
                  {

                      var wind = new Profile();
                      var entity = DataContext.GetInstance().AuthUser;
                      Manager copy = new Manager(entity);
                      ProfileViewModel context = new ProfileViewModel(copy, wind);
                      wind.DataContext = context;
                      if (entity.isAdmin == false)
                          wind.SalaryEdit.IsEnabled = false;
                      if (wind.ShowDialog() == true)
                      {
                            entity.CopyFields(copy);
                            entity.PasswordHash = copy.PasswordHash;
                            entity.PasswordSalt = copy.PasswordSalt;
                            DataContext.GetInstance().Entry(entity).State = EntityState.Modified;
                            DataContext.GetInstance().SaveChanges();
                      }
                  })));
            }
        }


        private RelayCommand changePageCommand;
        public RelayCommand ChangePageCommand
        {
            get
            {
                return changePageCommand ??
                  (changePageCommand = new RelayCommand((async obj =>
                  {
                      IPageViewModel View = obj as IPageViewModel;
                      if (View == null)
                          return;
                      await Task.Run( () => { View.Init(); }) ;
                      if (!PageViewModels.Contains(View))
                          PageViewModels.Add(View);
                      CurrentPageViewModel = PageViewModels
                          .FirstOrDefault(vm => vm == View);
                  })));
            }
        }

        public IPageViewModel GetViewByName(string name) => pageViewModels.FirstOrDefault(p => p.PageName == name);



        public List<IPageViewModel> PageViewModels
        {
            get
            {
                if (pageViewModels == null)
                    pageViewModels = new List<IPageViewModel>();
                return pageViewModels;
            }
        }

        public IPageViewModel CurrentPageViewModel
        {
            get => currentPageViewModel;
            set
            {
                if (currentPageViewModel != value)
                    Set(ref currentPageViewModel, value);
            }
        }

    }
}
