using ShopApplicationV.Models;
using ShopApplicationV.ViewModel;
using ShopApplicationV.Logic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShopApplicationV.Views;

namespace ShopApplicationV.ViewModels
{
    public class NotificationsViewModel : ViewModelBase
    {
        private ObservableCollection<Notification> notifications;

        public ObservableCollection<Notification> Notifications
        {
            get { return notifications; }
            set { Set(ref notifications, value); }
        }

        private Notification selectedNotification;
        public Notification SelectedNotification
        {
            get { return selectedNotification; }
            set { Set(ref selectedNotification, value); }
        }

        public NotificationsView wind;

        public NotificationsViewModel(ObservableCollection<Notification> notif, NotificationsView wind) {
            Notifications = notif;
            this.wind = wind;
        }

        private RelayCommand delete;
        public RelayCommand Delete
        {
            get
            {
                return delete ??
                    (delete = new RelayCommand((o) =>
                    {
                        DataContext.GetInstance().Notifications.Remove(SelectedNotification);
                        Notifications.Remove(SelectedNotification);
                        if (Notifications.Count == 0)
                        {
                            ((ApplicationViewModel)App.Current.Windows[0].DataContext).isCollectionEnabled();
                            wind.Close();
                        }
                        else SelectedNotification = Notifications.FirstOrDefault();
                        DataContext.GetInstance().SaveChanges();
                    }));
            }
        }

    }
}
