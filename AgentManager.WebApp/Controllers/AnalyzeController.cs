using FastFoodSystem.WebApp.Models;
using FastFoodSystem.WebApp.Models.Data;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NuGet.Protocol;
using System.Collections.Generic;


namespace FastFoodSystem.WebApp.Controllers
{
    public class AnalyzeController : Controller
    {
        private readonly FastFoodSystemDbContext _context;

        public AnalyzeController(FastFoodSystemDbContext context)
        {
            _context = context;

        }
        public IActionResult Index()
        {
            var overallRevenueData = new List<RevenueData>();

            // Tính toán tổng doanh thu cho mỗi ngày
            var ordersGroupedByDate = _context.FFSOrders
                .GroupBy(order => order.Date.Date)
                .Select(group => new { Date = group.Key, TotalRevenue = group.Sum(order => order.Cash) })
                .OrderBy(result => result.Date)
                .ToList();

            foreach (var result in ordersGroupedByDate)
            {
                overallRevenueData.Add(new RevenueData
                {
                    Date = result.Date.ToString("yyyy-MM-dd"),
                    Revenue = result.TotalRevenue
                });
            }

            // Thống kê số lượng sản phẩm đã bán theo mỗi loại sản phẩm
            var productStatistics = _context.FFSProductOrders
                .GroupBy(po => po.FFSProduct.Name)
                .Select(group => new ProductStatistics
                {
                    CategoryName = group.Key,
                    QuantitySold = group.Sum(po => po.Quantity)
                })
                .ToList();

            ViewData["ProductStatistics"] = productStatistics;

            return View(overallRevenueData);
        }

        public IActionResult FilteredRevenue(string startDate, string endDate)
        {
            DateTime startDateTime = DateTime.Parse(startDate);
            DateTime endDateTime = DateTime.Parse(endDate).AddDays(1);

            var orders = _context.FFSOrders
                .Where(order => order.Date >= startDateTime && order.Date < endDateTime)
                .ToList();

            var revenueData = new List<RevenueData>();

            foreach (var order in orders)
            {
                revenueData.Add(new RevenueData
                {
                    Date = order.Date.ToString("yyyy-MM-dd"),
                    Revenue = order.Cash
                });
            }

            // Tính toán thống kê sản phẩm cho các đơn hàng đã lọc
            var productStatistics = _context.FFSProductOrders
                .Where(po => orders.Select(order => order.FFSOrderId).Contains(po.FFSOrderId))
                .GroupBy(po => po.FFSProduct.Name)
                .Select(group => new ProductStatistics
                {
                    CategoryName = group.Key,
                    QuantitySold = group.Sum(po => po.Quantity)
                })
                .ToList();

            ViewData["ProductStatistics"] = productStatistics;

            // Sắp xếp dữ liệu doanh thu theo ngày
            revenueData = revenueData.OrderBy(data => data.Date).ToList();

            return View("Index", revenueData);
        }
        
    }
}
