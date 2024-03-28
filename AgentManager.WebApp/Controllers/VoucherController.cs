using FastFoodSystem.WebApp.Models;
using FastFoodSystem.WebApp.Models.Data;
using FastFoodSystem.WebApp.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;


namespace FastFoodSystem.WebApp.Controllers
{
    [Authorize (Roles = "Admin,Manager")]
    public class VoucherController : Controller
    {
        private readonly IRepository<FFSVoucher> _voucherRepository;

        public VoucherController(IRepository<FFSVoucher> voucherRepository)
        {
            _voucherRepository = voucherRepository;
        }

        public async Task<IActionResult> Index()
        {
            var voucher = await _voucherRepository.GetAllAsync();

            return View(voucher);
        }


        public async Task<IActionResult> Details(string? id)
        {
            var voucher = await _voucherRepository.GetByIdAsync(id);

            if (voucher == null) return NotFound();

            FFSVoucher voucher1 = new FFSVoucher()
            {
                FFSVoucherId = voucher.FFSVoucherId,
                Num = voucher.Num,
                Price = voucher.Price,
                StartDate = voucher.StartDate,
                EndDate = voucher.StartDate,
                State = voucher.State,
            };
            if (voucher1 == null) return NotFound();

            else return View(voucher1);
        }


        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var voucher = await _voucherRepository.GetByIdAsync(id);
            if (voucher == null)
            {
                return NotFound();
            }
            return View(voucher);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            await _voucherRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var voucher = await _voucherRepository.GetByIdAsync(id);
            if (voucher == null)
            {
                return NotFound();
            }
            return View(voucher);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, FFSVoucher voucher)
        {
            if (id != voucher.FFSVoucherId)
            {
                return NotFound();
            }
            else if (voucher.StartDate >= voucher.EndDate)
            {
                ViewBag.ErrorMessage = "Ngày bắt đầu phải bé hơn ngày kết thúc";
                return View();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    var updatedVoucher = new FFSVoucherBuilder()
                        .WithVoucherId(voucher.FFSVoucherId)
                        .WithNum(voucher.Num)
                        .WithPrice(voucher.Price)
                        .WithStartDate(voucher.StartDate)
                        .WithEndDate(voucher.EndDate)
                        .WithState(voucher.State)
                        .Build();

                    await _voucherRepository.UpdateAsync(updatedVoucher, id);
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    ModelState.AddModelError(String.Empty, "Có lỗi xảy ra khi cập nhập dữ liệu");
                }

            }

            return View(voucher);
        }


        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FFSVoucher voucher)
        {
            var chain = new ChainOfHanderIdVoucher();

            if (await _voucherRepository.GetByIdAsync(voucher.FFSVoucherId) != null)
            {
                ViewBag.ErrorMessage = "ID đã tồn tại";
                return View();
            }
            else if (chain.Handler(voucher.FFSVoucherId) != null)
            {
                ViewBag.ErrorMessage = chain.Handler(voucher.FFSVoucherId);
                return View();
            }
            else if (voucher.StartDate >= voucher.EndDate)
            {
                ViewBag.ErrorMessage = "Ngày bắt đầu phải bé hơn ngày kết thúc";
                return View();
            }

            if (ModelState.IsValid)
            {
                var newVoucher = new FFSVoucherBuilder()
                 .WithVoucherId(voucher.FFSVoucherId)
                 .WithNum(voucher.Num)
                 .WithPrice(voucher.Price)
                 .WithStartDate(voucher.StartDate)
                 .WithEndDate(voucher.EndDate)
                 .WithState(voucher.State)
                 .Build();

                await _voucherRepository.AddAsync(newVoucher);
                return RedirectToAction(nameof(Index));
            }
            return View(voucher);
        }
    }
}
