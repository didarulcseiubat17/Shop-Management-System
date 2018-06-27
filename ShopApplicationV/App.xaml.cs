using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using ShopApplicationV.ViewModels;
using ShopApplicationV.Logic;
using ShopApplicationV.Views;

namespace ShopApplicationV
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            //      DataContext.DataApi.Value.Managers.Count();
            //        ApplicationViewModel context = new ApplicationViewModel();
            MainWindow appwind = new MainWindow();
            var context = new ApplicationViewModel(appwind);
            appwind.DataContext = context;      
            if (context.PageViewModels.Count == 0)
                appwind.Close();
            else appwind.Show();
        }
    }
}
