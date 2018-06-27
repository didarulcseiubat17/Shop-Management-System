using System;
using ShopApplicationV.Models;
using ShopApplicationV.ViewModel;
using System.Collections.ObjectModel;
using ShopApplicationV.Logic;
using System.Data.Entity;
using ShopApplicationV.Views;
using System.Threading;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using MahApps.Metro.Controls.Dialogs;

namespace ShopApplicationV.ViewModels
{
    public class ManagersViewModel : ViewModelBase, IPageViewModel
    {
        public string PageName => "Managers";
        private Manager currentManager;

        private ObservableCollection<Manager> managers;

        public ObservableCollection<Manager> Managers
        {
            get { return managers; }
            set { Set(ref managers, value); }
        }



        public Manager CurrentManager
        {
            get => currentManager;
            set { Set(ref currentManager, value); }
        }

        public ManagersViewModel() {        }

        public void Init()
        {
            Task.Factory.StartNew( () =>
            {
                Managers = new ObservableCollection<Manager>(DataContext.GetInstance().Managers.Local.Where(p => p.isFired == null | p.isFired == false));
                Managers.Remove(DataContext.GetInstance().AuthUser);
            });
        }

        private RelayCommand editManager;
        public RelayCommand EditManager
        {
            get
            {
                return editManager ??
                    (editManager = new RelayCommand((o) =>
                    {
                        if (o == null)
                            return;
                        var wind = new ManagerWindow();
                        Manager entity = o as Manager;
                        Manager copy = new Manager(entity);
                        EditEntityViewModel<Manager> context = new EditEntityViewModel<Manager>(copy, wind);
                        wind.DataContext = context;
                        if (wind.ShowDialog() == true)
                        {
                            entity = DataContext.GetInstance().FindManager(entity.ManagerID);                          
                            if (entity != null)
                            {
                                if (entity.isAdmin != copy.isAdmin | entity.Salary != copy.Salary)
                                {
                                    string info = DataContext.GetInstance().AuthUser.DisplayName + " (id: "
                                    + DataContext.GetInstance().AuthUser.ManagerID + " ) : " + "\n";
                                    if (entity.isAdmin != copy.isAdmin)
                                    {
                                        if (copy.isAdmin == true)
                                            info += "You are admin now" + "\n";
                                        else info += "You are not admin anymore" + "\n";
                                    }
                                    if (entity.Salary != copy.Salary)
                                    {
                                        info += "Salary: " + entity.Salary + " -> " + copy.Salary;
                                    }
                                    DataContext.GetInstance().Notifications.Add(new Notification()
                                    {
                                        Message = info,
                                        CreationDate = DateTime.Now,
                                        ReceiverID = entity.ManagerID,
                                        Seen = false
                                    });
                                }
                                entity.CopyFields(copy);
                                entity = DataContext.GetInstance().FindManager(entity.ManagerID);
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
                CurrentManager = Managers.FirstOrDefault(s => s.SurName.StartsWith(search));
            }
        }

        

        private RelayCommand addManager;
        public RelayCommand AddManager
        {
            get
            {
                return addManager ??
                    (addManager = new RelayCommand(async (o) =>
                    {
                        var wind = new ManagerWindow();
                        Manager copy = new Manager() { BirthDate = DateTime.Now };
                        EditEntityViewModel<Manager> context = new EditEntityViewModel<Manager>(copy, wind);
                        wind.DataContext = context;
                        if (wind.ShowDialog() == true)
                        {
                            if (copy != null)
                            {
                                    if (copy != null)
                                    {
                                        var obj = DataContext.GetInstance().Managers.FirstOrDefault(x => x.Name == copy.Name && x.ManagerID != copy.ManagerID && x.SurName == copy.SurName
                                       && x.Salary == copy.Salary);
                                        if (obj != null)
                                        {
                                            MessageDialogResult result = await ((MahApps.Metro.Controls.MetroWindow)App.Current.Windows[0]).ShowMessageAsync("Duplicate", "Duplicate found. Changes will not be applied", MessageDialogStyle.Affirmative);
                                            if (result == MessageDialogResult.Affirmative)
                                                return;
                                        }
                                    DataContext.GetInstance().AddManager(copy);
                                        Managers.Add(copy);
                                    }
                            }
                        }

                    }));
            }
        }

        private RelayCommand deleteManager;
        public RelayCommand DeleteManager
        {
            get
            {
                return deleteManager ??
                    (deleteManager = new RelayCommand((o) =>
                    {
                        if (o != null)
                        {
                            Manager man = o as Manager;
                            man.isFired = true;
                            DataContext.GetInstance().Entry(man).State = EntityState.Modified;
                            DataContext.GetInstance().SaveChanges();
                            Managers.Remove(man);
                            CurrentManager = Managers.FirstOrDefault();
                        }
                    }));
            }
        }
    }
}
