using System.Threading.Tasks;

namespace ShopApplicationV.ViewModels
{
    interface IPageViewModel
    {
        string PageName { get; }
        void Init();
    }
}
