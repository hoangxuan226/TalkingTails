using TalkingTails.Business.Models.Dashboard;

namespace TalkingTails.Business.Interfaces
{
    public interface IDashboardService
    {
        Task<StatisticalDto> GetStatisticsAsync(DateTime? startDate, DateTime? endDate);
    }
}