using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MahApps.Metro.Controls;
using ShopApplicationV.ViewModel;
using ShopApplicationV.Validators;
using MahApps.Metro.Controls.Dialogs;
using System.Configuration;
using ShopApplicationV.Utils;
using ShopApplicationV.Logic;

namespace ShopApplicationV.ViewModels
{
    public class SettingsPageViewModel : ValidViewModelBase<SettingsPageViewModel>
    {
        MetroWindow wind;

        public SettingsPageViewModel(MetroWindow wind) : base(SettingsValidation.Singleton.Value)
        {
            this.wind = wind;
            Email = ConfigurationManager.AppSettings["Email"];
            Host = ConfigurationManager.AppSettings["Host"];
            RequiredCustomer = DataContext.GetInstance().requiredCustomer;
            DiscontRegularCustomer = DataContext.GetInstance().discontRegularCustomer;

        }

        private Decimal requiredCustomer;
        private Decimal discontRegularCustomer;
        private string email;
        private string host;
        private string password;

        public Decimal RequiredCustomer { get => requiredCustomer; set { Set(ref requiredCustomer, value); } }
        public Decimal DiscontRegularCustomer { get => discontRegularCustomer; set { Set(ref discontRegularCustomer, value); } }
        public string Email  { get => email; set { Set(ref email, value); } }
        public string Host { get => host; set { Set(ref host, value); } }
        public string Password { get => password; set { Set(ref password, value); } }

        private RelayCommand save;
        public RelayCommand Save
        {
            get
            {
                return save ??
                    (save = new RelayCommand(async (o) =>
                    {
                        if (IsValid == false)
                        {
                            MessageDialogResult result = await wind.ShowMessageAsync("Error", "Fields has errors. Fix it to save changes", MessageDialogStyle.Affirmative);
                            return;
                        }
                        DataContext.GetInstance().SetConfigValues(requiredCustomer, discontRegularCustomer);
                        var pass = ((System.Windows.Controls.PasswordBox)o).Password.ToString();
                        if (string.IsNullOrEmpty(pass))
                        {
                            wind.Close();
                            return;
                        }

                        if (Email != ConfigurationManager.AppSettings["Host"]) { }
                        var service = new MailService();
                        try
                        {
                            await service.ValidateMessage(Email, pass, Host);
                        }
                        catch
                        {
                            await wind.ShowMessageAsync("Error", "Check Internet connection or password");
                        }
                        while (true)
                        {
                            var choose = await wind.ShowInputAsync("Verification Code", "");
                            if (service.ValidateCode(choose))
                                break;
                            else
                            {
                                var end_res = await wind.ShowMessageAsync("Error", "Verification codes doesnt matches", MessageDialogStyle.AffirmativeAndNegative);
                                if (end_res == MessageDialogResult.Negative)
                                    return;
                            }
                        }
                        wind.Close();
                    }));
            }
        }

        private RelayCommand create;

        public RelayCommand Create
        {
            get
            {
                return create ??
                    (create = new RelayCommand(async (o) =>
                    {
                        if (IsValid == false)
                        {
                            MessageDialogResult result = await wind.ShowMessageAsync("Error", "Fields has errors. Fix it to create profile", MessageDialogStyle.Affirmative);
                            return;
                        }

                    }));
            }
        }


    }
}
