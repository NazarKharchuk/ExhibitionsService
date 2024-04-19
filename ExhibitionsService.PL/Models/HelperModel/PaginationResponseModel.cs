namespace ExhibitionsService.PL.Models.HelperModel
{
    public class PaginationResponseModel<T>
    {
        public List<T> PageContent { get; set; }
        public int TotalCount { get; set; }
    }
}
