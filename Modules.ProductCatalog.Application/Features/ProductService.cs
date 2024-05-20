using AutoMapper;
using EasyRepository.EFCore.Generic;
using EmployeeManagementSystem.Domain.Shared;
using Microsoft.Extensions.Caching.Memory;
using Modules.ProductCatalog.Application.Dtos;
using Modules.ProductCatalog.Application.Interfaces;
using PagedList;
using Shared.Utilities.Models.Entities;
using System.Net;
using Utilities.Shared.Services.GenericServices.Services;

namespace Modules.ProductCatalog.Application.Features
{
    public class ProductService : Service<Product>, IProductService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly string ProductsKey = "Product";
        public ProductService(IUnitOfWork unitOfWork, IMapper mapper, IMemoryCache memoryCache) : base(unitOfWork, mapper)
        {
            _memoryCache = memoryCache;
        }

        public override async Task<ServiceResponse<IPagedList<TListDto>>> GetMany<TListDto>()
        {
            if (!_memoryCache.TryGetValue(ProductsKey, out IEnumerable<ProductListDto> products))
            {
                var x = await base.GetMany<TListDto>();
                products = (IEnumerable<ProductListDto>?)x.Data;
                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromSeconds(45))
                    .SetAbsoluteExpiration(TimeSpan.FromSeconds(3600))
                    .SetPriority(CacheItemPriority.High);
                _memoryCache.Set(ProductsKey, products, cacheOptions);
            }
            return new ServiceResponse<IPagedList<TListDto>>
            {
                Data = (IPagedList<TListDto>)products,
                Success = true,
                StatusCode = (int)HttpStatusCode.OK,

            };
        }
    }
}
