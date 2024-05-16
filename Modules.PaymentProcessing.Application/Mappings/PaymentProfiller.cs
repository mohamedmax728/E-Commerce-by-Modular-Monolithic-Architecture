using AutoMapper;
using Modules.PaymentProcessing.Application.Dtos;
using Shared.Utilities.Models;
using Shared.Utilities.Models.Entities;

namespace Modules.PaymentProcessing.Application.Mappings
{
    public class PaymentProfiller : Profile
    {
        public PaymentProfiller()
        {
            CreateMap<Payment, PaymentListDto>();
            CreateMap<Payment, PaymentDetailsDto>();
            CreateMap<PaymentCreateDto, Payment>();
            CreateMap<AuditModel, AuditModelVM>();
        }
    }
}
