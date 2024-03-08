﻿using FastFoodSystem.WebApp.Models;
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
        private readonly FastFoodSystemDbContext _context;
        DBHelper dbHelper;
        private readonly IVoucherRepository _voucherRepository;


        public VoucherController(FastFoodSystemDbContext context, FastFoodSystemDbContext db, IVoucherRepository voucherRepository)
        {
            _context = context;
            dbHelper = new DBHelper(db);
            _voucherRepository = voucherRepository;
        }
        public async Task<IActionResult> Index(string searchText = "")
        {
            ViewBag.SearchText = searchText;
            var fFSVouchers = _context.FFSVouchers;

            if (!String.IsNullOrEmpty(searchText))
            {
                var fFSVouchersListSearch = _context.FFSVouchers
                    .Where(a => a.FFSVoucherId.Contains(searchText));

                return View(await fFSVouchersListSearch.ToListAsync());
            }
            return View(await fFSVouchers.ToListAsync());
        }
        public async Task<IActionResult> Details(string? id)
        {
            var voucher = dbHelper.GetVoucherByID(id);

            if (voucher == null) return NotFound();

            FFSVoucher voucher1 = new FFSVoucher()
            {
                FFSVoucherId = dbHelper.GetVoucherByID(id).FFSVoucherId,
                Num = dbHelper.GetVoucherByID(id).Num,
                Price = dbHelper.GetVoucherByID(id).Price,
                StartDate = dbHelper.GetVoucherByID(id).StartDate,
                EndDate = dbHelper.GetVoucherByID(id).StartDate,
                State = dbHelper.GetVoucherByID(id).State,
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


        // POST: Voucher/Delete/5
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
