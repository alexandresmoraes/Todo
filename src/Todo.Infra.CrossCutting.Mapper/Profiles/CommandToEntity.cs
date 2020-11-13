using AutoMapper;
using Todo.Domain.Commands.Requests.Todo;
using Todo.Domain.Entities.Task;

namespace Todo.Infra.CrossCutting.Mapper.Profiles
{
  public class CommandToEntity : Profile
  {
    public CommandToEntity()
    {
      CreateMap<CreateTaskCommand, Task>()
        .ForMember(src => src.Description, opt => opt.MapFrom(dst => dst.Description))
        .ForAllOtherMembers(x => x.Ignore());

      CreateMap<ChangeTaskCommand, Task>()
        .ForMember(src => src.Description, opt => opt.MapFrom(dst => dst.Description))
        .ForMember(src => src.Id, opt => opt.MapFrom(dst => dst.Id))
        .ForMember(src => src.Completed, opt => opt.MapFrom(dst => dst.Completed))
        .ForAllOtherMembers(x => x.Ignore());

      CreateMap<DeleteTaskCommand, Task>()
        .ForMember(src => src.Id, opt => opt.MapFrom(dst => dst.Id))
        .ForAllOtherMembers(x => x.Ignore());
    }
  }
}