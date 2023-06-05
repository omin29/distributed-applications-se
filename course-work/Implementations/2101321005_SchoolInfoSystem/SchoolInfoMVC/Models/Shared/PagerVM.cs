namespace SchoolInfoMVC.Models.Shared
{
    public class PagerVM
    {
        public int Page { get; set; } = 1;
        public int ItemsPerPage { get; set; } = 10;
        public int PagesCount { get; set; }
    }
}
