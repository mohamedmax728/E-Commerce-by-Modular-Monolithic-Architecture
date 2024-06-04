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
    public abstract class ReadService<TEntity> : IReadService<TEntity> where TEntity : class, new()
    {
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly IMapper _mapper;
        private readonly IRepository<TEntity> _repository;
        public ReadService(IUnitOfWork unitOfWork, IMapper mapper)
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
