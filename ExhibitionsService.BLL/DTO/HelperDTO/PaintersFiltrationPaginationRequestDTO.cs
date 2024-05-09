namespace ExhibitionsService.BLL.DTO.HelperDTO
{
    public class PaintersFiltrationPaginationRequestDTO : PaginationRequestDTO
    {
        public string? SortBy { get; set; }
        public string? SortOrder { get; set; }
    }
}
