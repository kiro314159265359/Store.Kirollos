using AutoMapper;
using Domain.Contracts;
using Domain.Exceptions;
using Domain.Models;
using Services.Abstractions;
using Services.Specifications;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class ProductService(IUnitOfWork unitOfWork, IMapper mapper) : IProductService
    {
        public async Task<PaginationResponse<ProductResultDto>> GetAllProductAsync(ProductSpecificationParamters specParams)
        {
            var spec = new ProductWithBrandsAndTypesSpicifications(specParams);

            // get all products through repository
            var products = await unitOfWork.GetRepository<Product, int>().GetAllAsync(spec);

            var specCount = new ProductWithCountSpecifications(specParams);

            var count = await unitOfWork.GetRepository<Product, int>().CountAsync(specCount);
            // Mapping from IEnumrable<product> to IEnumrable<productdto>
            var result = mapper.Map<IEnumerable<ProductResultDto>>(products);

            return new PaginationResponse<ProductResultDto>(specParams.PageIndex, specParams.PageSize, count, result);
        }
        public async Task<ProductResultDto?> GetProductByIdAsync(int id)
        {
            var spec = new ProductWithBrandsAndTypesSpicifications(id);

            var product = await unitOfWork.GetRepository<Product, int>().GetAsync(spec);

            var result = product is null ? throw new  ProductNotFoundExecption(id): mapper.Map<Product, ProductResultDto>(product);
            return result;
        }

        public async Task<IEnumerable<BrandResultDto>> GetAllBrandsAsync()
        {
            var brands = await unitOfWork.GetRepository<ProductBrand, int>().GetAllAsync();

            var result = mapper.Map<IEnumerable<BrandResultDto>>(brands);
            return result;
        }
        public async Task<IEnumerable<TypeResultDto>> GetAllTypesAsync()
        {
            var types = await unitOfWork.GetRepository<ProductType, int>().GetAllAsync();

            var result = mapper.Map<IEnumerable<TypeResultDto>>(types);
            return result;
        }
    }
}
