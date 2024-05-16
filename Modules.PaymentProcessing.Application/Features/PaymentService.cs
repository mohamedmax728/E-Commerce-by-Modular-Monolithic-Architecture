using AutoMapper;
using EasyRepository.EFCore.Generic;
using EmployeeManagementSystem.Domain.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Modules.PaymentProcessing.Application.Dtos;
using Modules.PaymentProcessing.Application.Interfaces;
using Modules.PaymentProcessing.Domain.Helpers;
using Shared.Utilities.Models.Entities;
using Stripe;
using Stripe.Checkout;
using System.Security.Claims;

namespace Modules.PaymentProcessing.Application.Features
{
    public class PaymentService : Utilities.Shared.Services.GenericServices.Services.Service<Payment>, IPaymentService
    {
        private readonly StripeSettings _stripeSettings;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PaymentService(IUnitOfWork unitOfWork, IMapper mapper
            , IHttpContextAccessor httpContextAccessor, IOptions<StripeSettings> stripeSettings) : base(unitOfWork, mapper)
        {
            _stripeSettings = stripeSettings.Value;
            _httpContextAccessor = httpContextAccessor;
        }

        public override async Task<ServiceResponse> Create<TAddDto>(TAddDto dto)
        {
            var addDto = dto as PaymentCreateDto;
            var cart = await _unitOfWork.Carts.Value.GetAsync(s => s.Id == addDto.CartId
            , x => x.Include(s => s.CartProducts).ThenInclude(s => s.Product));
            if (cart is null)
            {
                return new ServiceResponse
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Success = false,
                    Message = "Order Is Not Found"
                };
            }
            var sessionId = Pay(cart);
            if (sessionId == string.Empty)
            {
                return new ServiceResponse
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Payment Process Failed",
                    Success = false
                };
            }
            addDto.SessionId = sessionId;


            cart.Status = Shared.Utilities.Models.Enums.CartStatusEnum.Paid;

            return await base.Create(addDto);
        }
        string Pay(Cart cart)
        {
            StripeConfiguration.ApiKey = _stripeSettings.SecretKey;

            var options = new SessionCreateOptions
            {
                Customer = _httpContextAccessor.HttpContext.User.Claims
                    .First(p => p.Type == ClaimTypes.Name).Value,
                PaymentMethodTypes = new List<string> {
                    "card"
                },
                LineItems = cart.CartProducts.SelectMany(p =>
                {
                    return new List<SessionLineItemOptions>
                    {
                        new SessionLineItemOptions
                        {
                            PriceData = new SessionLineItemPriceDataOptions
                            {
                                Currency = "EGP",
                                UnitAmount = (long?)p.Product.Price,
                                ProductData = new SessionLineItemPriceDataProductDataOptions
                                {
                                    Name  = p.Product.Title,
                                    Description = p.Product.Description
                                }
                            },
                            Quantity = p.Quantity
                        }
                    };
                }).ToList(),
                Mode = "payment",
                SuccessUrl = "https://www.google.com/search?q=how+to+solve+problem+of+circle+reference+in+.net+core&rlz=1C1GCEU_enEG1067EG1067&oq=how+to+solve+problem+of+circle+reference+in+.net+core+&gs_lcrp=EgZjaHJvbWUyCwgAEEUYChg5GKABMgkIARAhGAoYoAHSAQg1MjgyajBqMagCALACAA&sourceid=chrome&ie=UTF-8"
            };
            try
            {
                var s = new SessionService().Create(options);

                return s.Id;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }
    }
}
