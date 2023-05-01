using MagicVilla_VillaAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MagicVilla_VillaAPI.Controllers
{
    //ControllerBase for API, Controller for views and MVC extra features that we don't need
    //using [controller] inside of route will automatically use the className - "controller" if you want to do that.
    [Route("api/VillaAPI")]
    [ApiController]
    public class VillaAPIController : ControllerBase
    {
        [HttpGet]
        public IEnumerable<Villa> GetVillas()
        {
            return new List<Villa>
            {
                new Villa { Id = 1, Name = "Pool View" },
                new Villa { Id = 2, Name = "Beach View" }
            };
        }
    }
}
