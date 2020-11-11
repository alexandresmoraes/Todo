using Microsoft.AspNetCore.Identity;
using System;

namespace Todo.Infra.CrossCutting.Auth.Entities
{
  public class RoleClaim : IdentityRoleClaim<Guid>
  {
  }
}