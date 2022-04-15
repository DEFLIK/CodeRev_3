using System;
using Bua.CodeRev.UserService.DAL.Models.Interfaces;

namespace Bua.CodeRev.UserService.DAL.Entities
{
    public class BaseEntity : IEntity
    {
        public Guid Id { get; set; }
    }
}