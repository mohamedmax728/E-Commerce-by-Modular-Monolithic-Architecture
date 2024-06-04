using AutoMapper;
using EasyRepository.EFCore.Generic;
using EmployeeManagementSystem.Domain.Shared;
using Microsoft.EntityFrameworkCore.Query;
using PagedList.Core;
using Shared.Utilities.Models;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using Utilities.Shared.Services.GenericServices.IServices;

namespace Utilities.Shared.Services.GenericServices.Services
{
    public abstract class Service<TEntity> : IService<TEntity> where TEntity : class, new()
    {
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly IMapper _mapper;
        private readonly IRepository<TEntity> _repository;
        public Service(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _repository = _unitOfWork.GetRepo<TEntity>();
        }
        #region Read
        public async virtual Task<ServiceResponse<TDetailsDto>> Get<TDetailsDto>(object Id) where TDetailsDto : class, new()
        {
            ThrowExceptionWhenRepositoryIsNotFound();

            var propertyId = GetProperty<TEntity>("Id");
            var propertyTypeId = propertyId.PropertyType;
            var convertedId = Convert.ChangeType(Id, propertyTypeId);
            TEntity record = null;
            if (propertyTypeId == typeof(string))
            {
                record = await _repository.GetAsync($"Id == \"{convertedId}\"");
            }
            else if (propertyTypeId == typeof(int))
            {
                record = await _repository.GetAsync($"Id == {convertedId}");
            }
            if (record is null)
            {
                return new ServiceResponse<TDetailsDto>()
                {
                    Success = false,
                    StatusCode = (int)HttpStatusCode.NotFound
                };
            }
            var item = typeof(TEntity) != typeof(TDetailsDto) ? _mapper.Map<TDetailsDto>(record) : record as TDetailsDto;
            return new ServiceResponse<TDetailsDto>()
            {
                Data = item,
                Success = true,
                StatusCode = (int)HttpStatusCode.OK,
            };
        }
        protected async Task<ServiceResponse<TDetailsDto>> Get<TDetailsDto>(
            Expression<Func<TEntity, bool>> predicate,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null) where TDetailsDto : class, new()
        {
            ThrowExceptionWhenRepositoryIsNotFound();
            TEntity record = await _repository.GetAsync(predicate, include);
            if (record is null)
            {
                return new ServiceResponse<TDetailsDto>()
                {
                    Success = false,
                    StatusCode = (int)HttpStatusCode.NotFound
                };
            }
            var item = typeof(TEntity) != typeof(TDetailsDto) ? _mapper.Map<TDetailsDto>(record) : record as TDetailsDto;
            return new ServiceResponse<TDetailsDto>()
            {
                Data = item,
                Success = true,
                StatusCode = (int)HttpStatusCode.OK,
            };
        }
        protected async Task<ServiceResponse<TDetailsDto>> Get<TDetailsDto>(object Id,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include) where TDetailsDto : class, new()
        {
            ThrowExceptionWhenRepositoryIsNotFound();

            var propertyId = GetProperty<TEntity>("Id");
            var propertyTypeId = propertyId.PropertyType;
            var convertedId = Convert.ChangeType(Id, propertyTypeId);
            TEntity record = null;
            if (propertyTypeId == typeof(string))
            {
                record = await _repository.GetAsync($"Id == \"{convertedId}\"", include);
            }
            else if (propertyTypeId == typeof(int))
            {
                record = await _repository.GetAsync($"Id == {convertedId}", include);
            }
            if (record is null)
            {
                return new ServiceResponse<TDetailsDto>()
                {
                    Success = false,
                    StatusCode = (int)HttpStatusCode.NotFound
                };
            }
            var item = typeof(TEntity) != typeof(TDetailsDto) ? _mapper.Map<TDetailsDto>(record) : record as TDetailsDto;
            return new ServiceResponse<TDetailsDto>()
            {
                Data = item,
                Success = true,
                StatusCode = (int)HttpStatusCode.OK,
            };
        }
        public async virtual Task<ServiceResponse<IPagedList<TListDto>>> GetMany<TListDto>() where TListDto : class, new()
        {
            ThrowExceptionWhenRepositoryIsNotFound();
            var list = await _repository.GetPagedListAsync<TListDto>
                (_mapper);
            return new ServiceResponse<IPagedList<TListDto>>
            {
                Data = list,
                Success = true,
                StatusCode = (int)HttpStatusCode.OK,
            };
        }
        protected async Task<ServiceResponse<IPagedList<TListDto>>> GetMany<TListDto>
            (Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include) where TListDto : class, new()
        {
            ThrowExceptionWhenRepositoryIsNotFound();
            var list = await _repository.GetPagedListAsync<TListDto>
                (_mapper, include: include);
            return new ServiceResponse<IPagedList<TListDto>>
            {
                Data = list,
                Success = true,
                StatusCode = (int)HttpStatusCode.OK,
            };
        }
        protected async Task<ServiceResponse<IPagedList<TListDto>>> GetMany<TListDto>
            (Expression<Func<TEntity, bool>> predicate,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null) where TListDto : class, new()
        {
            ThrowExceptionWhenRepositoryIsNotFound();
            var list = await _repository.GetPagedListAsync<TListDto>
                (_mapper, predicate, include: include);
            return new ServiceResponse<IPagedList<TListDto>>
            {
                Data = list,
                Success = true,
                StatusCode = (int)HttpStatusCode.OK,
            };
        }
        #endregion

