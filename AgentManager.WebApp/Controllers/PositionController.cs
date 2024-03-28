using FastFoodSystem.WebApp.Models.Data;
using FastFoodSystem.WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using FastFoodSystem.WebApp.Models.ViewModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using FastFoodSystem.WebApp.Models.ProxyPattern;
using Microsoft.AspNetCore.Identity;

namespace FastFoodSystem.WebApp.Controllers
{
    public class PositionController : Controller
    {
        IService service;
        public PositionController(FastFoodSystemDbContext db, IHttpContextAccessor contextAccessor)
        {
            service = new Proxy(db, contextAccessor);
        }

        public IActionResult Index()
        {
            ViewData["listPositions"] = service.GetPositions();
            return View();
        }
        public IActionResult Create()
        {
            return View();
        }
        
        [HttpPost]
        public IActionResult Create(PositionVM positionVM)
        {
            if (ModelState.IsValid)
            {
                Position position = new Position()
                {
                    PositionName = positionVM.tenChucVu,
                    
                };
                service.InsertPositions(position);
                return RedirectToAction("index");
            }
            return View(positionVM);
        }
        
        public IActionResult Delete(int id)
        {
            PositionVM positionVM = new PositionVM()
            {
                maChucVu = id,
                tenChucVu = service.GetPositionByID(id).PositionName,
            };
            if (positionVM == null)
                return NotFound();
            else return View(positionVM);
        }
        [HttpPost]
        public IActionResult Delete(PositionVM positionVM)
        {
            
            if (ModelState.IsValid)
            {
                if (service.GetStaffByIdPosition(positionVM.maChucVu) == null)
                {
                    service.DeletePositions(positionVM.maChucVu);
                    return RedirectToAction("index");
                }
                else Console.WriteLine("ERROR");

            }
            else Console.WriteLine("ERROR");
            return View(positionVM);
        }
        
        public IActionResult Edit(int id)
        {
            Position position = service.GetPositionByID(id);
            PositionVM positionVM = new PositionVM()
            {
                maChucVu = position.PositionId,
                tenChucVu = position.PositionName,
            };
            Console.WriteLine("Post Edit Positon Clone:", positionVM);
            if (positionVM == null) return NotFound();
            else return View(positionVM);
        }
        [HttpPost]
        public IActionResult Edit(PositionVM positionVM)
        {
            if (ModelState.IsValid)
            {
                // Truy xuất đối tượng Position cần chỉnh sửa từ cơ sở dữ liệu
                Position position = service.GetPositionByID(positionVM.maChucVu);

                if (position != null)
                {
                    // Cập nhật thông tin mới từ PositionVM
                    position.PositionName = positionVM.tenChucVu;

                    // Cập nhật thông tin vào cơ sở dữ liệu
                    service.EditPositions(position);

                    return RedirectToAction("index");
                }
                else
                {
                    return NotFound();
                }
            }
            return View(positionVM);
        }
    }
}
