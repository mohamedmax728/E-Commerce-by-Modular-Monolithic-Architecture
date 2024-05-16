using AutoMapper;
using EasyRepository.EFCore.Generic;
using Modules.ProductCatalog.Application.Interfaces;
using Shared.Utilities.Models.Entities;
using Utilities.Shared.Services.GenericServices.Services;

namespace Modules.ProductCatalog.Application.Features
{
    public class ProductService : Service<Product>, IProductService
    {
        public ProductService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }
    }
}
