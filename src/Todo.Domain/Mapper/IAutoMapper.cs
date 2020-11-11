namespace Todo.Domain.Mapper
{
  public interface IAutoMapper
  {
    TDestination Map<TDestination>(object source);
    TDestination Map<TSource, TDestination>(TSource source);
    TDestination Map<TSource, TDestination>(TSource source, TDestination destination);
  }
}