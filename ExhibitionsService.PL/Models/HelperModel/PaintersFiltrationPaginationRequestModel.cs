namespace ExhibitionsService.PL.Models.HelperModel
{
    public class PaintersFiltrationPaginationRequestModel : PaginationRequestModel
    {
        public string? SortBy { get; set; }
        public string? SortOrder { get; set; }
    }
}
