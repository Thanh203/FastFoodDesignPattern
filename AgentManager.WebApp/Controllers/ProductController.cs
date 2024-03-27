using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FastFoodSystem.WebApp.Models;
using FastFoodSystem.WebApp.Models.Data;
using FastFoodSystem.WebApp.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FastFoodSystem.WebApp.Controllers
{
    [Authorize(Roles = "Admin,Manager,Staff")]
    public class ProductController : Controller
    {
        private readonly IRepository<FFSProduct> _productRepository;
        private readonly IRepository<FFSProductCategory> _categoryRepository;

        public ProductController(IRepository<FFSProduct> productRepository, IRepository<FFSProductCategory> categoryRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }


        public async Task<ActionResult> Index()
        {
            var product = await _productRepository.GetAllAsync(); 

            return View(product);
        }


        public async Task<IActionResult> Details(string id)
        {
            var product = await _productRepository.GetByIdAsync(id);

            if (product == null) return NotFound();

            FFSProduct sanPhamVM = new FFSProduct()
            {
                FFSProductId = product.FFSProductId,
                Name = product.Name,
                Image = product.Image,
                Price = product.Price,
                FFSProductCategoryId = product.FFSProductCategoryId,
                Desc = product.Desc
            };

            if (sanPhamVM == null)
            {
                return NotFound();
            }
            else 
            {
                return View(sanPhamVM);
            };
        }


        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Create()
        {
            var categories = await _categoryRepository.GetAllAsync();

            ViewBag.Categories = new SelectList(categories, "FFSProductCategoryId", "Name");

            return View();
        }


        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Create(FFSProduct sanPhamVM)
        {
            if (await _productRepository.GetByIdAsync(sanPhamVM.FFSProductId) != null)
            {
                var categories = await _categoryRepository.GetAllAsync();

                ViewBag.Categories = new SelectList(categories, "FFSProductCategoryId", "Name");

                ViewBag.ErrorMessage = "ID đã tồn tại";

                return View();
            }

            if (ModelState.IsValid)
            {
                FFSProduct sanPham = new FFSProduct()
                {
                    FFSProductId = sanPhamVM.FFSProductId,
                    Name = sanPhamVM.Name,
                    Image = sanPhamVM.Image,
                    Price = sanPhamVM.Price,
                    Desc = sanPhamVM.Desc,
                    FFSProductCategoryId = sanPhamVM.FFSProductCategoryId
                };
                await _productRepository.AddAsync(sanPham);
                return RedirectToAction("index");
            }

            

            return View(sanPhamVM);
        }


        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sanPhamVM = await _productRepository.GetByIdAsync(id);

            if (sanPhamVM == null)
            {
                return NotFound();
            }
            else 
            {
                return View(sanPhamVM);
            };
        }


        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Delete(FFSProduct sanPhamVM)
        {
            await _productRepository.DeleteAsync(sanPhamVM.FFSProductId);
            return RedirectToAction("index");
        }

        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Edit(string id)
        {
            var product = await _productRepository.GetByIdAsync(id);

            if (product == null)
            {
                ViewBag.ErrorMessage = "Sản phẩm không tồn tại";
                return View();
            }

            FFSProduct sanPhamVM = new FFSProduct()
            {
                FFSProductId = product.FFSProductId,
                Name = product.Name,
                Image = product.Image,
                Price = product.Price,
                Desc = product.Desc,
                FFSProductCategoryId = product.FFSProductCategoryId,
            };

            if (sanPhamVM == null)
            {
                return NotFound();
            }
            else
            {
                return View(sanPhamVM);
            } ;
        }


        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Edit(FFSProduct sanPhamVM)
        {
            if (ModelState.IsValid)
            {
                FFSProduct sanPham = new FFSProduct()
                {
                    Name = sanPhamVM.Name,
                    Image = sanPhamVM.Image,
                    Price = sanPhamVM.Price,
                    Desc = sanPhamVM.Desc,
                    FFSProductId = sanPhamVM.FFSProductId,
                    FFSProductCategoryId = sanPhamVM.FFSProductCategoryId
                };

                await _productRepository.UpdateAsync(sanPham, sanPhamVM.FFSProductId);
                return RedirectToAction("index");
            }
            return View(sanPhamVM);
        }
    }
}
