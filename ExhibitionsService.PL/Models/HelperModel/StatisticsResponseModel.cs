namespace ExhibitionsService.PL.Models.HelperModel
{
    public class StatisticsResponseModel<T>
    {
        public List<T> StatisticsValue { get; set; }
        public DateTime? VeryFirstValue { get; set; }
        public DateTime? VeryLastValue { get; set; }
    }

    public class StatisticsLikesValueModel
    {
        public int LikesCount { get; set; }
        public DateTime? TimePeriodStart { get; set; }
        public DateTime? TimePeriodEnd { get; set; }
    }

    public class StatisticsRatingsValueModel
    {
        public RatingValueModel RatingsCount { get; set; }
        public DateTime? TimePeriodStart { get; set; }
        public DateTime? TimePeriodEnd { get; set; }
    }

    public class RatingValueModel(int _From0To1 = 0, int _From1To2 = 0, int _From2To3 = 0, int _From3To4 = 0, int _From4To5 = 0)
    {
        public int From0To1 { get; set; } = _From0To1;
        public int From1To2 { get; set; } = _From1To2;
        public int From2To3 { get; set; } = _From2To3;
        public int From3To4 { get; set; } = _From3To4;
        public int From4To5 { get; set; } = _From4To5;
    }
}
