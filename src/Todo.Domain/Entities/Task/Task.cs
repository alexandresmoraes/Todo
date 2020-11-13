using System;

namespace Todo.Domain.Entities.Task
{
  public class Task
  {
    public virtual Guid Id { get; set; }
    public virtual DateTime Date { get; set; } = DateTime.Now;
    public virtual string Description { get; set; }
    public virtual bool Completed { get; set; } = false;
  }
}