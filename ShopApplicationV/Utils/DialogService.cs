using System;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.Controls;
using System.Threading.Tasks;

namespace ShopApplicationV.Utils
{
    public class DialogService
    {
        private MetroWindow wind;
        public DialogService(MetroWindow wind)
        {
            this.wind = wind;
        }

        public async void CloseWind(string Title, string Message, MessageDialogStyle settings)
        {
            MessageDialogResult result = await wind.ShowMessageAsync(Title, Message);
            if (result == MessageDialogResult.Negative)
                await Task.Run(() => { wind.Close(); });
            else return;
               //else return;
        }

        //public async Task<MessageDialogResult> ConfirmAsync(string Title, string Message, MessageDialogStyle settings)
        //{
        //    wind.ShowMessageAsync(Title, Message, settings);
        //}

        public void Exception(Exception ex)
        {
            string message = $@"An exception thrown: {ex.Message}

{ex.ToString()}";
       //     MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public void ShowMessage(string message)
        {
     //       MessageBox.Show(message, "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public void Warning(string message)
        {
   //         MessageBox.Show(message, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
}
