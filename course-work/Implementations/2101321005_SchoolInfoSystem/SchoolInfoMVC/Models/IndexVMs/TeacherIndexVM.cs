using SchoolInfoMVC.Models.Interfaces;
using SchoolInfoMVC.Models.Shared;

namespace SchoolInfoMVC.Models.IndexVMs
{
    public class TeacherIndexVM : IViewModelWithPagination
    {
        public IEnumerable<TeacherVM> Teachers { get; set; } = null!;
        public PagerVM Pager { get; set; } = new PagerVM();
    }
}
