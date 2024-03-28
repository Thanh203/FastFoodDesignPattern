using FastFoodSystem.WebApp.Models.Data;
using FastFoodSystem.WebApp.Models;
using FastFoodSystem.WebApp.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Protocols;
using Newtonsoft.Json;
using NuGet.Protocol;
using Microsoft.EntityFrameworkCore;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using FastFoodSystem.WebApp.Models.ViewModel.TypeTips;

namespace FastFoodSystem.WebApp.Controllers
{
    //[Authorize(Roles = "Admin,Manager,Staff")]
    public class CartController : Controller
    {
        private readonly IHttpContextAccessor _contx;
        private readonly FastFoodSystemDbContext _context;
        private readonly UserManager<Staff> _userManager;
        private readonly ICartItemFactory _cartItemFactory;
        private PercentTips percentTips;

        DBHelper dBHelper;
        public CartController(FastFoodSystemDbContext db, FastFoodSystemDbContext context, UserManager<Staff> userManager, ICartItemFactory cartItemFactory)
        {
            _contx = new HttpContextAccessor();
            dBHelper = new DBHelper(db);
            _context = context;
            _userManager = userManager;
            _cartItemFactory = cartItemFactory;
        }
        public async Task<string> GetCurrentUserIdAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            return user?.Id;
        }
        public void RetrieveCartitem(out List<CartItem> list, out decimal bill)
        {
            list = new List<CartItem>(); // Gán giá trị mặc định cho list
            bill = 0;
            string cartItemsString = _contx.HttpContext.Session.GetString("CartItems");

            if (!string.IsNullOrEmpty(cartItemsString))
            {
                List<CartItem> cartItems = JsonConvert.DeserializeObject<List<CartItem>>(cartItemsString);
                List<CartItem> sanPhamVMs = new List<CartItem>();

                foreach (var cartItem in cartItems)
                {
                    CartItem sanPhamVM = _cartItemFactory.CreateCartItem(cartItem.FFSProductId, cartItem.Quantity);
                    bill += sanPhamVM.total;
                    sanPhamVMs.Add(sanPhamVM);
                }

                list = sanPhamVMs;
            }
        }

        public IActionResult Index(decimal discountAmount, decimal? tipAmount)
        {
            RetrieveCartitem(out List<CartItem> sanPhamVMs, out decimal bill);
            bool state = false;
            if (sanPhamVMs != null) state = true;
            ViewBag.Bill = bill;
            ViewBag.State = state;
            ViewBag.DiscountAmount = discountAmount;
            decimal actualTipAmount = tipAmount ?? 0;
            ViewBag.TipAmount = actualTipAmount;

            // Kiểm tra xem có thông báo lỗi không
            if (TempData.ContainsKey("ErrorMessage"))
            {
                ViewBag.ErrorMessage = TempData["ErrorMessage"];
            }

            return View(sanPhamVMs);
        }

        private decimal GetPromoCodeStrategy(decimal bill, FFSVoucher voucher)
        {
            if (voucher.State == "Phần trăm")
            {
                VoucherContextStrategy context = new VoucherContextStrategy(new PercentagePromoCodeStrategy());
                return context.CalculateDiscount(bill, voucher);
            }
            else if (voucher.State == "VND")
            {
                VoucherContextStrategy context = new VoucherContextStrategy(new AmountPromoCodeStrategy());
                return context.CalculateDiscount(bill, voucher);
            }
            return 0;
        }

        [HttpPost]
        public async Task<IActionResult> Index(string promoCode, decimal discountAmount, decimal tipAmount)
        {
            RetrieveCartitem(out List<CartItem> list, out decimal bill);
            var latestOrder = _context.FFSOrders.OrderByDescending(o => o.FFSOrderId).FirstOrDefault();
            FFSVoucher voucher = _context.FFSVouchers.FirstOrDefault(v => v.FFSVoucherId == promoCode);
            int newOrderId = 0;
            discountAmount = 0;
            string errorMessage = null;

            if (latestOrder != null)
            {
                newOrderId = latestOrder.FFSOrderId + 1;
            }

            // Tạo một hoá đơn mới với ID mới
            List<FFSProductOrder> productOrders = new List<FFSProductOrder>();
            foreach (var cartItem in list)
            {
                FFSProductOrder productOrder = new FFSProductOrder
                {
                    FFSProductId = cartItem.FFSProductId,
                    Quantity = cartItem.Quantity,
                    FFSOrderId = newOrderId
                };
                productOrders.Add(productOrder);
            }

            if (voucher != null)
            {
                if (!IsValidPromoCode(voucher))
                {
                    errorMessage = "Mã khuyến mãi đã hết hạn sử dụng.";
                }
                else
                {
                    discountAmount = GetPromoCodeStrategy(bill, voucher);
                    bill -= discountAmount;                   
                }
            }
            else
            {
                errorMessage = "Mã khuyến mãi không tồn tại.";
            }

            if (string.IsNullOrEmpty(errorMessage))
            {
                
                ViewBag.PromoCode = voucher.FFSVoucherId;
                ViewBag.DiscountAmount = discountAmount;
            }
            else
            {
                TempData["ErrorMessage"] = errorMessage;
            }

            bill += tipAmount;
            Console.WriteLine(tipAmount);
            var newOrder = new FFSOrder
            {
                //Attribute
                Date = DateTime.Now,
                StaffId = "069aef07-2023-40ed-8ba7-583506e1d277",
                Cash = Convert.ToDouble(bill),
                TableId = "Table456",
                FFSVoucherId = voucher.FFSVoucherId,
                FFSProductOrders = productOrders
            };

            _context.FFSOrders.Add(newOrder);
            _context.SaveChanges();
            Console.WriteLine("Add Success");

            return RedirectToAction("Bill", "Cart", new { id = newOrderId, tipAmount = tipAmount });
        }

