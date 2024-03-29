using FastFoodSystem.WebApp.Models.Data;
using FastFoodSystem.WebApp.Models.ViewModel;

namespace FastFoodSystem.WebApp.Models.ProxyPattern
{
    public interface IService
    {
        public List<Position> GetPositions();
        public Position GetPositionByID(int id);
        public Staff GetStaffByIdPosition(int id);
        public bool InsertPositions(Position position);
        public bool DeletePositions(int id);
        public bool EditPositions(Position position);
    }
}
