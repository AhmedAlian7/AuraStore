using E_Commerce.DataAccess.Enums;

public class AdminDashboardViewModel
{
    // Summary Cards
    public int TotalPageViews { get; set; }
    public double PageViewsChangePercent { get; set; }
    public int ExtraPageViewsThisYear { get; set; }

    public int TotalUsers { get; set; }
    public double UsersChangePercent { get; set; }
    public int ExtraUsersThisYear { get; set; }

    public int TotalOrders { get; set; }
    public double OrdersChangePercent { get; set; }
    public int ExtraOrdersThisYear { get; set; }

    public decimal TotalSales { get; set; }
    public double SalesChangePercent { get; set; }
    public decimal ExtraSalesThisYear { get; set; }

    // Unique Visitors Chart
    public List<int> UniqueVisitorsData { get; set; }
    public string VisitorsViewType { get; set; } // Week or Month

    // Income Overview
    public decimal ThisWeekIncome { get; set; }
    public List<decimal> WeeklyIncomeStats { get; set; }

    // Recent Orders
    public List<RecentOrderViewModel> RecentOrders { get; set; }
  
    // Analytics Report
    public double CompanyFinanceGrowth { get; set; }
    public double CompanyExpensesRatio { get; set; }
    public string BusinessRiskCases { get; set; }
    public List<decimal> MonthlyFinanceTrend { get; set; }

    // Sales Report
    public decimal NetProfit { get; set; }
    public List<SalesReportEntry> SalesReport { get; set; }

    // Transactions
    public List<TransactionViewModel> Transactions { get; set; }
}

public class RecentOrderViewModel
{
    public string TrackingNumber { get; set; }
    public string ProductName { get; set; }
    public int TotalOrder { get; set; }
    public OrderStatus Status { get; set; } // Approved, Pending, Rejected
    public decimal TotalAmount { get; set; }
}

public class SalesReportEntry
{
    public string Month { get; set; }
    public decimal Income { get; set; }
    public decimal CostOfSales { get; set; }
}

public class TransactionViewModel
{
    public string OrderId { get; set; }
    public DateTime DateTime { get; set; }
    public decimal Amount { get; set; }
    public double Percentage { get; set; }
}
