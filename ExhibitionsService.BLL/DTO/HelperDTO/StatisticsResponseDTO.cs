namespace ExhibitionsService.BLL.DTO.HelperDTO
{
    public class StatisticsResponseDTO<T>
    {
        public List<T> StatisticsValue { get; set; }
        public DateTime? VeryFirstValue { get; set; }
        public DateTime? VeryLastValue { get; set;}
    }

    public class StatisticsLikesValueDTO
    {
        public int LikesCount { get; set; }
        public DateTime? TimePeriodStart { get; set; }
        public DateTime? TimePeriodEnd { get; set; }
    }

    public class StatisticsRatingsValueDTO
    {
        public RatingValueDTO RatingsCount { get; set; }
        public DateTime? TimePeriodStart { get; set; }
        public DateTime? TimePeriodEnd { get; set; }
    }

    public class RatingValueDTO(int _From0To1 = 0, int _From1To2 = 0, int _From2To3 = 0, int _From3To4 = 0, int _From4To5 = 0)
    {
        public int From0To1 { get; set; } = _From0To1;
        public int From1To2 { get; set; } = _From1To2;
        public int From2To3 { get; set; } = _From2To3;
        public int From3To4 { get; set; } = _From3To4;
        public int From4To5 { get; set; } = _From4To5;
    }
}
