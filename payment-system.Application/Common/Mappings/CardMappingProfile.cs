using AutoMapper;
using payment_system.Application.DTOs.Card;
using payment_system.Domain.Entities;

namespace payment_system.Application.Common.Mappings
{
    /// <summary>
    /// Card entity ile DTO'lar arasında mapping
    /// CardNumber maskelenmiş formata dönüştürülür
    /// </summary>
    public class CardMappingProfile : Profile
    {
        public CardMappingProfile()
        {
            // Card Entity → CardDto
            CreateMap<Card, CardDto>()
                .ForMember(
                    dest => dest.CardNumber,
                    opt => opt.MapFrom(src => MaskCardNumber(src.CardNumber)))
                .ForMember(
                    dest => dest.Status,
                    opt => opt.MapFrom(src => src.Status));

            // CardDto → Card Entity (geriye dönüş)
            CreateMap<CardDto, Card>();

            // CreateCardRequest → Card Entity
            CreateMap<CreateCardRequest, Card>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.Account, opt => opt.Ignore())
                .ForMember(dest => dest.Transactions, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());
        }

        private static string MaskCardNumber(string cardNumber)
        {
            if (string.IsNullOrEmpty(cardNumber) || cardNumber.Length < 8)
                return cardNumber;

            var first4 = cardNumber.Substring(0, 4);
            var last4 = cardNumber.Substring(cardNumber.Length - 4);
            return $"{first4} **** **** {last4}";
        }
    }
}