        #region Write
        public virtual async Task<ServiceResponse> Create<TAddDto>(TAddDto dto) where TAddDto : class, new()
        {
            ThrowExceptionWhenRepositoryIsNotFound();

            var entity = _mapper.Map<TEntity>(dto);
            await _repository.AddAsync(entity);
            int result = await _unitOfWork.SaveChangesAsync();
            return new ServiceResponse()
            {
                Success = result > 0,
                StatusCode = (int)HttpStatusCode.OK,
                Message = "Added Susccefully!!"
            };
        }
        public virtual async Task<ServiceResponse> Update<TUpdateDto>(TUpdateDto dto) where TUpdateDto : class, new()
        {
            ThrowExceptionWhenRepositoryIsNotFound();
            var propertyIdInDTo = GetProperty<TUpdateDto>("Id");
            var propertyIdInEntity = GetProperty<TEntity>("Id");
            if (propertyIdInDTo.PropertyType != propertyIdInEntity.PropertyType)
            {
                throw new ArgumentException
                    ($"{nameof(dto)} Must have Id that type is {propertyIdInEntity.PropertyType.Name} " +
                    $"in {this.GetType().Name}.{nameof(MethodBase.GetCurrentMethod)}()");
            }
            var idValue = propertyIdInDTo!.GetValue(dto)!;
            TEntity item = null;
            if (propertyIdInEntity.PropertyType == typeof(string))
            {
                item = _repository.Get($"Id == \"{idValue}\"");
            }
            else
            {
                item = _repository.Get($"Id == {idValue}");
            }
            if (item == null)
            {
                return new ServiceResponse<TUpdateDto>()
                {
                    Success = false,
                    StatusCode = (int)HttpStatusCode.NotFound
                };
            }
            item = _mapper.Map(dto, item);
            _repository.Update(item);
            int result = await _unitOfWork.SaveChangesAsync();
            return new ServiceResponse<TUpdateDto>()
            {
                Success = result > 0,
                StatusCode = (int)HttpStatusCode.OK,
                Message = "Updated Susccefully!!"
            };
        }
        public virtual async Task<ServiceResponse> Delete(object id)
        {
            ThrowExceptionWhenRepositoryIsNotFound();

            var propertyId = GetProperty<TEntity>("Id");
            var propertyTypeId = propertyId.PropertyType;
            var convertedId = Convert.ChangeType(id, propertyTypeId);
            try
            {
                TEntity record = null;
                if (propertyTypeId == typeof(string))
                {
                    record = _repository.Get($"Id == \"{convertedId}\"");
                }
                else if (propertyTypeId == typeof(int))
                {
                    record = _repository.Get($"Id == {convertedId}");
                }
                if (record is null)
                {
                    return new ServiceResponse()
                    {
                        Success = false,
                        StatusCode = (int)HttpStatusCode.NotFound,
                        Message = "item Is Not Found!!"
                    };
                }
                _repository.Delete(record);
                int result = await _unitOfWork.SaveChangesAsync();
                if (result == 0)
                {
                    return new ServiceResponse()
                    {
                        Success = result > 0,
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        Message = "Fail To Delete!!"
                    };
                }
                return new ServiceResponse()
                {
                    Success = true,
                    StatusCode = (int)HttpStatusCode.OK,
                    Message = "Deleted Susccefully!!"
                };
            }
            catch (Exception e)
            {
                return new ServiceResponse()
                {
                    Success = false,
                    StatusCode = (int)HttpStatusCode.BadRequest
                };
            }
        }
        #endregion

        #region Throw Exceptions
        private PropertyInfo GetProperty<TClass>(string propertyName) where TClass : class
        {
            var property = typeof(TClass).GetProperty(propertyName);
            if (property == null)
            {
                System.Diagnostics.StackTrace stackTrace = new System.Diagnostics.StackTrace();
                var callingMethodName = stackTrace.GetFrame(1)?.GetMethod()!.Name;
                throw new ArgumentNullException
                    ($"{propertyName} Is not found In {typeof(TClass).Name} Model In {this.GetType().Name}.{callingMethodName}()");
            }
            return property;
        }
        private void ThrowExceptionWhenRepositoryIsNotFound()
        {
            if (_repository == null)
            {
                System.Diagnostics.StackTrace stackTrace = new System.Diagnostics.StackTrace();
                var callingMethodName = stackTrace.GetFrame(1)?.GetMethod()!.Name;
                throw new ArgumentNullException
                    ($"{nameof(_repository)} Is Null In {this.GetType().Name}.{callingMethodName}()");
            }
        }
        #endregion
    }
}
