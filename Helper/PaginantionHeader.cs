namespace LearnerDuo.Helper
{
    public class PaginantionHeader
    {
        public PaginantionHeader(int currentPage, int itemsPerpage, int totalItems, int totalPages)
        {
            CurrentPage = currentPage;
            PageSize = itemsPerpage;
            TotalCount = totalItems;
            TotalPages = totalPages;
        }

        public int CurrentPage { get; set; }   
        public int PageSize { get; set; }
        public int TotalCount {  get; set; }    
        public int TotalPages { get; set; }

    }
}
