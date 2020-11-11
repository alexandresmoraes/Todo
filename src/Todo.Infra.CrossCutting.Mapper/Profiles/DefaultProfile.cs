using AutoMapper;
using Todo.Domain.Commands.Responses.Auth;
using Todo.Domain.Dto.Auth;

namespace Todo.Infra.CrossCutting.Mapper.Profiles
{
  public class DefaultProfile : Profile
  {
    public DefaultProfile()
    {
      CreateMap<AccessTokenDto, LoginResponse>()
        .ForMember(x => x.IssuedUtc, opt => opt.Ignore());

      CreateMap<AccessTokenDto, RefreshTokenResponse>()
        .ForMember(x => x.IssuedUtc, opt => opt.Ignore());
    }
  }
}
