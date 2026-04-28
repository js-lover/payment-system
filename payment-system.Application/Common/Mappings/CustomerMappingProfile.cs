using AutoMapper;
using payment_system.Application.DTOs.Customer;
using payment_system.Application.DTOs.Account;
using payment_system.Domain.Entities;
using payment_system.Domain.Enums;

namespace payment_system.Application.Common.Mappings
{
    /// <summary>
    /// Mapping profile for customer-related DTOs.
    /// Handles:
    /// - Customer → CustomerDto (with nested User and Accounts)
    /// - Account → AccountDto (with Currency enum conversion)
    /// </summary>
    public class CustomerMappingProfile : Profile
    {
        public CustomerMappingProfile()
        {
            // Customer Entity → CustomerDto
            CreateMap<Customer, CustomerDto>()
                // Email comes from related User entity
                .ForMember(
                    dest => dest.Email,
                    opt => opt.MapFrom(src => src.User != null ? src.User.Email : string.Empty))
                // Accounts mapping (nested)
                .ForMember(
                    dest => dest.Accounts,
                    opt => opt.MapFrom(src => src.Accounts != null ? src.Accounts : new List<Account>()));

            // Account Entity → AccountDto
            CreateMap<Account, AccountDto>()
                // Currency enum → string
                .ForMember(
                    dest => dest.Currency,
                    opt => opt.MapFrom(src => src.Currency.ToString()));

            // CreateCustomerRequest → Customer Entity
            CreateMap<CreateCustomerRequest, Customer>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.Accounts, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());
        }
    }
}