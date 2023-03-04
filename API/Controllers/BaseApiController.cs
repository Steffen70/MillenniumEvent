using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using API.Data;
using API.Helpers;
using AutoMapper;

namespace API.Controllers
{
    [ServiceFilter(typeof(LogUserActivity))]
    [ApiController]
    [Route("Api/[controller]")]
    [Authorize]
    public abstract class BaseApiController : ControllerBase
    {
        protected readonly IMapper Mapper;
        protected readonly DataContext Context;

        protected BaseApiController(DataContext context, IMapper mapper)
        {
            Context = context;
            Mapper = mapper;
        }
    }
}