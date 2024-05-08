using ExhibitionsService.BLL.DTO.HelperDTO;
using ExhibitionsService.BLL.Infrastructure.Exceptions;

namespace ExhibitionsService.BLL.Services.Helpers
{
    public static class StatisticHelper
    {
        public static void ValidateDatesForStatistic(DateTime startDate, DateTime endDate, DateTime veryFirstValue,
            string dataFrequency, string periodSize)
        {
            if (startDate >= DateTime.Now)
                throw new ValidationException("Початкова дата для ститистики не може бути більшою за поточний момент");

            if (endDate < veryFirstValue)
                throw new ValidationException("Кінцева дата не може передувати даті першого запису в базі даних");

            switch (periodSize.ToLower())
            {
                case "day":
                    if (startDate.Hour != 0 || startDate.Minute != 0 || startDate.Second != 0)
                        throw new ValidationException("Для розміру періоду збору статистики 'день' - години, хвилини та секунди мають бути 00:00:00.");
                    break;
                case "month":
                    if (startDate.Day != 1 || startDate.Hour != 0 || startDate.Minute != 0 || startDate.Second != 0)
                        throw new ValidationException("Для розміру періоду збору статистики 'місяць' - дати мають бути на початку місяця (00:00:00 першого числа).");
                    break;
                case "year":
                    if (startDate.Month != 1 || startDate.Day != 1 || startDate.Hour != 0 || startDate.Minute != 0 || startDate.Second != 0)
                        throw new ValidationException("Для розміру періоду збору статистики 'рік' - дати мають бути на початку року (00:00:00 першого січня).");
                    break;
                case "decade":
                    if (startDate.Year % 10 != 0 || startDate.Month != 1 || startDate.Day != 1 || startDate.Hour != 0 || startDate.Minute != 0 || startDate.Second != 0)
                        throw new ValidationException("Для розміру періоду збору статистики 'десятиліття' - дати мають бути на початку року (00:00:00 першого січня) і бути роком, що кратний 10.");
                    break;
                default:
                    throw new ValidationException("Неправильне значення розміру періоду збору статистичних значень");
            }
            if (dataFrequency == periodSize)
                throw new ValidationException("Помилка в частотах чи періоді збору статистичних значень");
            if (CalculateIncreasedDate(startDate, periodSize) != endDate)
                throw new ValidationException("На правильна кінцева дата для статистичних даних");
        }

        public static DateTime CalculateIncreasedDate(DateTime date, string dataFrequency)
        {
            return dataFrequency.ToLower() switch
            {
                "hour" => date.AddHours(1),
                "day" => date.AddDays(1),
                "month" => date.AddMonths(1),
                "year" => date.AddYears(1),
                "decade" => date.AddYears(10),
                _ => throw new ValidationException("Неправильне значення частоти статистичних значень"),
            };
        }

        public static string GetPeriodFrequency(string periodSize)
        {
            return periodSize.ToLower() switch
            {
                "day" => "hour",
                "month" => "day",
                "year" => "month",
                "decade" => "year",
                _ => throw new ValidationException("Неправильне значення розміру часового періоду для збору статистики"),
            };
        }

        public static List<T> FillMissingIntervals<T>(
            List<T> groupedLikesList, string dataFrequency, DateTime periodStartDate, DateTime periodEndDate,
            Func<DateTime, DateTime, T> createEmptyDTO) where T : class
        {
            List<T> filledIntervals = [];

            DateTime currentIntervalStart = DateTime.SpecifyKind(periodStartDate, DateTimeKind.Unspecified);
            DateTime currentIntervalEnd;

            while (currentIntervalStart < periodEndDate)
            {
                currentIntervalEnd = DateTime.SpecifyKind(CalculateIncreasedDate(currentIntervalStart, dataFrequency), DateTimeKind.Unspecified);

                var existingEntry = groupedLikesList.FirstOrDefault(e => GetTimePeriodStart(e) == currentIntervalStart);
                if (existingEntry != null) filledIntervals.Add(existingEntry);
                else
                {
                    filledIntervals.Add(createEmptyDTO(currentIntervalStart, currentIntervalEnd));
                }

                currentIntervalStart = currentIntervalEnd;
            }

            return filledIntervals;
        }

        private static DateTime? GetTimePeriodStart(object dto)
        {
            if (dto is StatisticsLikesValueDTO likesDTO)
            {
                return likesDTO.TimePeriodStart;
            }
            else if (dto is StatisticsRatingsValueDTO ratingsDTO)
            {
                return ratingsDTO.TimePeriodStart;
            }
            return null;
        }
    }
}
