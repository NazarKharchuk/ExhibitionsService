namespace ExhibitionsService.PL.Models.HelperModel
{
    public class PaintingsFiltrationPaginationRequestModel : PaginationRequestModel
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
