using Microsoft.AspNetCore.Identity;
using System;

namespace Todo.Infra.CrossCutting.Auth.Entities
{
  public class UserClaim : IdentityUserClaim<Guid>
  {
  }
}