        [HttpPost]
        public IActionResult ApplyPromoCode(string promoCode)
        {
            RetrieveCartitem(out List<CartItem> list, out decimal bill);
            FFSVoucher voucher = _context.FFSVouchers.FirstOrDefault(v => v.FFSVoucherId == promoCode);
            decimal discountAmount = 0;
            string errorMessage = null;

            if (voucher != null)
            {
                if (!IsValidPromoCode(voucher))
                {
                    errorMessage = "Mã khuyến mãi đã hết hạn sử dụng.";
                }
                else
                {
                    discountAmount = GetPromoCodeStrategy(bill, voucher);
                    bill -= discountAmount;
                }
            }
            else
            {
                errorMessage = "Mã khuyến mãi không tồn tại.";
            }

            if (string.IsNullOrEmpty(errorMessage))
            {
                ViewBag.PromoCode = voucher.FFSVoucherId;
                ViewBag.DiscountAmount = discountAmount;
            }
            else
            {
                TempData["ErrorMessage"] = errorMessage;
            }

            return RedirectToAction("Index", new { discountAmount });
        }


        private bool IsValidPromoCode(FFSVoucher voucher)
        {
            DateTime currentDate = DateTime.Now;
            return currentDate >= voucher.StartDate && currentDate <= voucher.EndDate;
        }

        public IActionResult Bill(int id, decimal tipAmount)
        {
            RetrieveCartitem(out List<CartItem> lst, out decimal totalbill);
            var bill = _context.FFSOrders.FirstOrDefault(o => o.FFSOrderId == id);
            foreach(var item in lst)
            {
                Console.WriteLine(item.FFSProductId);
            }
            Console.WriteLine(lst.ToJson());
            ViewBag.Bill = bill;
            ViewBag.TipAmount = tipAmount;

            HttpContext.Session.Clear();

            return View(lst);
        }

        public IActionResult ExportToPdf(int id)
        {
            RetrieveCartitem(out List<CartItem> lst, out decimal totalbill);
            var bill = _context.FFSOrders.FirstOrDefault(o => o.FFSOrderId == id);

            using (MemoryStream ms = new MemoryStream())
            {
                Document document = new Document(PageSize.A6, 25, 25, 0, 0);
                PdfWriter writer = PdfWriter.GetInstance(document, ms);
                document.Open();

                // Đọc mẫu HTML từ tệp hoặc chuỗi HTML
                string htmlTemplate = System.IO.File.ReadAllText("Views/Shared/template2.html"); // Thay đổi đường dẫn

                // Tạo một StringBuilder để xây dựng nội dung HTML
                StringBuilder staffInfoHtml = new StringBuilder();

                foreach (var cart in lst)
                {
                    staffInfoHtml.Append($"<tr><td>{cart.tenSanPham}</td><td>{cart.Quantity}</td><td>{cart.gia}</td><td>{cart.total}</td></tr>");
                }

                htmlTemplate = htmlTemplate.Replace("{{StaffInfo}}", staffInfoHtml.ToString())
                                            .Replace("{{IdBill}}", id.ToString())
                                            .Replace("{{IdStaff}}", bill.StaffId)
                                            .Replace("{{Total}}", bill.Cash.ToString())
                                            .Replace("{{DateTime}}", bill.Date.ToString());


                // Render HTML thành PDF
                HTMLWorker worker = new HTMLWorker(document);
                using (StringReader sr = new StringReader(htmlTemplate))
                {
                    worker.Parse(sr);
                }
                document.Close();
                HttpContext.Session.Clear();
                byte[] pdfBytes = ms.ToArray();
                return File(pdfBytes, "application/pdf", "staff_info.pdf");
            }
        }
        [HttpPost]
        public IActionResult UpdateQuantity(string FFSProductId, int quantity)
        {
            RetrieveCartitem(out List<CartItem> list, out decimal bill);

            var cartItem = list.FirstOrDefault(item => item.FFSProductId == FFSProductId);

            if (cartItem != null)
            {
                cartItem.Quantity = quantity;

                _contx.HttpContext.Session.SetString("CartItems", JsonConvert.SerializeObject(list));
            }

            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult RemoveFromCart(string FFSProductId)
        {
            RetrieveCartitem(out List<CartItem> list, out decimal bill);

            var cartItemToRemove = list.FirstOrDefault(item => item.FFSProductId == FFSProductId);

            if (cartItemToRemove != null)
            {
                list.Remove(cartItemToRemove);

                _contx.HttpContext.Session.SetString("CartItems", JsonConvert.SerializeObject(list));
            }

            return RedirectToAction("Index");
        }
        public ActionResult Tips()
        {
            RetrieveCartitem(out List<CartItem> sanPhamVMs, out decimal bill);
            ViewBag.Bill = bill;
            // Trả về kết quả tính toán tips
            return View();
        }
        public ActionResult Calculate(decimal tipPercentage)
        {
            RetrieveCartitem(out List<CartItem> sanPhamVMs, out decimal bill);
            ViewBag.Bill = bill;
            // Tạo một instance của PercentTips với mức phần trăm được chọn
            percentTips = PercentTips.GetInstance(tipPercentage);

            // Tính toán tips sử dụng instance của PercentTips
            decimal tipAmount = percentTips.CalculateTip(bill);

            // Trả về kết quả tính toán tips
            return RedirectToAction("Index", "Cart", new { tipAmount = tipAmount });
        }

    }
}
