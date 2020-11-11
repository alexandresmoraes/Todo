using AutoMapper;
using Todo.Domain.Mapper;

namespace Todo.Infra.CrossCutting.Mapper
{
  public class AutoMapper : IAutoMapper
  {
    private readonly IMapper _mapper;

    public AutoMapper(IMapper mapper)
    {
      _mapper = mapper;
    }

    public TDestination Map<TSource, TDestination>(TSource source)
      => _mapper.Map<TSource, TDestination>(source);

    public TDestination Map<TSource, TDestination>(TSource source, TDestination destination)
      => _mapper.Map(source, destination);

    public TDestination Map<TDestination>(object source)
      => _mapper.Map<TDestination>(source);
  }
}