using FastFoodSystem.WebApp.Models.Data;
using FastFoodSystem.WebApp.Models.ViewModel;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace FastFoodSystem.WebApp.Models.ProxyPattern
{
    public class Proxy : IService
    {
        IHttpContextAccessor contextAccessor;
        IService dbHelper;
        UserManager<Staff> UserManager;
        private string[] roles = {"Admin1", "Manager1" };
        public Proxy(FastFoodSystemDbContext context, IHttpContextAccessor contextAccessor) 
        {
            this.contextAccessor = contextAccessor;
            UserManager = contextAccessor.HttpContext.RequestServices.GetService<UserManager<Staff>>();
            dbHelper = new DBHelper(context);
            //getRoleUser();
        }
        public async void getRoleUser()
        {
            var userId = contextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var s = await UserManager.FindByIdAsync(userId);
            roles = (await UserManager.GetRolesAsync(s)).ToArray();
        }
        public bool DeletePositions(int id)
        {
            if (roles.Contains("Manager") || roles.Contains("Admin"))
            {
                dbHelper.DeletePositions(id);
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool EditPositions(Position position)
        {
            if (roles.Contains("Manager") || roles.Contains("Admin"))
            {
                dbHelper.EditPositions(position);
                return true;
            }
            else
            {
                return false;
            }
        }

        public Position GetPositionByID(int id)
        {
            if (roles.Count() > 0)
            {
                return dbHelper.GetPositionByID(id);
            }
            return null;
        }

        public List<Position> GetPositions()
        {
            if (roles.Count() > 0)
            {
                return dbHelper.GetPositions();
            }
            return null;
        }

        public bool InsertPositions(Position position)
        {
            if (roles.Contains("Manager") || roles.Contains("Admin"))
            {
                dbHelper.InsertPositions(position);
                return true;
            }
            else
            {
                return false;
            }
        }

        public Staff GetStaffByIdPosition(int id)
        {
            if (roles.Count() > 0)
            {
                return dbHelper.GetStaffByIdPosition(id);
            }
            return null;
        }
    }
}
