using FastFoodSystem.WebApp.Models.Data;
using FastFoodSystem.WebApp.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AgentManager.WebApp.Controllers
{
    //[Authorize(Roles = "Admin,Manager,Staff")]
    public class CategoriesController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoriesController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }


        public async Task<IActionResult> Index()
        {
            var productCategories = await _categoryRepository.GetAllAsync();
            return View(productCategories);
        }


        [Authorize(Roles = "Admin,Manager")]
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Create(FFSProductCategory category)
        {
            if (await _categoryRepository.GetByIdAsync(category.FFSProductCategoryId) != null)
            {
                ViewBag.ErrorMessage = "ID đã tồn tại";
                return View();
            }

            if (ModelState.IsValid)
            {
                await _categoryRepository.AddAsync(category);
                return RedirectToAction(nameof(Index));
            }
            else 
            {
                Console.WriteLine("Error");
            };

            return View(category);
        }


        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _categoryRepository.GetByIdAsync(id);

            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }
       

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Edit(string id, FFSProductCategory productCategory)
        {
            if (id != productCategory.FFSProductCategoryId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _categoryRepository.UpdateAsync(productCategory, id);
                }
                catch (DbUpdateConcurrencyException)
                {
                    Console.WriteLine("Not found");
                    return NotFound();
                }
                return RedirectToAction(nameof(Index));
            }
            return View(productCategory);
        }


        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productCategory = await _categoryRepository.GetByIdAsync(id);
            if (productCategory == null)
            {
                return NotFound();
            }

            return View(productCategory);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            await _categoryRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
