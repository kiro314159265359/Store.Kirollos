using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Abstractions
{
    public interface IProductService
    {
        // when we get to the service area we use dtos
        // Get All Product
        //Task<IEnumerable<ProductResultDto>> GetAllProductAsync(int? brandId, int? typeId, string? sort, int pageIndex = 1, int pageSize = 5);
        Task<PaginationResponse<ProductResultDto>> GetAllProductAsync(ProductSpecificationParamters specParams);
        // Get Product by Id
        Task<ProductResultDto?> GetProductByIdAsync(int id);

        // Get all brands
        Task<IEnumerable<BrandResultDto>> GetAllBrandsAsync();


        // Get all types
        Task<IEnumerable<TypeResultDto>> GetAllTypesAsync();
    }
}
