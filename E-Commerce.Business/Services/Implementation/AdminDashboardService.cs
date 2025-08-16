using E_Commerce.Business.Services.Interfaces;
using E_Commerce.DataAccess.Entities;
using E_Commerce.DataAccess.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Business.Services.Implementation
{
    public class AdminDashboardService : IAdminDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        public AdminDashboardService(IUnitOfWork unitOfWork , UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async Task<AdminDashboardViewModel> GetDashboardDataAsync()
        {
            // Example data retrieval
            var users = _userManager.Users.ToList();
            var orders = await _unitOfWork.Orders.GetAllAsync("OrderItems");
           //var visits = await _unitOfWork.Visits.GetAllAsync(); // page views

            var thisYear = DateTime.Now.Year;
            var lastYear = thisYear - 1;

            //var pageViewsThisYear = visits.Count(v => v.CreatedAt.Year == thisYear);
            //var pageViewsLastYear = visits.Count(v => v.CreatedAt.Year == lastYear);

            var usersThisYear = users.Count(u => u.CreatedAt.Year == thisYear);
            var usersLastYear = users.Count(u => u.CreatedAt.Year == lastYear);

            var ordersThisYear = orders.Count(o => o.CreatedAt.Year == thisYear);
            var ordersLastYear = orders.Count(o => o.CreatedAt.Year == lastYear);

            var salesThisYear = orders
                .Where(o => o.CreatedAt.Year == thisYear)
                .Sum(o => o.TotalAmount);

            var salesLastYear = orders
                .Where(o => o.CreatedAt.Year == lastYear)
                .Sum(o => o.TotalAmount);

            var model = new AdminDashboardViewModel
            {
                //TotalPageViews = pageViewsThisYear,
                //PageViewsChangePercent = GetPercentChange(pageViewsLastYear, pageViewsThisYear),
                //ExtraPageViewsThisYear = pageViewsThisYear - pageViewsLastYear,

                TotalUsers = users.Count,
                UsersChangePercent = GetPercentChange(usersLastYear, usersThisYear),
                ExtraUsersThisYear = usersThisYear - usersLastYear,

                TotalOrders = orders.Count(),
                OrdersChangePercent = GetPercentChange(ordersLastYear, ordersThisYear),
                ExtraOrdersThisYear = ordersThisYear - ordersLastYear,

                TotalSales = salesThisYear,
                SalesChangePercent = GetPercentChange((double)salesLastYear, (double)salesThisYear),
                ExtraSalesThisYear = salesThisYear - salesLastYear,

                RecentOrders = orders
                    .OrderByDescending(o => o.CreatedAt)
                    .Take(10)
                    .Select(o => new RecentOrderViewModel
                    {
                        TrackingNumber = "",
                        ProductName = o.OrderItems.OrderBy(o => o.CreatedAt).Select(o => o.Product.Name).FirstOrDefault(),
                        TotalOrder = o.OrderItems.Count(),
                        Status = o.OrderStatus,
                        TotalAmount = o.TotalAmount
                    }).ToList(),

                Transactions = orders
                    .OrderByDescending(o => o.CreatedAt)
                    .Take(3)
                    .Select(o => new TransactionViewModel
                    {
                        OrderId = o.Id.ToString(),
                        DateTime = o.CreatedAt,
                        Amount = o.TotalAmount,
                        Percentage = new Random().Next(5, 80) // placeholder
                    }).ToList(),

                ThisWeekIncome = orders
                    .Where(o => o.CreatedAt >= DateTime.Now.AddDays(-7))
                    .Sum(o => o.TotalAmount),

                WeeklyIncomeStats = GetMockBarData(7),
                MonthlyFinanceTrend = GetMockLineData(12),

                CompanyFinanceGrowth = 45.14,
                CompanyExpensesRatio = 0.58,
                BusinessRiskCases = "Low",

                SalesReport = new List<SalesReportEntry>
            {
                new() { Month = "Jan", Income = 180, CostOfSales = 120 },
                new() { Month = "Feb", Income = 90, CostOfSales = 50 },
                new() { Month = "Mar", Income = 140, CostOfSales = 80 },
                new() { Month = "Apr", Income = 110, CostOfSales = 140 },
                new() { Month = "May", Income = 120, CostOfSales = 160 },
                new() { Month = "Jun", Income = 150, CostOfSales = 100 },
            }
            };

            return model;
        }

        private double GetPercentChange(double last, double current)
        {
            if (last == 0) return 100;
            return Math.Round(((current - last) / last) * 100, 2);
        }

        private List<decimal> GetMockBarData(int count)
        {
            var random = new Random();
            return Enumerable.Range(1, count).Select(_ => (decimal)random.Next(1000, 20000)).ToList();
        }

        private List<decimal> GetMockLineData(int count)
        {
            var random = new Random();
            return Enumerable.Range(1, count).Select(_ => (decimal)random.Next(10000, 50000)).ToList();
        }
    }
}
