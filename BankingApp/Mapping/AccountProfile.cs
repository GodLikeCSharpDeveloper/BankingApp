using AutoMapper;
using BankingApp.Entities;
using BankingApp.Models;

namespace BankingApp.Mapping
{
    public class AccountProfile : Profile
    {
        public AccountProfile()
        {
            CreateMap<CreateAccountDto, AccountEntity>()
                .ForMember(dest => dest.Balance, opt => opt.MapFrom(src => src.InitialBalance));

            CreateMap<AccountEntity, GetAccountDto>()
                .ForMember(dest => dest.Balance, opt => opt.MapFrom(src => src.Balance))
                .ForMember(dest => dest.AccountNumber, opt => opt.MapFrom(src => src.AccountNumber));
        }
    }
}