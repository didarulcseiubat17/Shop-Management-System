using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MahApps.Metro.Controls;
using ShopApplicationV.Models;
using ShopApplicationV.Logic;
using ShopApplicationV.ViewModel;
using ShopApplicationV.Validators;
using System.Security;
using MahApps.Metro.Controls.Dialogs;
using ShopApplicationV.Utils;

namespace ShopApplicationV.ViewModels
{
    public class LoginViewModel : ValidViewModelBase<LoginViewModel>
    {
        MetroWindow wind;
        LoginService loginservice;

        public LoginViewModel(MetroWindow wind, LoginService service) : base(LoginValidation.Singleton.Value)
        {
            this.wind = wind;
            BirthDate = DateTime.Now;
            loginservice = service;
        }

        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                Set(ref name, value);
            }
        }


        private string surname;
        public string SurName
        {
            get { return surname; }
            set
            {
                Set(ref surname, value);
            }
        }

        private string email;
        public string Email
        {
            get { return email; }
            set { Set(ref email, value); }
        }

        private DateTime birthdate;
        public DateTime BirthDate
        {
            get { return birthdate; }
            set { Set(ref birthdate, value); }
        }


        private RelayCommand reset;
        public RelayCommand Reset
        {
            get
            {
                return reset ??
                    (reset = new RelayCommand(async (o) =>
                    {

                        loginservice.UserExist(email);
                        if (loginservice.UserExist(email) == false)
                        {
                            MessageDialogResult invalid = await wind.ShowMessageAsync("Error", "User with following email doesnt exist.", MessageDialogStyle.Affirmative);
                            return;
                        }
                        string code = EncryptService.GeneratePassword();
                        try
                        {
                            await MailService.SendMess(email, "Auth code", $"Your auth code for current session: \n{code}\n Change your password as soon as it possible");
                        }
                        catch
                        {
                            await wind.ShowMessageAsync("Error", "Check Internet connection");
                            return;
                        }
                        while(true)
                        {
                            var result = await wind.ShowInputAsync("Verification Code", "Enter your restore code");
                            if (result == null)
                                return;
                            else if (loginservice.AuthenticateViaCode(code, result, email))
                                break;
                            else
                            {
                                var end_res = await wind.ShowMessageAsync("Error", "Verification codes doesnt matches", MessageDialogStyle.AffirmativeAndNegative);
                                if (end_res == MessageDialogResult.Negative)
                                    return;
                            }
                        }
                        if (loginservice.AuthUser.isFired != null && loginservice.AuthUser.isFired == true)
                            await wind.ShowMessageAsync("Login", "Oops. You are fired.", MessageDialogStyle.Affirmative);
                        else
                        {
                            MessageDialogResult done = await wind.ShowMessageAsync("Login", "Success.", MessageDialogStyle.Affirmative);
                            wind.DialogResult = true;
                        }
                        wind.Close();
                        //ShopApplicationV.Utils.MailService.SendMess();

                    }));
            }
        }

        private RelayCommand auth;
        public RelayCommand Auth
        {
            get
            {
                return auth ??
                    (auth = new RelayCommand( async (o) =>
                    {
                        var pass = ((System.Windows.Controls.PasswordBox)o).Password.ToString();
                        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(pass))
                            return;
                        loginservice.Authenticate(email, pass);
                        if (loginservice.AuthUser == null)
                        {
                            MessageDialogResult invalid = await wind.ShowMessageAsync("Error", "Invalid login or password.", MessageDialogStyle.Affirmative);
                            return;
                        }
                        //// authentication successful
                        //return user;
                        if (loginservice.AuthUser.isFired != null && loginservice.AuthUser.isFired == true)
                            await wind.ShowMessageAsync("Login", "Oops. You are fired.", MessageDialogStyle.Affirmative);

                        else { MessageDialogResult done = await wind.ShowMessageAsync("Login", "Success.", MessageDialogStyle.Affirmative);
                            wind.DialogResult = true;
                        }
                        wind.Close();
                        //ShopApplicationV.Utils.MailService.SendMess();

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
                            MessageDialogResult result = await wind.ShowMessageAsync("Error","Fields has errors. Fix it to create profile", MessageDialogStyle.Affirmative);
                            return;
                        }
                        //UnSafe. Temp (probably) solution
                        var pass = ((System.Windows.Controls.PasswordBox)o).Password.ToString();

                        if (loginservice.PassIsMatch(pass) == false)
                        {
                            MessageDialogResult invalid = await wind.ShowMessageAsync("Invalid Password", "Password doent match. Watch tool tip.", MessageDialogStyle.Affirmative);
                                return;
                        }

                        if (DataContext.GetInstance().Managers.Select(p=>p).Any(x => x.Email == Email))
                        {
                            MessageDialogResult invalid = await wind.ShowMessageAsync("Error", "User already exist", MessageDialogStyle.Affirmative);
                            return;
                        }
                        loginservice.Create(Email, Name, SurName, BirthDate, pass);
                        MessageDialogResult res = await wind.ShowMessageAsync("Create", "Created", MessageDialogStyle.Affirmative);
                        Name = SurName = Email = ((System.Windows.Controls.PasswordBox)o).Password = "";
                        BirthDate = DateTime.Now;
                    }));
            }
        }


    }
}
