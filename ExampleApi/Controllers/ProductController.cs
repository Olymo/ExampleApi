using Doublel.DynamicQueryBuilder;
using Doublel.DynamicQueryBuilder.Attributes;
using Doublel.DynamicQueryBuilder.Search;
using Doublel.QueryableBuilder;
using ExampleApi.Entities;
using ExampleApi.Validations;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ExampleApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        // GET: api/<ProductController>
        [HttpGet]
        public IActionResult Get([FromQuery] ProductSearch search)
        {
            return Ok(InMemmoryDatabase.Products.AsQueryable().BuildDynamicQuery(search));
        }

        // GET api/<ProductController>/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var product = InMemmoryDatabase.Products.FirstOrDefault(x => x.Id == id);

            if(product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }

        // POST api/<ProductController>
        [HttpPost]
        public IActionResult Post([FromBody] Product data, [FromServices] AddProductValidation validator)
        {
            var validate = validator.Validate(data);

            if (!validate.IsValid)
            {
                return UnprocessableEntity(validate.Errors.ToList().Select(x => new
                {
                    PropertyName = x.PropertyName,
                    Error = x.ErrorMessage
                }));
            }

            var product = new Product
            {
                Id = InMemmoryDatabase.Products.AsQueryable().OrderByDescending(x => x.Id).FirstOrDefault().Id + 1,
                Name = data.Name,
                Price = data.Price,
                Category = InMemmoryDatabase.Categories.AsQueryable().FirstOrDefault(x => x.Id == data.CategoryId)
            };

            InMemmoryDatabase.Products.Add(product);
            return NoContent();
        }

        // PUT api/<ProductController>/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Product data, [FromServices] UpdateProductValidation validator)
        {
            var product = InMemmoryDatabase.Products.FirstOrDefault(x => x.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            var validate = validator.Validate(data);

            if (!validate.IsValid)
            {
                return UnprocessableEntity(validate.Errors.ToList().Select(x => new
                {
                    PropertyName = x.PropertyName,
                    Error = x.ErrorMessage
                }));
            }

            product.Name = data.Name;
            product.Price = data.Price;
            product.Category = InMemmoryDatabase.Categories.AsQueryable().FirstOrDefault(x => x.Id == data.CategoryId);

            return NoContent();
        }

        // DELETE api/<ProductController>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var product = InMemmoryDatabase.Products.FirstOrDefault(x => x.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            InMemmoryDatabase.Products.Remove(product);

            return NoContent();
        }
    }

    public class ProductSearch : SortablePagedSearch
    {
        [QueryProperty(ComparisonOperator.Contains)]
        public string Name { get; set; }
        [QueryProperty(ComparisonOperator.MoreThanOrEqualsTo, "Price")]
        public decimal? MinPrice { get; set; }
        [QueryProperty(ComparisonOperator.LessThanOrEqualsTo, "Price")]
        public decimal? MaxPrice { get; set; }
        [QueryProperty(ComparisonOperator.Equals, "Category.Id")]
        public int? CategoryId { get; set; }
    }
}
