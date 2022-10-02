using System;
using Microsoft.AspNetCore.Mvc;
using UserService.DAL.Models.Interfaces;
using UserService.Helpers;

namespace UserService.Controllers
{
    public class ParentController : Controller
    {
        protected readonly IDbRepository DbRepository;
        
        protected ParentController(IDbRepository dbRepository)
        {
            DbRepository = dbRepository;
        }

        protected static Tuple<Guid, string> TryParseGuid(string id, string nameOfId)
            => GuidParser.TryParse(id, nameOfId);
    }
}