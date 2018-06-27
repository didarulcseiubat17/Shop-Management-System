using ShopApplicationV.Utils;
using ShopApplicationV.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ShopApplicationV.Models;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace ShopApplicationV.ViewModels
{
    public class EditEntityViewModel<T> where T : ValidViewModelBase<T>, new()
    {

        public T Entity { get; private set; }
        public MetroWindow Wind { get; }
        public DialogService Service { get; }

        public EditEntityViewModel(T ent, MetroWindow wind)
        {
            Service = new DialogService(Wind);
            Entity = ent;
            Wind = wind;
        }

        private RelayCommand close;
        public virtual RelayCommand Close
        {
            get
            {
                return close ??
                    (close = new RelayCommand(async (o) =>
                    {
                        if (Entity.IsValid)
                            Wind.DialogResult = true;
                        else
                        {
                            if (Service != null)
                            {

                                string Title = "Confirm";
                                string Message = "Entity has errors. Press OK to exit without changes or " +
                                "CANCEL to keep edit";
                                MessageDialogResult result = await Wind.ShowMessageAsync(Title, Message, MessageDialogStyle.AffirmativeAndNegative);

                                if (result == MessageDialogResult.Affirmative)
                                    Wind.Close();
                                else return;
                            }
                            else Wind.Close();
                        }
                    }));
            }
        }



    }
}
