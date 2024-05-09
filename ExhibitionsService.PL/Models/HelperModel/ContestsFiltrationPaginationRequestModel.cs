namespace ExhibitionsService.PL.Models.HelperModel
{
    public class ContestsFiltrationPaginationRequestModel : PaginationRequestModel
    {
        public int? PaintingId { get; set; }
        public List<int>? TagsIds { get; set; }
        public bool? NeedConfirmation { get; set; }
        public string? Status { get; set; }
        public string? SortBy { get; set; }
        public string? SortOrder { get; set; }
    }
}
