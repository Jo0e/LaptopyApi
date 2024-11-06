using Laptopy.Data;
using Laptopy.Models;
using Laptopy.Repository.IRepository;

namespace Laptopy.Repository
{
    public class ContactUsRepository : Repository<ContactUs>, IContactUsRepository
    {
        public ContactUsRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
