namespace ExhibitionsService.BLL.DTO.HelperDTO
{
    public class ContestsFiltrationPaginationRequestDTO : PaginationRequestDTO
    {
        public int? PaintingId { get; set; }
        public List<int>? TagsIds { get; set; }
        public bool? NeedConfirmation { get; set; }
        public string? Status { get; set; }
        public string? SortBy { get; set; }
        public string? SortOrder { get; set; }
    }
}
