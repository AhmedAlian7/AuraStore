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
            var users = _userManager.Users.ToList();
            var orders = await _unitOfWork.Orders.GetAllAsync("OrderItems,OrderItems.Product");
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

            // Weekly income stats (last 7 days)
            var weeklyIncomeStats = new List<decimal>();
            for (int i = 6; i >= 0; i--)
            {
                var day = DateTime.Today.AddDays(-i);
                var dayTotal = orders.Where(o => o.CreatedAt.Date == day).Sum(o => o.TotalAmount);
                weeklyIncomeStats.Add(dayTotal);
            }

            // Monthly finance trend (last 12 months)
            var monthlyFinanceTrend = new List<decimal>();
            for (int i = 11; i >= 0; i--)
            {
                var month = DateTime.Today.AddMonths(-i);
                var monthTotal = orders.Where(o => o.CreatedAt.Year == month.Year && o.CreatedAt.Month == month.Month).Sum(o => o.TotalAmount);
                monthlyFinanceTrend.Add(monthTotal);
            }

            // Sales report (last 6 months)
            var salesReport = new List<SalesReportEntry>();
            for (int i = 5; i >= 0; i--)
            {
                var month = DateTime.Today.AddMonths(-i);
                var income = orders.Where(o => o.CreatedAt.Year == month.Year && o.CreatedAt.Month == month.Month).Sum(o => o.TotalAmount);
                // For cost of sales, you might want to sum up product costs or similar. Here, just use 60% as a placeholder.
                var costOfSales = income * 0.6m;
                salesReport.Add(new SalesReportEntry { Month = month.ToString("MMM"), Income = income, CostOfSales = costOfSales });
            }

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
                        TrackingNumber = o.Id.ToString(),
                        //ProductName = o.OrderItems.OrderBy(oi => oi.CreatedAt).Select(oi => oi.Product.Name).FirstOrDefault(),
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
                        Percentage = 0 // You can calculate a real percentage if you have the data
                    }).ToList(),

                ThisWeekIncome = orders
                    .Where(o => o.CreatedAt >= DateTime.Now.AddDays(-7))
                    .Sum(o => o.TotalAmount),

                WeeklyIncomeStats = weeklyIncomeStats,
                MonthlyFinanceTrend = monthlyFinanceTrend,

                CompanyFinanceGrowth = 0, // You can calculate this if you have the data
                CompanyExpensesRatio = 0, // You can calculate this if you have the data
                BusinessRiskCases = "N/A", // You can calculate this if you have the data

                SalesReport = salesReport
            };

            return model;
        }

        private double GetPercentChange(double last, double current)
        {
            if (last == 0) return 100;
            return Math.Round(((current - last) / last) * 100, 2);
        }
    }
}
