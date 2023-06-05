using SchoolInfoMVC.Models.Shared;

namespace SchoolInfoMVC.Models.Interfaces
{
    public interface IViewModelWithPagination
    {
        public PagerVM Pager { get; set; }
    }
}
