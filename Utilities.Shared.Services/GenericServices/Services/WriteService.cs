using AutoMapper;
using EasyRepository.EFCore.Generic;
using EmployeeManagementSystem.Domain.Shared;
using Shared.Utilities.Models;
using System.Net;
using System.Reflection;
using Utilities.Shared.Services.GenericServices.IServices;

namespace Utilities.Shared.Services.GenericServices.Services
{
    public abstract class WriteService<TEntity> : IWriteService<TEntity> where TEntity : class, new()
    {
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly IMapper _mapper;
        private readonly IRepository<TEntity> _repository;
        public WriteService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _repository = _unitOfWork.GetRepo<TEntity>();
        }
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
