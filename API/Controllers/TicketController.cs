using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class TicketController : BaseApiController
    {
        public TicketController(DataContext context, IMapper mapper) : base(context, mapper) { }

        [Authorize(Policy = "RequirePromoterRole")]
        [HttpPost("Send")]
        public async Task<ActionResult> Send([FromQuery] string email)
        {
            if (email == null) return BadRequest();

            if (email == "test") return StatusCode(208); //Already Reported

            return StatusCode(201); // Created
        }
    }
}
