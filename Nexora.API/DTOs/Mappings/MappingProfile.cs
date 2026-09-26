using AutoMapper;
using Nexora.API.DTOs.Finance;
using Nexora.Application.DTOs.Finance;

namespace Nexora.API.DTOs.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<TransactionHistoryDto, TransactionHistoryResponse>();
    }
}