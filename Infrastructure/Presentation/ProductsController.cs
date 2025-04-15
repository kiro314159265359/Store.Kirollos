using Microsoft.AspNetCore.Mvc;
using Services.Abstractions;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController(IServiceManager serviceManager) : ControllerBase
    {
        // endpoint: public non-static method

        // sort : name asc or desc [default based on name]
        // sort : price asc or desc
        [HttpGet]
        public async Task<IActionResult> GetAllProducts([FromQuery]ProductSpecificationParamters SpecParams)
        {
            var result = await serviceManager.ProductService.GetAllProductAsync(SpecParams);
            if (result is null)
                return NotFound();
            return Ok(result);
        }

        [HttpGet("{Id}")]
        public async Task<IActionResult> GetProductById(int Id)
        {
            var result = await serviceManager.ProductService.GetProductByIdAsync(Id);
            if (result is null)
                return NotFound();
            return Ok(result);
        }

        [HttpGet("brands")]
        public async Task<IActionResult> GetAllBrands()
        {
            var brands = await serviceManager.ProductService.GetAllBrandsAsync();
            if (brands is null)
                return NotFound();
            return Ok(brands);
        }

        [HttpGet("types")]
        public async Task<IActionResult> GetAllTypes()
        {
            var types = await serviceManager.ProductService.GetAllTypesAsync();
            if (types is null)
                return NotFound();
            return Ok(types);
        }
    }
}
