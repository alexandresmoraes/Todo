using Microsoft.AspNetCore.Identity;
using System;

namespace Todo.Infra.CrossCutting.Auth.Entities
{
  public class User : IdentityUser<Guid>
  {
    public virtual string Firstname { get; set; }
    public virtual string Lastname { get; set; }
  }
}