namespace ExhibitionsService.PL.Models.HelperModel
{
    public class ExhibitionFiltrationPaginationRequestModel : PaginationRequestModel
    {
        public int? PaintingId { get; set; }
        public List<int>? TagsIds { get; set; }
        public bool? NeedConfirmation { get; set; }
        public string? SortBy { get; set; }
        public string? SortOrder { get; set; }
    }
}
