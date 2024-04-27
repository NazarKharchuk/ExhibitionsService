namespace ExhibitionsService.BLL.DTO.HelperDTO
{
    public class PaintingInfoDTO
    {
        public int PaintingId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CretionDate { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public string ImagePath { get; set; }
        public string? Location { get; set; }
        
        public int LikesCount { get; set; }
        public bool? IsLiked { get; set; }
        public int RatingCount { get; set; }
        public decimal AvgRating { get; set; }
        public int ExhibitionApplicationsCount { get; set; }
        public int ContestApplicationsCount { get; set; }
        public int ContestVictoriesCount { get; set; }

        public PainterDTO Painter { get; set; }

        public List<GenreDTO> Genres { get; set; } = [];
        public List<StyleDTO> Styles { get; set; } = [];
        public List<MaterialDTO> Materials { get; set; } = [];
        public List<TagDTO> Tags { get; set; } = [];
    }
}
