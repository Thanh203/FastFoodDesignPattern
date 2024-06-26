﻿using FastFoodSystem.WebApp.Models;
using FastFoodSystem.WebApp.Models.Data;
using FastFoodSystem.WebApp.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NuGet.Protocol;
using Microsoft.AspNetCore.Authorization;
using iTextSharp.text.pdf;
using FastFoodSystem.WebApp.Models.Order;

namespace AgentManager.WebApp.Controllers
{
    //[Authorize (Roles = "Admin,Manager,Staff")]
    public class OrderController : Controller
    {
        // Setting Session form item cart
        private readonly ILogger<OrderController> logger;
        private readonly IHttpContextAccessor _contx;
        private readonly FastFoodSystemDbContext _context;
        private readonly ICartItemFactory _cartItemFactory;

        //
        public int SelectedCategoryId { get; set; }
        DBHelper dBHelper;
        public OrderController(ILogger<OrderController> logger, IHttpContextAccessor contx, FastFoodSystemDbContext context, ICartItemFactory cartItemFactory)
        {
            this.logger = logger;
            _contx = contx;
            _context = context;
            _cartItemFactory = cartItemFactory;
        }

        // GET: OrderController
        // Trong controller
        public async Task<IActionResult> Index(string selectedCategoryId = "BG")
        {
            var model = new ProductCategoryViewModel();
            model.Categories = await _context.FFSProductCategories.ToListAsync();

            if (selectedCategoryId != "")
            {
                model.Products = await _context.FFSProducts.Where(p => p.FFSProductCategoryId == selectedCategoryId).ToListAsync();
            }
            else
            {
                model.Products = await _context.FFSProducts.ToListAsync();
            }

            ViewBag.Categories = model.Categories;
            ViewBag.SelectedCategoryId = selectedCategoryId; // Truyền danh mục đã chọn vào ViewBag

            return View(model);
        }

        List<CartItem> cartItems = new List<CartItem>(); // Danh sách sản phẩm trong giỏ hàng
        [HttpPost]
        public IActionResult Index([FromBody] CartItem data)
        {
            if (!String.IsNullOrEmpty(_contx.HttpContext.Session.GetString("IdCurrentOrder")))
            {
                return BadRequest("Hãy hoàn thành đơn hàng hiện tại hoặc hủy nó");
            }
            try
            {
                string cartItemsString = _contx.HttpContext.Session.GetString("CartItems");
                cartItems = JsonConvert.DeserializeObject<List<CartItem>>(cartItemsString);
            }
            catch { }
            if (data != null)
            {
                string productId = data.FFSProductId;
                int quantity = data.Quantity;
                // Tìm sản phẩm trong danh sách giỏ hàng
                var existingItem = cartItems.FirstOrDefault(item => item.FFSProductId == productId);

                if (existingItem != null)
                {
                    // Cập nhật số lượng nếu sản phẩm đã tồn tại
                    existingItem.Quantity = quantity;
                }
                else
                {
                    // Thêm sản phẩm mới vào giỏ hàng
                    var product = _cartItemFactory.CreateCartItem(productId, quantity);
                    cartItems.Add(product);
                }

                //Session saving - 1hour
                string cartItemString = JsonConvert.SerializeObject(cartItems);
                _contx.HttpContext.Session.SetString("CartItems", cartItemString);
            }
            return Ok(); // Trả về kết quả Ajax thành công
        }

        public IActionResult ListOrder(string searchText = "")
        {
            ViewBag.SearchText = searchText;
            var orders = _context.FFSOrders.ToList();

            if (!String.IsNullOrEmpty(searchText))
            {
                orders = _context.FFSOrders
                    .Where(a => a.FFSOrderId == int.Parse(searchText)).ToList();

            }

            orders.Reverse();


            foreach(var order in orders)
            {
                Console.WriteLine(order.FFSOrderId);
            }
            return View(orders);
        }
        
        public IActionResult Delete(int id)
        {
            var order = _context.FFSOrders.FirstOrDefault(_context => _context.FFSOrderId == id);
            Console.WriteLine(order.ToJson());
            List<FFSProductOrder> products = _context.FFSProductOrders
            .Where(item => item.FFSOrderId == id)
            .OrderBy(item => item.FFSOrderId)
            .ToList();
            ViewBag.Products = products;
            List<CartItem> _products = new List<CartItem> { };
            foreach (var product in products)
            {
                var obj = _context.FFSProducts.FirstOrDefault(item => item.FFSProductId == product.FFSProductId);
                CartItem _product = _cartItemFactory.CreateCartItem(obj.FFSProductId, product.Quantity);
                _products.Add(_product);
            }

            return View(order);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            Console.WriteLine("ID:");
            Console.WriteLine(id);
            if (_context.FFSOrders == null)
            {
                return Problem("Entity set 'FastFoodSystemDbContext.FFSOrders'  is null.");
            }
            OrderProcessor orderProcessor = new OrderProcessor(id, _context);
            orderProcessor.DeleteOrder();

            return RedirectToAction(nameof(ListOrder));
        }
        public IActionResult Details(int id)
        {
            var order = _context.FFSOrders.FirstOrDefault(_context => _context.FFSOrderId == id);
            List<FFSProductOrder> products = _context.FFSProductOrders
            .Where(item => item.FFSOrderId == id)
            .OrderBy(item => item.FFSOrderId)
            .ToList();
            ViewBag.Products = products;
            List<CartItem> _products = new List<CartItem> { };
            foreach (var product in products)
            {
                var obj = _context.FFSProducts.FirstOrDefault(item => item.FFSProductId == product.FFSProductId);
                CartItem _product = _cartItemFactory.CreateCartItem(obj.FFSProductId, product.Quantity);

                _products.Add(_product);
            }
            
            return View(order);
        }
        public async Task<IActionResult> Edit(int id)
        {
            var order = _context.FFSOrders.FirstOrDefault(_context => _context.FFSOrderId == id);
            Console.WriteLine(order.ToJson());
            List<FFSProductOrder> products = _context.FFSProductOrders
            .Where(item => item.FFSOrderId == id)
            .OrderBy(item => item.FFSOrderId)
            .ToList();
            ViewBag.Products = products;
            List<CartItem> _products = new List<CartItem> { };
            foreach (var product in products)
            {
                var obj = _context.FFSProducts.FirstOrDefault(item => item.FFSProductId == product.FFSProductId);
                CartItem _product = _cartItemFactory.CreateCartItem(obj.FFSProductId, product.Quantity);

                _products.Add(_product);
            }

            return View(order);
        }
        [HttpPost, ActionName("Update")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateComfirm(int id, FFSOrder updatedOrder, List<FFSProductOrder> products)
        {

            OrderProcessor orderProcessor = new OrderProcessor(id, _context);
            orderProcessor.EditOrder(updatedOrder, products);

            ViewBag.Products = products;
            return RedirectToAction(nameof(ListOrder));
        }
    }
}
