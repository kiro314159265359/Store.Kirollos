using AutoMapper;
using Domain.Models;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.MappingProfiles
{
    internal class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<Product, ProductResultDto>()
               .ForMember(d => d.BrandName, o => o.MapFrom(S => S.ProductBrand.Name))
               .ForMember(d => d.TypeName, o => o.MapFrom(S => S.ProductType.Name))
                //.ForMember(d => d.PictureUrl, o => o.MapFrom(S => $"https://localhost:7187/{S.PictureUrl}"));
               .ForMember(d => d.PictureUrl, o => o.MapFrom<PictureUrlResolver>());

            CreateMap<ProductBrand, BrandResultDto>();
            CreateMap<ProductType, TypeResultDto>();
        }
    }
}
