using DjavaLib.Core.Models;

namespace DjavaLib.Core.Data
{
    public interface IStatisticsRepository
    {
        StatisticsModel GetGeneralStatistics();
    }
}