using System;

namespace UserService.DAL.Models.Interfaces
{
    public interface IEntity
    {
        public Guid Id { get; set; }
    }
}