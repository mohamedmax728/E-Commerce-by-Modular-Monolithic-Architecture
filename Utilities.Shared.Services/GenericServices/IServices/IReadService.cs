using EmployeeManagementSystem.Domain.Shared;
using PagedList.Core;

namespace Utilities.Shared.Services.GenericServices.IServices
{
    public interface IReadService<TEntity> where TEntity : class, new()
    {
        Task<ServiceResponse<TDetailsDto>> Get<TDetailsDto>(object Id) where TDetailsDto : class, new();
        Task<ServiceResponse<IPagedList<TListDto>>> GetMany<TListDto>() where TListDto : class, new();
    }
}
