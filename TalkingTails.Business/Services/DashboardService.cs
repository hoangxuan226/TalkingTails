using Microsoft.EntityFrameworkCore;
using TalkingTails.Business.Interfaces;
using TalkingTails.Business.Models.Dashboard;
using TalkingTails.Repository.Entities;
using TalkingTails.Repository.Interfaces;

namespace TalkingTails.Business.Services
{
    public class DashboardService(IUnitOfWork unitOfWork) : IDashboardService
    {
        public async Task<StatisticalDto> GetStatisticsAsync(DateTime? startDate, DateTime? endDate)
        {
            var totalPets = await unitOfWork.GenericRepository<Pet>().GetQueryable()
                .CountAsync(p => (startDate == null || p.CreatedAt >= startDate) &&
                                 (endDate == null || p.CreatedAt <= endDate));
            var totalAdoptedPets = await unitOfWork.GenericRepository<AdoptedPet>().GetQueryable()
                .CountAsync(p => (startDate == null || p.CreatedAt >= startDate) &&
                                 (endDate == null || p.CreatedAt <= endDate));
            var totalDonationAmount = unitOfWork.GenericRepository<Donation>().GetQueryable()
                .Where(d => (startDate == null || d.CreatedAt >= startDate) &&
                            (endDate == null || d.CreatedAt <= endDate))
                .Sum(d => d.Amount);
            var totalBlogs = await unitOfWork.GenericRepository<Blog>().GetQueryable()
                .CountAsync(b => (startDate == null || b.CreatedAt >= startDate) &&
                                 (endDate == null || b.CreatedAt <= endDate));
            return new StatisticalDto
            {
                TotalPets = totalPets,
                TotalAdoptedPets = totalAdoptedPets,
                TotalDonationAmount = totalDonationAmount,
                TotalBlogs = totalBlogs
            };
        }
    }
}