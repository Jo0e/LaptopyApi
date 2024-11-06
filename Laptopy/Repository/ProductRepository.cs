using Laptopy.Data;
using Laptopy.Models;
using Laptopy.Repository.IRepository;

namespace Laptopy.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
