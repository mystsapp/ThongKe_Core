using ThongKe.Data.Repository;

namespace ThongKe.Services
{
    public interface IThongKeService
    {

    }
    public class ThongKeService : IThongKeService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ThongKeService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
    }
}
