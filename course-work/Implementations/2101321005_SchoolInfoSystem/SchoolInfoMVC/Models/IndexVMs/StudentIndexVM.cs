using SchoolInfoMVC.Models.Interfaces;
using SchoolInfoMVC.Models.Shared;

namespace SchoolInfoMVC.Models.IndexVMs
{
    public class StudentIndexVM : IViewModelWithPagination
    {
        public IEnumerable<StudentVM> Students { get; set; } = null!;
        public PagerVM Pager { get; set; } = new PagerVM();
    }
}
