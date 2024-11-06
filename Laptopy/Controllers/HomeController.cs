using Laptopy.Models;
using Laptopy.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Laptopy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly IProductRepository productRepository;
        private readonly IContactUsRepository contactUsRepository;

        public HomeController(IProductRepository productRepository, IContactUsRepository contactUsRepository)
        {
            this.productRepository = productRepository;
            this.contactUsRepository = contactUsRepository;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var products = productRepository.GetAll(includeProp: [e => e.Category]);
            return Ok(products.ToList());
        }


        [HttpGet("Details")]
        public IActionResult Details(int productId)
        {
            var product = productRepository.GetOne(includeProp: [e => e.Category ,p=>p.ProductImages],
                expression: p => p.Id == productId);
            if (product != null)
            {
                return Ok(product);
            }
            return NotFound();
        }



        [HttpGet("Products")]
        //[Route("Products")]
        public IActionResult Products(string? brand = null, decimal minPrice = 1000, decimal maxPrice = 1000000,
            string? rating = null, int pageNumber = 1)
        {
            var products = productRepository.GetAll();
            if (brand != null)
            {
                products = productRepository.GetAll(expression: e => e.Model == brand);
            }
            if (minPrice != 1000 || maxPrice != 1000000)
            {
                products = productRepository.GetAll
                    (expression: p => p.Price >= minPrice && p.Price <= maxPrice);
            }
            if (!string.IsNullOrEmpty(rating))
            {
                int ratingValue;
                if (int.TryParse(rating, out ratingValue))
                {
                    products = productRepository.GetAll(expression: p => p.Rating == ratingValue);
                }
            }
            int pageSize = 10;
            var paginated = products.Skip((pageNumber - 1) * pageSize).Take(pageSize);
            if (!paginated.Any())
            {
                return NotFound();
            }
            return Ok(paginated.ToList());
        }


        [HttpGet("Search")]
        public IActionResult Search(string name, string model)
        {
            var products = productRepository.GetAll(expression: p => p.Name == name || p.Model == model);
            if (products.Any())
            {
                return Ok(products);
            }
            return NotFound();
        }

        [HttpPost("ContactUs")]
        public IActionResult ContactUs(ContactUs contactUs)
        {
            if (ModelState.IsValid) 
            {
                contactUsRepository.Add(contactUs);
                contactUsRepository.Commit();
                return Ok();
            }
            return NotFound();
        }




    }
}
