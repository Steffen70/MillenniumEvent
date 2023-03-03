﻿using System;
using System.Collections;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace API.Controllers
{
    [AllowAnonymous]
    public class TicketController : BaseApiController
    {
        private readonly IOptions<ApiSettings> _apiSettings;
        private readonly EmailService _service;

        public TicketController(DataContext context, IMapper mapper, IOptions<ApiSettings> apiSettings, EmailService service) : base(context, mapper)
        {
            _apiSettings = apiSettings;
            _service = service;
        }

        [Authorize(Policy = "RequirePromoterRole")]
        [HttpPost("Send")]
        public async Task<ActionResult> Send([FromQuery] string email)
        {
            var regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            if (!regex.Match(email).Success)
                return BadRequest();

            if (Context.Tickets.Any(t => t.Email == email))
                return StatusCode(208);

            var ticket = new Ticket(User.GetUserId(), email);

            Context.Tickets.Add(ticket);
            var logicTicket = Mapper.Map<Logic.Ticket>(ticket);

            var flyer = Image.FromFile($@".\Data\{_apiSettings.Value.FlyerImageName}");
            var bitmap = logicTicket.GenerateTicketBitmap(flyer);
            logicTicket.SendViaMail(_service, bitmap);

            await Context.SaveChangesAsync();

            return StatusCode(201);
        }
    }
}
