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
        public AdminDashboardService(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async Task<AdminDashboardViewModel> GetDashboardDataAsync()
        {
            var users = _userManager.Users.ToList();
            var orders = await _unitOfWork.Orders.GetAllAsync("OrderItems,OrderItems.Product");

            var thisYear = DateTime.Now.Year;
            var lastYear = thisYear - 1;

            // User statistics
            var usersThisYear = users.Count(u => u.CreatedAt.Year == thisYear);
            var usersLastYear = users.Count(u => u.CreatedAt.Year == lastYear);

            // Order statistics
            var ordersThisYear = orders.Count(o => o.CreatedAt.Year == thisYear);
            var ordersLastYear = orders.Count(o => o.CreatedAt.Year == lastYear);

            // Sales statistics
            var salesThisYear = orders
                .Where(o => o.CreatedAt.Year == thisYear)
                .Sum(o => o.TotalAmount);

            var salesLastYear = orders
                .Where(o => o.CreatedAt.Year == lastYear)
                .Sum(o => o.TotalAmount);

            // Page views simulation (you'll need to implement actual page view tracking)
            var totalPageViews = users.Count * 15; // Simulate 15 page views per user on average
            var pageViewsThisYear = usersThisYear * 15;
            var pageViewsLastYear = usersLastYear * 15;

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
            decimal totalIncome = 0;
            decimal totalCostOfSales = 0;

            for (int i = 5; i >= 0; i--)
            {
                var month = DateTime.Today.AddMonths(-i);
                var income = orders.Where(o => o.CreatedAt.Year == month.Year && o.CreatedAt.Month == month.Month).Sum(o => o.TotalAmount);
                var costOfSales = income * 0.6m; // Assuming 60% cost of sales

                totalIncome += income;
                totalCostOfSales += costOfSales;

                salesReport.Add(new SalesReportEntry
                {
                    Month = month.ToString("MMM"),
                    Income = income,
                    CostOfSales = costOfSales
                });
            }

            // Unique visitors data (simulated based on user registration patterns)
            var uniqueVisitorsData = new List<int>();
            for (int i = 6; i >= 0; i--)
            {
                var day = DateTime.Today.AddDays(-i);
                var dayUsers = users.Count(u => u.CreatedAt.Date == day);
                // Simulate that registered users represent about 5% of unique visitors
                var estimatedVisitors = Math.Max(dayUsers * 20, dayUsers + new Random().Next(50, 200));
                uniqueVisitorsData.Add(estimatedVisitors);
            }

            // Calculate financial metrics
            var totalRevenue = orders.Sum(o => o.TotalAmount);
            var totalCosts = totalRevenue * 0.6m; // Assuming 60% cost ratio
            var netProfit = totalIncome - totalCostOfSales;

            // Company finance growth (comparing this year to last year)
            var companyFinanceGrowth = GetPercentChange((double)salesLastYear, (double)salesThisYear);

            // Company expenses ratio (costs vs revenue)
            var companyExpensesRatio = totalRevenue > 0 ? Math.Round((double)(totalCosts / totalRevenue * 100), 1) : 0;

            // Business risk assessment based on order patterns
            var recentOrdersCount = orders.Count(o => o.CreatedAt >= DateTime.Now.AddDays(-30));
            var previousMonthOrdersCount = orders.Count(o => o.CreatedAt >= DateTime.Now.AddDays(-60) && o.CreatedAt < DateTime.Now.AddDays(-30));
            var businessRiskCases = recentOrdersCount < previousMonthOrdersCount * 0.8 ? "Medium Risk" : "Low Risk";

            var model = new AdminDashboardViewModel
            {
                // Page Views
                TotalPageViews = totalPageViews,
                PageViewsChangePercent = GetPercentChange(pageViewsLastYear, pageViewsThisYear),
                ExtraPageViewsThisYear = pageViewsThisYear - pageViewsLastYear,

                // Users
                TotalUsers = users.Count,
                UsersChangePercent = GetPercentChange(usersLastYear, usersThisYear),
                ExtraUsersThisYear = usersThisYear - usersLastYear,

                // Orders
                TotalOrders = orders.Count(),
                OrdersChangePercent = GetPercentChange(ordersLastYear, ordersThisYear),
                ExtraOrdersThisYear = ordersThisYear - ordersLastYear,

                // Sales
                TotalSales = salesThisYear,
                SalesChangePercent = GetPercentChange((double)salesLastYear, (double)salesThisYear),
                ExtraSalesThisYear = salesThisYear - salesLastYear,

                // Recent Orders
                RecentOrders = orders
                    .OrderByDescending(o => o.CreatedAt)
                    .Take(10)
                    .Select(o => new RecentOrderViewModel
                    {
                        TrackingNumber = o.Id.ToString(),
                        ProductName = o.OrderItems.OrderBy(oi => oi.CreatedAt).Select(oi => oi.Product.Name).FirstOrDefault() ?? "N/A",
                        TotalOrder = o.OrderItems.Count(),
                        Status = o.OrderStatus,
                        TotalAmount = o.TotalAmount
                    }).ToList(),

                // Transactions
                Transactions = orders
                    .OrderByDescending(o => o.CreatedAt)
                    .Take(3)
                    .Select(o => new TransactionViewModel
                    {
                        OrderId = o.Id.ToString(),
                        DateTime = o.CreatedAt,
                        Amount = o.TotalAmount,
                        Percentage = Math.Round(new Random().NextDouble() * 15 + 5, 1) // Simulate 5-20% growth
                    }).ToList(),

                // Income
                ThisWeekIncome = orders
                    .Where(o => o.CreatedAt >= DateTime.Now.AddDays(-7))
                    .Sum(o => o.TotalAmount),

                // Chart Data
                WeeklyIncomeStats = weeklyIncomeStats,
                MonthlyFinanceTrend = monthlyFinanceTrend.TakeLast(6).ToList(), // Last 6 months for the trend chart
                UniqueVisitorsData = uniqueVisitorsData,

                // Analytics
                CompanyFinanceGrowth = Math.Max(0, companyFinanceGrowth), // Ensure non-negative for progress bar
                CompanyExpensesRatio = companyExpensesRatio,
                BusinessRiskCases = businessRiskCases,

                // Sales Report
                SalesReport = salesReport,
                NetProfit = netProfit
            };

            return model;
        }

        private double GetPercentChange(double last, double current)
        {
            if (last == 0 && current == 0) return 0;
            if (last == 0) return 100;
            return Math.Round(((current - last) / last) * 100, 2);
        }
    }
}