using ExhibitionsService.PL.Models.Genre;
using ExhibitionsService.PL.Models.Material;
using ExhibitionsService.PL.Models.Painter;
using ExhibitionsService.PL.Models.Style;
using ExhibitionsService.PL.Models.Tag;

namespace ExhibitionsService.PL.Models.Painting
{
    public class PaintingInfoModel
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

        public PainterModel Painter { get; set; }

        public List<GenreModel> Genres { get; set; } = [];
        public List<StyleModel> Styles { get; set; } = [];
        public List<MaterialModel> Materials { get; set; } = [];
        public List<TagModel> Tags { get; set; } = [];
    }
}
