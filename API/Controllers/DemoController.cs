using API.Data;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [AllowAnonymous]
    public class DemoController : BaseApiController
    {
        public DemoController(DataContext context, IMapper mapper) : base(context, mapper)
        {
        }

        [HttpGet("HelloWorld")]
        public ActionResult HelloWorld([FromQuery] string name)
        {
            return Ok($"Hello {name}!");
        }
    }
}