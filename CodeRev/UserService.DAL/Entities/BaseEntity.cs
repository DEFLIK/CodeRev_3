using System;
using UserService.DAL.Models.Interfaces;

namespace UserService.DAL.Entities
{
    public class BaseEntity : IEntity
    {
        public Guid Id { get; set; }
    }
}