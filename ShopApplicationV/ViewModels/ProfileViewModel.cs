using MahApps.Metro.Controls;
using ShopApplicationV.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShopApplicationV.Utils;
using ShopApplicationV.ViewModel;
using MahApps.Metro.Controls.Dialogs;

namespace ShopApplicationV.ViewModels
{
    public class ProfileViewModel : EditEntityViewModel<Manager>
    {
        private LoginService loginservice = new LoginService();

        public ProfileViewModel(Manager ent, MetroWindow wind) : base(ent, wind)
        {

        }

        private RelayCommand save;
        public RelayCommand Save
        {
            get
            {
                return save ??
                    (save = new RelayCommand(async (o) =>
                    {
                        var pass = ((System.Windows.Controls.PasswordBox)o).Password.ToString();
                        if (Entity.IsValid && (pass == null | pass == ""))
                            Wind.DialogResult = true;
                        else
                        {
                            if (Entity.IsValid == false)
                            {

                                MessageDialogResult result = await Wind.ShowMessageAsync("Confirm",
                                    "Fields has errors.Press OK to exit without changes or " +
                                    "CANCEL to keep edit", MessageDialogStyle.AffirmativeAndNegative);
                                if (result == MessageDialogResult.Affirmative)
                                {
                                    Wind.Close();
                                    return;
                                }
                                else return;
                            }
                            if (loginservice.PassIsMatch(pass) == false)
                            {
                                MessageDialogResult result = await Wind.ShowMessageAsync("PassError",
                                    "Invalid password. Press OK to exit without changes or CANCEL to try again", MessageDialogStyle.AffirmativeAndNegative);
                                if (result == MessageDialogResult.Affirmative)
                                {
                                    Wind.Close();
                                    return;
                                }
                                else return;
                            }
                            loginservice.UpdatePass(Entity, pass);
                            Wind.DialogResult = true;
                        }
                    }));
            }
        }

    }
}
