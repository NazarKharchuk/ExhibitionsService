namespace ExhibitionsService.BLL.DTO.HelperDTO
{
    public class PaintingsFiltrationPaginationRequestDTO : PaginationRequestDTO
    {
        public int? PainterId { get; set; }
        public List<int>? TagsIds { get; set; }
        public List<int>? GenresIds { get; set; }
        public List<int>? MaterialsIds { get; set; }
        public List<int>? StylesIds { get; set; }
        public string? SortBy { get; set; }
        public string? SortOrder { get; set; }
    }
}
