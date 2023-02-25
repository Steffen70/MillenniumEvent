using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using API.Data;
using API.Helpers;
using AutoMapper;

namespace API.Controllers
{
    [ServiceFilter(typeof(LogUserActivity))]
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public abstract class BaseApiController : ControllerBase
    {
        protected readonly IMapper Mapper;
        protected readonly UnitOfWork UnitOfWork;

        protected BaseApiController(UnitOfWork unitOfWork, IMapper mapper)
        {
            UnitOfWork = unitOfWork;
            Mapper = mapper;
        }
    }
}