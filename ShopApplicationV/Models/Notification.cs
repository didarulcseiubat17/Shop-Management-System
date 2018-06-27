using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopApplicationV.Models
{
    public class Notification : ViewModel.ViewModelBase
    {
        public int NotificationID { get; set; }
        public int ReceiverID { get; set; }
        public DateTime CreationDate { get; set; }
        public string Message { get; set; }
        private bool seen;
        public bool Seen
        {
            get { return seen; }
            set { Set(ref seen, value); }
        }
    }
}
