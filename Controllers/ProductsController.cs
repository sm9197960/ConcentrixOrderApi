using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MyStoreApi.Data;
using MyStoreApi.Models;
using System.IO;
using System.Xml.Linq;

namespace MyStoreApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private IWebHostEnvironment _hostEnvironment;
        private readonly List<string> listCategories = new List<string>()
        {
            "Phones", "Computers", "Accessories","Printers","Cameras","Others"
        };

        public ProductsController(ApplicationDbContext dbContext, IWebHostEnvironment environment)
        {
            this._dbContext = dbContext;
            _hostEnvironment = environment;
        }

        [HttpGet("categories")]
        public IActionResult GetCategories()
        {
            return Ok(listCategories);
        }

        [HttpGet]
        public IActionResult GetProducts(string? search, string? category, int? minPrice, int? maxPrice
            ,string? sort, string? order, int? page)
        {
            IQueryable<Product> query = _dbContext.Products;

            //Search Functionality
            if(search != null)
            {
                query = query.Where(p => p.Name.Contains(search) || p.Description.Contains(search));
            }

            if (category != null)
            {
                query = query.Where(p => p.Category == category);
            }

            if(minPrice != null)
            {
                query = query.Where(p => p.Price >= minPrice);
            }

            if (maxPrice != null)
            {
                query = query.Where(p => p.Price <= maxPrice);
            }

            //Sorting Functionality
            if (sort == null) sort = "id";
            if (order == null || order != "asc") order = "desc";

            if(sort.ToLower() == "name")
            {
                if(order == "asc")
                {
                    query = query.OrderBy(p => p.Name);
                }
                else
                {
                    query = query.OrderByDescending(p => p.Name);
                }
            }
            else if (sort.ToLower() == "brand")
            {
                if (order == "asc")
                {
                    query = query.OrderBy(p => p.Brand);
                }
                else
                {
                    query = query.OrderByDescending(p => p.Brand);
                }
            }
            else if (sort.ToLower() == "category")
            {
                if (order == "asc")
                {
                    query = query.OrderBy(p => p.Category);
                }
                else
                {
                    query = query.OrderByDescending(p => p.Category);
                }
            }
            else if (sort.ToLower() == "price")
            {
                if (order == "asc")
                {
                    query = query.OrderBy(p => p.Price);
                }
                else
                {
                    query = query.OrderByDescending(p => p.Price);
                }
            }
            else if (sort.ToLower() == "date")
            {
                if (order == "asc")
                {
                    query = query.OrderBy(p => p.CreatedAt);
                }
                else
                {
                    query = query.OrderByDescending(p => p.CreatedAt);
                }
            }
            else
            {
                if (order == "asc")
                {
                    query = query.OrderBy(p => p.Id);
                }
                else
                {
                    query = query.OrderByDescending(p => p.Id);
                }
            }

            //Pagination Functionality
            if (page == null || page < 1) page = 1;
            int pageSize = 5;
            int totalPages = 0;

            decimal count = query.Count();
            totalPages = (int)Math.Ceiling(count/pageSize);

            query = query.Skip((int)(page -1)* pageSize).Take(pageSize);

            var products = query.ToList();

            var response = new
            {
                Product = products,
                TotalPages = totalPages,
                PageSize = pageSize,
                Page = page
            };

            return Ok(response);
        }

        [HttpGet("{id}")]
        public IActionResult GetProduct(int id)
        {
            var product = _dbContext.Products.Find(id);
            if(product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        [HttpPost]
        public IActionResult CreateProducts([FromForm]ProductDto productDto)
        {
            if (!listCategories.Contains(productDto.Category))
            {
                ModelState.AddModelError("Category", "Please select a Valid Category");
                return BadRequest(ModelState);
            }

            if (productDto == null)
            {
                ModelState.AddModelError("ImageFile", "The Image file is required");
                return BadRequest(ModelState);
            }

            //Save the file on the server
            string imageFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            imageFileName += Path.GetExtension(productDto.ImageFile.FileName);

            string imageFolder = _hostEnvironment.WebRootPath + "/images/products/";

            using(var stream = System.IO.File.Create(imageFolder + imageFileName))
            {
                productDto .ImageFile.CopyTo(stream);
            }

            Product product = new Product()
            {
                Name = productDto.Name,
                Brand = productDto.Brand,
                Category = productDto.Category,
                Price = productDto.Price,
                Description = productDto.Description ?? "",
                ImageFileName = imageFileName,
                CreatedAt = DateTime.Now
            };

            _dbContext.Products.Add(product);
            _dbContext.SaveChanges();
            
            return Ok(product);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateProduct(int id,[FromForm] ProductDto productDto)
        {
            if (!listCategories.Contains(productDto.Category))
            {
                ModelState.AddModelError("Category", "Please select a Valid Category");
                return BadRequest(ModelState);
            }

            var product = _dbContext.Products.Find(id);
            if(product == null)
            {
                return NotFound();
            }

            string ImageFileName = product.ImageFileName;
            if(productDto.ImageFile != null)
            {
                //Save the image on the server
                string imageFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                imageFileName += Path.GetExtension(productDto.ImageFile.FileName);

                string imageFolder = _hostEnvironment.WebRootPath + "/images/products/";
                using (var stream = System.IO.File.Create(imageFolder + imageFileName))
                {
                    productDto.ImageFile.CopyTo(stream);
                }

                //Delete the old image
                System.IO.File.Delete(imageFolder + product.ImageFileName);
            }

            product.Name = productDto.Name;
            product.Brand = productDto.Brand;
            product.Category = productDto.Category;
            product.Price = productDto.Price;
            product.Description = productDto.Description ?? "";
            product.ImageFileName = ImageFileName;
            _dbContext.SaveChanges();

            return Ok(product);    
        }

        [HttpDelete]
        public IActionResult DeleteProduct(int id)
        {
            var product = _dbContext.Products.Find(id);

            if(product == null) { return NotFound(); }

            string imageFolder = _hostEnvironment.WebRootPath + "/images/products/";
            System.IO.File.Delete(imageFolder + product.ImageFileName);

            _dbContext.Products.Remove(product);
            _dbContext.SaveChanges();
            return Ok(product);
        }
    
    }
}
