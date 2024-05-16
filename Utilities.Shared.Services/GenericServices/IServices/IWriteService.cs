using EmployeeManagementSystem.Domain.Shared;

namespace Utilities.Shared.Services.GenericServices.IServices
{
    public interface IWriteService<TEntity> where TEntity : class, new()
    {
        Task<ServiceResponse> Create<TAddDto>(TAddDto dto) where TAddDto : class, new();
        Task<ServiceResponse> Update<TUpdateDto>(TUpdateDto dto) where TUpdateDto : class, new();
        Task<ServiceResponse> Delete(object id);
    }
}
