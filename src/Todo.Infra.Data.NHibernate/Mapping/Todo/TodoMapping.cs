using FluentNHibernate.Mapping;
using Todo.Domain.Entities.Task;

namespace Todo.Infra.Data.NHibernate.Mapping.Todo
{
  public class TodoMapping : ClassMap<Task>
  {
    public TodoMapping()
    {
      Schema("todo");
      Table("task");
      Id(x => x.Id).GeneratedBy.GuidComb().Unique();
      Map(x => x.Date).Not.Nullable();
      Map(x => x.Description).Length(256).Not.Nullable();
      Map(x => x.Completed).Not.Nullable();
    }
  }
}
