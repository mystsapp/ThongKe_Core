using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ThongKe.Data.Models_QLTour;

namespace ThongKe.Data.Repository.QLTour
{
    public interface ITourKindRepository
    {
        IEnumerable<Tourkind> GetAll();

        Task<Tourkind> GetByIdAsync(int id);
        Tourkind GetById(int id);

        IEnumerable<Tourkind> Find(Func<Tourkind, bool> predicate);
    }
    public class TourKindRepository : ITourKindRepository
    {
        private readonly qltourContext _qltourContext;

        public TourKindRepository(qltourContext qltourContext)
        {
            _qltourContext = qltourContext;
        }
        public IEnumerable<Tourkind> Find(Func<Tourkind, bool> predicate)
        {
            return _qltourContext.Tourkind.Where(predicate);
        }

        public IEnumerable<Tourkind> GetAll()
        {
            return _qltourContext.Tourkind;
        }

        public Tourkind GetById(int id)
        {
            return _qltourContext.Tourkind.Find(id);
        }

        public async Task<Tourkind> GetByIdAsync(int id)
        {
            return await _qltourContext.Tourkind.FindAsync(id);
        }
    }
}
