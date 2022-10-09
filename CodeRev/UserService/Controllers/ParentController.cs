using System;
using Microsoft.AspNetCore.Mvc;
using UserService.DAL.Models.Interfaces;

namespace UserService.Controllers
{
    public class ParentController : Controller
    {
        protected readonly IDbRepository _dbRepository;
        
        protected ParentController(IDbRepository dbRepository)
        {
            _dbRepository = dbRepository;
        }

        protected Tuple<Guid, string> TryParseGuid(string id, string nameOfId)
        {
            
            var guid = new Guid();
            string errorString = null;
            try
            {
                guid = Guid.Parse(id);
            }
            catch (ArgumentNullException)
            {
                errorString = $"{nameOfId} to be parsed is null";
            }
            catch (FormatException)
            {
                errorString = $"{nameOfId} should be in UUID format";
            }
            return new Tuple<Guid, string>(guid, errorString);
        }
    }
}