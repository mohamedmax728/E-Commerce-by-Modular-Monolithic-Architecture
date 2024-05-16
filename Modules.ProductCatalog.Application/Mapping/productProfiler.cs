using AutoMapper;
using Modules.ProductCatalog.Application.Dtos;
using Shared.Utilities.Models;
using Shared.Utilities.Models.Entities;

namespace Modules.ProductCatalog.Application.Mapping
{
    public class productProfiler : Profile
    {
        public productProfiler()
        {
            CreateMap<ProductCreateDto, Product>();
            CreateMap<ProductUpdateDto, Product>();
            CreateMap<Product, ProductDetailsDto>();
            CreateMap<Product, ProductListDto>();
            CreateMap<AuditModel, AuditModelVM>();
        }
    }
}
