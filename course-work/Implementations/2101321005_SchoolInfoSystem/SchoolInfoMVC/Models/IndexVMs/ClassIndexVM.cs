using SchoolInfoMVC.Models.Interfaces;
using SchoolInfoMVC.Models.Shared;

namespace SchoolInfoMVC.Models.IndexVMs
{
    public class ClassIndexVM : IViewModelWithPagination
    {
        public IEnumerable<ClassVM> Classes { get; set; } = null!;
        public PagerVM Pager { get; set; } = new PagerVM();
    }
}
