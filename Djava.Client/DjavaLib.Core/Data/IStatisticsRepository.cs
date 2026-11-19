using DjavaLib.Models;

namespace DjavaLib.Core.Data
{
    public interface IStatisticsRepository
    {
        StatisticsModel GetGeneralStatistics();
    }
}